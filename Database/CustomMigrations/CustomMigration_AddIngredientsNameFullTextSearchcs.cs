using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Model;

namespace Database.CustomMigrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("CustomMigration_AddIngredientsNameFullTextSearch")]
    public class AddIngredientsNameFullTextSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG IngredientsFullTextCatalog", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON [dbo].[Ingredients] ([Name]) KEY INDEX PK_Ingredients ON IngredientsFullTextCatalog WITH CHANGE_TRACKING AUTO;", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON [dbo].[Ingredients]", true);
            migrationBuilder.Sql("DROP FULLTEXT CATALOG IngredientsFullTextCatalog", true);
        }
    }
}
