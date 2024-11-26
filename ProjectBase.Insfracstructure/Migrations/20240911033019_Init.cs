using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectBase.Insfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blacklists",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false, defaultValue: "68b90fbd-9a87-4277-809f-73422c634c09"),
                    Token = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blacklists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false, defaultValue: "6e0ce4ef-172b-481f-9429-a7efb0e02789"),
                    Code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false, defaultValue: "cf90d3ea-8126-4f15-ac50-4808c7146ce1"),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 9, 11, 3, 30, 18, 742, DateTimeKind.Utc).AddTicks(1388))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductOnSales",
                columns: table => new
                {
                    ProductId = table.Column<string>(type: "text", nullable: false),
                    BranchId = table.Column<string>(type: "text", nullable: false),
                    IsOnSale = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOnSales", x => new { x.ProductId, x.BranchId });
                });

            migrationBuilder.CreateTable(
                name: "ProductTypes",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Desc = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 9, 11, 3, 30, 18, 737, DateTimeKind.Utc).AddTicks(4667)),
                    RoleName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "StatisticBills",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false, defaultValue: new DateOnly(2024, 9, 11)),
                    Revenue = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    BillQuantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ProductQuantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticBills", x => x.Date);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordSalt = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValue: ""),
                    Fullname = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Bio = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true, defaultValue: ""),
                    VerifyToken = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 9, 11, 3, 30, 18, 735, DateTimeKind.Utc).AddTicks(6238)),
                    ResetPasswordToken = table.Column<string>(type: "text", nullable: true),
                    ResetPasswordTokenExpiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TokenExpiredTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: true),
                    IsEmailConfirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsAccountBlocked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    BranchID = table.Column<string>(type: "text", nullable: true),
                    ReportToId = table.Column<string>(type: "character varying(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                    table.ForeignKey(
                        name: "FK_Users_Branches_BranchID",
                        column: x => x.BranchID,
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Users_ReportToId",
                        column: x => x.ReportToId,
                        principalTable: "Users",
                        principalColumn: "Username");
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false, defaultValue: "1b20e912-0e35-44c6-be51-3120ae5048cd"),
                    TotalPrice = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    DiscountPrice = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ShipperUsername = table.Column<string>(type: "character varying(100)", nullable: true),
                    Username = table.Column<string>(type: "character varying(100)", nullable: false),
                    CustomerUsername = table.Column<string>(type: "text", nullable: true),
                    CustomerFullname = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    Log = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 9, 11, 3, 30, 18, 737, DateTimeKind.Utc).AddTicks(8837))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bills_Users_ShipperUsername",
                        column: x => x.ShipperUsername,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bills_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false, defaultValue: "abd93545-3485-471c-9da0-de47fd0602e4"),
                    Code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    Quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Desc = table.Column<string>(type: "text", nullable: true),
                    CreatorUsername = table.Column<string>(type: "character varying(100)", nullable: false),
                    ProductTypeCode = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_ProductTypes_ProductTypeCode",
                        column: x => x.ProductTypeCode,
                        principalTable: "ProductTypes",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_Products_Users_CreatorUsername",
                        column: x => x.CreatorUsername,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Username = table.Column<string>(type: "character varying(100)", nullable: false),
                    RoleCode = table.Column<int>(type: "integer", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 9, 11, 3, 30, 18, 736, DateTimeKind.Utc).AddTicks(3960))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.RoleCode, x.Username });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleCode",
                        column: x => x.RoleCode,
                        principalTable: "Roles",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username");
                });

            migrationBuilder.CreateTable(
                name: "BillDetails",
                columns: table => new
                {
                    BillId = table.Column<string>(type: "text", nullable: false),
                    ProductName = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillDetails", x => new { x.BillId, x.ProductName });
                    table.ForeignKey(
                        name: "FK_BillDetails_Bills_BillId",
                        column: x => x.BillId,
                        principalTable: "Bills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductCoupons",
                columns: table => new
                {
                    ProductId = table.Column<string>(type: "text", nullable: false),
                    CouponId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCoupons", x => new { x.ProductId, x.CouponId });
                    table.ForeignKey(
                        name: "FK_ProductCoupons_Coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCoupons_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Branches",
                columns: new[] { "Id", "CreateAt", "IsDeleted", "Name" },
                values: new object[] { "test", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Default branch" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Code", "RoleName" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Manager" },
                    { 3, "Staff" },
                    { 4, "Shipper" },
                    { 5, "Customer" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Username", "Bio", "BranchID", "Email", "Fullname", "PasswordHash", "PasswordSalt", "PhoneNumber", "ReportToId", "ResetPasswordToken", "ResetPasswordTokenExpiredAt", "TokenExpiredTime", "Type", "VerifyToken" },
                values: new object[] { "admin", "", null, "admin@gmail.com", "admin", "29bb9e004a0efd038f405392d21bfed74e800a705a708cf21e3f4a6b86fe8e47", "28fd1e9ee04e431277f4aabbf620953ca374e5190e0d59404671b0324b72eaee", "", null, "123123123124243434", null, null, null, "" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleCode", "Username" },
                values: new object[] { 1, "admin" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Username", "Bio", "BranchID", "Email", "Fullname", "PasswordHash", "PasswordSalt", "PhoneNumber", "ReportToId", "ResetPasswordToken", "ResetPasswordTokenExpiredAt", "TokenExpiredTime", "Type", "VerifyToken" },
                values: new object[] { "manager", "", "test", "manager@gmail.com", "manager's full name", "29bb9e004a0efd038f405392d21bfed74e800a705a708cf21e3f4a6b86fe8e47", "28fd1e9ee04e431277f4aabbf620953ca374e5190e0d59404671b0324b72eaee", "", null, "", null, null, null, "" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleCode", "Username" },
                values: new object[] { 2, "manager" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Username", "Bio", "BranchID", "Email", "Fullname", "PasswordHash", "PasswordSalt", "PhoneNumber", "ReportToId", "ResetPasswordToken", "ResetPasswordTokenExpiredAt", "TokenExpiredTime", "Type", "VerifyToken" },
                values: new object[] { "staff", "", "test", "staff@gmail.com", "staff name", "29bb9e004a0efd038f405392d21bfed74e800a705a708cf21e3f4a6b86fe8e47", "28fd1e9ee04e431277f4aabbf620953ca374e5190e0d59404671b0324b72eaee", "", "manager", "", null, null, null, "" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleCode", "Username" },
                values: new object[] { 3, "staff" });

            migrationBuilder.CreateIndex(
                name: "IX_Bills_ShipperUsername",
                table: "Bills",
                column: "ShipperUsername");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_Username",
                table: "Bills",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCoupons_CouponId",
                table: "ProductCoupons",
                column: "CouponId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCoupons_ProductId",
                table: "ProductCoupons",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatorUsername",
                table: "Products",
                column: "CreatorUsername");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductTypeCode",
                table: "Products",
                column: "ProductTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_Username",
                table: "UserRoles",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BranchID",
                table: "Users",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ReportToId",
                table: "Users",
                column: "ReportToId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillDetails");

            migrationBuilder.DropTable(
                name: "Blacklists");

            migrationBuilder.DropTable(
                name: "ProductCoupons");

            migrationBuilder.DropTable(
                name: "ProductOnSales");

            migrationBuilder.DropTable(
                name: "StatisticBills");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "ProductTypes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Branches");
        }
    }
}
