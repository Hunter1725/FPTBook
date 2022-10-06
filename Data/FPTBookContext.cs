using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FPTBook.Models;

    public class FPTBookContext : DbContext
    {
        public FPTBookContext (DbContextOptions<FPTBookContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder){
            modelBuilder.Entity<FPTBook.Models.Book>()
            .Property(p => p.Price).HasColumnType("decimal(18,4)");
        }

        public DbSet<FPTBook.Models.Category> Category { get; set; } = default!;

        public DbSet<FPTBook.Models.Book>? Book { get; set; }

        public DbSet<FPTBook.Models.Author>? Author { get; set; }

        public DbSet<FPTBook.Models.Publisher>? Publisher { get; set; }

        public DbSet<FPTBook.Models.User>? User { get; set; }

    }
