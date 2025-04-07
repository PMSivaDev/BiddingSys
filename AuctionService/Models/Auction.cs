public class Auction
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public AuctionType Type { get; set; }       // One of the 3 Auction types - BuyNow / Dynamic / Target
    public decimal? TargetPrice { get; set; }   // Final price that seller targets for
    public decimal StartPrice { get; set; }     // Lower base price to start with the auction
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsActive { get; set; }
    public bool IsClosed { get; set; }
}
