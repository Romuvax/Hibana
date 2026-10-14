using System.Security.Claims;
using Dapper;
using Npgsql;

namespace AuthService;

public static class UserDataEndpoint
{
    public static void MapUserDataEndpoint(this IEndpointRouteBuilder app, string connString)
    {
        app.MapGet("/dashboard", (ClaimsPrincipal user) =>
        {
            return "Hello World!";
        }).RequireAuthorization("player");

        app.MapPost("/user/inventory/{itemId}", (string itemId, ClaimsPrincipal user, ILogger<Program> logger) =>
        {
            // Handle inventory update logic here
            return $"Item updated: {itemId}";
        });

        app.MapPost("/user/{userId:guid}/profile/{profileId:guid}/inventory/", async (Guid userId, Guid profileId, ItemResourceRequest req) =>
        {
            const string sql_user = """SELECT user_id FROM "profile" WHERE user_id = @UserId AND profile_id = @ProfileId""";
            const string sql_item = """INSERT INTO "profile_inventory" (id, profile_id, item_def_id, quantity) VALUES(@ItemId, @ProfileId, @ItemDefId, @Quantity)""";
            Guid itemId = Guid.NewGuid();
    
    
    
            await using (var conn =  new NpgsqlConnection(connString))
            {
                await conn.OpenAsync();
                await using (var new_item_transact = await conn.BeginTransactionAsync())
                {
                    await conn.ExecuteAsync(sql_item, 
                        new { 
                            ItemId = itemId, 
                            ProfileId = profileId,
                            ItemDefId = req.ItemDefId,
                            Quantity = req.Quantity
                    
                        }
                        , new_item_transact);
            
                    await new_item_transact.CommitAsync();
                }
                await conn.CloseAsync();
        
                return Results.Ok();
            }
        });
    }
}