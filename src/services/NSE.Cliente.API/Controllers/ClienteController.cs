using Microsoft.AspNetCore.Mvc;
using NSE.Cliente.API.Application.Commands;
using NSE.Core.Mediator;
using NSE.WebApi.Core.Controllers;
using System;
using System.Threading.Tasks;

namespace NSE.Cliente.API.Controllers
{
    [Route("clientes")]
    [ApiController]
    public class ClienteController : MainController
    {
        private readonly IMediatorHandler _mediatorHandler;

        public ClienteController(IMediatorHandler mediatorHandler)
        {
            _mediatorHandler = mediatorHandler;
        }

        [HttpGet("clientes")]
        public async Task<IActionResult> Index()
        {
            var resultado = await _mediatorHandler.EnviarComando(new RegistrarClienteCommand(Guid.NewGuid(), "Henrique",
                "henrique@henrique.com", "21200665074"));

            return CustomResponse(resultado);
        }
    }
}
