using Inno_Shop_Product.Extasions.Configure;
using Inno_Shop_Product.Extasions.Mediator;
using Inno_Shop_Product.Extasions.Repostiories;
using Inno_Shop_Product.Middleware;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRepositories();

builder.Services.AddConfigure(configuration);
builder.Services.AddMediator();

var app = builder.Build();
app.UseException();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
