using Microsoft.EntityFrameworkCore;

public class AuctionDbContext : DbContext
{
    public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options) { }

    public DbSet<Auction> Auctions { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<AuctionEvent> AuctionEvents { get; set; }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    base.OnModelCreating(modelBuilder);

    //    // Configure the relationship between Auction and Car
    //    modelBuilder.Entity<Auction>()
    //        .HasOne(a => a.Car)
    //        .WithMany(c => c.Auctions)
    //        .HasForeignKey(a => a.CarId)
    //        .OnDelete(DeleteBehavior.Cascade);
    //}
}
