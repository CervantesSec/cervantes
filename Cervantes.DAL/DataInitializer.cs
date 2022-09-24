using System;
using Cervantes.CORE;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace Cervantes.DAL;

public class DataInitializer
{
    public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        Contracts.IVulnCategoryManager vulnCategoryManager,
        Contracts.IOrganizationManager organizationManager, Contracts.IReportTemplateManager reportTemplateManager)
    {
        SeedRoles(roleManager);
        SeedUsers(userManager);
        SeedVulnCategories(vulnCategoryManager);
        SeedOrganization(organizationManager);
        SeedTemplates(reportTemplateManager,userManager);
    }

    private static void SeedUsers(UserManager<ApplicationUser> userManager)
    {
        if (userManager.FindByEmailAsync("admin@cervantes.local").Result == null)
        {
            var user = new ApplicationUser();
            user.UserName = "admin@cervantes.local";
            user.Email = "admin@cervantes.local";
            user.FullName = "Admin";
            user.Position = "Admin";

            var result = userManager.CreateAsync(user, "Admin123.").Result;

            if (result.Succeeded) userManager.AddToRoleAsync(user, "Admin").Wait();
        }
    }


    private static void SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        if (!roleManager.RoleExistsAsync("Admin").Result)
        {
            var role = new IdentityRole();
            role.Name = "Admin";
            var roleResult = roleManager.CreateAsync(role).Result;
        }


        if (!roleManager.RoleExistsAsync("SuperUser").Result)
        {
            var role = new IdentityRole();
            role.Name = "SuperUser";
            var roleResult = roleManager.CreateAsync(role).Result;
        }


        if (!roleManager.RoleExistsAsync("User").Result)
        {
            var role = new IdentityRole();
            role.Name = "User";
            var roleResult = roleManager.CreateAsync(role).Result;
        }


        if (!roleManager.RoleExistsAsync("Client").Result)
        {
            var role = new IdentityRole();
            role.Name = "Client";
            var roleResult = roleManager.CreateAsync(role).Result;
        }
    }

    private static void SeedVulnCategories(Contracts.IVulnCategoryManager vulnCategoryManager)
    {
        if (vulnCategoryManager.GetAll().Count() == 0)
        {
            var category = new VulnCategory();
            category.Name = "Access Controls";
            vulnCategoryManager.Add(category);
            vulnCategoryManager.Context.SaveChanges();

            var category2 = new VulnCategory();
            category2.Name = "Auditing and logging";
            vulnCategoryManager.Add(category2);
            vulnCategoryManager.Context.SaveChanges();

            var category3 = new VulnCategory();
            category3.Name = "Authentication";
            vulnCategoryManager.Add(category3);
            vulnCategoryManager.Context.SaveChanges();

            var category4 = new VulnCategory();
            category4.Name = "Authorization";
            vulnCategoryManager.Add(category4);
            vulnCategoryManager.Context.SaveChanges();

            var category5 = new VulnCategory();
            category5.Name = "Configuration";
            vulnCategoryManager.Add(category5);
            vulnCategoryManager.Context.SaveChanges();

            var category6 = new VulnCategory();
            category6.Name = "Cryptography";
            vulnCategoryManager.Add(category6);
            vulnCategoryManager.Context.SaveChanges();

            var category7 = new VulnCategory();
            category7.Name = "Data Exposure";
            vulnCategoryManager.Add(category7);
            vulnCategoryManager.Context.SaveChanges();

            var category8 = new VulnCategory();
            category8.Name = "Data Validation";
            vulnCategoryManager.Add(category8);
            vulnCategoryManager.Context.SaveChanges();

            var category9 = new VulnCategory();
            category9.Name = "Denial of Service";
            vulnCategoryManager.Add(category9);
            vulnCategoryManager.Context.SaveChanges();

            var category10 = new VulnCategory();
            category10.Name = "Error Reporting";
            vulnCategoryManager.Add(category10);
            vulnCategoryManager.Context.SaveChanges();

            var category11 = new VulnCategory();
            category11.Name = "Injection";
            vulnCategoryManager.Add(category11);
            vulnCategoryManager.Context.SaveChanges();

            var category12 = new VulnCategory();
            category12.Name = "Patching";
            vulnCategoryManager.Add(category12);
            vulnCategoryManager.Context.SaveChanges();

            var category13 = new VulnCategory();
            category13.Name = "Session Management";
            vulnCategoryManager.Add(category13);
            vulnCategoryManager.Context.SaveChanges();

            var category14 = new VulnCategory();
            category14.Name = "Timing";
            vulnCategoryManager.Add(category14);
            vulnCategoryManager.Context.SaveChanges();
        }
    }

    private static void SeedOrganization(Contracts.IOrganizationManager organizationManager)
    {
        if (organizationManager.GetAll().FirstOrDefault() == null)
        {
            var org = new Organization();
            org.Id = 1;
            org.Name = "Cervantes";
            org.Url = "https://github.com/CervantesSec";
            org.ContactEmail = "cervantes@cervantes.com";
            org.ContactName = "Cervantes";
            org.ContactPhone = "+034 111 11 11 11";
            org.Description = "Cervantes Platform";
            organizationManager.Add(org);
            organizationManager.Context.SaveChanges();
        }
    }

    public static void SeedTemplates(Contracts.IReportTemplateManager reportTemplateManager, UserManager<ApplicationUser> userManager)
    {
        if (reportTemplateManager.GetAll().Count() == 0)
        {
            ApplicationUser admin =  userManager.FindByEmailAsync("admin@cervantes.local").Result;
            var templateEN = new ReportTemplate();
            templateEN.Id = Guid.NewGuid();
            templateEN.Name = "Default English";
            templateEN.Description = "Default English Template";
            templateEN.Language = Language.English;
            templateEN.CreatedDate = DateTime.Now.ToUniversalTime();
            templateEN.FilePath = "/Attachments/Templates/templateEN.dotx";
            reportTemplateManager.Add(templateEN);
            
            var templateES = new ReportTemplate();
            templateES.Id = Guid.NewGuid();
            templateES.Name = "Default Español";
            templateES.Description = "Default Spanish Template";
            templateES.Language = Language.Español;
            templateES.CreatedDate = DateTime.Now.ToUniversalTime();
            templateES.FilePath = "/Attachments/Templates/templateES.dotx";
            reportTemplateManager.Add(templateES);

            reportTemplateManager.Context.SaveChanges();
        }
        
    }
}