using FPTBook.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FPTBook.Areas.Identity.Data;

public class FPTBookIdentityDbContext : IdentityDbContext<BookUser>
{
    public FPTBookIdentityDbContext(DbContextOptions<FPTBookIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
       base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<FPTBook.Models.Book>()
             .Property(p => p.Price).HasColumnType("decimal(18,4)");
        builder.Entity<FPTBook.Models.Author>()
             .HasIndex(p => p.Name).IsUnique();

    }

    public DbSet<FPTBook.Models.Category> Category { get; set; } = default!;

    public DbSet<FPTBook.Models.Book>? Book { get; set; }

    public DbSet<FPTBook.Models.Author>? Author { get; set; }

    public DbSet<FPTBook.Models.Publisher>? Publisher { get; set; }

    public DbSet<FPTBook.Models.Order> Order { get; set; }
    public DbSet<FPTBook.Models.OrderItem> OrderItem { get; set; }
}
