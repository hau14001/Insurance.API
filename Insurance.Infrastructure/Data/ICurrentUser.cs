using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace Insurance.Infrastructure.Data
{
    public interface ICurrentUser
    {
        Guid GetId();

        string GetName();
    }

    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContext)
        {
            _httpContextAccessor = httpContext;
        }

        public Guid GetId()
        {
            var identifier = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return String.IsNullOrEmpty(identifier) ? Guid.Empty : new Guid(identifier);
        }

        public string GetName() => _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
    }
}