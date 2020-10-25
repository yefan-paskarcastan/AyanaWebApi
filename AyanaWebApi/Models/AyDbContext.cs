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
        public DbSet<NLog> NLogs { get; set; }

        public DbSet<RutorListItem> RutorListItems { get; set; }
        public DbSet<RutorItem> RutorItems { get; set; }
        public DbSet<RutorItemSpoiler> RutorItemSpoilers { get; set; }
        public DbSet<RutorItemImg> RutorItemImgs { get; set; }

        public DbSet<NnmclubListItem> NnmclubListItems { get; set; }
        public DbSet<NnmclubItem> NnmclubItems { get; set; }
        public DbSet<NnmclubItemSpoiler> NnmclubItemSpoilers { get; set; }
        public DbSet<NnmclubItemImg> NnmclubItemImgs { get; set; }

        public DbSet<SoftPost> SoftPosts { get; set; }
        public DbSet<SoftResult> SoftResults { get; set; }
        public DbSet<DictionaryValue> DictionaryValues { get; set; }

        #region Tables of setting
        public DbSet<RutorCheckListInput> RutorCheckListInputs { get; set; }
        public DbSet<RutorParseItemInput> RutorParseItemInputs { get; set; }
        public DbSet<NnmclubCheckListInput> NnmclubCheckListInputs { get; set; }
        /*public DbSet<NnmclubParseItemInput> NnmclubParseItemInputs { get; set; }*/
        public DbSet<DriverToSoftInput> DriverToSoftInputs { get; set; }
        public DbSet<ImghostParsingInput> ImghostParsingInputs { get; set; }
        public DbSet<SoftPostInput> SoftPostInputs { get; set; }
        #endregion
    }
}
