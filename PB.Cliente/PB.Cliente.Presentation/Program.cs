using PB.Cliente.Presentation.Middlewares;
using PB.Cliente.Infrastructure.Repository;
using PB.Cliente.Infrastructure.Messaging;
using PB.Cliente.Infrastructure.Context;
using PB.Cliente.Application.Interfaces;
using PB.Cliente.Application.Services;
using PB.Cliente.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using FluentValidation;
using PB.Cliente.Application.Validators;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ClienteDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
// Conexão RabbitMQ
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory
    {
        HostName = builder.Configuration["RabbitMQ:Host"],
        UserName = builder.Configuration["RabbitMQ:User"],
        Password = builder.Configuration["RabbitMQ:Password"]
    };
    return factory.CreateConnection();
});

builder.Services.AddScoped<IMessagePublisher, RabbitMQPublisher>();
builder.Services.AddValidatorsFromAssemblyContaining<RegistrarClienteValidator>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();