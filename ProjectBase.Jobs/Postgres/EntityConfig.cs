using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectBase.Jobs.Core.Entities;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Jobs.Postgres
{
    [ExcludeFromCodeCoverage]
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(x => x.Id).HasDefaultValue(Guid.NewGuid().ToString());
            builder.Property(x => x.Code).ValueGeneratedOnAdd();

            builder.Property(x => x.Status).HasDefaultValue(Status.Disable);
            builder.Property(u => u.Price).HasDefaultValue(0);
            builder.Property(u => u.Quantity).HasDefaultValue(0);
            builder.Property(u => u.IsDeleted).HasDefaultValue(false);

            builder.HasQueryFilter(x => !x.IsDeleted);
            // add index to delete field to improve performance

            //builder.HasOne(r => r.Creator)
            //    .WithMany()
            //    .HasForeignKey(x => x.CreatorUsername);
        }
    }

    [ExcludeFromCodeCoverage]
    public class BillConfig : IEntityTypeConfiguration<Bill>
    {
        public void Configure(EntityTypeBuilder<Bill> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(x => x.Id).HasDefaultValue(Guid.NewGuid().ToString());

            builder.Property(x => x.CreateAt).HasDefaultValue(DateTime.UtcNow);
            builder.Property(u => u.DiscountPrice).HasDefaultValue(0);
            builder.Property(u => u.TotalPrice).HasDefaultValue(0);

            builder.HasMany(x => x.BillDetails)
                .WithOne()
                .HasForeignKey(e => e.BillId);

            //builder
            //    .HasOne(x => x.User)
            //    .WithMany(e => e.Bills)
            //    .HasForeignKey(e => e.Username);
        }
    }

    [ExcludeFromCodeCoverage]
    public class BillDetailsConfig : IEntityTypeConfiguration<BillDetails>
    {
        public void Configure(EntityTypeBuilder<BillDetails> builder)
        {
            builder.HasKey(r => new { r.BillId, r.ProductName });
        }
    }

    [ExcludeFromCodeCoverage]
    public class StatisticBillsConfig : IEntityTypeConfiguration<StatisticBill>
    {
        public void Configure(EntityTypeBuilder<StatisticBill> builder)
        {
            builder.HasKey(r => r.Date);
            builder.Property(x => x.Date).HasDefaultValue(DateOnly.FromDateTime(DateTime.UtcNow));

            builder.Property(x => x.Revenue).IsRequired().HasDefaultValue(0);
            builder.Property(x => x.BillQuantity).IsRequired().HasDefaultValue(0);
            builder.Property(x => x.ProductQuantity).IsRequired().HasDefaultValue(0);
        }
    }

    [ExcludeFromCodeCoverage]
    public class BranchConfig : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(x => x.Id).HasDefaultValue(Guid.NewGuid().ToString());

            builder.Property<int>(x => x.Code).ValueGeneratedOnAdd();
            builder.HasQueryFilter(x => !x.IsDeleted);

        }
    }
}
