using System.Data;
using Core.Base.Contracts;
using Core.Base.Logging;
using Core.Configuration.MetricsPrometheus;
using Dapper;
using MassTransit;
using Npgsql;

namespace AddContatoConsumer.Eventos
{
    public class AdicionaContatoConsumer : IConsumer<ContatoDTO>
    {
        private readonly string? _connectionString;
        private readonly LoggerService _loggerService;

        public AdicionaContatoConsumer(IConfiguration configuration, LoggerService loggerService)
        {
            _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            _loggerService = loggerService;
        }

        public async Task Consume(ConsumeContext<ContatoDTO> context)
        {
            var contato = context.Message;
            _loggerService.LogInfo($"Recebendo mensagem: {contato.nome_contato}, {contato.telefone_contato}, {contato.email_contato}");

            using IDbConnection db = new NpgsqlConnection(_connectionString);

            try
            {
                string sql = @"
                INSERT INTO contatos (nome_contato, telefone_contato, email_contato) 
                VALUES (@nome_contato, @telefone_contato, @email_contato);";

                await db.ExecuteAsync(sql, contato);
                MetricsConfiguration.IncrementMessagesProcessed();
                _loggerService.LogInfo("Contato salvo no banco de dados com sucesso.");
            }
            catch (Exception ex)
            {
                MetricsConfiguration.IncrementMessagesFailed();
                _loggerService.LogError($"Erro ao processar mensagem: {ex.Message}");
            }
        }
    }
}
