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

        // Configura��o da conex�o com o PostgreSQL
        builder.Services.AddScoped<IDbConnection>(sp =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            return new NpgsqlConnection(connectionString);
        });

        // Registro de depend�ncias
        builder.Services.AddScoped<IContatoRepository, ContatoRepository>();
        builder.Services.AddScoped<IContatoService, ContatoService>();

        // Adiciona suporte a controladores
        builder.Services.AddControllers();

        // Configura��o do Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configura��o do Prometheus (para instrumentar HttpClient, se utilizado)
        builder.Services.UseHttpClientMetrics();

        // Adiciona suporte a autoriza��o
        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configura��o do pipeline de requisi��es HTTP
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();
        app.UseMetricServer();  // Exposi��o do endpoint /metrics para o Prometheus
        app.UseHttpMetrics();   // Instrumenta as requisi��es HTTP
        app.UseAuthorization();

        // Mapeia os controladores
        app.MapControllers();

        // Configura a URL e a porta para ambiente de produ��o (exemplo Docker)
        if (app.Environment.IsProduction())
        {
            app.Urls.Add("http://0.0.0.0:7072");
        }

        app.Run();
    }
}
