using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class ChangeKeyWordToKeyword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AllergyKeyWords",
                table: "Ingredients",
                newName: "AllergyKeywords");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AllergyKeywords",
                table: "Ingredients",
                newName: "AllergyKeyWords");
        }
    }
}
