using AuctionService.Data;
using AuctionService.Controllers; // <-- Needed for extension method
using Microsoft.EntityFrameworkCore;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Configuration;



var builder = WebApplication.CreateBuilder(args);

// Database setup
builder.Services.AddDbContext<AuctionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    });
});

// Is this required? Not sure!
// Hosted Service for MassTransit
builder.Services.AddMassTransitHostedService();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Register Auction endpoints
app.MapAuctionEndpoints();

app.Run();