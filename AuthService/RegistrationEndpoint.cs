using System.Security.Claims;
using Dapper;
using Npgsql;

namespace AuthService;

public static class RegistrationEndpoint
{
    public static void MapRegistrationEndpoints(this IEndpointRouteBuilder app, string connString)
    {
        app.MapPost("/signup", async (HttpContext ctx, SignUpRequest req, ILogger<Program> log) =>
        {
            // Handle signup logic here
            
            const string sql_user = """INSERT INTO "user" (id, username, email, password_hash) VALUES(@UserId, @Username, @Email, @Password)""";
            const string sql_profile = """INSERT INTO "profile" (id, user_id, name) VALUES(@ProfileId, @UserId, @Name)""";
            Guid user_uid =  Guid.NewGuid();
            Guid profile_uid = Guid.NewGuid();
            
            await using (var conn = new NpgsqlConnection(connString))
            {
                await conn.OpenAsync();
                // ... signup logic ...
                await using (var signup_transac = await conn.BeginTransactionAsync())
                {
                    await conn.ExecuteAsync(sql_user,new
                    {
                        UserId = user_uid,
                        Username = req.Name,
                        Email = req.Email,
                        Password = req.Password
                    }, signup_transac);
                    await conn.ExecuteAsync(sql_profile, new
                    {
                        ProfileId = profile_uid, 
                        UserId = user_uid,
                        Name = req.Name
                    }, signup_transac);

                    await signup_transac.CommitAsync();
                }
                await conn.CloseAsync();

                return Results.Ok();
            }
        });

        app.MapPost("/login", (HttpContext ctx, TokenFactory tokenFactory) =>
        {
            // Handle login logic here
            var claims = new List<Claim>();
            claims.Add(new Claim("sub", "Kano"));
            var accessToken = tokenFactory.WriteToken(claims, expiresInMins: 15);

            
            return new
            {
                message = "Login successful!",
                access_token = accessToken
            };
        });

        app.MapPost("/logout", (HttpContext ctx, ClaimsPrincipal user, ILogger<Program> logger) =>
        {
            // Handle logout logic here
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            logger.LogInformation("Logout successful, {userId}", userId);
            return $"Logout successful {userId} or {ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)}!";
        }).RequireAuthorization();
    }
}