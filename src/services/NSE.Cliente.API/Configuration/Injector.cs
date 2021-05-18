using Microsoft.Extensions.DependencyInjection;
using NSE.Cliente.API.Data;

namespace NSE.Cliente.API.Configuration
{
    public static class Injector
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<ClientesContext>();
        }
    }

}
