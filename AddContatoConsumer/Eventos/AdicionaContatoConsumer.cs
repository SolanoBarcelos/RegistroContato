using System.Data;
using System.Threading.Tasks;
using Dapper;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Npgsql;
using CoreContato.DTOs;
using CoreContato.Service;

namespace AddContatoConsumer.Eventos
{
    public class AdicionaContatoConsumer : IConsumer<ContatoDTO>
    {
        private readonly string _connectionString;
        private readonly LoggerService _loggerService;

        public AdicionaContatoConsumer(IConfiguration configuration, LoggerService loggerService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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
                _loggerService.LogInfo("Contato salvo no banco de dados com sucesso.");
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Erro ao processar mensagem: {ex.Message}");
            }
        }
    }
}
