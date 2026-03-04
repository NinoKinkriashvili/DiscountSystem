using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscountsSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExtraSettingsField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MerchantApprovedOfferEditWindowMinutes",
                table: "Settings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MerchantApprovedOfferEditWindowMinutes",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
