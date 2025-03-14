using Core.Base.Contracts;
using Core.Base.Logging;
using Core.Base.Utils.Validate;
using Core.Configuration.Database;
using Core.Configuration.MassTransit;
using Core.Configuration.MetricsPrometheus;
using Core.Configuration.WebApp;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<LoggerService>();
builder.Services.AddScoped<ContatoValidateService>();

WebAppConfiguration.ConfigureServices(builder.Services);
DatabaseConfiguration.ConfigureDatabase(builder);
MassTransitConfiguration.ConfigureMassTransitProducer(builder.Services);

var app = builder.Build();

MetricsConfiguration.ConfigureMetrics(app);
WebAppConfiguration.ConfigureApp(app);

app.Run();