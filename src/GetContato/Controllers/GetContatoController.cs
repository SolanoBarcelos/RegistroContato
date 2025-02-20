using GetContato.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace GetContato.Controllers
{
    [ApiController]
    [Route("/GetContato")]
    public class ContatoController : ControllerBase
    {
        private readonly IContatoService _contatoService;

        public ContatoController(IContatoService contatoService)
        {
            _contatoService = contatoService;
        }

        // Verifica se a API está no ar
        [HttpGet("up")]
        public IActionResult Up()
        {
            return Ok("API is running");
        }

        // Retorna todos os contatos
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            return Ok(_contatoService.GetAllContatos());
        }

        // Retorna um contato específico pelo ID
        [HttpGet("GetById/{id_contato}")]
        public IActionResult GetById(int id_contato)
        {
            var contato = _contatoService.GetContatoById(id_contato);
            if (contato == null)
                return NotFound();

            return Ok(contato);
        }

        // Retorna todos os contatos pelo DDD informado
        [HttpGet("GetContatosByDDD/{ddd}")]
        public IActionResult GetByDDD(string ddd)
        {
            return Ok(_contatoService.GetContatosByDDD(ddd));
        }
    }
}
