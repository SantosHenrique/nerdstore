using NSE.Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSE.Catalogo.API.Models
{
    public interface IProdutoRepository : IRepository<Produto>, IAggregateRoot
    {
        Task<IEnumerable<Produto>> ObterTodos();
        Task<Produto> ObterId(Guid id);

        void Adicionar(Produto produto);
        void Atualizar(Produto produto);
    }
}
