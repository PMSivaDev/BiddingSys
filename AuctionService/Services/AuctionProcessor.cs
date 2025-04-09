using Microsoft.EntityFrameworkCore;
namespace AuctionService.Services;

public class AuctionProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuctionProcessor> _logger;

    public AuctionProcessor(IServiceProvider serviceProvider, ILogger<AuctionProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AuctionProcessor background service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();

                // Find auctions expired by time, but still holds "Active" status
                var expiredAuctions = await dbContext.Auctions
                    .Include(a => a.Car) // Include the related Car
                    .ThenInclude(c => c.User) // Include the related User (Seller)
                    .Where(a => a.EndDate <= DateTime.UtcNow && a.Status == "Active")
                    .Take(100) // Process in batches of 100
                    .ToListAsync(stoppingToken);

                foreach (var auction in expiredAuctions)
                {
                    _logger.LogInformation("Auction ID {AuctionId} has expired but is still active. Notifying the seller.", auction.AuctionId);

                    // Notify the seller to consider the highest bid
                    await NotifySellerAsync(auction, dbContext);

                    // Log the event in the AuctionEvents table
                    await LogAuctionEventAsync(auction, dbContext);

                    // Optionally, update the auction status to "Pending Seller Decision"
                    auction.Status = "PendingSellerDecision";
                    auction.UpdatedAt = DateTime.UtcNow;
                }

                // Save changes to the database
                if (expiredAuctions.Any())
                {
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing expired auctions.");
            }

            // Wait for a specified interval before checking again
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }

        _logger.LogInformation("AuctionProcessor background service is stopping.");
    }

    private async Task NotifySellerAsync(Auction auction, AuctionDbContext dbContext)
    {
        if (auction.Car?.User == null)
        {
            _logger.LogWarning("Seller information is missing for Auction ID {AuctionId}.", auction.AuctionId);
            return;
        }

        var seller = auction.Car.User;

        // Create a notification for the seller
        var notification = new Notification
        {
            UserId = seller.UserId,
            Message = $"Your auction for car '{auction.Car.Make} {auction.Car.Model}' has expired. The highest bid is {auction.HighestBidPrice:C}.",
            Type = "Auction Expired",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await dbContext.Notifications.AddAsync(notification);

        _logger.LogInformation("Notification created for Seller ID {SellerId} about Auction ID {AuctionId}.", seller.UserId, auction.AuctionId);

        // TODO: Add logic to send email or SMS to the seller if required
    }

    private async Task LogAuctionEventAsync(Auction auction, AuctionDbContext dbContext)
    {
        var auctionEvent = new AuctionEvent
        {
            AuctionId = auction.AuctionId,
            EventType = "Auction Expired",
            EventDetails = $"Auction expired on {DateTime.UtcNow}. Highest bid: {auction.HighestBidPrice:C}.",
            CreatedAt = DateTime.UtcNow
        };

        await dbContext.AuctionEvents.AddAsync(auctionEvent);

        _logger.LogInformation("Auction event logged for Auction ID {AuctionId}.", auction.AuctionId);
    }
}
