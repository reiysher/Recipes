using System.Reflection;
using StateSync.Api;
using StateSync.Api.Features.Users;
using StateSync.Api.Shared.Abstractions;
using StateSync.Api.Shared.Endpoints;
using StateSync.Api.Shared.OpenApi;
using StateSync.Api.Shared.Persistence;

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

// todo: ISlice
builder.Services
    .AddTransient<ChangeUserEmail.Handler>()
    .AddTransient<ChangeUserPersonalInfo.Handler>()
    .AddTransient<GetUser.Handler>()
    .AddTransient<RegisterUser.Handler>();

// todo: Sync CQRS
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddTransient<IProjectionHandler, UserSummaryProjectionHandler>();

var webApplication = builder.Build();

if (webApplication.Environment.IsDevelopment())
{
    webApplication
        .UseSwagger()
        .UseSwaggerUI();
}

webApplication.MapEndpoints();

await webApplication.Services.InitializeDatabase();

await webApplication.RunAsync();