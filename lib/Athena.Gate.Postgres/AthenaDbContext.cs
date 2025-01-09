using Athena.Gate.Postgres.Models;
using Microsoft.EntityFrameworkCore;

namespace Athena.Gate.Postgres;

public sealed class AthenaDbContext(DbContextOptions<AthenaDbContext> options) : DbContext(options)
{
    public DbSet<UserAccountDataModel> UserAccounts { get; set; }
    public DbSet<ServiceAccountDataModel> ServiceAccounts { get; set; }
    public DbSet<UserRoleDataModel> UserRoles { get; set; }
    public DbSet<ServiceRoleDataModel> ServiceRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("athena");

        CreateUserAccounts(modelBuilder);
        CreateServiceAccounts(modelBuilder);
        CreateRoles(modelBuilder);
        CreateUserRoles(modelBuilder);
        CreateServiceRoles(modelBuilder);
    }

    private static void CreateRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRoleDataModel>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<UserRoleDataModel>()
            .Property(x => x.Description)
            .HasMaxLength(256);

        modelBuilder.Entity<UserRoleDataModel>()
            .Property(x => x.DisplayName)
            .HasMaxLength(128);

        modelBuilder.Entity<UserRoleDataModel>()
            .Property(x => x.Id)
            .HasMaxLength(128)
            .IsRequired();
    }

    private static void CreateServiceRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServiceAccountDataModel>()
            .HasMany(x => x.Roles)
            .WithMany(x => x.Accounts)
            .UsingEntity<ServiceAccountScopeDataModel>()
            .ToTable("ServiceRoles");
    }

    private static void CreateUserRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccountDataModel>()
            .HasMany(x => x.Roles)
            .WithMany(x => x.Accounts)
            .UsingEntity<UserAccountScopeDataModel>()
            .ToTable("UserRoles");
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