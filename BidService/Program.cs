using BidService.Data;
using BidService.Endpoints;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Configuration;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<BidDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") + ";Max Pool Size=10;"));

builder.Services.AddMemoryCache();


// Swagger
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


var app = builder.Build();

if (true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapBidEndpoints();
//app.MapControllers();
app.Run();
