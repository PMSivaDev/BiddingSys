﻿namespace AuctionService.Data
{
    using Microsoft.EntityFrameworkCore;
    //using AuctionService.Models;

    public class AuctionDbContext : DbContext
    {
        public AuctionDbContext(DbContextOptions<AuctionDbContext> options)
            : base(options) { }

        public DbSet<Auction> Auctions { get; set; }
    }
}
