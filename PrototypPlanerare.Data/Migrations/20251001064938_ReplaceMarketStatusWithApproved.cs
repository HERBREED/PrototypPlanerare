using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrototypPlanerare.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceMarketStatusWithApproved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "MarketQuoteApproved",
                table: "Items",
                type: "INTEGER",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 64,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MarketQuoteApproved",
                table: "Items",
                type: "TEXT",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldMaxLength: 64,
                oldNullable: true);
        }
    }
}
