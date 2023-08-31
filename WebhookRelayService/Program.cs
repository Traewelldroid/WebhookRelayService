using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using System.Diagnostics;
using WebhookRelayService;
using WebhookRelayService.BackgroundServices;
using WebhookRelayService.Models;
using WebhookRelayService.Repositories;
using WebhookRelayService.Services;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("Startup...");

var builder = WebApplication.CreateBuilder(args);
var settings = builder.Configuration.GetSection("WebhookRelaySettings").Get<Settings>();

builder.WebHost.UseSentry(s =>
{
    s.Dsn = settings.SentryDsn;
    s.Debug = Debugger.IsAttached;
    s.TracesSampleRate = 1;
});

builder.Logging.ClearProviders();
builder.Host.UseNLog();

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
