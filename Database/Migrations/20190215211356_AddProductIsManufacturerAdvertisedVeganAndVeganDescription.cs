using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class AddProductIsManufacturerAdvertisedVeganAndVeganDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsManufacturerAdvertisedVegan",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VeganDescription",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsManufacturerAdvertisedVegan",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "VeganDescription",
                table: "Products");
        }
    }
}
