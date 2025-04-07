namespace AuctionService.Events
{
    public class AuctionBidPlacedEvent
    {
        public int AuctionId { get; set; }
        public decimal Amount { get; set; }
        public int BidderId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
