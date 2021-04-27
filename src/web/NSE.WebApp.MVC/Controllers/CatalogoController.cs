﻿using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Services;
using System;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Controllers
{
    public class CatalogoController : Controller
    {
        private ICatalogoServiceRefit _catalogoService;

        public CatalogoController(ICatalogoServiceRefit catalogoService)
        {
            _catalogoService = catalogoService;
        }

        [HttpGet]
        [Route("")]
        [Route("vitrine")]
        public async Task<IActionResult> Index()
        {
            var produtos = await _catalogoService.ObterTodos();
            return View(produtos);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Detalhe(Guid id)
        {
            var produto = await _catalogoService.ObterPorId(id);
            return View(produto);
        }
    }
}
