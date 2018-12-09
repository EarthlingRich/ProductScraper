using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class AddProductIsVegan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVegan",
                table: "Products",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVegan",
                table: "Products");
        }
    }
}
