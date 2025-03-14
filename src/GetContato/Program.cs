using Core.Configuration.Database;
using Core.Configuration.MetricsPrometheus;
using Core.Configuration.WebApp;
using Core.Base.Logging;
using GetContato.Intefaces;
using GetContato.Repository;
using GetContato.Service;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        WebAppConfiguration.ConfigureServices(builder.Services);
        DatabaseConfiguration.ConfigureDatabase(builder);

        builder.Services.AddScoped<IContatoRepository, ContatoRepository>();
        builder.Services.AddScoped<IContatoService, ContatoService>();

        builder.Services.AddScoped<LoggerService>();

        var app = builder.Build();

        MetricsConfiguration.ConfigureMetrics(app);
        WebAppConfiguration.ConfigureApp(app);

        app.Run();
    }
}
