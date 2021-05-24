using NSE.Core.DomainObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSE.Cliente.API.Models
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        void Adicionar(Models.Cliente cliente);

        Task<IEnumerable<Cliente>> ObterTodos();
        Task<Cliente> ObterPorCpf(string cpf);
    }
}
