using Microsoft.AspNetCore.Mvc;
using Dapper.Contrib.Extensions;
using System.Data;
using CoreContato.Models;

namespace DeleteContato.Controllers
{
    [ApiController]
    [Route("/deleteContato")]
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

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            // Cria o objeto com o Id e usa o Dapper.Contrib para deletar diretamente
            var contato = new Contato { id_contato = id };
            bool result = _connection.Delete(contato);

            if (result)
                return Ok(new { message = "Contato deletado com sucesso." });

            return NotFound(new { message = "Contato não encontrado." });
        }
    }
}