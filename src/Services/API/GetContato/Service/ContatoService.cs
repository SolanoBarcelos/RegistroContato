using CoreContato.DTOs;
using CoreContato.Models;
using GetContato.Intefaces;

namespace GetContato.Service
{
    public class ContatoService : IContatoService
    {
        private readonly IContatoRepository _contatoRepository;

        public ContatoService(IContatoRepository contatoRepository)
        {
            _contatoRepository = contatoRepository;
        }

        public IEnumerable<Contato> GetAllContatos()
        {
            return _contatoRepository.GetAll();
        }

        public Contato GetContatoById(int id_contato)
        {
            return _contatoRepository.GetById(id_contato);
        }

        public IEnumerable<Contato> GetContatosByDDD(string ddd)
        {
            if (string.IsNullOrEmpty(ddd) || ddd.Length != 2)
            {
                throw new ArgumentException("DDD inválido");
            }

            return _contatoRepository.GetByDDD(ddd);
        }


    }
}
