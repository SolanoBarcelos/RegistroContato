using CoreContato.DTOs;
using CoreContato.Models;

namespace GetContato.Intefaces
{
    public interface IContatoService
    {
        IEnumerable<Contato> GetAllContatos();
        Contato GetContatoById(int id);
        IEnumerable<Contato> GetContatosByDDD(string ddd);

    }
}
