using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrototypPlanerare.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchasingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PurchasingMaterialsEta",
                table: "Items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PurchasingPasteFileReceived",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PurchasingPcbDrawingReceived",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchasingPcbEta",
                table: "Items",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchasingMaterialsEta",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PurchasingPasteFileReceived",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PurchasingPcbDrawingReceived",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PurchasingPcbEta",
                table: "Items");
        }
    }
}
