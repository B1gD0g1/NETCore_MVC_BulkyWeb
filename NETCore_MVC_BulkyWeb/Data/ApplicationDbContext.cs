using Microsoft.EntityFrameworkCore;
using NETCore_MVC_BulkyWeb.Models;

namespace NETCore_MVC_BulkyWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
               
        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Seed Data
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1},
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2},
                new Category { Id = 3, Name = "History", DisplayOrder = 3}
                );
        }
    }
}
