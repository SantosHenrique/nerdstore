using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSE.Cliente.API.Application.Commands;
using NSE.Cliente.API.Application.Events;
using NSE.Cliente.API.Data;
using NSE.Cliente.API.Data.ClienteRepository;
using NSE.Cliente.API.Models;
using NSE.Cliente.API.Services;
using NSE.Core.Mediator;

namespace NSE.Cliente.API.Configuration
{
    public static class Injector
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<ClientesContext>();
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddScoped<IRequestHandler<RegistrarClienteCommand, ValidationResult>, ClienteCommandHandler>();
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<INotificationHandler<ClienteRegistradoEvent>, ClienteEventHandler>();

            services.AddHostedService<RegistroClienteIntegretaionHandler>();
        }
    }

}
