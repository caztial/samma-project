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
    public DbSet<Consent> Consents { get; set; } = null!;
    public DbSet<EmergencyContact> EmergencyContacts { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<Identification> Identifications { get; set; } = null!;

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

            // HasMany for EmergencyContacts (1:N)
            entity
                .HasMany(up => up.EmergencyContacts)
                .WithOne()
                .HasForeignKey(ec => ec.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // HasMany for Addresses (1:N)
            entity
                .HasMany(up => up.Addresses)
                .WithOne()
                .HasForeignKey(a => a.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // HasMany for Identifications (1:N)
            entity
                .HasMany(up => up.Identifications)
                .WithOne()
                .HasForeignKey(i => i.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // HasMany for Consents (1:N)
            entity
                .HasMany(up => up.Consents)
                .WithOne()
                .HasForeignKey(c => c.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // EmergencyContact Configuration (all encrypted)
        // ============================================
        builder.Entity<EmergencyContact>(entity =>
        {
            entity.HasKey(ec => ec.Id);
            entity.Property(ec => ec.Name).HasMaxLength(500);
            entity.Property(ec => ec.ContactNumber).HasMaxLength(500);
            entity.Property(ec => ec.Relationship).HasMaxLength(100);
            entity.Property(ec => ec.Email).HasMaxLength(500);
        });

        // ============================================
        // Address Configuration
        // ============================================
        builder.Entity<Address>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Line1).HasMaxLength(500);
            entity.Property(a => a.Line2).HasMaxLength(500);
            entity.Property(a => a.Suburb).HasMaxLength(200);
            entity.Property(a => a.StateProvince).HasMaxLength(200);
            entity.Property(a => a.Country).HasMaxLength(200);
            entity.Property(a => a.Postcode).HasMaxLength(20);
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
        // Consent Configuration
        // ============================================
        builder.Entity<Consent>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.TermId).HasMaxLength(100);
            entity.Property(c => c.TermLink).HasMaxLength(2000);
            entity.Property(c => c.TermsVersion).HasMaxLength(50);
            entity.Property(c => c.IpAddress).HasMaxLength(45);
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
