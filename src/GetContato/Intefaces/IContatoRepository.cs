using GetContato.Models;

namespace GetContato.Intefaces
{
    public interface IContatoRepository
    {
        IEnumerable<Contato> GetAll();
        Contato GetById(int id_contato);
        IEnumerable<Contato> GetByDDD(string ddd);

    }
}
