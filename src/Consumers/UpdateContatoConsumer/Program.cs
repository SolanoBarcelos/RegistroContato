using Microsoft.AspNetCore.Builder;
using Core.Base.Logging;
using Core.Base.Utils.Validate;
using UpdateContatoConsumer.Eventos;
using Core.Configuration.Database;
using Core.Configuration.MassTransit;
using Core.Configuration.MetricsPrometheus;
using Core.Configuration.WebApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<LoggerService>();
builder.Services.AddScoped<ContatoValidateService>();

WebAppConfiguration.ConfigureServices(builder.Services);
DatabaseConfiguration.ConfigureDatabase(builder);
MassTransitConfiguration.ConfigureMassTransit<AtualizaContatoConsumer>(builder.Services, "RABBITMQ_UPDATE_CONTATO");

var app = builder.Build();

MetricsConfiguration.ConfigureMetrics(app);
WebAppConfiguration.ConfigureApp(app);

app.Run();