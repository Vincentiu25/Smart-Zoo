using Ardalis.EFCore.Extensions;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Infrastructure.Database;

/// <summary>
/// This is the database context used to connect with the database and links the ORM, Entity Framework, with it.
/// </summary>
public sealed class WebAppDatabaseContext : DbContext
{
    public WebAppDatabaseContext(DbContextOptions<WebAppDatabaseContext> options, bool migrate = true) : base(options)
    {
        if (migrate)
        {
            Database.Migrate();
        }
    }

    // DbSet pentru User
    public DbSet<User> Users => Set<User>();

    // DbSet pentru animalele din grădina zoologică
    public DbSet<ZooAnimal> ZooAnimals => Set<ZooAnimal>();

    // DbSet pentru specii
    public DbSet<Species> Species => Set<Species>();

    //  DbSet-uri noi
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Profession> Professions => Set<Profession>();
    public DbSet<EmployeeProfile> EmployeeProfiles => Set<EmployeeProfile>();

    //  DbSet pentru tabela many-to-many
    public DbSet<EmployeeZooAnimal> EmployeeZooAnimals => Set<EmployeeZooAnimal>();

    /// <summary>
    /// Here additional configuration for the ORM is performed.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // activăm extensia "unaccent" din PostgreSQL și configurăm entitățile prin FluentAPI
        modelBuilder.HasPostgresExtension("unaccent")
            .ApplyAllConfigurationsFromCurrentAssembly();

        //  Index unic pe Email în User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Configurarea relației many-to-one între ZooAnimal și Species
        modelBuilder.Entity<ZooAnimal>()
            .HasOne(animal => animal.Species)
            .WithMany(species => species.Animals)
            .HasForeignKey(animal => animal.SpeciesId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relație one-to-many între Profession și Employees
        modelBuilder.Entity<Profession>()
            .HasMany(p => p.Employees)
            .WithOne(e => e.Profession)
            .HasForeignKey(e => e.ProfessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relație one-to-one între Employee și EmployeeProfile
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Profile)
            .WithOne(p => p.Employee)
            .HasForeignKey<EmployeeProfile>(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurăm cheia compusă pentru tabela intermediară many-to-many
        modelBuilder.Entity<EmployeeZooAnimal>()
            .HasKey(eza => new { eza.EmployeeId, eza.ZooAnimalId });

        // Configurăm relațiile pentru tabela intermediară many-to-many
        modelBuilder.Entity<Employee>()
            .HasMany(e => e.EmployeeZooAnimals)
            .WithOne(eza => eza.Employee)
            .HasForeignKey(eza => eza.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ZooAnimal>()
            .HasMany(z => z.EmployeeZooAnimals)
            .WithOne(eza => eza.ZooAnimal)
            .HasForeignKey(eza => eza.ZooAnimalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
