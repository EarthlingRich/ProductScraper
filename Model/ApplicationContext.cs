using Microsoft.EntityFrameworkCore;
using Model.Models;
using static Model.Models.Ingredient;

namespace Model
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new IngredientConfiguration());
            modelBuilder.Entity<ProductIngredient>().HasKey(_ => new { _.ProductId, _.IngredientId });
            modelBuilder.Entity<ProductIngredient>()
                .HasOne(_ => _.Product)
                .WithMany("ProductIngredients");
        }
    }
}