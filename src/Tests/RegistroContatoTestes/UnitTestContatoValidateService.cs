using Xunit;
using CoreContato.DTOs;
using System;

namespace RegistroContatoTestes
{
    public class UnitTestContatoValidateService
    {
        private readonly ContatoValidateService _contatoValidateService;

        public UnitTestContatoValidateService()
        {
            _contatoValidateService = new ContatoValidateService();
        }

        [Fact]
        public void ValidarContato_ContatoValido_NaoDeveLancarExcecao()
        {
            // Arrange
            var contato = new ContatoDTO
            {
                nome_contato = "João Silva",
                email_contato = "joao.silva@exemplo.com",
                telefone_contato = "12345678901"
            };

            // Act & Assert
            _contatoValidateService.ValidateContato(contato);
        }

        [Fact]
        public void ValidarContato_EmailInvalido_LancaExcecao()
        {
            // Arrange
            var contato = new ContatoDTO
            {
                nome_contato = "João Silva",
                email_contato = "email-invalido",
                telefone_contato = "12345678901"
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _contatoValidateService.ValidateContato(contato));
            Assert.Contains("E-mail inválido", exception.Message);
        }

        [Fact]
        public void ValidarContato_TelefoneInvalido_LancaExcecao()
        {
            // Arrange
            var contato = new ContatoDTO
            {
                nome_contato = "João Silva",
                email_contato = "joao.silva@exemplo.com",
                telefone_contato = "12345"
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _contatoValidateService.ValidateContato(contato));
            Assert.Contains("Telefone inválido", exception.Message);
        }

        [Fact]
        public void ValidarContato_NomeFaltando_LancaExcecao()
        {
            // Arrange
            var contato = new ContatoDTO
            {
                nome_contato = null,
                email_contato = "joao.silva@exemplo.com",
                telefone_contato = "12345678901"
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _contatoValidateService.ValidateContato(contato));
            Assert.Contains("Nome do contato é obrigatório", exception.Message);
        }

        [Fact]
        public void ValidarContato_TelefoneVazio_ValidoQuandoAtualizacaoParcial()
        {
            // Arrange
            var contato = new ContatoDTO
            {
                nome_contato = "João Silva",
                email_contato = "joao.silva@exemplo.com",
                telefone_contato = "",
            };

            // Act & Assert
            _contatoValidateService.ValidateContato(contato, isPartialUpdate: true);
        }
    }
}
