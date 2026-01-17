using Application.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace Infrastructure.Auth
{
    public sealed class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _http;
        public CurrentUser(IHttpContextAccessor http) => _http = http;

        public Guid UserId =>
     Guid.TryParse(
         _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
         out var id)
     ? id
     : Guid.Empty;

        public string Email =>
            _http.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

        public IReadOnlyCollection<string> Roles
            => _http.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray()
               ?? Array.Empty<string>();

        public bool IsInRole(string role) => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}
