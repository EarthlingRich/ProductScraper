using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class AddVeganType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VeganType",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE dbo.Products SET VeganType = 1 WHERE IsVegan = 1");

            migrationBuilder.DropColumn(
                name: "IsVegan",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "VeganType",
                table: "Ingredients",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVegan",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE dbo.Products SET IsVegan = 1 WHERE VeganType = 1");

            migrationBuilder.DropColumn(
                name: "VeganType",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "VeganType",
                table: "Ingredients");
        }
    }
}
