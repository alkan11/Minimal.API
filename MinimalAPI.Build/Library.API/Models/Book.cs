using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Library.API.Models
{
    public sealed class Book
    {
        [Key]
        public string Isbn { get; set; }
        public string Title { get; set; }
        public string ShortDesc { get; set; }
        public int PageCount { get; set; }
        public DateTime PublishDate { get; set; }

    }
}
