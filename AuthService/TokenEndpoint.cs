using System.Security.Claims;

namespace AuthService;

public static class TokenEndpoint
{
    public static void MapTokenEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/token", (TokenRequest req, TokenFactory tokenFactory, ILogger<Program> logger) =>
        {
            // Handle token generation logic here
            var uid = Guid.CreateVersion7();
            var claims = new List<Claim>();
            claims.Add(new Claim("sub", $"hibana|{uid}"));
            claims.Add(new Claim("role", req.Role));

            string scopes = string.Empty;
            if (req.Scopes != null)
            {
                scopes = string.Join(" ", req.Scopes);
            }
    
            claims.Add(new Claim("scope", scopes));
            var token = tokenFactory.WriteToken(claims, expiresInMins: 15);
            var token1 = tokenFactory.WriteToken(claims, expiresInDays: 30);
            return new
            {
                message = $"Token generated for {req.Email}",
                access_token = token,
                refresh_token = token1,
            };
        });
    }
}