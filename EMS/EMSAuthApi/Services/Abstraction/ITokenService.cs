using Entities;
using System.Security.Claims;

namespace EMSAuthApi.Services.Abstraction
{
    public interface ITokenService
    {
        string GenerateToken(User user);

        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
