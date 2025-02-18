using CoreContato.DTOs;
using CoreContato.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using GetContato.Intefaces;
using System.Data;
using System.Data.Common;

namespace GetContato.Repository
{
    public class ContatoRepository : IContatoRepository
    {

            private readonly IDbConnection _dbConnection;

            public ContatoRepository(IDbConnection dbconnection)
            {
                _dbConnection = dbconnection;
            }

            public IEnumerable<Contato> GetAll()
            {
                return _dbConnection.GetAll<Contato>();
            }

            public Contato GetById(int id_contato)
            {
                //return _connection.Get<Contato>(id_contato);
                var query = "SELECT * FROM Contatos WHERE id_contato = @id_contato";
                return _dbConnection.QuerySingleOrDefault<Contato>(query, new { id_contato });
            }

            public IEnumerable<Contato> GetByDDD(string ddd)
            {
                var query = "SELECT * FROM contatos WHERE telefone_contato LIKE @DDD";
                return _dbConnection.Query<Contato>(query, new { DDD = $"{ddd}%" });
            }
    }
}

