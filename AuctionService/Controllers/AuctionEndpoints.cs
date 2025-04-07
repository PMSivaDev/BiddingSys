using AuctionService.Controllers;
//using AuctionService.Models;
using AuctionService.Data;
using Microsoft.EntityFrameworkCore;
namespace AuctionService.Controllers;

public static class AuctionEndpoints
{

    public static void MapAuctionEndpoints(this WebApplication app)
    {
        // Recieve a new AUCTION object with details and add it to the dbo.Auctions table
        app.MapPost("/auctions", async (Auction auction, AuctionDbContext db) =>
        {
            auction.Id = Guid.NewGuid();
            auction.IsActive = true;
            auction.IsClosed = false;

            db.Auctions.Add(auction);
            await db.SaveChangesAsync();

            // TODO: Send AuctionCreated event

            return Results.Created($"/auctions/{auction.Id}", auction);
        });

        // Fetch the details of an ACTION with the given Acution Id
        app.MapGet("/auctions/{id:guid}", async (Guid id, AuctionDbContext db) =>
        {
            var auction = await db.Auctions.FindAsync(id);
            return auction != null ? Results.Ok(auction) : Results.NotFound();
        });

        // Update an AUCTION status to be CLOSED
        app.MapPost("/auctions/{id:guid}/close", async (Guid id, AuctionDbContext db) =>
        {
            var auction = await db.Auctions.FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null)
                return Results.NotFound();

            auction.IsActive = false;
            auction.IsClosed = true;

            await db.SaveChangesAsync();

            // TODO: Send AuctionClosed event

            return Results.Ok(auction);
        });
    }
}
