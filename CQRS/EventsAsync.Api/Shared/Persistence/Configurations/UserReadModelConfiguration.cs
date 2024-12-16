using EventsAsync.Api.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventsAsync.Api.Shared.Persistence.Configurations;

internal sealed class UserReadModelConfiguration : IEntityTypeConfiguration<UserReadModel>
{
    public void Configure(EntityTypeBuilder<UserReadModel> builder)
    {
        builder.HasKey(readModel => readModel.Id);
        builder.ToTable("user_reads");

        builder.Property(readModel => readModel.Id)
            .ValueGeneratedNever();
    }
}