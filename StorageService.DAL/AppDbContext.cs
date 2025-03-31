using Microsoft.EntityFrameworkCore;
using StorageService.Abstraction.Models;

namespace StorageService.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<ArbitrageDifference> ArbitrageDifferences { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
