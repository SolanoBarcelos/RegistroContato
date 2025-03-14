using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Core.Base.Utils.Env;
using Microsoft.AspNetCore.Builder;

namespace Core.Configuration.MassTransit
{
    public static class MassTransitConfiguration
    {
        public static void ConfigureMassTransit<TConsumer>(IServiceCollection services, string filaEnv)
            where TConsumer : class, IConsumer
        {
            var rabbitConfig = new
            {
                Servidor = EnvUtils.GetEnv("RABBITMQ_HOST"),
                Usuario = EnvUtils.GetEnv("RABBITMQ_USER"),
                Senha = EnvUtils.GetEnv("RABBITMQ_PASS"),
                NomeFila = EnvUtils.GetEnv(filaEnv, "DefaultQueue")
            };

            services.AddMassTransit(mt =>
            {
                mt.AddConsumer<TConsumer>();
                mt.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitConfig.Servidor, "/", h =>
                    {
                        h.Username(rabbitConfig.Usuario);
                        h.Password(rabbitConfig.Senha);
                    });

                    cfg.ReceiveEndpoint(rabbitConfig.NomeFila, e =>
                    {
                        e.ConfigureConsumer<TConsumer>(context);
                    });
                });
            });

        }

        public static void ConfigureMassTransitProducer(IServiceCollection services)
        {
            var rabbitConfig = new
            {
                Servidor = EnvUtils.GetEnv("RABBITMQ_HOST"),
                Usuario = EnvUtils.GetEnv("RABBITMQ_USER"),
                Senha = EnvUtils.GetEnv("RABBITMQ_PASS")
            };

            services.AddMassTransit(mt =>
            {
                mt.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitConfig.Servidor, "/", h =>
                    {
                        h.Username(rabbitConfig.Usuario);
                        h.Password(rabbitConfig.Senha);
                    });
                });
            });
        }
    }
}