using Microsoft.AspNetCore.Builder;
using AddContatoConsumer.Eventos;
using Core.Base.Logging;
using Core.Base.Utils.Validate;
using Core.Configuration.WebApp;
using Core.Configuration.Database;
using Core.Configuration.MassTransit;
using Core.Configuration.MetricsPrometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<LoggerService>();
builder.Services.AddScoped<ContatoValidateService>();

WebAppConfiguration.ConfigureServices(builder.Services);
DatabaseConfiguration.ConfigureDatabase(builder);
MassTransitConfiguration.ConfigureMassTransit<AdicionaContatoConsumer>(builder.Services, "RABBITMQ_ADD_CONTATO");

var app = builder.Build();

MetricsConfiguration.ConfigureMetrics(app);
WebAppConfiguration.ConfigureApp(app);

app.Run();
