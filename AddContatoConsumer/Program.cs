using CoreContato.Service;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus; // Necess�rio para expor as m�tricas
using AddContatoConsumer.Eventos;
using AddContatoConsumer.Eventos;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Adicionando os servi�os necess�rios
builder.Services.AddScoped<LoggerService>();
builder.Services.AddScoped<ContatoValidateService>();

// Configura��o de MassTransit
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

// Configura��o do Prometheus
var app = builder.Build();
app.UseMetricServer(); // Exposi��o do endpoint /metrics para o Prometheus
app.UseHttpMetrics(); // M�tricas HTTP autom�ticas

// Configura��o para o ambiente de produ��o
if (app.Environment.IsProduction())
{
    app.Urls.Add("http://0.0.0.0:7070");
}

app.Run();
