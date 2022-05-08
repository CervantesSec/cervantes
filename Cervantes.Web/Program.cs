using Cervantes.CORE;
using Cervantes.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
using Microsoft.EntityFrameworkCore;

namespace Cervantes.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Debug("init main");
            //CreateHostBuilder(args).Build().Run();
            try
            {

                //CreateHostBuilder(args).Build().Run();
                var host = CreateHostBuilder(args).Build();

                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Services.AddControllersWithViews();

                // NLog: Setup NLog for Dependency injection
                builder.Logging.ClearProviders();
                builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                builder.Host.UseNLog();


                using (var scope = host.Services.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;

                    try
                    {
                        
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        db.Database.Migrate();
                        
                        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                        var vulnCategoryManager = serviceProvider.GetRequiredService<Contracts.IVulnCategoryManager>();
                        var organizationManager = serviceProvider.GetRequiredService<Contracts.IOrganizationManager>();
                        
                        DataInitializer.SeedData(userManager, roleManager, vulnCategoryManager, organizationManager);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }

                host.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).UseNLog();
    }
}
