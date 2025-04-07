namespace BidService.Models;

public class Bid
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
