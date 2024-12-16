﻿using EventsAsync.Api.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventsAsync.Api;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services
            .AddSingleton(TimeProvider.System);

        return services;
    }

    public static IServiceCollection RegisterPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            string connectionString = configuration.GetConnectionString("Default")!;

            options.UseNpgsql(connectionString, builder =>
            {
                builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("ef_migration_history");
            });
        });

        return services;
    }

    public static async Task InitializeDatabase(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }
}