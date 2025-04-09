using AuctionService.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace AuctionService.Controllers;

public static class AuctionEndpoints
{
    public static void MapAuctionEndpoints(this WebApplication app)
    {
        // Get all auctions
        app.MapGet("/auctions/list", async (AuctionDbContext db) =>
            await db.Auctions.OrderByDescending(a => a.CreatedAt).ToListAsync())
            .WithName("GetAllAuctions");


        // Create a new auction( one of the 3 types )
        app.MapPost("/auctions/create", async (Auction auction, AuctionDbContext db) =>
        {
            // Set default metadata
            auction.Status = "Active";
            auction.CreatedAt = DateTime.UtcNow;
            auction.UpdatedAt = DateTime.UtcNow;

            // Validation based on auction type
            switch (auction.AuctionType)
            {
                case "BuyNow":
                    if (auction.TargetPrice == null || auction.TargetPrice <= 0)
                    {
                        return Results.BadRequest("TargetPrice is required and must be a positive number for 'BuyNow' auctions.");
                    }
                    break;

                case "Target":
                    if (auction.TargetPrice == null || auction.TargetPrice <= 0)
                    {
                        return Results.BadRequest("TargetPrice is required and must be a positive number for 'Target' auctions.");
                    }
                    break;

                case "Dynamic":
                    break;

                default:
                    return Results.BadRequest("Invalid or missing AuctionType. Valid types: BuyNow, Dynamic, Target.");
            }

            // Initialize the highest bid with the base price
            auction.HighestBidPrice = auction.BasePrice;

            // Save the auction
            db.Auctions.Add(auction);
            await db.SaveChangesAsync();

            // TODO: Implement event publishing logic for AuctionCreated using MassTransit or another messaging system.
            return Results.Created($"/auctions/{auction.AuctionId}", auction);
        });


        // Set

        // Get auction by ID
        app.MapGet("/auctions/get/{id:int}", async (int id, AuctionDbContext db, ILogger<Program> logger) =>
        {
            var auction = await db.Auctions.FindAsync(id);

            if (auction is null)
            {
                logger.LogWarning("Auction with ID {AuctionId} not found.", id);
                return Results.NotFound($"No auction found with ID {id}.");
            }

            return Results.Ok(auction);
        });


        // POST: Close an auction
        app.MapPost("/auctions/close/{id:int}", async (int id, AuctionDbContext db, ILogger<Program> logger) =>
        {
            var auction = await db.Auctions.FindAsync(id);

            if (auction is null)
            {
                logger.LogWarning("Attempt to close non-existent auction with ID {AuctionId}", id);
                return Results.NotFound($"Auction with ID {id} not found.");
            }

            if (auction.Status == "Closed")
            {
                logger.LogInformation("Auction with ID {AuctionId} is already closed.", id);
                return Results.BadRequest("Auction is already closed.");
            }

            auction.Status = "Closed";
            auction.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            logger.LogInformation("Auction with ID {AuctionId} was successfully closed.", id);

            // TODO: Publish AuctionClosed event
            return Results.Ok(auction);
        });


        // PUT: Alternative way to close an auction
        app.MapPut("/auctions/close/{id:int}", async (int id, AuctionDbContext db, ILogger<Program> logger) =>
        {
            var auction = await db.Auctions.FindAsync(id);

            if (auction is null)
            {
                logger.LogWarning("Cant close the non-existent auction with ID {AuctionId}", id);
                return Results.NotFound($"Auction with ID {id} not found.");
            }

            if (auction.Status == "Closed")
            {
                logger.LogInformation("Auction with ID {AuctionId} is already closed.", id);
                return Results.BadRequest("Auction is already closed.");
            }

            auction.Status = "Closed";
            auction.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            logger.LogInformation("Auction with ID {AuctionId} was successfully closed via PUT.", id);
            return Results.Ok(auction);
        });

        // Get all auctions for a specific seller
        app.MapGet("/auctions/list/seller/{sellerId:int}", async (int sellerId, AuctionDbContext db, ILogger<Program> logger) =>
        {
            logger.LogInformation("Fetching auctions for Seller ID {SellerId}.", sellerId);

            var auctions = await db.Auctions
                .Where(a => a.Car != null && a.Car.UserId == sellerId) // Ensure the Car and Seller relationship is used
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            if (auctions.Count == 0)
            {
                logger.LogWarning("No auctions found for Seller ID {SellerId}.", sellerId);
                return Results.NotFound($"No auctions found for Seller ID {sellerId}.");
            }

            logger.LogInformation("Retrieved {AuctionCount} auctions for Seller ID {SellerId}.", auctions.Count, sellerId);
            return Results.Ok(auctions);
        });

    }
}
