using FluentValidation;
using System;

namespace NSE.Carrinho.API.Models
{
    public class CarrinhoItem
    {
        public CarrinhoItem()
        {
            Id = new Guid();
        }

        public Guid Id { get; private set; }
        public Guid ProdutoId { get; private set; }
        public string Nome { get; private set; }
        public int Quantidade { get; private set; }
        public decimal Valor { get; private set; }
        public string Imagem { get; private set; }

        public Guid CarrinhoId { get; private set; }
        public CarrinhoCliente CarrinhoCliente { get; private set; }

        internal void AssociarCarrinho(Guid carrinhoId)
        {
            CarrinhoId = carrinhoId;
        }

        internal decimal CalcularValor()
        {
            return Quantidade * Valor;
        }

        internal void AdicinarUnidades(int qtd)
        {
            Quantidade += qtd;
        }

        internal void AtualizarUnidades(int qtd)
        {
            Quantidade = qtd;
        }

        internal bool Validar()
        {
            return new ItemCarrinhoValidation().Validate(this).IsValid;
        }

        public class ItemCarrinhoValidation : AbstractValidator<CarrinhoItem>
        {
            public ItemCarrinhoValidation()
            {
                RuleFor(c => c.ProdutoId)
                        .NotEqual(Guid.Empty)
                        .WithMessage("Id do produto inválido");

                RuleFor(c => c.Nome)
                    .NotEmpty()
                    .WithMessage("O nome do produto não foi informado");

                RuleFor(C => C.Quantidade)
                    .GreaterThan(0)
                    .WithMessage(item => $"A quantidade mínima para o {item.Nome} é 1");

                RuleFor(c => c.Quantidade)
                    .LessThan(5)
                    .WithMessage(item => $"A quantidade máxima para o {item.Nome} é 15");

                RuleFor(c => c.Valor)
                    .GreaterThan(0)
                    .WithMessage(item => $"O valor do {item.Nome} precisa ser maior que 0");
            }
        }
    }
}
