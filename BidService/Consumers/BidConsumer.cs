//using AuctionService.Events;
//using MassTransit;
//using BidService.Data;
//using BidService.Models;


//namespace BidService.Consumers
//{
//    public class BidConsumer : IConsumer<AuctionBidPlacedEvent>  
//    {
//        private readonly BidDbContext _context;

//        public BidConsumer(BidDbContext context)
//        {
//            _context = context;
//        }

//        public async Task Consume(ConsumeContext<AuctionBidPlacedEvent> context)
//        {
//            var eventMessage = context.Message;

//            // Save bid to database or process it
//            var bid = new Bid
//            {
//                AuctionId = eventMessage.AuctionId,
//                Amount = eventMessage.Amount,
//                Timestamp = DateTime.UtcNow
//            };

//            _context.Bids.Add(bid);
//            await _context.SaveChangesAsync();
//        }
//    }
//}
