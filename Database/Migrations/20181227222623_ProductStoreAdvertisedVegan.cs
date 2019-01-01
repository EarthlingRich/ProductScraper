using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class ProductStoreAdvertisedVegan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "StoreAdvertisedVegan",
                table: "Products",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreAdvertisedVegan",
                table: "Products");
        }
    }
}
