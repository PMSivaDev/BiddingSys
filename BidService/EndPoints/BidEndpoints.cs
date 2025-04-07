using BidService.Data;
using BidService.Models;
using Microsoft.EntityFrameworkCore;

namespace BidService.Endpoints;

public static class BidEndpoints
{
    public static void MapBidEndpoints(this WebApplication app)
    {
        // Receive a Bid info and persist in dB
        app.MapPost("/bids", async (Bid bid, BidDbContext db) =>
        {
            bid.Id = Guid.NewGuid();
            bid.CreatedAt = DateTime.UtcNow;
            bid.Status = "Pending";

            db.Bids.Add(bid);
            await db.SaveChangesAsync();

            // TODO: Publish event (BidPlaced)
            return Results.Created($"/bids/{bid.Id}", bid);
        });

        app.MapGet("/bids/auction/{auctionId:guid}", async (Guid auctionId, BidDbContext db) =>
        {
            var bids = await db.Bids
                .Where(b => b.AuctionId == auctionId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return Results.Ok(bids);
        });
    }
}
