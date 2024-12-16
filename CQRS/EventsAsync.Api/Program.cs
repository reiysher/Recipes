using System.Reflection;
using EventsAsync.Api;
using EventsAsync.Api.Features.Users;
using EventsAsync.Api.Shared.Abstractions;
using EventsAsync.Api.Shared.Endpoints;
using EventsAsync.Api.Shared.EventSourcing.Extensions;
using EventsAsync.Api.Shared.EventSourcing.Projections;
using EventsAsync.Api.Shared.OpenApi;
using EventsAsync.Api.Shared.Persistence;

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
    options.AddAsyncProjection<UserProjection>();
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