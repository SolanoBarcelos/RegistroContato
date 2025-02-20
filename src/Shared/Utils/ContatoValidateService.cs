using Shared.Contracts;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace Shared.Utils
{
    public class ContatoValidateService
    {

        public void ValidateContato(ContatoDTO contato, bool isPartialUpdate = false)
        {
            List<string> erros = new List<string>();

            if (!isPartialUpdate || !string.IsNullOrEmpty(contato.nome_contato))
            {
                if (string.IsNullOrEmpty(contato.nome_contato))
                {
                    erros.Add("Nome do contato é obrigatório.");
                }
            }

            if (!isPartialUpdate || !string.IsNullOrEmpty(contato.email_contato))
            {
                if (!IsValidEmail(contato.email_contato ?? ""))
                {
                    erros.Add("E-mail inválido.");
                }
            }

            if (!isPartialUpdate || !string.IsNullOrEmpty(contato.telefone_contato))
            {
                if (!IsValidTelefone(contato.telefone_contato ?? ""))
                {
                    erros.Add("Telefone inválido.");
                }
            }

            if (erros.Count > 0)
            {
                throw new ArgumentException(string.Join(" | ", erros));
            }
        }



        private bool IsValidEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && email.Contains("@");
        }

        private bool IsValidTelefone(string telefone)
        {
            var regex = new Regex(@"^\d{11}$");
            return regex.IsMatch(telefone);
        }
    }

}
