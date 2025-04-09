
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class Auction
{
    public int AuctionId { get; set; }
    public int CarId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public string AuctionType { get; set; } = string.Empty; // Buy Now / Dynamic / Target
    [Precision(18, 2)] 
    public decimal? TargetPrice { get; set; }
    [Precision(18, 2)]
    public decimal? BasePrice { get; set; }
    [Precision(18, 2)]
    public decimal? HighestBidPrice { get; set; }

    public string Status { get; set; } = "Active"; // Active / Closed

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Car? Car { get; set; } // Add this if missing
}

/// <summary>
/// ////////////Other models for relevant tables
/// </summary>

// Car Model
public class Car
{
    public int CarId { get; set; }
    public int UserId { get; set; } // Foreign key to Users
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string VIN { get; set; } = string.Empty; // Unique
    public string Condition { get; set; } = string.Empty;
    [Precision(18, 2)]
    public decimal ListingPrice { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Active"; // Example: Active, Sold, Pending
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public User? User { get; set; } // The seller of the car
    public ICollection<Auction>? Auctions { get; set; } // Auctions associated with the car
    // Navigation property
}

// User Model
public class User
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty; // Unique and required
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = "Bidder"; // Example: Seller, Bidder
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Car>? Cars { get; set; } // Cars listed by the user
    public ICollection<Notification>? Notifications { get; set; } // Notifications for the user
}

// Notification Model
public class Notification
{
    public int NotificationId { get; set; }
    public int UserId { get; set; } // Foreign key to Users
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "General"; // Example: Auction Status Change, Bid Update
    public bool IsRead { get; set; } = false; // Whether the notification was read by the user
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public User? User { get; set; } // The user who received the notification
}

// AuctionEvent Model
public class AuctionEvent
{
    [Key]
    public int EventId { get; set; } // Primary key
    public int AuctionId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EventDetails { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Auction? Auction { get; set; }
}


