using CoreContato.Service;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus; // Necessário para expor as métricas
using AddContatoConsumer.Eventos;
using AddContatoConsumer.Eventos;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Adicionando os serviços necessários
builder.Services.AddScoped<LoggerService>();
builder.Services.AddScoped<ContatoValidateService>();

// Configuração de MassTransit
var massTransitConfig = configuration.GetSection("MassTransit");

builder.Services.AddMassTransit(mt =>
{
    mt.AddConsumer<AdicionaContatoConsumer>(); // Registra o consumidor

    mt.UsingRabbitMq((context, cfg) =>
    {
        // Configuração do RabbitMQ usando as credenciais do appsettings
        cfg.Host(massTransitConfig["Servidor"], "/", h =>
        {
            h.Username(massTransitConfig["Usuario"]);
            h.Password(massTransitConfig["Senha"]);
        });

        // Configurando o endpoint de recebimento de mensagens
        var nomeFila = massTransitConfig["NomeFila"];
        if (string.IsNullOrEmpty(nomeFila))
        {
            throw new Exception("A chave 'MassTransit:NomeFila' não está configurada corretamente.");
        }

        // Recebendo mensagens da fila configurada no appsettings.json
        cfg.ReceiveEndpoint(nomeFila, e =>
        {
            e.ConfigureConsumer<AdicionaContatoConsumer>(context);
        });
    });
});

// Configuração do Prometheus
var app = builder.Build();
app.UseMetricServer(); // Exposição do endpoint /metrics para o Prometheus
app.UseHttpMetrics(); // Métricas HTTP automáticas

// Configuração para o ambiente de produção
if (app.Environment.IsProduction())
{
    app.Urls.Add("http://0.0.0.0:7071");
}

app.Run();
