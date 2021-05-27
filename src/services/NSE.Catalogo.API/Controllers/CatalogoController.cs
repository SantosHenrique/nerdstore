using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.Catalogo.API.Models;
using NSE.WebApi.Core.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static NSE.WebApi.Core.Identidade.CustomAuthorization;

namespace NSE.Catalogo.API.Controllers
{
    [Authorize]
    [Route("catalogo")]
    [ApiController]
    public class CatalogoController : MainController
    {
        private readonly IProdutoRepository _produtoRepository;
        public CatalogoController(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        [AllowAnonymous]
        [HttpGet("produtos")]
        public async Task<IEnumerable<Produto>> Index()
        {
            return await _produtoRepository.ObterTodos();
        }

        [ClaimsAuthorize("Catalogo","Ler")]
        [HttpGet("produtos/{id}")]
        public async Task<Produto> Detalhe(Guid id)
        {
            return await _produtoRepository.ObterId(id);
        }
    }
}
