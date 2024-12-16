using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventsAsync.Api.Shared.EventSourcing.Entities.Configurations;

internal class EventStreamConfiguration
    : IEntityTypeConfiguration<EventStream>
{
    public void Configure(EntityTypeBuilder<EventStream> builder)
    {
        builder.HasKey(s => s.Id);
        builder.ToTable("streams");

        builder.Property(s => s.Id)
            .ValueGeneratedNever();

        builder.Property(s => s.Type)
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(s => s.Version)
            .IsRequired();

        builder.HasMany(s => s.Events)
            .WithOne()
            .HasForeignKey(e => e.StreamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
