using System.Reflection;
using EventsSync.Api;
using EventsSync.Api.Features.Users;
using EventsSync.Api.Shared.Abstractions;
using EventsSync.Api.Shared.Endpoints;
using EventsSync.Api.Shared.EventSourcing.Enums;
using EventsSync.Api.Shared.EventSourcing.Extensions;
using EventsSync.Api.Shared.OpenApi;
using EventsSync.Api.Shared.Persistence;
using EventsSync.Api.Shared.Persistence.Projections;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        var schemaHelper = new SwashbuckleSchemaHelper();
        options.CustomSchemaIds(type => schemaHelper.GetSchemaId(type));
    });
builder.Services
    .RegisterApplicationServices()
    .RegisterPersistence(builder.Configuration)
    .AddEndpoints(Assembly.GetExecutingAssembly());

builder.Services
    .AddTransient<ChangeUserEmail.Handler>()
    .AddTransient<ChangeUserPersonalInfo.Handler>()
    .AddTransient<GetUser.Handler>()
    .AddTransient<RegisterUser.Handler>();


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddEventSourcing(options =>
{
    options.AddProjection<UserProjection>(ProjectionLifecycle.Inline);
});

var webApplication = builder.Build();

if (webApplication.Environment.IsDevelopment())
{
    webApplication
        .UseSwagger()
        .UseSwaggerUI();
}

webApplication.MapEndpoints();

await webApplication.Services.InitializeDatabase();

webApplication.Run();