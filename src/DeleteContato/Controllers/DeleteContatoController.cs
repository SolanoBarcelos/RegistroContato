using Microsoft.AspNetCore.Mvc;
using Dapper.Contrib.Extensions;
using System.Data;
using DeleteContato.Models;

namespace DeleteContato.Controllers
{
    [ApiController]
    [Route("/contatos")]
    public class DeleteContatoController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public DeleteContatoController(IDbConnection connection)
        {
            _connection = connection;
        }

        [HttpGet("up")]
        public IActionResult Up()
        {
            return Ok("API is running");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var contato = _connection.Get<Contato>(id);
            if (contato == null)
                return NotFound(new { message = "Contato não encontrado." });

            bool result = _connection.Delete(contato);
            if (!result)
                return StatusCode(500, new { message = "Erro ao tentar deletar o contato." });

            return Ok(new { message = $"Contato {id} deletado com sucesso." });
        }
    }
}