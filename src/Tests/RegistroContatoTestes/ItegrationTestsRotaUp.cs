using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RegistroContatoTestes
{
    public class ItegrationTestsRotaUp : IClassFixture<WebApplicationFactory<GetContato.Controllers.ContatoController>>,
                                        IClassFixture<WebApplicationFactory<DeleteContato.Controllers.DeleteContatoController>>
    {
        private readonly HttpClient _clientGetContato;
        private readonly HttpClient _clientDeleteContato;

        public ItegrationTestsRotaUp(WebApplicationFactory<GetContato.Controllers.ContatoController> factoryGetContato,
                                      WebApplicationFactory<DeleteContato.Controllers.DeleteContatoController> factoryDeleteContato)
        {
            _clientGetContato = factoryGetContato.CreateClient();
            _clientDeleteContato = factoryDeleteContato.CreateClient();
        }

        [Fact]
        public async Task TestarRotaUpGetContato_DeveRetornarOk()
        {
            // Act
            var response = await _clientGetContato.GetAsync("/GetContato/up");

            // Assert
            response.EnsureSuccessStatusCode(); // Verifica se o código de status é 2xx
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("API is running", content);
        }

        [Fact]
        public async Task TestarRotaUpDeleteContato_DeveRetornarOk()
        {
            // Act
            var response = await _clientDeleteContato.GetAsync("/deleteContato/up");

            // Assert
            response.EnsureSuccessStatusCode(); // Verifica se o código de status é 2xx
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("API is running", content);
        }
    }
}
