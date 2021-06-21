using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSE.Carrinho.API.Data;
using NSE.WebApi.Core.Usuario;

namespace NSE.Carrinho.API.Config
{
    public static class Injector
    {
        public static void AddRegisterServices(this IServiceCollection services)
        {
            services.AddScoped<CarrinhoContext>();
            services.AddScoped<IAspNetUser, AspNetUser>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
