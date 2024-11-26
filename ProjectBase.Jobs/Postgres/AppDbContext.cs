using Microsoft.EntityFrameworkCore;
using ProjectBase.Jobs.Core.Entities;

namespace ProjectBase.Jobs.Postgres
{
    public class AppDbContext : DbContext
    {
        public DbSet<StatisticBill> StatisticBills { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillDetails> BillDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ProductConfig());
            modelBuilder.ApplyConfiguration(new BillConfig());
            modelBuilder.ApplyConfiguration(new BillDetailsConfig());
            modelBuilder.ApplyConfiguration(new BranchConfig());
            modelBuilder.ApplyConfiguration(new StatisticBillsConfig());
        }
    }
}
