using Microsoft.EntityFrameworkCore;
using StateAsync.Api.Shared.Entities;

namespace StateAsync.Api.Shared.Persistence;

internal sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("users");

        modelBuilder.Entity<UserProjection>().ToTable("user_reads");
        modelBuilder.Entity<UserEmailProjection>().ToTable("user_email_reads");

        modelBuilder.Entity<SyncEntity>().ToTable("synchronization");
        modelBuilder.Entity<SyncEntity>().HasKey(e => e.Type);
        modelBuilder.Entity<SyncEntity>().HasIndex(e => e.Type);
        modelBuilder.Entity<SyncEntity>().HasIndex(e => e.HasChanges);

        // todo: into registration
        modelBuilder.Entity<SyncEntity>().HasData(new SyncEntity
        {
            Type = typeof(User).AssemblyQualifiedName!,
            HasChanges = false,
            LastSyncUtc = DateTime.MinValue,
        });
    }
}