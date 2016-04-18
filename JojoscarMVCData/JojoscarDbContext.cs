using JojoscarMVCCommun;
using System.Data.Entity;

namespace JojoscarMVCDBLayer
{
    public class JojoscarDbContext : DbContext
    {
        public JojoscarDbContext() : base("Jojoscar" + 2016) { }
        public JojoscarDbContext(int year) : base("Jojoscar" + year)  { }

        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<CategoryNomineeModel> CategoryNominees { get; set; }
        public DbSet<GuestModel> Guests { get; set; }
        public DbSet<TableModel> Tables { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new CategoryMapping());
            modelBuilder.Configurations.Add(new CategoryNomineeMapping());
            modelBuilder.Configurations.Add(new GuestMapping());
            modelBuilder.Configurations.Add(new TableMapping());
        }
    }
}