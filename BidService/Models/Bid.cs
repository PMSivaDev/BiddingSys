namespace BidService.Models;


public class Bid
{
    public int BidId { get; set; }

    public int AuctionId { get; set; }
    public int UserId { get; set; }

    public decimal BidAmount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Optional nav props
    public Auction? Auction { get; set; }
}
