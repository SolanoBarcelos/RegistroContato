using Core.Base.Contracts;
using Core.Base.Logging;
using Core.Configuration.MetricsPrometheus;
using Core.Base.Utils.Validate;
using Dapper;
using MassTransit;
using Npgsql;


namespace UpdateContatoConsumer.Eventos
{
    public class AtualizaContatoConsumer : IConsumer<ContatoDTO>
    {
        private readonly string? _connectionString;
        private readonly LoggerService _loggerService;
        private readonly ContatoValidateService _contatoValidateService;

        public AtualizaContatoConsumer(IConfiguration configuration, LoggerService loggerService, ContatoValidateService contatoValidateService)
        {
            _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            _loggerService = loggerService;
            _contatoValidateService = contatoValidateService;
        }

        private async Task<ContatoDTO> GetContatoById(int id_contato)
        {
            using var db = new NpgsqlConnection(_connectionString);
            return await db.QueryFirstOrDefaultAsync<ContatoDTO>("SELECT * FROM contatos WHERE id_contato = @id_contato", new { id_contato });
        }

        public async Task Consume(ConsumeContext<ContatoDTO> context)
        {
            var contato = context.Message;
            _loggerService.LogInfo($"Recebendo atualização: {contato.id_contato}, {contato.nome_contato}, {contato.telefone_contato}, {contato.email_contato}");

            try
            {
                var contatoExistente = await GetContatoById(contato.id_contato.Value);
                if (contatoExistente == null)
                {
                    throw new Exception("Contato não encontrado");
                }

                contatoExistente.nome_contato = string.IsNullOrEmpty(contato.nome_contato) ? contatoExistente.nome_contato : contato.nome_contato;
                contatoExistente.telefone_contato = string.IsNullOrEmpty(contato.telefone_contato) ? contatoExistente.telefone_contato : contato.telefone_contato;
                contatoExistente.email_contato = string.IsNullOrEmpty(contato.email_contato) ? contatoExistente.email_contato : contato.email_contato;

                _contatoValidateService.ValidateContato(contatoExistente, isPartialUpdate: true);

                var sql = @"UPDATE contatos 
                            SET nome_contato = COALESCE(@nome_contato, nome_contato), 
                                telefone_contato = COALESCE(@telefone_contato, telefone_contato), 
                                email_contato = COALESCE(@email_contato, email_contato) 
                            WHERE id_contato = @id_contato;";

                using var db = new NpgsqlConnection(_connectionString);
                int rowsAffected = await db.ExecuteAsync(sql, contatoExistente);

                if (rowsAffected > 0)
                {
                    MetricsConfiguration.IncrementMessagesProcessed();
                    _loggerService.LogInfo("Contato atualizado com sucesso.");
                }
                else
                {
                    _loggerService.LogWarning("Nenhum contato atualizado. Verifique se o ID é válido.");
                }
            }
            catch (Exception ex)
            {
                MetricsConfiguration.IncrementMessagesFailed();
                _loggerService.LogError($"Erro ao atualizar contato: {ex.Message}");
            }
        }
    }
}
