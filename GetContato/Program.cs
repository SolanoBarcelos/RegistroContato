using GetContato.Intefaces;
using GetContato.Repository;
using GetContato.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Prometheus;
using System.Data;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuração da conexão com o PostgreSQL
        builder.Services.AddScoped<IDbConnection>(sp =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            return new NpgsqlConnection(connectionString);
        });

        // Registro de dependências
        builder.Services.AddScoped<IContatoRepository, ContatoRepository>();
        builder.Services.AddScoped<IContatoService, ContatoService>();

        // Adiciona suporte a controladores
        builder.Services.AddControllers();

        // Configuração do Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configuração do Prometheus (para instrumentar HttpClient, se utilizado)
        builder.Services.UseHttpClientMetrics();

        // Adiciona suporte a autorização
        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configuração do pipeline de requisições HTTP
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();
        app.UseMetricServer();  // Exposição do endpoint /metrics para o Prometheus
        app.UseHttpMetrics();   // Instrumenta as requisições HTTP
        app.UseAuthorization();

        // Mapeia os controladores
        app.MapControllers();

        // Configura a URL e a porta para ambiente de produção (exemplo Docker)
        if (app.Environment.IsProduction())
        {
            app.Urls.Add("http://0.0.0.0:7073");
        }

        app.Run();
    }
}
