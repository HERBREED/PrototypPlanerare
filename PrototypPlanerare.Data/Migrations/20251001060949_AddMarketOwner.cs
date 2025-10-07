using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrototypPlanerare.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MarketOwner",
                table: "Items",
                type: "TEXT",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarketOwner",
                table: "Items");
        }
    }
}
