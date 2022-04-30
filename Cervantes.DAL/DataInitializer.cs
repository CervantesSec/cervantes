using Cervantes.CORE;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace Cervantes.DAL
{
    public class DataInitializer
    {

        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, Contracts.IVulnCategoryManager vulnCategoryManager,
            Contracts.IOrganizationManager organizationManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
            SeedVulnCategories(vulnCategoryManager);
            SeedOrganization(organizationManager);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByEmailAsync("admin@cervantes.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "admin@cervantes.com";
                user.Email = "admin@cervantes.com";

                IdentityResult result = userManager.CreateAsync(user, "Admin123.").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }

        }


        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Admin";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }


            if (!roleManager.RoleExistsAsync("SuperUser").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "SuperUser";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }


            if (!roleManager.RoleExistsAsync("User").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "User";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }


            if (!roleManager.RoleExistsAsync("Client").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Client";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
        }

        private static void SeedVulnCategories(Contracts.IVulnCategoryManager vulnCategoryManager)
        {
            if (vulnCategoryManager.GetAll().Count() == 0)
            {
                VulnCategory category = new VulnCategory();
                category.Name = "Access Controls";
                vulnCategoryManager.Add(category);
                vulnCategoryManager.Context.SaveChanges();

                VulnCategory category2 = new VulnCategory();
                category2.Name = "Auditing and logging";
                vulnCategoryManager.Add(category2);
                vulnCategoryManager.Context.SaveChanges();

                VulnCategory category3 = new VulnCategory();
                category3.Name = "Authentication";
                vulnCategoryManager.Add(category3);
                vulnCategoryManager.Context.SaveChanges();

                VulnCategory category4 = new VulnCategory();
                category4.Name = "Authorization";
                vulnCategoryManager.Add(category4);
                vulnCategoryManager.Context.SaveChanges();

                VulnCategory category5 = new VulnCategory();
                category5.Name = "Configuration";
                vulnCategoryManager.Add(category5);
                vulnCategoryManager.Context.SaveChanges();

                VulnCategory category6 = new VulnCategory();
                category6.Name = "Cryptography";
                vulnCategoryManager.Add(category6);
                vulnCategoryManager.Context.SaveChanges();

                VulnCategory category7 = new VulnCategory();
                category7.Name = "Data Exposure";
                vulnCategoryManager.Add(category7);
                vulnCategoryManager.Context.SaveChanges();

                VulnCategory category8 = new VulnCategory();
                category8.Name = "Data Validation";
                vulnCategoryManager.Add(category8);
                vulnCategoryManager.Context.SaveChanges();

                VulnCategory category9 = new VulnCategory();
                category9.Name = "Denial of Service";
                vulnCategoryManager.Add(category9);
                vulnCategoryManager.Context.SaveChanges();

                VulnCategory category10 = new VulnCategory();
                category10.Name = "Error Reporting";
                vulnCategoryManager.Add(category10);
                vulnCategoryManager.Context.SaveChanges();

                VulnCategory category11 = new VulnCategory();
                category11.Name = "Injection";
                vulnCategoryManager.Add(category11);
                vulnCategoryManager.Context.SaveChanges();

                VulnCategory category12 = new VulnCategory();
                category12.Name = "Patching";
                vulnCategoryManager.Add(category12);
                vulnCategoryManager.Context.SaveChanges();

                VulnCategory category13 = new VulnCategory();
                category13.Name = "Session Management";
                vulnCategoryManager.Add(category13);
                vulnCategoryManager.Context.SaveChanges();

                VulnCategory category14 = new VulnCategory();
                category14.Name = "Timing";
                vulnCategoryManager.Add(category14);
                vulnCategoryManager.Context.SaveChanges();
            }
        }

        private static void SeedOrganization(Contracts.IOrganizationManager organizationManager)
        {
            if (organizationManager.GetAll().FirstOrDefault() == null)
            {
                Organization org = new Organization();
                org.Id = 1;
                org.Name = "Cervantes";
                org.Url = "https://github.com/mesquidar/Cervantes";
                org.ContactEmail = "cervantes@cervantes.com";
                org.ContactName = "Cervantes";
                org.ContactPhone = "1111111";
                org.Description = "Cervantes Platform";
                organizationManager.Add(org);
                organizationManager.Context.SaveChanges();
            }
        }


    }
}

