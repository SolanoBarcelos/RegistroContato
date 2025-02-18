using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Prometheus; // <-- Importante para UseMetricServer e UseHttpMetrics
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração da conexão com o PostgreSQL
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new NpgsqlConnection(connectionString);
});

// 2. Adiciona suporte a controladores
builder.Services.AddControllers();

// 3. Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. Configuração do Prometheus:
//    - Instrumentar o uso de HttpClient (se usar HttpClient no código)
builder.Services.UseHttpClientMetrics();

// 5. Adiciona suporte a autorização
builder.Services.AddAuthorization();

var app = builder.Build();

// 6. Configuração do pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// (Opcional) app.UseHttpsRedirection();

// 7. Expor métricas no endpoint /metrics
app.UseMetricServer();   // <--- /metrics

// 8. Medir requisições HTTP recebidas
app.UseHttpMetrics();

app.UseAuthorization();

// 9. Mapeia os controladores
app.MapControllers();

// 10. Configura as portas para ambiente de produção (Docker)
if (app.Environment.IsProduction())
{
    app.Urls.Add("http://0.0.0.0:7072");
}

app.Run();
