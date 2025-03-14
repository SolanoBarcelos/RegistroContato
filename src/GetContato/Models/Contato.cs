using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;
using KeyAttribute = Dapper.Contrib.Extensions.KeyAttribute;

namespace GetContato.Models
{
    [Table("contatos")]
    public class Contato
    {
        [Key]
        [Column("id_contato")]
        public int id_contato { get; set; }

        [Column("nome_contato")]
        public string? nome_contato { get; set; }

        [Column("telefone_contato")]
        public string? telefone_contato { get; set; }

        [Column("email_contato")]
        public string? email_contato { get; set; }

    }
}