using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Services;
using NSE.WebApp.MVC.Services.Handlers;
using System;

namespace NSE.WebApp.MVC.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
            services.AddHttpClient<IAuthService, AuthService>();

            services.AddHttpClient<ICatalogoService, CatalogoService>()
              .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

            #region Refit
            /*
            var uri = configuration.GetSection("CatalogoUrl").Value;

            services.AddHttpClient("Refit", options =>
            {
                options.BaseAddress = new Uri(uri);
            }).AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
            .AddTypedClient(Refit.RestService.For<ICatalogoServiceRefit>);*/
            #endregion

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUser, AspNetUser>();
        }
    }
}
