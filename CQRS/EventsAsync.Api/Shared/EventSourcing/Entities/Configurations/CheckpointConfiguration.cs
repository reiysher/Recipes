using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventsAsync.Api.Shared.EventSourcing.Entities.Configurations;

internal sealed class CheckpointConfiguration : IEntityTypeConfiguration<Checkpoint>
{
    public void Configure(EntityTypeBuilder<Checkpoint> builder)
    {
        builder.HasKey(checkpoint => checkpoint.ProjectionName);
        builder.ToTable("checkpoints");

        builder.Property(checkpoint => checkpoint.ProjectionName)
            .HasMaxLength(1024)
            .ValueGeneratedNever();

        builder.Property(checkpoint => checkpoint.Value)
            .ValueGeneratedNever()
            .IsRequired(true);
    }
}