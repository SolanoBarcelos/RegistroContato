using CoreContato.DTOs;
using CoreContato.Service;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace UpdateContatoProducer.Controllers
{
    [ApiController]
    [Route("/RegistroContato")]
    public class UpdateContatoProducerController : ControllerBase
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly LoggerService _loggerService;
        private readonly ContatoValidateService _contatoValidateService;
        private readonly IConfiguration _configuration;

        public UpdateContatoProducerController(
            ISendEndpointProvider sendEndpointProvider,
            LoggerService loggerService,
            ContatoValidateService contatoValidateService,
            IConfiguration configuration)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _loggerService = loggerService;
            _contatoValidateService = contatoValidateService;
            _configuration = configuration;
        }

        [HttpGet("upUpdateContato")]
        public IActionResult Up()
        {
            return Ok("API is running");
        }

        [HttpPost("UpdateContatoProducer")]
        public async Task<IActionResult> UpdateContatoProducer([FromBody] ContatoDTO contato)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { mensagem = "Erro de validação", detalhes = "Campos inválidos" });
            }

            try
            {
                _contatoValidateService.ValidateContato(contato, isPartialUpdate: true);
            }
            catch (ArgumentException ex)
            {
                _loggerService.LogError($"Erro de validação: {ex.Message}");
                return BadRequest(new { mensagem = "Erro de validação", detalhes = ex.Message });
            }

            try
            {
                var nomeFila = _configuration["MassTransit:NomeFilaUpdateContato"];
                var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{nomeFila}"));
                await sendEndpoint.Send(contato);

                _loggerService.LogInfo($"Mensagem de atualização enviada para a fila {nomeFila}.");
                return Accepted(new { mensagem = "Mensagem enviada para processamento" });
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Erro ao enviar mensagem para a fila: {ex.Message}");
                return StatusCode(500, new { mensagem = "Erro interno ao enviar a mensagem", detalhes = ex.Message });
            }
        }
    }
}
