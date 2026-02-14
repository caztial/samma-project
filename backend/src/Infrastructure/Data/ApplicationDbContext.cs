using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure ApplicationUser
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.HasIndex(u => u.NormalizedEmail).IsUnique();
            entity.HasIndex(u => u.NormalizedUserName).IsUnique();

            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
            entity.Property(u => u.ProfileImageUrl).HasMaxLength(500);
        });

        // Configure Identity roles
        builder.Entity<IdentityRole>(entity =>
        {
            entity.HasIndex(r => r.NormalizedName).IsUnique();
            entity.Property(r => r.Name).HasMaxLength(256);
            entity.Property(r => r.NormalizedName).HasMaxLength(256);
        });

        // Configure Identity role claims
        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.HasKey(rc => rc.Id);
            entity.HasIndex(rc => rc.RoleId);
        });

        // Configure Identity user logins
        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.HasKey(l => new { l.LoginProvider, l.ProviderKey });
        });

        // Configure Identity user tokens
        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.HasKey(t => new
            {
                t.UserId,
                t.LoginProvider,
                t.Name
            });
        });

        // Configure Identity user claims
        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.HasKey(uc => uc.Id);
            entity.HasIndex(uc => uc.UserId);
        });
    }
}
