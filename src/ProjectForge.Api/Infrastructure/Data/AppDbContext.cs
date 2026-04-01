using Microsoft.EntityFrameworkCore;
using ProjectForge.Api.Domain.Entities;
using ProjectForge.Api.Infrastructure.Data.Configurations;

namespace ProjectForge.Api.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTask> Tasks => Set<ProjectTask>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Incident> Incidents => Set<Incident>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectTaskConfiguration());
        modelBuilder.ApplyConfiguration(new NoteConfiguration());
        modelBuilder.ApplyConfiguration(new IncidentConfiguration());

        // TODO: Add global query filters for soft-delete if IsDeleted flag is introduced.
        // Example: modelBuilder.Entity<Project>().HasQueryFilter(p => !p.IsDeleted);
    }
}
