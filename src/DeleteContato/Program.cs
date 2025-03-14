using Core.Configuration.Database;
using Core.Configuration.MetricsPrometheus;
using Core.Configuration.WebApp;
using Core.Base.Logging;
using Npgsql;
using Prometheus;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

WebAppConfiguration.ConfigureServices(builder.Services);
DatabaseConfiguration.ConfigureDatabase(builder);

builder.Services.AddScoped<LoggerService>();

var app = builder.Build();

MetricsConfiguration.ConfigureMetrics(app);
WebAppConfiguration.ConfigureApp(app);

app.Run();