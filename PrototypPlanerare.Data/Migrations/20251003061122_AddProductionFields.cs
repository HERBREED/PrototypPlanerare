using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrototypPlanerare.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ProductionHasEftermontering",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProductionHasPrep",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProductionHasSmtPrimary",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProductionHasSmtSecondary",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProductionHasTest",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProductionHasTht",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProductionStartDate",
                table: "Items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductionToNumber",
                table: "Items",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductionHasEftermontering",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ProductionHasPrep",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ProductionHasSmtPrimary",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ProductionHasSmtSecondary",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ProductionHasTest",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ProductionHasTht",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ProductionStartDate",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ProductionToNumber",
                table: "Items");
        }
    }
}
