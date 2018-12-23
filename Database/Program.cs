using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Model;

namespace Database
{
    public class Program : IDesignTimeDbContextFactory<ApplicationContext>
    {
        public ApplicationContext CreateDbContext(string[] args)
        {
            var builder = GetOptionsBuilder();
            return new ApplicationContext(builder.Options);
        }

        static void Main(string[] args)
        {
            var builder = GetOptionsBuilder();
            using (var context = new ApplicationContext(builder.Options))
            {
                context.Database.Migrate();
            }
        }

        static DbContextOptionsBuilder<ApplicationContext> GetOptionsBuilder()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationContext>();
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), _ => _.MigrationsAssembly("Database"));

            return builder;
        }
    }
}
