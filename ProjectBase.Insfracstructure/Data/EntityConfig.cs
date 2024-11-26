using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectBase.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Insfracstructure.Data
{
    [ExcludeFromCodeCoverage]
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Username);
            builder.Property(x => x.CreateAt).HasDefaultValue(DateTime.UtcNow);

            builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Fullname).IsRequired().HasMaxLength(200);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(200);

            builder.Property(u => u.Type).IsRequired(false);

            builder.Property(u => u.PhoneNumber).HasMaxLength(50).HasDefaultValue("").IsRequired(false);
            builder.Property(u => u.Bio).HasMaxLength(150).HasDefaultValue("").IsRequired(false);

            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(100);
            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(100);

            builder.Property(u => u.IsAccountBlocked).HasDefaultValue(false);
            builder.Property(u => u.IsEmailConfirmed).HasDefaultValue(false);

            builder.HasOne(x => x.Branch)
                .WithMany(x => x.Users)
                .HasForeignKey(u => u.BranchID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.ReportTo)
                .WithOne()
                .HasForeignKey<User>(x => x.ReportToId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    [ExcludeFromCodeCoverage]
    public class UserRoleConfig : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey(r => new { r.RoleCode, r.Username });
            builder.Property(x => x.CreateAt).HasDefaultValue(DateTime.UtcNow);

            builder.Property(r => r.Username).IsRequired();
            builder.Property(r => r.RoleCode).IsRequired();

            builder.HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleCode)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    [ExcludeFromCodeCoverage]
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Code);
            builder.Property<int>(x => x.Code).ValueGeneratedOnAdd();

            builder.Property(x => x.CreateAt).HasDefaultValue(DateTime.UtcNow);
            builder.Property(r => r.RoleName).IsRequired();

        }
    }

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

            builder.HasOne(r => r.Creator)
                .WithMany()
                .HasForeignKey(x => x.CreatorUsername);

            builder.HasOne(r => r.ProductType)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.ProductTypeCode)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    [ExcludeFromCodeCoverage]
    public class ProductTypeConfig : IEntityTypeConfiguration<ProductType>
    {
        public void Configure(EntityTypeBuilder<ProductType> builder)
        {
            builder.HasKey(r => r.Code);
            builder.Property(x => x.Code).ValueGeneratedOnAdd();
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }

    [ExcludeFromCodeCoverage]
    public class BlacklistConfig : IEntityTypeConfiguration<Blacklist>
    {
        public void Configure(EntityTypeBuilder<Blacklist> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(x => x.Id).HasDefaultValue(Guid.NewGuid().ToString());
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

            builder.Property(u => u.CustomerUsername).IsRequired(false);
            builder.Property(u => u.Note).IsRequired(false);

            builder.HasMany(x => x.BillDetails)
                .WithOne()
                .HasForeignKey(e => e.BillId);

            builder
                .HasOne(x => x.User)
                .WithMany(e => e.Bills)
                .HasForeignKey(e => e.Username);

            builder
                .HasOne(x => x.Shipper)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(e => e.ShipperUsername)
                .OnDelete(DeleteBehavior.Cascade);
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
    public class CouponConfig : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(x => x.Id).HasDefaultValue(Guid.NewGuid().ToString());

            builder.Property(x => x.CreateAt).HasDefaultValue(DateTime.UtcNow);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }

    [ExcludeFromCodeCoverage]
    public class ProductCouponConfig : IEntityTypeConfiguration<ProductCoupon>
    {
        public void Configure(EntityTypeBuilder<ProductCoupon> builder)
        {
            builder.HasKey(r => new { r.ProductId, r.CouponId });

            builder.HasOne(x => x.Product)
                .WithOne()
                .HasForeignKey<ProductCoupon>(x => x.ProductId);

            builder.HasOne(x => x.Coupon)
                .WithOne()
                .HasForeignKey<ProductCoupon>(x => x.CouponId);
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

    [ExcludeFromCodeCoverage]
    public class ProductOnSaleConfig : IEntityTypeConfiguration<ProductOnSale>
    {
        public void Configure(EntityTypeBuilder<ProductOnSale> builder)
        {
            builder.HasKey(r => new { r.ProductId, r.BranchId });
        }
    }
}
