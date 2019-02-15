using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class RenameIsStoreAdvertisedVegan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StoreAdvertisedVegan",
                table: "Products",
                newName: "IsStoreAdvertisedVegan");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsStoreAdvertisedVegan",
                table: "Products",
                newName: "StoreAdvertisedVegan");
        }
    }
}
