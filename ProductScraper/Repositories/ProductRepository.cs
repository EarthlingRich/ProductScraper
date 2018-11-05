using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Dapper.Contrib.Extensions;
using ProductScraper.Models;

namespace ProductScraper.Repositories
{
    public class ProductRepository
    {
        IDbConnection _connection { get { return new SqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString); } }

        public void Add(Product product)
        {
            using (IDbConnection dbConnection = _connection)
            {
                dbConnection.Insert(product);
            }
        }

        public void Update(Product product)
        {
            using (IDbConnection dbConnection = _connection)
            {
                dbConnection.Update(product);
            }
        }

        public Product GetByUrl(string url)
        {
            using (IDbConnection dbConnection = _connection)
            {
                string sql = "SELECT * FROM Products WHERE Url = @Url;";
                return dbConnection.QuerySingleOrDefault<Product>(sql, new { Url = url });
            }
        }
    }
}
