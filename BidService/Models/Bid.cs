using System.ComponentModel.DataAnnotations;

namespace BidService.Models;


/// <summary>
/// Represents a bid placed on an auction.
/// </summary>
public class Bid
{
    /// <summary>
    /// Unique identifier for the bid.
    /// </summary>
    public int BidId { get; set; }

    /// <summary>
    /// The ID of the auction this bid is associated with.
    /// </summary>
    public int AuctionId { get; set; }

    /// <summary>
    /// The ID of the user who placed the bid.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The amount of the bid.
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "Bid amount must be greater than zero.")]
    public decimal BidAmount { get; set; }

    /// <summary>
    /// The date and time when the bid was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The date and time when the bid was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The auction this bid is associated with.
    /// </summary>
    public Auction Auction { get; set; } = null!;
}


public class Auction
{
    public int AuctionId { get; set; }
    public int CarId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string AuctionType { get; set; } = string.Empty; // Buy Now / Dynamic / Target
    public decimal? TargetPrice { get; set; }
    public decimal? BasePrice { get; set; }

    public decimal? HighestBidPrice { get; set; }

    public string Status { get; set; } = "Active"; // Active / Closed

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
