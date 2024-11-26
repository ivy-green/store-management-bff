using Microsoft.EntityFrameworkCore;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Insfracstructure.Data
{
    [ExcludeFromCodeCoverage]
    public class AppDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillDetails> BillDetails { get; set; }
        public DbSet<Blacklist> Blacklists { get; set; }
        public DbSet<StatisticBill> StatisticBills { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<ProductCoupon> ProductCoupons { get; set; }
        public DbSet<ProductOnSale> ProductOnSales { get; set; }
        public DbSet<Branch> Branches { get; set; }

        public AppDBContext(DbContextOptions options) : base(options)
        {
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SoftDeleteInterceptor();

            return base.SaveChangesAsync(cancellationToken);
        }

        private void SoftDeleteInterceptor()
        {
            var softDeleteEntry = ChangeTracker
                            .Entries<ISoftDelete>()
                            .Where(x => x.State == EntityState.Deleted);

            foreach (var entityEntry in softDeleteEntry)
            {
                entityEntry.State = EntityState.Modified;
                entityEntry.Property(nameof(ISoftDelete.IsDeleted)).CurrentValue = true;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new UserRoleConfig());
            modelBuilder.ApplyConfiguration(new RoleConfig());
            modelBuilder.ApplyConfiguration(new BillDetailsConfig());
            modelBuilder.ApplyConfiguration(new BillConfig());
            modelBuilder.ApplyConfiguration(new ProductConfig());
            modelBuilder.ApplyConfiguration(new ProductTypeConfig());
            modelBuilder.ApplyConfiguration(new BlacklistConfig());
            modelBuilder.ApplyConfiguration(new StatisticBillsConfig());
            modelBuilder.ApplyConfiguration(new CouponConfig());
            modelBuilder.ApplyConfiguration(new ProductCouponConfig());
            modelBuilder.ApplyConfiguration(new BranchConfig());
            modelBuilder.ApplyConfiguration(new ProductOnSaleConfig());

            modelBuilder.SeedingData();
        }
    }
}
