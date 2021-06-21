using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NSE.Carrinho.API.Models
{
    public class CarrinhoCliente
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public decimal ValorTotal { get; set; }
        public List<CarrinhoItem> Itens { get; set; } = new List<CarrinhoItem>();
        public ValidationResult ValidationResult { get; set; }

        public CarrinhoCliente(Guid clienteId)
        {
            Id = Guid.NewGuid();
            ClienteId = clienteId;
        }

        public CarrinhoCliente()
        {
        }

        internal void AdicionarItem(CarrinhoItem item)
        {
            item.AssociarCarrinho(Id);

            if (CarrinhoItemExistente(item))
            {
                var itemExistente = ObterProdutoPorId(item.ProdutoId);
                itemExistente.AdicinarUnidades(item.Quantidade);

                item = itemExistente;
                Itens.Remove(itemExistente);
            }

            Itens.Add(item);
            CalcularValorTotal();
        }

        internal void AtualizarItem(CarrinhoItem item)
        {
            var itemExistente = ObterProdutoPorId(item.ProdutoId);

            Itens.Remove(itemExistente);
            Itens.Add(item);

            CalcularValorTotal();
        }

        internal void RemoverItem(CarrinhoItem item)
        {
            Itens.Remove(ObterProdutoPorId(item.ProdutoId));
            CalcularValorTotal();
        }

        internal void AtualizarUnidades(CarrinhoItem item, int unidades)
        {
            item.AtualizarUnidades(unidades);
            AtualizarItem(item);
        }

        internal CarrinhoItem ObterProdutoPorId(Guid produtoId)
        {
            return Itens.FirstOrDefault(i => i.ProdutoId == produtoId);
        }

        internal void CalcularValorTotal()
        {
            ValorTotal = Itens.Sum(i => i.CalcularValor());
        }

        internal bool CarrinhoItemExistente(CarrinhoItem item)
        {
            return Itens.Any(i => i.ProdutoId == item.ProdutoId);
        }

        internal bool EhValido()
        {
            var erros = Itens.SelectMany(i => new CarrinhoItem.ItemCarrinhoValidation().Validate(i).Errors)
                .ToList();

            erros.AddRange(new CarrinhoClienteValidation().Validate(this).Errors);

            ValidationResult = new ValidationResult(erros);

            return ValidationResult.IsValid;
        }
        public class CarrinhoClienteValidation : AbstractValidator<CarrinhoCliente>
        {
            public CarrinhoClienteValidation()
            {
                RuleFor(c => c.ClienteId)
                    .NotEqual(Guid.Empty)
                    .WithMessage("Cliente não reconhecido");

                RuleFor(c => c.Itens.Count)
                    .GreaterThan(0)
                    .WithMessage("O carrinho não possui itens");

                RuleFor(c => c.ValorTotal)
                    .GreaterThan(0)
                    .WithMessage("O valor total do carrinho precisa ser maior que 01");
            }
        }
    }
}
