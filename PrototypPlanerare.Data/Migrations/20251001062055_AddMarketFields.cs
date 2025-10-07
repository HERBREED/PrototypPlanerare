using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrototypPlanerare.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MarketKickoffDate",
                table: "Items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarketNotes",
                table: "Items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarketQuoteNumber",
                table: "Items",
                type: "TEXT",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarketQuoteApproved",
                table: "Items",
                type: "TEXT",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MarketQuoted",
                table: "Items",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarketKickoffDate",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MarketNotes",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MarketQuoteNumber",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MarketQuoteApproved",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MarketQuoted",
                table: "Items");
        }
    }
}
