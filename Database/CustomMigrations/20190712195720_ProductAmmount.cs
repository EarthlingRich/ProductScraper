using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.CustomMigrations
{
    public partial class ProductAmmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ammount",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ammount",
                table: "Products");
        }
    }
}
