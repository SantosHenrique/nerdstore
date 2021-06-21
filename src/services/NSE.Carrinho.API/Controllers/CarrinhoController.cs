using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSE.Carrinho.API.Data;
using NSE.Carrinho.API.Models;
using NSE.WebApi.Core.Controllers;
using NSE.WebApi.Core.Usuario;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.Carrinho.API.Controllers
{
    [Route("carrinho")]
    [Authorize]
    public class CarrinhoController : MainController
    {
        private readonly IAspNetUser _user;
        private readonly CarrinhoContext _context;
        public CarrinhoController(IAspNetUser user, CarrinhoContext context)
        {
            _user = user;
            _context = context;
        }

        public async Task<CarrinhoCliente> Obter()
        {
            return await ObterCarrinhoCliente() ?? new CarrinhoCliente();
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarItemCarrinho(CarrinhoItem item)
        {
            var carrinho = await ObterCarrinhoCliente();
            if (carrinho is null)
            {
                ManipularNovoCarrinho(item);
            }
            else
            {
                ManipularCarrinhExistente(carrinho, item);
            }

            ValidarCarrinho(carrinho);


            if (!OperacaoValida()) return CustomResponse();

            await PersistirDados();

            return CustomResponse();
        }

        [HttpPut("{produtoId}")]
        public async Task<IActionResult> AtualizarItemCarrinho(Guid produtoId, CarrinhoItem item)
        {
            var carrinho = await ObterCarrinhoCliente();
            var itemCarrinho = await ObterItemCarrinhoValidado(produtoId, carrinho, item);

            if (itemCarrinho == null)
                return CustomResponse();

            carrinho.AtualizarUnidades(itemCarrinho, item.Quantidade);

            ValidarCarrinho(carrinho);
            if (!OperacaoValida()) return CustomResponse();

            _context.CarrinhoItens.Update(itemCarrinho);
            _context.CarrinhoCliente.Update(carrinho);

            await PersistirDados();

            return CustomResponse();
        }

        [HttpDelete("{produtoId}")]
        public async Task<IActionResult> RemoverItemCarrinho(Guid produtoId)
        {
            var carrinho = await ObterCarrinhoCliente();
            var itemCarrinho = await ObterItemCarrinhoValidado(produtoId, carrinho);

            ValidarCarrinho(carrinho);
            if (!OperacaoValida()) return CustomResponse();

            carrinho.RemoverItem(itemCarrinho);

            _context.CarrinhoItens.Remove(itemCarrinho);
            _context.CarrinhoCliente.Update(carrinho);

            await PersistirDados();

            return CustomResponse();
        }

        private void ManipularNovoCarrinho(CarrinhoItem item)
        {
            var carrinho = new CarrinhoCliente(_user.ObterUserId);
            carrinho.AdicionarItem(item);

            _context.CarrinhoCliente.Add(carrinho);
        }

        private void ManipularCarrinhExistente(CarrinhoCliente carrinho, CarrinhoItem item)
        {
            var produtoItemExistente = carrinho.CarrinhoItemExistente(item);

            carrinho.AdicionarItem(item);

            if (produtoItemExistente)
            {
                _context.CarrinhoItens.Update(carrinho.ObterProdutoPorId(item.ProdutoId));
            }
            else
            {
                _context.CarrinhoItens.Add(item);
            }

            _context.CarrinhoCliente.Update(carrinho);
        }

        private async Task<CarrinhoCliente> ObterCarrinhoCliente()
            => await _context.CarrinhoCliente
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(cancellationToken => cancellationToken.ClienteId == _user.ObterUserId);

        private async Task<CarrinhoItem> ObterItemCarrinhoValidado(
            Guid produtoId, CarrinhoCliente carrinho, CarrinhoItem item = null)
        {
            if (item != null && produtoId != item.ProdutoId)
            {
                AdicionarErrosProcessamento("O item não correpsonde ao informado");
                return null;
            }

            if (carrinho is null)
            {
                AdicionarErrosProcessamento("Carrinho não encontrado");
                return null;
            }

            var itemCarrinho =
                await _context.CarrinhoItens.FirstOrDefaultAsync(i => i.CarrinhoId == carrinho.Id &&
                i.ProdutoId == produtoId);

            if (itemCarrinho is null || !carrinho.CarrinhoItemExistente(itemCarrinho))
            {
                AdicionarErrosProcessamento("O item não está no carrinho");
                return null;
            }

            return itemCarrinho;
        }

        private async Task PersistirDados()
        {
            var result = await _context.SaveChangesAsync();
            if (result <= 0)
                AdicionarErrosProcessamento("Não foi possível persistir os dados no banco");
        }

        private bool ValidarCarrinho(CarrinhoCliente carrinho)
        {
            if (carrinho.EhValido())
                return true;

            carrinho.ValidationResult.Errors.ToList()
                .ForEach(e => AdicionarErrosProcessamento(e.ErrorMessage));

            return false;
        }
    }
}
