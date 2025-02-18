using CoreContato.DTOs;
using CoreContato.Service;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus; // Importa para uso do Prometheus

var builder = WebApplication.CreateBuilder(args);

// Configura��o de servi�os
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configura��o de middlewares e roteamento
ConfigureApp(app);

app.Run();

// M�todos auxiliares para organiza��o do c�digo
void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    services.AddScoped<LoggerService>();
    services.AddScoped<ContatoValidateService>();

    var massTransitConfig = configuration.GetSection("MassTransit");

    services.AddMassTransit(x =>
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(massTransitConfig["Servidor"], "/", h =>
            {
                h.Username(massTransitConfig["Usuario"]);
                h.Password(massTransitConfig["Senha"]);
            });

            // Defini��o expl�cita da fila para Update Contato
            cfg.ReceiveEndpoint(massTransitConfig["NomeFila"], e => { });
        });
    });

    services.AddMassTransitHostedService();
}

void ConfigureApp(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Configura a URL e a porta para ambiente de produ��o (Docker)
    if (app.Environment.IsProduction())
    {
        app.Urls.Add("http://0.0.0.0:7075");
    }

    // Exposi��o do endpoint /metrics para o Prometheus
    app.UseMetricServer();
    app.UseHttpMetrics();

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
}
