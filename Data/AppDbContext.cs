using Microsoft.EntityFrameworkCore;

namespace MyBackend.Data
{
    public class AppDbContext : DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        //public DbSet<Product> products => Set<Product>();
    }
}