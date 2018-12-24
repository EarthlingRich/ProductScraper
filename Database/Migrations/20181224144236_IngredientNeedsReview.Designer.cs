﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Model;

namespace Database.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20181224144236_IngredientNeedsReview")]
    partial class IngredientNeedsReview
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Model.Models.Ingredient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AllergyKeywordsString")
                        .HasColumnName("AllergyKeyWords");

                    b.Property<string>("KeywordsString")
                        .HasColumnName("Keywords");

                    b.Property<string>("Name");

                    b.Property<bool>("NeedsReview");

                    b.Property<int>("VeganType");

                    b.HasKey("Id");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("Model.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AllergyInfo");

                    b.Property<int?>("CategoryId");

                    b.Property<string>("Ingredients");

                    b.Property<bool>("IsNew");

                    b.Property<bool>("IsProcessed");

                    b.Property<string>("Name");

                    b.Property<int>("StoreType");

                    b.Property<string>("Url");

                    b.Property<int>("VeganType");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Model.Models.ProductCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("Model.Models.ProductIngredient", b =>
                {
                    b.Property<int>("ProductId");

                    b.Property<int>("IngredientId");

                    b.HasKey("ProductId", "IngredientId");

                    b.HasIndex("IngredientId");

                    b.ToTable("ProductIngredient");
                });

            modelBuilder.Entity("Model.Models.ProductProductCategory", b =>
                {
                    b.Property<int>("ProductId");

                    b.Property<int>("ProductCategoryId");

                    b.HasKey("ProductId", "ProductCategoryId");

                    b.HasIndex("ProductCategoryId");

                    b.ToTable("ProductProductCategory");
                });

            modelBuilder.Entity("Model.Models.StoreCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ProductCategoryId");

                    b.Property<int>("StoreType");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("ProductCategoryId");

                    b.ToTable("StoreCategories");
                });

            modelBuilder.Entity("Model.Models.WorkloadItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Message");

                    b.Property<int?>("ProductId");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("WorkloadItems");
                });

            modelBuilder.Entity("Model.Models.Product", b =>
                {
                    b.HasOne("Model.Models.ProductCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("Model.Models.ProductIngredient", b =>
                {
                    b.HasOne("Model.Models.Ingredient", "Ingredient")
                        .WithMany()
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Model.Models.Product", "Product")
                        .WithMany("ProductIngredients")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Model.Models.ProductProductCategory", b =>
                {
                    b.HasOne("Model.Models.ProductCategory", "ProductCategory")
                        .WithMany()
                        .HasForeignKey("ProductCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Model.Models.Product", "Product")
                        .WithMany("ProductProductCategories")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Model.Models.StoreCategory", b =>
                {
                    b.HasOne("Model.Models.ProductCategory", "ProductCategory")
                        .WithMany("StoreCategories")
                        .HasForeignKey("ProductCategoryId");
                });

            modelBuilder.Entity("Model.Models.WorkloadItem", b =>
                {
                    b.HasOne("Model.Models.Product", "Product")
                        .WithMany("WorkloadItems")
                        .HasForeignKey("ProductId");
                });
#pragma warning restore 612, 618
        }
    }
}
