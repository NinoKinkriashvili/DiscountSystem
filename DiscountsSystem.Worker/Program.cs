using DiscountsSystem.Application;
using DiscountsSystem.Infrastructure;
using DiscountsSystem.Worker.Options;
using DiscountsSystem.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);


builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);


builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.Configure<WorkerOptions>(
    builder.Configuration.GetSection("WorkerOptions"));


builder.Services.AddHostedService<ReservationCleanupWorker>();
builder.Services.AddHostedService<OfferExpirationWorker>();

var host = builder.Build();
host.Run();
