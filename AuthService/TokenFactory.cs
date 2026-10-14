using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AuthService;

public class TokenFactory
{
    private SigningCredentials _creds;
    public TokenFactory(SigningCredentials creds)
    {
        _creds = creds;
    }

    public string WriteToken(List<Claim> secondaryClaims, double expiresInMins = 15, double expiresInHours = 0, double expiresInDays = 0)
    {
        JwtSecurityToken jst = new JwtSecurityToken("urn:authserv", "ucr:client",
            secondaryClaims,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(expiresInDays).AddHours(expiresInHours).AddMinutes(expiresInMins), _creds);

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(jst);
    }
}