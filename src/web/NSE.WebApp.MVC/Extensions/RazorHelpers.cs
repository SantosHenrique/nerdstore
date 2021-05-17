using Microsoft.AspNetCore.Mvc.Razor;

namespace NSE.WebApp.MVC.Extensions
{
    public static class RazorHelpers
    {
        public static string MensagemEstoque(this RazorPage page, int quantidade)
        {
            return quantidade > 0 ? $"Apenas {quantidade} em estoque!" : "Produto esgotado";
        }
    }
}
