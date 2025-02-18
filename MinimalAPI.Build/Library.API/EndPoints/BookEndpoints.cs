using Library.API.Models;
using Library.API.Services.Abstract;
using Library.API.Services;
using Library.API.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;

namespace Library.API.EndPoints
{
    public static class BookEndpoints
    {
        public static void UseBookEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("login", (JwtProvider jwtProvider) =>
            {
                return Results.Ok(new { Token = jwtProvider.CreateToken() });
            }).WithTags("Login");

            app.MapPost("books", async (Book book, [FromServices] IBookService bookService) =>
            {
                BookValidator validator = new();
                ValidationResult validationResult = validator.Validate(book);

                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
                }
                var result = await bookService.CreateAsync(book);
                if (!result) return Results.BadRequest("Hata oluştu");

                return Results.Ok(result);
            });

            //[Authorize] ile kapattık ve token kullanımını gördük
            app.MapGet("books", [Authorize] async (IBookService bookService, CancellationToken cancellationToken) =>
            {
                return await bookService.GetAllAsync(cancellationToken);
            });

            app.MapGet("searchBook", async (string search, IBookService bookService) =>
            {
                var result = await bookService.GetByIsbnAsync(search);
                if (result is not Book) return Results.NotFound();

                return Results.Ok(result);
            });
        }
    }
}
