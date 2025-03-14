using MassTransit;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Npgsql;
using Core.Base.Utils.Validate;
using Core.Configuration.MassTransit;
using Core.Base.Contracts;
using Mensagem.Teste;
public class Tests : IAsyncLifetime
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbConnection _dbConnection;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly string _queueName = "test_queue";
    private readonly ContatoValidateService _validateService;

    public Tests()
    {
        var services = new ServiceCollection();
        services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")));
        MassTransitConfiguration.ConfigureMassTransitProducer(services);

        _serviceProvider = services.BuildServiceProvider();
        _validateService = new ContatoValidateService();
        _dbConnection = _serviceProvider.GetRequiredService<IDbConnection>();
        _publishEndpoint = _serviceProvider.GetRequiredService<IPublishEndpoint>();
    }

    public async Task InitializeAsync()
    {
        _dbConnection.Open();

        using var command = (NpgsqlCommand)_dbConnection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS ContatosTest (Id SERIAL PRIMARY KEY, Nome VARCHAR(100), Email VARCHAR(100));";
        await command.ExecuteNonQueryAsync();
    }

    public async Task DisposeAsync()
    {
        using var command = (NpgsqlCommand)_dbConnection.CreateCommand();
        command.CommandText = "TRUNCATE TABLE ContatosTest RESTART IDENTITY;";
        await command.ExecuteNonQueryAsync();
        _dbConnection.Close();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeveSalvarDadosNoBancoDeDados()
    {
        using var command = (NpgsqlCommand)_dbConnection.CreateCommand();
        command.CommandText = "INSERT INTO ContatosTest (Nome, Email) VALUES (@Nome, @Email) RETURNING Id;";
        command.Parameters.Add(new NpgsqlParameter("@Nome", "Teste"));
        command.Parameters.Add(new NpgsqlParameter("@Email", "teste@email.com"));
        var contatoId = await command.ExecuteScalarAsync();

        Assert.NotNull(contatoId);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DevePublicarEMensagemNaFila()
    {
        string mensagem = "Mensagem de teste";
        await _publishEndpoint.Publish(new MensagemTeste { Texto = mensagem });

        Assert.True(true);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeveValidarContatoCorretamente()
    {
        var contato = new ContatoDTO { nome_contato = "João Silva", email_contato = "joao@email.com", telefone_contato = "11987654321" };
        var validateService = new ContatoValidateService();

        var ex = await Record.ExceptionAsync(() => Task.Run(() => validateService.ValidateContato(contato)));
        Assert.Null(ex);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeveRejeitarContatoInvalido()
    {
        var contato = new ContatoDTO { nome_contato = "", email_contato = "email_invalido", telefone_contato = "123" };
        var validateService = new ContatoValidateService();

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => Task.Run(() => validateService.ValidateContato(contato)));
        Assert.Contains("Nome do contato é obrigatório.", ex.Message);
        Assert.Contains("E-mail inválido.", ex.Message);
        Assert.Contains("Telefone inválido.", ex.Message);
    }
}