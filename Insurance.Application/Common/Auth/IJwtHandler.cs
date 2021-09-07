using System;
using System.Security.Claims;

namespace Insurance.Application.Common.Auth
{
    public interface IJwtHandler
    {
        string GenerateAccessToken(Guid userId, string username, string ip);

        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromToken(string token, bool validateLifeTime = true);
    }
}