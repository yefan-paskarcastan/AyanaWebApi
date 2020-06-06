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

        public DbSet<Log> Logs { get; set; }

        public DbSet<RutorListItem> RutorListItems { get; set; }

        public DbSet<RutorItem> RutorItems { get; set; }

        public DbSet<RutorItemSpoiler> RutorItemSpoilers { get; set; }

        public DbSet<RutorItemImg> RutorItemImgs { get; set; }
    }
}
