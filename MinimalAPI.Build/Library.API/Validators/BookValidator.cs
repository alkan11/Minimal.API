using FluentValidation;
using Library.API.Models;

namespace Library.API.Validators
{
    public sealed class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(x => x.Isbn).MinimumLength(13).MaximumLength(13);
            RuleFor(x => x.Title).MinimumLength(3);
            RuleFor(x=>x.ShortDesc).MinimumLength(3);
            RuleFor(x=>x.PageCount).GreaterThan(10);
        }
    }
}
