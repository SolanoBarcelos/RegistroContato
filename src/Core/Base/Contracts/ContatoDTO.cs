using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Base.Contracts
{
    public class ContatoDTO
    {
        public int? id_contato { get; set; }
        public string? nome_contato { get; set; }
        public string? telefone_contato { get; set; }
        public string? email_contato { get; set; }

        public ContatoDTO() { }

        public ContatoDTO(string nome_contato, string telefone_contato, string email_contato)
        {
            this.nome_contato = nome_contato;
            this.telefone_contato = telefone_contato;
            this.email_contato = email_contato;
        }
    }

    
}
