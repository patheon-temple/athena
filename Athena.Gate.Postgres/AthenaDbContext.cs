using Athena.Gate.Postgres.Models;
using Microsoft.EntityFrameworkCore;

namespace Athena.Gate.Postgres;

public sealed class AthenaDbContext(DbContextOptions<AthenaDbContext> options) : DbContext(options)
{
    public DbSet<UserAccountDataModel> UserAccounts { get; set; }
    public DbSet<ServiceAccountDataModel> ServiceAccounts { get; set; }
    public DbSet<UserClaimDataModel> UserClaims { get; set; }
    public DbSet<ServiceClaimDataModel> ServiceClaims { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("athena");

        CreateUserAccounts(modelBuilder);
        CreateServiceAccounts(modelBuilder);
        CreateClaims(modelBuilder);
        CreateUserClaims(modelBuilder);
        CreateServiceClaims(modelBuilder);
    }

    private static void CreateClaims(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserClaimDataModel>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<UserClaimDataModel>()
            .Property(x => x.Description)
            .HasMaxLength(256);

        modelBuilder.Entity<UserClaimDataModel>()
            .Property(x => x.DisplayName)
            .HasMaxLength(128);

        modelBuilder.Entity<UserClaimDataModel>()
            .Property(x => x.Id)
            .HasMaxLength(128)
            .IsRequired();
    }

    private static void CreateServiceClaims(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServiceAccountDataModel>()
            .HasMany(x => x.Claims)
            .WithMany(x => x.Accounts)
            .UsingEntity<ServiceAccountScopeDataModel>()
            .ToTable("ServicesClaims");
    }

    private static void CreateUserClaims(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccountDataModel>()
            .HasMany(x => x.Claims)
            .WithMany(x => x.Accounts)
            .UsingEntity<UserAccountScopeDataModel>()
            .ToTable("UsersClaims");
    }

    private static void CreateServiceAccounts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServiceAccountDataModel>()
            .HasKey(b => b.Id);

        modelBuilder.Entity<ServiceAccountDataModel>()
            .Property(b => b.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<ServiceAccountDataModel>().Property(b=>b.Name).HasMaxLength(256)
            .IsRequired();
        
        modelBuilder.Entity<ServiceAccountDataModel>()
            .Property(b => b.AuthorizationCode)
            .HasMaxLength(256);
    }

    private static void CreateUserAccounts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccountDataModel>()
            .HasKey(b => b.Id);

        modelBuilder.Entity<UserAccountDataModel>()
            .Property(b => b.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<UserAccountDataModel>()
            .Property(b => b.DeviceId)
            .HasMaxLength(128)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<UserAccountDataModel>()
            .Property(b => b.Username)
            .HasMaxLength(128)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<UserAccountDataModel>()
            .Property(b => b.PasswordHash);
    }
}