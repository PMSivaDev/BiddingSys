
public class Auction
{
    public int AuctionId { get; set; }
    public int CarId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string AuctionType { get; set; } = string.Empty; // Buy Now / Dynamic / Target
    public decimal? TargetPrice { get; set; }
    public decimal? BasePrice { get; set; }

    public string Status { get; set; } = "Active"; // Active / Closed

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

//public class Auction
//{
//    public Guid Id { get; set; }
//    public Guid CarId { get; set; }
//    public AuctionType Type { get; set; }       // One of the 3 Auction types - BuyNow / Dynamic / Target
//    public decimal? TargetPrice { get; set; }   // Final price that seller targets for
//    public decimal StartPrice { get; set; }     // Lower base price to start with the auction
//    public DateTime StartTime { get; set; }
//    public DateTime EndTime { get; set; }
//    public bool IsActive { get; set; }
//    public bool IsClosed { get; set; }
//}
