using FluentValidation.Results;
using Library.API.Context;
using Library.API.EndPoints;
using Library.API.Models;
using Library.API.Services;
using Library.API.Services.Abstract;
using Library.API.Services.Concrete;
using Library.API.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettingsLocal.json", true, true);//kendi appsettings dosyamýzý kullanmak için tanýmladýk
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = "alkan",
        ValidAudience = "alkan",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mysecretkeymysecretkeymysecretkeymysecretkeymysecretkeymysecretkeymysecretkeymysecretkey"))
    };
});
builder.Services.AddAuthorization();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("MyDb");
});

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<JwtProvider>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();


app.UseBookEndpoints();



app.Run();
