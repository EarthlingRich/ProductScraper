using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class ExposeProductActivities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductActivity_Products_ProductId",
                table: "ProductActivity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductActivity",
                table: "ProductActivity");

            migrationBuilder.RenameTable(
                name: "ProductActivity",
                newName: "ProductActivities");

            migrationBuilder.RenameIndex(
                name: "IX_ProductActivity_ProductId",
                table: "ProductActivities",
                newName: "IX_ProductActivities_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductActivities",
                table: "ProductActivities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductActivities_Products_ProductId",
                table: "ProductActivities",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductActivities_Products_ProductId",
                table: "ProductActivities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductActivities",
                table: "ProductActivities");

            migrationBuilder.RenameTable(
                name: "ProductActivities",
                newName: "ProductActivity");

            migrationBuilder.RenameIndex(
                name: "IX_ProductActivities_ProductId",
                table: "ProductActivity",
                newName: "IX_ProductActivity_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductActivity",
                table: "ProductActivity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductActivity_Products_ProductId",
                table: "ProductActivity",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
