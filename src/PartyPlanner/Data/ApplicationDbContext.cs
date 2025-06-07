using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace PartyPlanner.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Fix MySQL BLOB/TEXT column in key specification issue by setting max length
        builder.Entity<IdentityUser>(entity =>
        {
            entity.Property(m => m.Id).HasMaxLength(255);
            entity.Property(m => m.NormalizedEmail).HasMaxLength(255);
            entity.Property(m => m.NormalizedUserName).HasMaxLength(255);
        });

        builder.Entity<IdentityRole>(entity =>
        {
            entity.Property(m => m.Id).HasMaxLength(255);
            entity.Property(m => m.NormalizedName).HasMaxLength(255);
        });

        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.Property(m => m.LoginProvider).HasMaxLength(255);
            entity.Property(m => m.ProviderKey).HasMaxLength(255);
            entity.Property(m => m.UserId).HasMaxLength(255);
        });

        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.Property(m => m.UserId).HasMaxLength(255);
            entity.Property(m => m.RoleId).HasMaxLength(255);
        });

        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.Property(m => m.UserId).HasMaxLength(255);
            entity.Property(m => m.LoginProvider).HasMaxLength(255);
            entity.Property(m => m.Name).HasMaxLength(255);
        });

        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.Property(m => m.RoleId).HasMaxLength(255);
        });

        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.Property(m => m.UserId).HasMaxLength(255);
        });
    }
}
