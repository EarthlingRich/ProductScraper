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

        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductActivity> ProductActivities { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<StoreCategory> StoreCategories { get; set; }
        public DbSet<WorkloadItem> WorkloadItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductIngredient>().HasKey(_ => new { _.ProductId, _.IngredientId });
            modelBuilder.Entity<ProductIngredient>()
                .HasOne(_ => _.Product)
                .WithMany("ProductIngredients");

            modelBuilder.Entity<ProductProductCategory>().HasKey(_ => new { _.ProductId, _.ProductCategoryId });
            modelBuilder.Entity<ProductProductCategory>()
                .HasOne(_ => _.Product)
                .WithMany("ProductProductCategories");

            modelBuilder.Entity<Product>()
                .HasMany(_ => _.WorkloadItems)
                .WithOne(_ => _.Product)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasMany(_ => _.ProductActivities)
                .WithOne(_ => _.Product)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasIndex(_ => _.Code);
        }
    }
}