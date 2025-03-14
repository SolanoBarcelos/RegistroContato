using Core.Base.Contracts;
using Core.Base.Logging;
using Core.Base.Utils.Validate;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace UpdateContatoProducer.Controllers
{
    [ApiController]
    [Route("/contatos")]
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

        [HttpGet("up")]
        public IActionResult Up()
        {
            return Ok("API is running");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContatoProducer(int id, [FromBody] ContatoDTO contato)
        {
            if (contato == null)
            {
                _loggerService.LogError("Requisição inválida: corpo da requisição está vazio.");
                return BadRequest(new { mensagem = "O corpo da requisição não pode ser vazio." });
            }

            if (!ModelState.IsValid)
            {
                _loggerService.LogError("Erro de validação: Campos inválidos.");
                return BadRequest(new { mensagem = "Erro de validação", detalhes = "Campos inválidos" });
            }

            try
            {
                _contatoValidateService.ValidateContato(contato, isPartialUpdate: false);
            }
            catch (ArgumentException ex)
            {
                _loggerService.LogError($"Erro de validação: {ex.Message}");
                return BadRequest(new { mensagem = "Erro de validação", detalhes = ex.Message });
            }

            try
            {
                var nomeFila = _configuration["RABBITMQ_UPDATE_CONTATO"];
                if (string.IsNullOrEmpty(nomeFila))
                {
                    _loggerService.LogError("Nome da fila não configurado.");
                    return StatusCode(500, new { mensagem = "Erro interno", detalhes = "Nome da fila não está configurado." });
                }

                contato.id_contato = id;
                var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{nomeFila}"));

                await sendEndpoint.Send(contato);

                _loggerService.LogInfo($"Mensagem de atualização enviada para a fila {nomeFila}.");
                return Accepted(new { mensagem = "Mensagem enviada para processamento." });
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Erro ao enviar mensagem para a fila: {ex.Message}");
                return StatusCode(500, new { mensagem = "Erro interno ao enviar a mensagem", detalhes = ex.Message });
            }
        }
    }
}
