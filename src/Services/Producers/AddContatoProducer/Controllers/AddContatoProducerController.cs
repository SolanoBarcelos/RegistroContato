using CoreContato.DTOs;
using CoreContato.Service;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/AddContato")]
public class AddContatoProducerController : ControllerBase
{
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly LoggerService _loggerService;
    private readonly ContatoValidateService _contatoValidateService;
    private readonly IConfiguration _configuration;

    public AddContatoProducerController(
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

    [HttpGet("upAddContato")]
    public IActionResult Up()
    {
        return Ok("API is running");
    }

    [HttpPost("AddContatoProducer")]
    public async Task<IActionResult> PostContatoProducer([FromBody] ContatoDTO contato)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { mensagem = "Erro de validação", detalhes = "Campos inválidos" });
        }

        try
        {
            _contatoValidateService.ValidateContato(contato);
        }
        catch (ArgumentException ex)
        {
            _loggerService.LogError($"Erro de validação: {ex.Message}");
            return BadRequest(new { mensagem = "Erro de validação", detalhes = ex.Message });
        }

        try
        {
            // Leitura do nome da fila a partir da configuração
            var nomeFila = _configuration["MassTransit:NomeFila"];

            // Verifique se o nome da fila foi obtido corretamente
            if (string.IsNullOrEmpty(nomeFila))
            {
                return BadRequest(new { mensagem = "Nome da fila não configurado", detalhes = "A chave 'MassTransit:NomeFila' não foi encontrada." });
            }

            // Criação da URI para o endpoint da fila
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{nomeFila}"));

            // Envio da mensagem
            await sendEndpoint.Send(contato);

            _loggerService.LogInfo($"Mensagem enviada para a fila {nomeFila}.");
            return Accepted(new { mensagem = "Mensagem enviada para processamento" });
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"Erro ao enviar mensagem para a fila: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno ao enviar a mensagem", detalhes = ex.Message });
        }
    }

}
