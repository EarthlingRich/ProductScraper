using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class FixProductActivityCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductActivity_Products_ProductId",
                table: "ProductActivity");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductActivity_Products_ProductId",
                table: "ProductActivity",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductActivity_Products_ProductId",
                table: "ProductActivity");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductActivity_Products_ProductId",
                table: "ProductActivity",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
