using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_of_at_least_128_bits"));
SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.WebHost.UseUrls("http://localhost:4000");
builder.Services.AddOpenApi();
builder.Services.AddSingleton<TokenFactory>(sp =>
    new TokenFactory(creds));
builder.Services.AddAuthentication(authn =>
{
    authn.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "urn:authserv",
        ValidateAudience = true,
        ValidAudience = "ucr:client",
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        
        ValidateLifetime = true
    };
});
builder.Services.AddAuthorization(authz =>
{
    authz.AddPolicy("player", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("role", "player");
    });
});

var app = builder.Build();
var connString = app.Configuration.GetConnectionString("Hibana")!;
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", (HttpContext ctx) =>
{
    var userAgent = ctx.Request.Headers.UserAgent;
    return $"Hello World! {ctx.Request.Host} (User-Agent: {userAgent})";
}).RequireAuthorization("player");

app.MapGet("/health", () => "OK");

app.MapTokenEndpoints();

app.MapRegistrationEndpoints(connString);

app.MapUserDataEndpoint(connString);

app.Run();

internal record SignUpRequest(string Email, string Password, string Name);
internal record TokenRequest(string Email, string Password, string[]? Scopes, string Role);
internal record ItemResourceRequest(long ItemDefId, long Quantity);