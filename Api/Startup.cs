﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model;
using System;
using AutoMapper;
using DataTables.AspNet.AspNetCore;
using Api.Controllers;
using Api.Models;
using Hangfire;
using Api.Application;
using Microsoft.Extensions.Hosting;
using Hangfire.PostgreSql;

namespace Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            IConfigurationRoot appSettings = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{HostingEnvironment.EnvironmentName}.json", optional: true)
                .Build();

            services.AddAutoMapper(typeof(ApiMapperConfiguration), typeof(ApplicationMapperConfiguration));
            services.RegisterDataTables();

            services.AddDbContext<ApplicationContext>(_ => _.UseNpgsql(appSettings.GetConnectionString("DefaultConnection")));
            services.AddHangfire(_ => _.UsePostgreSqlStorage(appSettings.GetConnectionString("DefaultConnection")));

            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddRazorPages();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:8080")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler($"/{HomeController.RouteName}/{nameof(HomeController.Error)}");
                app.UseHsts();
            }

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //Hangfire
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                AppPath = null,
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
            });
            app.UseHangfireServer();

            app.UseRouting();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
