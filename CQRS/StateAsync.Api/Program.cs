using System.Reflection;
using StateAsync.Api;
using StateAsync.Api.Features.Users;
using StateAsync.Api.Shared.Abstractions;
using StateAsync.Api.Shared.BackgroundJobs;
using StateAsync.Api.Shared.Endpoints;
using StateAsync.Api.Shared.OpenApi;
using StateAsync.Api.Shared.Persistence;

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

// todo: Async CQRS
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddTransient<IAsyncProjectionHandler, UserAsyncProjectionHandler>();

builder.Services
    .AddBackgroundJobs();

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