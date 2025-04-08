using AuctionService.Controllers;
//using AuctionService.Models;
using AuctionService.Data;
using Microsoft.EntityFrameworkCore;
namespace AuctionService.Controllers;

public static class AuctionEndpoints
{
    public static void MapAuctionEndpoints(this WebApplication app)
    {
        // Get all auctions
        app.MapGet("/auctions", async (AuctionDbContext db) =>
            await db.Auctions.OrderByDescending(a => a.CreatedAt).ToListAsync())
            .WithName("GetAuctions");

        // Create a new auction
        app.MapPost("/auctions", async (Auction auction, AuctionDbContext db) =>
        {
            auction.Status = "Active";
            auction.CreatedAt = DateTime.UtcNow;
            auction.UpdatedAt = DateTime.UtcNow;

            db.Auctions.Add(auction);
            await db.SaveChangesAsync();

            // TODO: Publish AuctionCreated event
            return Results.Created($"/auctions/{auction.AuctionId}", auction);
        });

        // Get auction by ID
        app.MapGet("/auctions/{id:int}", async (int id, AuctionDbContext db) =>
        {
            var auction = await db.Auctions.FindAsync(id);
            return auction is not null ? Results.Ok(auction) : Results.NotFound();
        });

        // Close an auction (update status)
        app.MapPost("/auctions/{id:int}/close", async (int id, AuctionDbContext db) =>
        {
            var auction = await db.Auctions.FindAsync(id);
            if (auction is null)
                return Results.NotFound();

            auction.Status = "Closed";
            auction.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            // TODO: Publish AuctionClosed event
            return Results.Ok(auction);
        });

        // Alternative close route using PUT
        app.MapPut("/auctions/{id:int}/close", async (int id, AuctionDbContext db) =>
        {
            var auction = await db.Auctions.FindAsync(id);
            if (auction is null)
                return Results.NotFound();

            auction.Status = "Closed";
            auction.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return Results.Ok(auction);
        });
    }
}
