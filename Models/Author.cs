namespace FPTBook.Models;
using System.ComponentModel.DataAnnotations;
public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<Book>? Books { get; set; }
}