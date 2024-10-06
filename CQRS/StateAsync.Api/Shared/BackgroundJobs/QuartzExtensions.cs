using Quartz;
using StateAsync.Api.Shared.Persistence;

namespace StateAsync.Api.Shared.BackgroundJobs;

internal static class QuartzExtensions
{
    public static IServiceCollectionQuartzConfigurator ConfigureJob<T>(
        this IServiceCollectionQuartzConfigurator options,
        string cronExpression)
        where T : IJob
    {
        var jobKey = JobKey.Create(nameof(T));

        options
            .AddJob<T>(builder => builder.WithIdentity(jobKey))
            .AddTrigger(trigger =>
                trigger
                    .ForJob(jobKey)
                    .WithCronSchedule(cronExpression));

        return options;
    }

    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
            options.ConfigureJob<SyncJob>("*/5 * * * * ?");
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
