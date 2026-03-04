using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscountsSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCouponCodeToCouponPurchases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Add as nullable first (so existing rows won't all get the same value)
            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "CouponPurchases",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            // 2) Backfill existing rows with unique values
            migrationBuilder.Sql(@"
                UPDATE cp
                SET CouponCode = CONCAT('CPN-', UPPER(REPLACE(CONVERT(varchar(36), NEWID()), '-', '')))
                FROM CouponPurchases cp
                WHERE cp.CouponCode IS NULL OR cp.CouponCode = '';
            ");

            // 3) Make column required
            migrationBuilder.AlterColumn<string>(
                name: "CouponCode",
                table: "CouponPurchases",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            // 4) Create unique index
            migrationBuilder.CreateIndex(
                name: "IX_CouponPurchases_CouponCode",
                table: "CouponPurchases",
                column: "CouponCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CouponPurchases_CouponCode",
                table: "CouponPurchases");

            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "CouponPurchases");
        }
    }
}