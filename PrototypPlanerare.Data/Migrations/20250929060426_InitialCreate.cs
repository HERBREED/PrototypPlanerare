using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrototypPlanerare.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EcoNumber = table.Column<string>(type: "TEXT", nullable: true),
                    AoNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Customer = table.Column<string>(type: "TEXT", nullable: true),
                    Product = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentRevision = table.Column<string>(type: "TEXT", nullable: true),
                    NewRevision = table.Column<string>(type: "TEXT", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    OwnerMarket = table.Column<string>(type: "TEXT", nullable: true),
                    OwnerEngineering = table.Column<string>(type: "TEXT", nullable: true),
                    OwnerPurchasing = table.Column<string>(type: "TEXT", nullable: true),
                    OwnerPlanning = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Area = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    State = table.Column<string>(type: "TEXT", nullable: false),
                    Assignee = table.Column<string>(type: "TEXT", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemSteps_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemSteps_ItemId",
                table: "ItemSteps",
                column: "ItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemSteps");

            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
