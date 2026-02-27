using Core.Entities;
using Core.Entities.Questions;
using Core.Entities.Questions.ValueObjects;
using Core.Entities.Sessions;
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

    // DbSets for Question aggregate
    public DbSet<Question> Questions { get; set; } = null!;
    public DbSet<McqQuestion> MCQQuestions { get; set; } = null!;
    public DbSet<McqAnswerOption> AnswerOptions { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<QuestionTag> QuestionTags { get; set; } = null!;

    // DbSets for Session aggregate
    public DbSet<Session> Sessions { get; set; } = null!;
    public DbSet<SessionParticipant> SessionParticipants { get; set; } = null!;
    public DbSet<SessionQuestion> SessionQuestions { get; set; } = null!;
    public DbSet<QuestionAttempt> QuestionAttempts { get; set; } = null!;
    public DbSet<ParticipantAnswer> ParticipantAnswers { get; set; } = null!;
    public DbSet<ParticipantMCQAnswer> ParticipantMCQAnswers { get; set; } = null!;

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
                    address
                        .Property(a => a.Suburb)
                        .HasMaxLength(200)
                        .HasColumnName("AddressSuburb");
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
                    consent
                        .Property(c => c.TermId)
                        .HasMaxLength(100)
                        .HasColumnName("ConsentTermId");
                    consent
                        .Property(c => c.TermLink)
                        .HasMaxLength(2000)
                        .HasColumnName("ConsentTermLink");
                    consent
                        .Property(c => c.TermsVersion)
                        .HasMaxLength(50)
                        .HasColumnName("ConsentTermsVersion");
                    consent.Property(c => c.AcceptedAt).HasColumnName("ConsentAcceptedAt");
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
        // Question Configuration (TPT - Table Per Type)
        // ============================================
        // Base Question table
        builder.Entity<Question>(entity =>
        {
            entity.HasKey(q => q.Id);
            entity.Property(q => q.Number).HasMaxLength(50).IsRequired();
            entity.Property(q => q.Text).HasMaxLength(2000).IsRequired();
            entity.Property(q => q.Description).HasMaxLength(5000);
            entity.Property(q => q.CreatedBy).HasMaxLength(450).IsRequired();

            // Index for searching by creator
            entity.HasIndex(q => q.CreatedBy);

            // Index for searching by number
            entity.HasIndex(q => q.Number);

            // OwnsMany for MediaMetadatas (1:N) - Collection of media attachments
            entity.OwnsMany(
                q => q.MediaMetadatas,
                media =>
                {
                    media.WithOwner().HasForeignKey("QuestionId");
                    media.Property(m => m.Url).HasMaxLength(2000);
                    media.Property(m => m.DurationSeconds);
                    media.Property(m => m.MimeType).HasMaxLength(100);
                    media.Property(m => m.ThumbnailUrl).HasMaxLength(2000);
                    media.Property(m => m.MediaType);
                }
            );
        });

        // ============================================
        // MCQQuestion Configuration (TPT - inherits from Question)
        // ============================================
        builder.Entity<McqQuestion>(entity =>
        {
            // TPT mapping: MCQQuestions table has Id as FK to Questions table
            entity.ToTable("MCQQuestions");

            // HasMany for AnswerOptions (1:N) - Only MCQ questions have answer options
            entity
                .HasMany(q => q.AnswerOptions)
                .WithOne(o => o.McqQuestion)
                .HasForeignKey(o => o.McqQuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // AnswerOption Configuration
        // ============================================
        builder.Entity<McqAnswerOption>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Text).HasMaxLength(1000).IsRequired();

            // Index for ordering
            entity.HasIndex(o => new { o.McqQuestionId, o.Order });
        });

        // ============================================
        // Tag Configuration
        // ============================================
        builder.Entity<Tag>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).HasMaxLength(100).IsRequired();
            entity.Property(t => t.NormalizedName).HasMaxLength(100).IsRequired();

            // Unique index for normalized name (tags are reusable)
            entity.HasIndex(t => t.NormalizedName).IsUnique();
        });

        // ============================================
        // QuestionTag Configuration (Join Table)
        // ============================================
        builder.Entity<QuestionTag>(entity =>
        {
            entity.HasKey(qt => qt.Id);

            // Composite unique index to prevent duplicate tag assignments
            entity.HasIndex(qt => new { qt.QuestionId, qt.TagId }).IsUnique();

            // Relationships
            entity
                .HasOne(qt => qt.Question)
                .WithMany(q => q.QuestionTags)
                .HasForeignKey(qt => qt.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(qt => qt.Tag)
                .WithMany(t => t.QuestionTags)
                .HasForeignKey(qt => qt.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // Session Configuration
        // ============================================
        builder.Entity<Session>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).HasMaxLength(200).IsRequired();
            entity.Property(s => s.Code).HasMaxLength(20).IsRequired();
            entity.Property(s => s.Location).HasMaxLength(500);
            entity.Property(s => s.CreatedBy).HasMaxLength(450).IsRequired();

            // Unique index for session code
            entity.HasIndex(s => s.Code).IsUnique();

            // HasMany for Participants (1:N)
            entity
                .HasMany(s => s.Participants)
                .WithOne(p => p.Session)
                .HasForeignKey(p => p.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // HasMany for SessionQuestions (1:N)
            entity
                .HasMany(s => s.SessionQuestions)
                .WithOne(sq => sq.Session)
                .HasForeignKey(sq => sq.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // SessionParticipant Configuration
        // ============================================
        builder.Entity<SessionParticipant>(entity =>
        {
            entity.HasKey(sp => sp.Id);

            // Unique constraint for session + user
            entity.HasIndex(sp => new { sp.SessionId, sp.UserId }).IsUnique();

            // Relationship to User
            entity
                .HasOne(sp => sp.User)
                .WithMany()
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // SessionQuestion Configuration
        // ============================================
        builder.Entity<SessionQuestion>(entity =>
        {
            entity.HasKey(sq => sq.Id);

            // Unique constraint for session + question
            entity.HasIndex(sq => new { sq.SessionId, sq.QuestionId }).IsUnique();

            // Relationship to Question
            entity
                .HasOne(sq => sq.Question)
                .WithMany()
                .HasForeignKey(sq => sq.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // HasMany for Attempts (1:N)
            entity
                .HasMany(sq => sq.Attempts)
                .WithOne(a => a.SessionQuestion)
                .HasForeignKey(a => a.SessionQuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // QuestionAttempt Configuration
        // ============================================
        builder.Entity<QuestionAttempt>(entity =>
        {
            entity.HasKey(qa => qa.Id);

            // Unique constraint for session question + attempt number
            entity.HasIndex(qa => new { qa.SessionQuestionId, qa.AttemptNumber }).IsUnique();
        });

        // ============================================
        // ParticipantAnswer Configuration (TPT - Table Per Type)
        // ============================================
        // Base ParticipantAnswer table
        builder.Entity<ParticipantAnswer>(entity =>
        {
            entity.HasKey(pa => pa.Id);
            entity.Property(pa => pa.AnswerType).HasMaxLength(50).IsRequired();

            // Unique constraint for participant + attempt (one answer per participant per attempt)
            entity
                .HasIndex(pa => new { pa.SessionParticipantId, pa.QuestionAttemptId })
                .IsUnique();

            // Relationship to SessionParticipant
            entity
                .HasOne(pa => pa.Participant)
                .WithMany(p => p.Answers)
                .HasForeignKey(pa => pa.SessionParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship to QuestionAttempt
            entity
                .HasOne(pa => pa.Attempt)
                .WithMany(a => a.Answers)
                .HasForeignKey(pa => pa.QuestionAttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship to Question
            entity
                .HasOne(pa => pa.Question)
                .WithMany()
                .HasForeignKey(pa => pa.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============================================
        // ParticipantMCQAnswer Configuration (TPT)
        // ============================================
        builder.Entity<ParticipantMCQAnswer>(entity =>
        {
            // TPT mapping: ParticipantMCQAnswers table has Id as FK to ParticipantAnswers table
            entity.ToTable("ParticipantMCQAnswers");
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
