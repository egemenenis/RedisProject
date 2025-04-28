using Microsoft.EntityFrameworkCore;

namespace RedisProject.API.Models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }

        public DbSet<Product> Products { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Product>().HasData(
                new Product() { Id = 1, Name = "Car 1" },
                new Product() { Id = 2, Name = "Car 2" },
                new Product() { Id = 3, Name = "Car 3" },
                new Product() { Id = 4, Name = "Car 4" });

            base.OnModelCreating(modelBuilder);
        }
    }
}
