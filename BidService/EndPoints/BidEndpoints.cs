using Azure.Identity;
using BidService.Data;
using BidService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BidService.Endpoints;

public static class BidEndpoints
{
    /// <summary>
    /// Maps the bid-related endpoints to the application.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    public static void MapBidEndpoints(this WebApplication app)
    {
        // Endpoint to submit a new bid
        app.MapPost("/bids/create", async (Bid bid, BidDbContext db, ILogger<Program> logger) =>
        {
            logger.LogInformation("New bid submission received for Auction ID {AuctionId} with an amount of {BidAmount}.", bid.AuctionId, bid.BidAmount);

            // Try to find the auction for the given AuctionId
            var auction = await db.Auctions.FindAsync(bid.AuctionId);
            if (auction is null)
            {
                logger.LogWarning("Auction with ID {AuctionId} was not found.", bid.AuctionId);
                return Results.BadRequest($"Auction with ID {bid.AuctionId} does not exist.");
            }

            // Ensure the auction is active
            if ( auction.Status != "Active")
            {
                logger.LogWarning("Auction with ID {AuctionId} is not active. Current status: {Status}.", bid.AuctionId, auction.Status);
                return Results.BadRequest("You cannot place a bid on a closed or inactive auction.");
            }

            if (!IsValidBid(bid, auction))
            {
                return Results.BadRequest($"Your bid must be higher than the current highest bid: {auction.HighestBidPrice:C}.");
            }

            // Helper method for validation
            bool IsValidBid(Bid bid, Auction auction)
            {
                return bid.BidAmount > 0 && bid.BidAmount > (auction.HighestBidPrice ?? 0);
            }


            // Update the auction with the new highest bid
            logger.LogInformation("Bid amount {BidAmount} is valid for Auction ID {AuctionId}. Updating ... ", bid.BidAmount, bid.AuctionId);
            auction.HighestBidPrice = bid.BidAmount;
            auction.UpdatedAt = DateTime.UtcNow;

            // Check if the auction should be closed automatically
            var auctionType = auction.AuctionType;
            var targetPrice = auction.TargetPrice ?? 0;

            if ((auctionType == "Target" || auctionType == "BuyNow") && (bid.BidAmount >= targetPrice ))
            {
                auction.Status = "Closed";
                logger.LogInformation("Auction ID {AuctionId} has been closed automatically as the bid amount {BidAmount} met or exceeded the target price {TargetPrice}.", bid.AuctionId, bid.BidAmount, targetPrice);

                //TODO 1 : Implement notification logic for seller and bidder about auction closure.                

            }

            // Save the bid to the database
            bid.CreatedAt = DateTime.UtcNow;
            bid.UpdatedAt = DateTime.UtcNow;

            db.Bids.Add(bid);
            await db.SaveChangesAsync();

            logger.LogInformation("Bid with ID {BidId} has been successfully created for Auction ID {AuctionId}.", bid.BidId, bid.AuctionId);

            // TODO: Implement event publishing logic for BidPlaced and AuctionClosed.
            return Results.Created($"/bids/{bid.BidId}", bid);
        });

        // Fetch all bids from the database and cache them for 5 minutes
        app.MapGet("/bids/list", async ([FromServices] BidDbContext db, [FromServices] IMemoryCache cache, [FromServices] ILogger<Program> logger) =>
        {
            const string cacheKey = "AllBids";
            if (!cache.TryGetValue(cacheKey, out List<Bid> bids))
            {
                logger.LogInformation("Fetching all bids from the database.");
                bids = await db.Bids
                    .AsNoTracking()
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();

                // Cache the result for 5 minutes
                cache.Set(cacheKey, bids, TimeSpan.FromMinutes(5));
            }
            else
            {
                logger.LogInformation("Fetching all bids from cache.");
            }

            return Results.Ok(bids);
        });

        // Endpoint to get all bids for a specific auction
        app.MapGet("/bids/list/auction/{auctionId:int}", async (int auctionId, BidDbContext db, ILogger<Program> logger) =>
        {
            logger.LogInformation("Fetching all bids for Auction ID {AuctionId}.", auctionId);
            var bids = await db.Bids
                .AsNoTracking() // This is read-only query. Avoid change tracking to improve performance.
                .Where(b => b.AuctionId == auctionId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            if (bids.Count == 0)
            {
                logger.LogInformation("No bids found for Auction ID {AuctionId}.", auctionId);
                return Results.NoContent();
            }
            
            logger.LogInformation("Successfully fetched {BidCount} bids for Auction ID {AuctionId}.", bids.Count, auctionId);
            return Results.Ok(bids);
        });
    }

    // Helper method for validation
    static bool IsValidBid(Bid bid, Auction auction)
    {
        return bid.BidAmount > 0 && bid.BidAmount > (auction.HighestBidPrice ?? 0);
    }
}
