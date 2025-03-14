using GetContato.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace GetContato.Controllers
{
    [ApiController]
    [Route("/contatos")]
    public class ContatoController : ControllerBase
    {
        private readonly IContatoService _contatoService;

        public ContatoController(IContatoService contatoService)
        {
            _contatoService = contatoService;
        }

        [HttpGet("up")]
        public IActionResult Up()
        {
            return Ok("API is running");
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_contatoService.GetAllContatos());
        }

        [HttpGet("{id_contato}")]
        public IActionResult GetById(int id_contato)
        {
            var contato = _contatoService.GetContatoById(id_contato);
            if (contato == null)
                return NotFound();

            return Ok(contato);
        }

        [HttpGet("ddd/{ddd}")]
        public IActionResult GetByDDD(string ddd)
        {
            return Ok(_contatoService.GetContatosByDDD(ddd));
        }
    }
}
