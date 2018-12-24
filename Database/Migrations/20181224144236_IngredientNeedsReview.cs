using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class IngredientNeedsReview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NeedsReview",
                table: "Ingredients",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NeedsReview",
                table: "Ingredients");
        }
    }
}
