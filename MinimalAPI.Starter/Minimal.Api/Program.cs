using Microsoft.AspNetCore.Mvc;
using Minimal.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<PeopleService>();//E�er DI kullan�yorsak bu �ekilde servis tan�m� yap�larak devam edilir.
builder.Services.AddScoped<GuidGenerator>();//E�er DI kullan�yorsak bu �ekilde servis tan�m� yap�larak devam edilir.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
//app.Urls.Add("https://localhost:7777");//Uygulamay� istedi�imiz port �zerinden aya�a kald�rd�k.

app.MapGet("get-example", () => "Hello Get");
app.MapPost("post-example", () => "Hello Post");

//Lamda ile tek sat�rda direk d�n�� yapabiliriz.
app.MapGet("ok-object", () => Results.Ok(new { Message = "API  ba�ar�l�." }));

//E�er api i�erisnde bir tak�m i�ler yap�lacaksa normka api yap�s� gibi i�lemleri yapt�ktan sonra return ile istedi�imiz d�n��� yapabiliriz.
app.MapGet("slow-api", async () =>
{
    await Task.Delay(1000);

    return Results.Ok(new { Message = "Slow api �al��l�yor..." });
});

//�ok kullan�lan metodlar haricinde �zellikle bir metod belirtmek istersek .MapMethods ile istedi�imi kayna�� kullanabiliriz.
app.MapMethods("options-or-head", new[] { "HEAD", "OPTIONS" }, () => "Options veya Head");

//sadece i�erde de�il d��arda tan�mlanan yap�lar�da kullanabiliriz.
var handler = () => "handler tan�ml�...";
app.MapGet("handler", handler);

//e�er bir s�n�ftaki methodu kullanmak istersek bu �ekilde tan�mlama yapabilriz.
app.MapGet("some-method", Example.SomeMethod);
//E�er method parametreli ise bu �ekilde parametreleri aktarabiliriz.
//app.MapGet("some-method", ()=>Example.SomeMethod());

//Apiye parametre g�ndermek i�in kullan�l�r. Ancak sadece bu �kelde verdi�imiz zmaan request i�in string bir de�er g�nderildi�i zaman hata al�n�r.
//Yap�lmas� gereken �ey yap�lacak olan istek tipinin sadece hangi tipteki istendi�ini belirtmektir. ({age:int}) �ekilnde.
app.MapGet("get-params/{age:int}", (int age) =>
{
    return $"Yas:{age}";
});

//endpoint i�in bekledi�im parametrede yap�ma g�re regex kural� tan�mlayabiliyorum.
app.MapGet("cars/{carsId:regex(^[a-c0-8]+$)}", (string carsId) =>
{
    return $"Car Id:{carsId}";
});

//endpoint i�in bekledi�im parametrede yap�ma g�re istedi�im kural� tan�mlayabiliyorum.
app.MapGet("books/{isbn:length(12)}", (string isbn) =>
{
    return $"book isbn:{isbn}";
});

//query params kullanarak servis �zerinen veriyi �ektik. Peki nas�l searchTermin root params de�ilde queryparams oldu�unu bildi. Burada e�er people/search/{searchTerm} olsayd� rootparams olurdu.
app.MapGet("people/search", (string? searchTerm, PeopleService peopleService) =>
{
    if (string.IsNullOrEmpty(searchTerm)) return Results.NotFound();
    var result = peopleService.Search(searchTerm);
    return Results.Ok(result);
});

//routeParams ve queryParams birlikte kullan�lde�.�stenlirse [FromQuery(Name ="q")] etiketiyle tipini belirlebiliriz ve Name="q" ile bilikte art�k queryParams yazmam�za gerek kalmaz.
app.MapGet("mix/{routeParams}", ([FromRoute] string routeParams, [FromQuery(Name = "q")] int queryParams, [FromServices] GuidGenerator guidGenerator) =>
{
    return $"{routeParams} {queryParams} {guidGenerator.NewGuid}";
});

//bu method ile url i�indeki istekleri yakalayabiliriz.
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
