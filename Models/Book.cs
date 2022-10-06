using System.ComponentModel.DataAnnotations;

namespace FPTBook.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? AuthorID { get; set; }
        public Author? Author { get; set; }
        public int? PublisherID{ get; set; }
        public Publisher? Publisher { get; set; }
        public string? Poster{get; set;}
        public int CategoryID { get; set; }
        public Category? Category { get; set; }
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public decimal Price { get; set; }
    }
}