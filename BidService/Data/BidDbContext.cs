using Microsoft.EntityFrameworkCore;
using BidService.Models;

namespace BidService.Data;

public class BidDbContext : DbContext
{
    public BidDbContext(DbContextOptions<BidDbContext> options) : base(options) { }

    public DbSet<Bid> Bids { get; set; }

    public DbSet<Auction> Auctions { get; set; }



}
