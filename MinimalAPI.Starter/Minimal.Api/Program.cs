using Microsoft.AspNetCore.Mvc;
using Minimal.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<PeopleService>();//Eðer DI kullanýyorsak bu þekilde servis tanýmý yapýlarak devam edilir.
builder.Services.AddScoped<GuidGenerator>();//Eðer DI kullanýyorsak bu þekilde servis tanýmý yapýlarak devam edilir.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
//app.Urls.Add("https://localhost:7777");//Uygulamayý istediðimiz port üzerinden ayaða kaldýrdýk.

app.MapGet("get-example", () => "Hello Get");
app.MapPost("post-example", () => "Hello Post");

//Lamda ile tek satýrda direk dönüþ yapabiliriz.
app.MapGet("ok-object", () => Results.Ok(new { Message = "API  baþarýlý." }));

//Eðer api içerisnde bir takým iþler yapýlacaksa normka api yapýsý gibi iþlemleri yaptýktan sonra return ile istediðimiz dönüþü yapabiliriz.
app.MapGet("slow-api", async () =>
{
    await Task.Delay(1000);

    return Results.Ok(new { Message = "Slow api çalýþlýyor..." });
});

//Çok kullanýlan metodlar haricinde özellikle bir metod belirtmek istersek .MapMethods ile istediðimi kaynaðý kullanabiliriz.
app.MapMethods("options-or-head", new[] { "HEAD", "OPTIONS" }, () => "Options veya Head");

//sadece içerde deðil dýþarda tanýmlanan yapýlarýda kullanabiliriz.
var handler = () => "handler tanýmlý...";
app.MapGet("handler", handler);

//eðer bir sýnýftaki methodu kullanmak istersek bu þekilde tanýmlama yapabilriz.
app.MapGet("some-method", Example.SomeMethod);
//Eðer method parametreli ise bu þekilde parametreleri aktarabiliriz.
//app.MapGet("some-method", ()=>Example.SomeMethod());

//Apiye parametre göndermek için kullanýlýr. Ancak sadece bu þkelde verdiðimiz zmaan request için string bir deðer gönderildiði zaman hata alýnýr.
//Yapýlmasý gereken þey yapýlacak olan istek tipinin sadece hangi tipteki istendiðini belirtmektir. ({age:int}) þekilnde.
app.MapGet("get-params/{age:int}", (int age) =>
{
    return $"Yas:{age}";
});

//endpoint için beklediðim parametrede yapýma göre regex kuralý tanýmlayabiliyorum.
app.MapGet("cars/{carsId:regex(^[a-c0-8]+$)}", (string carsId) =>
{
    return $"Car Id:{carsId}";
});

//endpoint için beklediðim parametrede yapýma göre istediðim kuralý tanýmlayabiliyorum.
app.MapGet("books/{isbn:length(12)}", (string isbn) =>
{
    return $"book isbn:{isbn}";
});

//query params kullanarak servis üzerinen veriyi çektik. Peki nasýl searchTermin root params deðilde queryparams olduðunu bildi. Burada eðer people/search/{searchTerm} olsaydý rootparams olurdu.
app.MapGet("people/search", (string? searchTerm, PeopleService peopleService) =>
{
    if (string.IsNullOrEmpty(searchTerm)) return Results.NotFound();
    var result = peopleService.Search(searchTerm);
    return Results.Ok(result);
});

//routeParams ve queryParams birlikte kullanýldeý.Ýstenlirse [FromQuery(Name ="q")] etiketiyle tipini belirlebiliriz ve Name="q" ile bilikte artýk queryParams yazmamýza gerek kalmaz.
app.MapGet("mix/{routeParams}", ([FromRoute] string routeParams, [FromQuery(Name = "q")] int queryParams, [FromServices] GuidGenerator guidGenerator) =>
{
    return $"{routeParams} {queryParams} {guidGenerator.NewGuid}";
});

//bu method ile url içindeki istekleri yakalayabiliriz.
app.MapGet("http", async (HttpRequest request, HttpResponse response) =>
{
    var quryString = request.QueryString;
    await response.WriteAsync($"qurystring value:{quryString}");
});

app.MapGet("logging", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello Log");
});

app.MapControllers();
app.Run();
