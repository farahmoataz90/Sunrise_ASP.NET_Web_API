using Microsoft.EntityFrameworkCore;
using Sunrise.Models;

namespace Sunrise.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }


    }
}
