using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebhookRelayService;
using WebhookRelayService.BackgroundServices;
using WebhookRelayService.Models;
using WebhookRelayService.Repositories;
using WebhookRelayService.Services;

var builder = WebApplication.CreateBuilder(args);
var settings = builder.Configuration.GetSection("WebhookRelaySettings").Get<Settings>();

builder.WebHost.UseSentry(s =>
{
    s.Dsn = settings.SentryDsn;
    s.Debug = Debugger.IsAttached;
    s.TracesSampleRate = 1;
});

builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConfiguration(builder.Configuration.GetSection("Logging"));
    config.AddConsole();
    config.AddDebug();
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));

// Add repositories and services
builder.Services.AddSingleton(settings);
builder.Services.AddTransient<IWebhookService, WebhookService>();
builder.Services.AddTransient<IWebhookUserService, WebhookUserService>();
builder.Services.AddTransient<IHttpService, HttpService>();
builder.Services.AddTransient<IWebhookUserRepository, WebhookUserRepository>();

builder.Services.AddHostedService<ClearWebhookUsers>();

var connectionString = $"{settings.PostgresConnection}{Environment.GetEnvironmentVariable("PG_PASSWORD") ?? ""}";
builder.Services.AddDbContext<WebhookRelayContext>((options) => options.UseNpgsql(connectionString));

var app = builder.Build();
app.UseAuthorization();
app.UseSentryTracing();
app.MapControllers();

using var scope = app.Services.CreateScope();
using var dataContext = scope.ServiceProvider.GetRequiredService<WebhookRelayContext>();
dataContext.Database.Migrate();

app.Run();
