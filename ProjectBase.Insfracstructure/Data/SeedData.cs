using Microsoft.EntityFrameworkCore;
using ProjectBase.Domain.Entities;

namespace ProjectBase.Insfracstructure.Data
{
    public static class SeedData
    {
        public static ModelBuilder SeedingData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .HasData([
                    new Role {
                        Code = 1,
                        RoleName = "Admin",
                    },
                    new Role {
                        Code = 2,
                        RoleName = "Manager",
                    },
                    new Role {
                        Code = 3,
                        RoleName = "Staff",
                    },
                    new Role {
                        Code = 4,
                        RoleName = "Shipper",
                    },
                    new Role {
                        Code = 5,
                        RoleName = "Customer",
                    }]);

            modelBuilder.Entity<Branch>().HasData([
                new Branch{
                    Id = "test",
                    Name = "Default branch",
                    IsDeleted = false,
                }]);

            modelBuilder.Entity<User>()
                .HasData([
                    new User {
                        Username = "admin",
                        Email = "admin@gmail.com",
                        Fullname = "admin",
                        PasswordHash = "29bb9e004a0efd038f405392d21bfed74e800a705a708cf21e3f4a6b86fe8e47",
                        PasswordSalt = "28fd1e9ee04e431277f4aabbf620953ca374e5190e0d59404671b0324b72eaee", // 12345678
                        ResetPasswordToken = "123123123124243434",
                        VerifyToken = "",
                    },new User {
                        Username = "manager",
                        Email = "manager@gmail.com",
                        Fullname = "manager's full name",
                        PasswordHash = "29bb9e004a0efd038f405392d21bfed74e800a705a708cf21e3f4a6b86fe8e47",
                        PasswordSalt = "28fd1e9ee04e431277f4aabbf620953ca374e5190e0d59404671b0324b72eaee", // 12345678
                        ResetPasswordToken = "",
                        VerifyToken = "",
                        BranchID = "test"
                    },new User {
                        Username = "staff",
                        Email = "staff@gmail.com",
                        Fullname = "staff name",
                        PasswordHash = "29bb9e004a0efd038f405392d21bfed74e800a705a708cf21e3f4a6b86fe8e47",
                        PasswordSalt = "28fd1e9ee04e431277f4aabbf620953ca374e5190e0d59404671b0324b72eaee", // 12345678
                        ResetPasswordToken = "",
                        VerifyToken = "",
                        BranchID = "test",
                        ReportToId = "manager"
                    }]);

            modelBuilder.Entity<UserRole>()
                .HasData([
                    new UserRole {
                        Username = "admin",
                        RoleCode = 1
                    },new UserRole {
                        Username = "manager",
                        RoleCode = 2
                    },new UserRole {
                        Username = "staff",
                        RoleCode = 3
                    }]);

            return modelBuilder;
        }
    }
}
