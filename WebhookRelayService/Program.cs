using Microsoft.EntityFrameworkCore;
using Sentry;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using WebhookRelayService;
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

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));

// Add repositories and services
builder.Services.AddSingleton(settings);
builder.Services.AddTransient<IWebhookService, WebhookService>();
builder.Services.AddTransient<IWebhookUserService, WebhookUserService>();
builder.Services.AddTransient<IWebhookUserRepository, WebhookUserRepository>();

builder.Services.AddDbContext<WebhookRelayContext>((options) => options.UseNpgsql(settings.PostgresConnection));

var app = builder.Build();
app.UseAuthorization();
app.UseSentryTracing();
app.MapControllers();

using var scope = app.Services.CreateScope();
using var dataContext = scope.ServiceProvider.GetRequiredService<WebhookRelayContext>();
dataContext.Database.Migrate();

app.Run();
