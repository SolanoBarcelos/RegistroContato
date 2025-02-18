using CoreContato.DTOs;
using CoreContato.Service;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using AddContatoConsumer;
using AddContatoConsumer.Eventos;

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


    builder.Services.AddMassTransit(mt =>
    {
        mt.AddConsumer<AdicionaContatoConsumer>(); // Registra o consumidor

        mt.UsingRabbitMq((context, cfg) =>
        {
            // Configura��o do RabbitMQ usando as credenciais do appsettings
            cfg.Host(massTransitConfig["Servidor"], "/", h =>
            {
                h.Username(massTransitConfig["Usuario"]);
                h.Password(massTransitConfig["Senha"]);
            });

            // Configurando o endpoint de recebimento de mensagens
            var nomeFila = massTransitConfig["NomeFila"];
            if (string.IsNullOrEmpty(nomeFila))
            {
                throw new Exception("A chave 'MassTransit:NomeFila' n�o est� configurada corretamente.");
            }

            // Recebendo mensagens da fila configurada no appsettings.json
            cfg.ReceiveEndpoint(nomeFila, e =>
            {
                e.ConfigureConsumer<AdicionaContatoConsumer>(context);
            });
        });
    });
}

void ConfigureApp(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Configura��o da URL em ambiente de produ��o (para Docker, por exemplo)
    if (app.Environment.IsProduction())
    {
        app.Urls.Add("http://0.0.0.0:7071");
    }

    // Adiciona o endpoint /metrics (Prometheus)
    app.UseMetricServer();

    // (Opcional) Adiciona m�tricas de requisi��es HTTP automaticamente
    app.UseHttpMetrics();

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
}
