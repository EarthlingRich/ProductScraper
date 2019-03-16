using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Model;

namespace Database.CustomMigrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("CustomMigration_AddProductsNameFullTextSearch")]
    public class AddProductsNameFullTextSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG ProductsFullTextCatalog", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON [dbo].[Products] ([Name]) KEY INDEX PK_Products ON ProductsFullTextCatalog WITH CHANGE_TRACKING AUTO;", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON [dbo].[Products]", true);
            migrationBuilder.Sql("DROP FULLTEXT CATALOG ProductsFullTextCatalog", true);
        }
    }
}
