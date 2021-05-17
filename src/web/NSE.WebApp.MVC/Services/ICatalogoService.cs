using NSE.WebApp.MVC.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
{
    public interface ICatalogoService
    {
        Task<ProdutoViewModel> GetById(Guid id);
        Task<IEnumerable<ProdutoViewModel>> GetAll();
    }

    public interface ICatalogoServiceRefit
    {
        [Get("/catalogo/produtos/")]
        Task<IEnumerable<ProdutoViewModel>> ObterTodos();

        [Get("/catalogo/produtos/{id}")]
        Task<ProdutoViewModel> ObterPorId(Guid id);
    }
}
