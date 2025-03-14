using Dapper;
using Dapper.Contrib.Extensions;
using GetContato.Models;
using System.Data;
using GetContato.Intefaces;

namespace GetContato.Repository
{
    public class ContatoRepository : IContatoRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<ContatoRepository> _logger;

        public ContatoRepository(IDbConnection dbConnection, ILogger<ContatoRepository> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public IEnumerable<Contato> GetAll()
        {
            try
            {
                _logger.LogInformation("Buscando todos os contatos.");
                return _dbConnection.GetAll<Contato>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar contatos: {ex.Message}");
                return new List<Contato>();
            }
        }

        public Contato GetById(int id_contato)
        {
            try
            {
                _logger.LogInformation($"Buscando contato com ID {id_contato}");
                var query = "SELECT * FROM contatos WHERE id_contato = @id_contato";
                return _dbConnection.QuerySingleOrDefault<Contato>(query, new { id_contato });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar contato por ID: {ex.Message}");
                return null;
            }
        }

        public IEnumerable<Contato> GetByDDD(string ddd)
        {
            try
            {
                _logger.LogInformation($"Buscando contatos com DDD {ddd}");
                var query = "SELECT * FROM contatos WHERE telefone_contato LIKE CONCAT(@DDD, '%')";
                return _dbConnection.Query<Contato>(query, new { DDD = ddd });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar contatos por DDD: {ex.Message}");
                return new List<Contato>();
            }
        }
    }
}
