using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;
using NSE.WebApp.MVC.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Controllers
{
    public class IdentidadeController : MainController
    {
        private readonly IAuthService _autenticacaoService;

        public IdentidadeController(IAuthService autenticacaoService)
        {
            _autenticacaoService = autenticacaoService;
        }

        [HttpGet("nova-conta")]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost("nova-conta")]
        public async Task<IActionResult> Registro(UsuarioRegistro usuarioRegistro)
        {
            if (!ModelState.IsValid)
                return View(usuarioRegistro);

            var response = await _autenticacaoService.Registro(usuarioRegistro);

            if (ResponsePossuiErros(response.ResponseResult))
                return View(usuarioRegistro);

            await RealizarLogin(response);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("login")]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UsuarioLogin usuarioLogin, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(usuarioLogin);

            var response = await _autenticacaoService.Login(usuarioLogin);

            if (ResponsePossuiErros(response.ResponseResult))
                return View(usuarioLogin);

            await RealizarLogin(response);

            if (returnUrl == null)
                return RedirectToAction("Index", "Home");

            return LocalRedirect(returnUrl);
        }

        [HttpGet("sair")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public async Task RealizarLogin(UsuarioRespostaLogin usuarioRespostaLogin)
        {
            var token = ObterTokenFormatado(usuarioRespostaLogin.AcessToken);

            var claims = new List<Claim>();
            claims.Add(new Claim("JWT", usuarioRespostaLogin.AcessToken));
            claims.AddRange(token.Claims);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties()
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                IsPersistent = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        private static JwtSecurityToken ObterTokenFormatado(string jwtToken)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
        }
    }
}
