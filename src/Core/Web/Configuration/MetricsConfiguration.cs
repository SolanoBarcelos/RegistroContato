using Microsoft.AspNetCore.Builder;
using Prometheus;
using Core.Base.Utils.Env;

namespace Core.Configuration.MetricsPrometheus
{
    public static class MetricsConfiguration
    {
        private static readonly Counter MessagesProcessed = Metrics.CreateCounter(
            "messages_processed_total",
            "Número total de mensagens processadas nos Consumers."
        );

        private static readonly Counter MessagesFailed = Metrics.CreateCounter(
            "messages_failed_total",
            "Número total de mensagens que falharam no processamento."
        );

        public static void ConfigureMetrics(WebApplication app)
        {
            if (EnvUtils.GetEnvBool("ENABLE_METRICS"))
            {
                app.UseMetricServer();
                app.UseHttpMetrics();
                Console.WriteLine("Métricas Prometheus ativadas.");
            }
        }

        public static void IncrementMessagesProcessed() => MessagesProcessed.Inc();

        public static void IncrementMessagesFailed() => MessagesFailed.Inc();
    }
}
