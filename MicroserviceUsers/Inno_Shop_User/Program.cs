using Application.Configuration;
using Application.Interfaces;
using Application.Services;
using Data.Repositories;
using Domain.Interfaces;
using Inno_Shop_User.Extensions.Configure;
using Inno_Shop_User.Extensions.Mediator;
using Inno_Shop_User.Extensions.Repositories;
using Inno_Shop_User.Extensions.Services;
using Inno_Shop_User.Extensions.Validator;
using Inno_Shop_User.Middleware.Week_3_Inno_PreTrainee.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Week_3_Inno_PreTrainee.Data.Context;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("Products", client =>
{
    var baseUrl = configuration["Services:ProductsBaseUrl"];
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddServices();
builder.Services.AddRepositories();

builder.Services.AddConfigure(configuration);
builder.Services.AddJwtAuth(configuration);
builder.Services.AddValidation();
builder.Services.AddMediator();

var app = builder.Build();
app.UseException();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }