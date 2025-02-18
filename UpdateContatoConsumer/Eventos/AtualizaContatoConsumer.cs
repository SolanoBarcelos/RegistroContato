using System.Data;
using System.Threading.Tasks;
using Dapper;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Npgsql;
using CoreContato.DTOs;
using CoreContato.Service;

namespace UpdateContatoConsumer.Eventos
{
    public class AtualizaContatoConsumer : IConsumer<ContatoDTO>
    {
        private readonly string _connectionString;
        private readonly LoggerService _loggerService;
        private readonly ContatoValidateService _contatoValidateService;

        public AtualizaContatoConsumer(IConfiguration configuration, LoggerService loggerService, ContatoValidateService contatoValidateService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _loggerService = loggerService;
            _contatoValidateService = contatoValidateService;
        }

        private async Task<ContatoDTO> GetContatoById(int id_contato)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            return await db.QueryFirstOrDefaultAsync<ContatoDTO>("SELECT * FROM contatos WHERE id_contato = @id_contato", new { id_contato });
        }

        public async Task Consume(ConsumeContext<ContatoDTO> context)
        {
            var contato = context.Message;
            _loggerService.LogInfo($"Recebendo atualização: {contato.id_contato}, {contato.nome_contato}, {contato.telefone_contato}, {contato.email_contato}");

            try
            {
                var validaContato = await GetContatoById(contato.id_contato.Value);

                if (validaContato == null)
                {
                    throw new Exception("Contato não encontrado");
                }

                if (!string.IsNullOrEmpty(contato.nome_contato))
                {
                    validaContato.nome_contato = contato.nome_contato;
                }

                if (!string.IsNullOrEmpty(contato.telefone_contato))
                {
                    validaContato.telefone_contato = contato.telefone_contato;
                }

                if (!string.IsNullOrEmpty(contato.email_contato))
                {
                    validaContato.email_contato = contato.email_contato;
                }

                _contatoValidateService.ValidateContato(validaContato, isPartialUpdate: true);

                string sql = @"UPDATE contatos SET 
                                nome_contato = COALESCE(@nome_contato, nome_contato), 
                                telefone_contato = COALESCE(@telefone_contato, telefone_contato), 
                                email_contato = COALESCE(@email_contato, email_contato) 
                              WHERE id_contato = @id_contato;";

                using IDbConnection db = new NpgsqlConnection(_connectionString);
                int rowsAffected = await db.ExecuteAsync(sql, validaContato);

                if (rowsAffected > 0)
                    _loggerService.LogInfo("Contato atualizado com sucesso.");
                else
                    _loggerService.LogWarning("Nenhum contato atualizado. Verifique se o ID é válido.");
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Erro ao atualizar contato: {ex.Message}");
            }
        }
    }
}

