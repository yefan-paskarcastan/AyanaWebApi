using Microsoft.EntityFrameworkCore;

namespace AyanaWebApi.Models
{
    public class AyDbContext : DbContext
    {
        public AyDbContext()
        {
        }

        public AyDbContext(DbContextOptions<AyDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        public DbSet<User> Users { get; set; }
    }
}
