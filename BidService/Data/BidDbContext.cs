using Microsoft.EntityFrameworkCore;
using BidService.Models;

namespace BidService.Data;

public class BidDbContext : DbContext
{
    public BidDbContext(DbContextOptions<BidDbContext> options) : base(options) { }

    public DbSet<Bid> Bids { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bid>().ToTable("Bids");

        modelBuilder.Entity<Bid>().Property(b => b.Status)
            .HasMaxLength(50);
    }
}
