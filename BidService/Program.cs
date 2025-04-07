using BidService.Data;
using BidService.Endpoints;
using Microsoft.EntityFrameworkCore;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<BidDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// RabbitMQ via MassTransit
builder.Services.AddMassTransit(cfg =>
{
    cfg.UsingRabbitMq((ctx, bus) =>
    {
        bus.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapBidEndpoints();

app.Run();
