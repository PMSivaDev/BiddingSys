using BidService.Data;
using BidService.Models;
using Microsoft.EntityFrameworkCore;

namespace BidService.Endpoints;

public static class BidEndpoints
{

    public static void MapBidEndpoints(this WebApplication app)
    {
        // Get all bids
        app.MapGet("/bids", async (BidDbContext db) =>
        {
            var bids = await db.Bids
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return Results.Ok(bids);
        });

        // Submit a new bid
        app.MapPost("/bids", async (Bid bid, BidDbContext db) =>
        {
            bid.CreatedAt = DateTime.UtcNow;
            bid.UpdatedAt = DateTime.UtcNow;

            db.Bids.Add(bid);
            await db.SaveChangesAsync();

            // TODO: Publish BidPlaced event
            return Results.Created($"/bids/{bid.BidId}", bid);
        });

        // Get bids for a specific auction
        app.MapGet("/bids/auction/{auctionId:int}", async (int auctionId, BidDbContext db) =>
        {
            var bids = await db.Bids
                .Where(b => b.AuctionId == auctionId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return Results.Ok(bids);
        });
    }
}
