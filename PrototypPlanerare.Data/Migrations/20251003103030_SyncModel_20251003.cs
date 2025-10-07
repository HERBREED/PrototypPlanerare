using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrototypPlanerare.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel_20251003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemSteps_Items_ItemId",
                table: "ItemSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemSteps",
                table: "ItemSteps");

            migrationBuilder.RenameTable(
                name: "ItemSteps",
                newName: "ItemStep");

            migrationBuilder.RenameIndex(
                name: "IX_ItemSteps_ItemId",
                table: "ItemStep",
                newName: "IX_ItemStep_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemStep",
                table: "ItemStep",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemStep_Items_ItemId",
                table: "ItemStep",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemStep_Items_ItemId",
                table: "ItemStep");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemStep",
                table: "ItemStep");

            migrationBuilder.RenameTable(
                name: "ItemStep",
                newName: "ItemSteps");

            migrationBuilder.RenameIndex(
                name: "IX_ItemStep_ItemId",
                table: "ItemSteps",
                newName: "IX_ItemSteps_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemSteps",
                table: "ItemSteps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemSteps_Items_ItemId",
                table: "ItemSteps",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
