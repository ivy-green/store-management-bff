using System.Security.Claims;

namespace ProjectBase.Insfracstructure.Services.Jwts
{
    public interface IJwtService
    {
        string GenerateSecurityToken(IEnumerable<Claim> claims);
        DateTime GetTokenExpiry(string token);
    }
}