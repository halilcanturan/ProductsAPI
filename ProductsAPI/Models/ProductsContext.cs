using Microsoft.EntityFrameworkCore;

namespace ProductsAPI.Models
{
    public class ProductsContext : DbContext
    {
        public ProductsContext(DbContextOptions<ProductsContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) //seed datanın basit kullanımı
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, ProductName = "Iphone 14", Price = 2000, IsActive = true },
                new Product { ProductId = 2, ProductName = "Iphone 15", Price = 3000, IsActive = true },
                new Product { ProductId = 3, ProductName = "Iphone 16", Price = 4000, IsActive = true },
                new Product { ProductId = 4, ProductName = "Iphone 17", Price = 5000, IsActive = true },
                new Product { ProductId = 5, ProductName = "Iphone 18", Price = 9000, IsActive = true }
                );
        }
        public DbSet<Product> Products { get; set; }
    }
}
