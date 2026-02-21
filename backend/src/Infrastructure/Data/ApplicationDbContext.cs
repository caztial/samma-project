using Core.Entities;
using Core.Entities.UserProfiles;
using Core.Entities.ValueObjects;
using Core.Services;
using Infrastructure.Data.Encryption;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IEncryptionService? _encryptionService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IEncryptionService encryptionService
    )
        : base(options)
    {
        _encryptionService = encryptionService;
    }

    // DbSets for UserProfile aggregate
    // Note: Biometrics and Contact are owned types (configured via OwnsOne), not separate DbSets
    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<UserAddress> UserAddresses { get; set; } = null!;
    public DbSet<UserConsent> UserConsents { get; set; } = null!;
    public DbSet<EmergencyContact> EmergencyContacts { get; set; } = null!;
    public DbSet<Identification> Identifications { get; set; } = null!;
    public DbSet<Education> Educations { get; set; } = null!;
    public DbSet<BankAccount> BankAccounts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply encryption converters if service is available
        if (_encryptionService != null)
        {
            builder.ApplyEncryption(_encryptionService);
        }

        // ============================================
        // ApplicationUser Configuration
        // ============================================
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.HasIndex(u => u.NormalizedEmail).IsUnique();
            entity.HasIndex(u => u.NormalizedUserName).IsUnique();
        });

        // ============================================
        // UserProfile Configuration
        // ============================================
        builder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(up => up.Id);

            // Encrypted fields - increased to accommodate AES encryption overhead
            entity.Property(up => up.FirstName).HasMaxLength(500);
            entity.Property(up => up.LastName).HasMaxLength(500);
            entity.Property(up => up.ProfileImageUrl).HasMaxLength(500);

            // OwnsOne for Contact (1:1) - encrypted
            entity.OwnsOne(
                up => up.Contact,
                contact =>
                {
                    contact
                        .Property(c => c.ContactNumber)
                        .HasMaxLength(500)
                        .HasColumnName("ContactNumber");
                    contact.Property(c => c.Email).HasMaxLength(500).HasColumnName("ContactEmail");
                }
            );

            // OwnsOne for Biometrics (1:1) - encrypted, Base64 encoded
            entity.OwnsOne(
                up => up.Biometrics,
                bio =>
                {
                    bio.Property(b => b.FingerPrint)
                        .HasMaxLength(5000)
                        .HasColumnName("FingerPrint");
                    bio.Property(b => b.Face).HasMaxLength(5000).HasColumnName("Face");
                }
            );

            // HasMany for UserAddresses (1:N)
            entity
                .HasMany(up => up.Addresses)
                .WithOne(ua => ua.UserProfile)
                .HasForeignKey(ua => ua.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // HasMany for EmergencyContacts (1:N)
            entity
                .HasMany(up => up.EmergencyContacts)
                .WithOne(ec => ec.UserProfile)
                .HasForeignKey(ec => ec.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // HasMany for Identifications (1:N)
            entity
                .HasMany(up => up.Identifications)
                .WithOne(i => i.UserProfile)
                .HasForeignKey(i => i.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // HasMany for UserConsents (1:N)
            entity
                .HasMany(up => up.Consents)
                .WithOne(uc => uc.UserProfile)
                .HasForeignKey(uc => uc.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // HasMany for Educations (1:N)
            entity
                .HasMany(up => up.Educations)
                .WithOne(e => e.UserProfile)
                .HasForeignKey(e => e.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // HasMany for BankAccounts (1:N)
            entity
                .HasMany(up => up.BankAccounts)
                .WithOne(ba => ba.UserProfile)
                .HasForeignKey(ba => ba.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // UserAddress Configuration
        // ============================================
        builder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(ua => ua.Id);
            entity.Property(ua => ua.Type).HasMaxLength(50);

            // OwnsOne for Address value object
            entity.OwnsOne(
                ua => ua.Address,
                address =>
                {
                    address.Property(a => a.Line1).HasMaxLength(500).HasColumnName("AddressLine1");
                    address.Property(a => a.Line2).HasMaxLength(500).HasColumnName("AddressLine2");
                    address.Property(a => a.Suburb).HasMaxLength(200).HasColumnName("AddressSuburb");
                    address
                        .Property(a => a.StateProvince)
                        .HasMaxLength(200)
                        .HasColumnName("AddressStateProvince");
                    address
                        .Property(a => a.Country)
                        .HasMaxLength(200)
                        .HasColumnName("AddressCountry");
                    address
                        .Property(a => a.Postcode)
                        .HasMaxLength(20)
                        .HasColumnName("AddressPostcode");
                }
            );
        });

        // ============================================
        // EmergencyContact Configuration
        // ============================================
        builder.Entity<EmergencyContact>(entity =>
        {
            entity.HasKey(ec => ec.Id);
            entity.Property(ec => ec.Name).HasMaxLength(500);
            entity.Property(ec => ec.Relationship).HasMaxLength(100);

            // OwnsOne for Contact value object (encrypted)
            entity.OwnsOne(
                ec => ec.Contact,
                contact =>
                {
                    contact
                        .Property(c => c.ContactNumber)
                        .HasMaxLength(500)
                        .HasColumnName("ContactNumber");
                    contact.Property(c => c.Email).HasMaxLength(500).HasColumnName("ContactEmail");
                }
            );
        });

        // ============================================
        // Identification Configuration (Value encrypted)
        // ============================================
        builder.Entity<Identification>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Type).HasMaxLength(100);
            entity.Property(i => i.Value).HasMaxLength(500);
        });

        // ============================================
        // UserConsent Configuration
        // ============================================
        builder.Entity<UserConsent>(entity =>
        {
            entity.HasKey(uc => uc.Id);

            // OwnsOne for Consent value object
            entity.OwnsOne(
                uc => uc.Consent,
                consent =>
                {
                    consent.Property(c => c.TermId).HasMaxLength(100).HasColumnName("ConsentTermId");
                    consent
                        .Property(c => c.TermLink)
                        .HasMaxLength(2000)
                        .HasColumnName("ConsentTermLink");
                    consent
                        .Property(c => c.TermsVersion)
                        .HasMaxLength(50)
                        .HasColumnName("ConsentTermsVersion");
                    consent
                        .Property(c => c.AcceptedAt)
                        .HasColumnName("ConsentAcceptedAt");
                    consent
                        .Property(c => c.IpAddress)
                        .HasMaxLength(45)
                        .HasColumnName("ConsentIpAddress");
                }
            );
        });

        // ============================================
        // Education Configuration
        // ============================================
        builder.Entity<Education>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Institution).HasMaxLength(500);
            entity.Property(e => e.Degree).HasMaxLength(200);
            entity.Property(e => e.FieldOfStudy).HasMaxLength(200);
            entity.Property(e => e.Grade).HasMaxLength(50);
            entity.Property(e => e.CertificateNumber).HasMaxLength(200);
        });

        // ============================================
        // BankAccount Configuration (encrypted fields)
        // ============================================
        builder.Entity<BankAccount>(entity =>
        {
            entity.HasKey(ba => ba.Id);
            entity.Property(ba => ba.BankName).HasMaxLength(200);
            entity.Property(ba => ba.AccountType).HasMaxLength(50);
            entity.Property(ba => ba.AccountHolderName).HasMaxLength(500);
            entity.Property(ba => ba.AccountNumber).HasMaxLength(500);
            entity.Property(ba => ba.BranchCode).HasMaxLength(50);
        });

        // ============================================
        // Identity Role Configuration
        // ============================================
        builder.Entity<IdentityRole>(entity =>
        {
            entity.HasIndex(r => r.NormalizedName).IsUnique();
            entity.Property(r => r.Name).HasMaxLength(256);
            entity.Property(r => r.NormalizedName).HasMaxLength(256);
        });

        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.HasKey(rc => rc.Id);
            entity.HasIndex(rc => rc.RoleId);
        });

        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.HasKey(l => new { l.LoginProvider, l.ProviderKey });
        });

        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.HasKey(t => new
            {
                t.UserId,
                t.LoginProvider,
                t.Name
            });
        });

        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.HasKey(uc => uc.Id);
            entity.HasIndex(uc => uc.UserId);
        });
    }
}