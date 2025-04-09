//using AuctionService.Data;
using AuctionService.Controllers; // <-- Needed for extension method
using Microsoft.EntityFrameworkCore;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Configuration;
using AuctionService.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AuctionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") + ";Max Pool Size=10;"));

// Swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MassTransit (RabbitMQ) singleton
builder.Services.AddMassTransitHostedService(true);

// Register the AuctionProcessor as a hosted service ( to run the background Auction status scanning )
builder.Services.AddHostedService<AuctionProcessor>();

// MassTransit (RabbitMQ) Setup with correct method
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    // Register consumers here if needed
    // x.AddConsumer<MyConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.PrefetchCount = 16; // Optimize message processing
        cfg.UseConcurrencyLimit(8); // Limit concurrent consumers
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Register Auction endpoints
app.MapAuctionEndpoints();
app.Run();