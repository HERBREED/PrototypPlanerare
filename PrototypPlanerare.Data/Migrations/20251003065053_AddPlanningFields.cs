using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrototypPlanerare.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanningFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PlanningConfirmedDeliveryDate",
                table: "Items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanningDeliveredQty",
                table: "Items",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PlanningExcessMaterial",
                table: "Items",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PlanningInvoiced",
                table: "Items",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PlanningOrderReceived",
                table: "Items",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlanningOwner",
                table: "Items",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlanningConfirmedDeliveryDate",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PlanningDeliveredQty",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PlanningExcessMaterial",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PlanningInvoiced",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PlanningOrderReceived",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PlanningOwner",
                table: "Items");
        }
    }
}
