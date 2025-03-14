using Core.Base.Utils.Env;
using Core.Configuration.MetricsPrometheus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Configuration.WebApp
{
    public static class WebAppConfiguration
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddAuthorization();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });
        }
        public static void ConfigureApp(WebApplication app)
        {
            var environment = EnvUtils.GetEnv("ASPNETCORE_ENVIRONMENT");
            Console.WriteLine($"Rodando no ambiente: {environment}");

            if (environment == "Development")
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            MetricsConfiguration.ConfigureMetrics(app);

            app.MapControllers();


        }
    }
}