using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace NSE.WebApi.Core.Usuario
{
    public class AspNetUser : IAspNetUser
    {
        private readonly IHttpContextAccessor _acessor;
        public AspNetUser(IHttpContextAccessor acessor)
        {
            _acessor = acessor;
        }
        public string Name => _acessor.HttpContext.User.Identity.Name;

        public Guid ObterUserId => EstaAutenticado() ? Guid.Parse(_acessor.HttpContext.User.GetUserClaimValue("sub")) : Guid.Empty;

        public string ObterUserEmail => EstaAutenticado() ? _acessor.HttpContext.User.GetUserClaimValue("email") : string.Empty;

        public string ObterUserToken => EstaAutenticado() ? _acessor.HttpContext.User.FindFirstValue("JWT") : string.Empty;

        public bool EstaAutenticado() => _acessor.HttpContext.User.Identity.IsAuthenticated;

        public IEnumerable<Claim> ObterClaims()
        => _acessor.HttpContext.User.Claims;

        public HttpContext ObterHttpContext()
        => _acessor.HttpContext;

        public bool PossuiRole(string role)
        => _acessor.HttpContext.User.IsInRole(role);

        IEnumerable<Claim> IAspNetUser.ObterClaims()
        {
            throw new NotImplementedException();
        }
    }
}

