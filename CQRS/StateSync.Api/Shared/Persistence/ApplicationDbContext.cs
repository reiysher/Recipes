using Microsoft.EntityFrameworkCore;
using StateSync.Api.Shared.Entities;

namespace StateSync.Api.Shared.Persistence;

internal sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("users");

        modelBuilder.Entity<UserProjection>().ToTable("user_reads");
    }
}