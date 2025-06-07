using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PartyPlanner.Models;

namespace PartyPlanner.Data;

public sealed class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    private const int KeyLength = 255;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // ---------- DbSets ----------
    public DbSet<Party>      Parties     => Set<Party>();
    public DbSet<Question>   Questions   => Set<Question>();
    public DbSet<Option>     Options     => Set<Option>();
    public DbSet<Invitation> Invitations => Set<Invitation>();
    public DbSet<Response>   Responses   => Set<Response>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);             // <-- Identity eerst
        
        // ⬇️  registreert álle IEntityTypeConfiguration<T> in deze assembly
        b.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        ConfigureIdentity(b);                // lengte-fixes
        ConfigureDomain(b);                  // eigen tabellen
    }

    // ------------------------------------------------------------------
    // 1. Identity-kolommen ⇢ VARCHAR(255) i.p.v. (long)TEXT
    // ------------------------------------------------------------------
    private static void ConfigureIdentity(ModelBuilder b)
    {
        // Users ---------------------------------------------------------
        b.Entity<IdentityUser>(e =>
        {
            e.Property(u => u.Id).HasMaxLength(KeyLength);
            e.Property(u => u.NormalizedEmail).HasMaxLength(KeyLength);
            e.Property(u => u.NormalizedUserName).HasMaxLength(KeyLength);
        });

        // Roles ---------------------------------------------------------
        b.Entity<IdentityRole>(e =>
        {
            e.Property(r => r.Id).HasMaxLength(KeyLength);
            e.Property(r => r.NormalizedName).HasMaxLength(KeyLength);
        });

        // Logins / tokens / claims --------------------------------------
        b.Entity<IdentityUserLogin<string>>(e =>
        {
            e.Property(l => l.LoginProvider).HasMaxLength(KeyLength);
            e.Property(l => l.ProviderKey).HasMaxLength(KeyLength);
            e.Property(l => l.UserId).HasMaxLength(KeyLength);
        });

        b.Entity<IdentityUserRole<string>>(e =>
        {
            e.Property(r => r.UserId).HasMaxLength(KeyLength);
            e.Property(r => r.RoleId).HasMaxLength(KeyLength);
        });

        b.Entity<IdentityUserToken<string>>(e =>
        {
            e.Property(t => t.UserId).HasMaxLength(KeyLength);
            e.Property(t => t.LoginProvider).HasMaxLength(KeyLength);
            e.Property(t => t.Name).HasMaxLength(KeyLength);
        });

        b.Entity<IdentityRoleClaim<string>>(e =>
            e.Property(c => c.RoleId).HasMaxLength(KeyLength));

        b.Entity<IdentityUserClaim<string>>(e =>
            e.Property(c => c.UserId).HasMaxLength(KeyLength));
    }

    // ------------------------------------------------------------------
    // 2. Domein-tabellen
    // ------------------------------------------------------------------
    private static void ConfigureDomain(ModelBuilder b)
    {
        // ---- Party ----------------------------------------------------
        b.Entity<Party>(e =>
        {
            e.HasMany(p => p.Questions)
             .WithOne(q => q.Party)
             .HasForeignKey(q => q.PartyId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(p => p.Invitations)
             .WithOne(i => i.Party)
             .HasForeignKey(i => i.PartyId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ---- Question -------------------------------------------------
        b.Entity<Question>(e =>
        {
            // enum -> tinyint
            e.Property(q => q.Type).HasConversion<byte>();

            e.HasMany(q => q.Options)
             .WithOne(o => o.Question)
             .HasForeignKey(o => o.QuestionId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(q => q.Responses)
             .WithOne(r => r.Question)
             .HasForeignKey(r => r.QuestionId);
        });

        // ---- Invitation ----------------------------------------------
        b.Entity<Invitation>(e =>
        {
            e.HasIndex(i => i.Code).IsUnique();

            e.HasMany(i => i.Responses)
             .WithOne(r => r.Invitation)
             .HasForeignKey(r => r.InvitationId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ---- Response -------------------------------------------------
        b.Entity<Response>(e =>
        {
            e.HasIndex(r => new { r.InvitationId, r.QuestionId }).IsUnique();

            e.HasOne(r => r.SelectedOption)
             .WithMany()
             .HasForeignKey(r => r.SelectedOptionId)
             .OnDelete(DeleteBehavior.SetNull);
        });
    }
} 