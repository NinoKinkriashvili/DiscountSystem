using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscountsSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMerchantEditWindowToSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MerchantApprovedOfferEditWindowMinutes",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAtUtc",
                table: "Offers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MerchantApprovedOfferEditWindowMinutes",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ApprovedAtUtc",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "Offers");
        }
    }
}
