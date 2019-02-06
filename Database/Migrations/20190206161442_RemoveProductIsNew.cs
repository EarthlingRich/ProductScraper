using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class RemoveProductIsNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNew",
                table: "Products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNew",
                table: "Products",
                nullable: false,
                defaultValue: false);
        }
    }
}
