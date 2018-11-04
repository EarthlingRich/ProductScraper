using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper.Contrib.Extensions;
using ProductScraper.Models;

namespace ProductScraper.Repositories
{
    public class ProductRepository
    {
        IDbConnection _connection { get { return new SqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString); } }

        public void Create(Product product)
        {
            using (IDbConnection dbConnection = _connection)
            {
                dbConnection.Insert(product);
            }
        }
    }
}
