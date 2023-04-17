using System;
using Cervantes.CORE;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Xml;
using Cervantes.Contracts;
using Microsoft.AspNetCore.Hosting;

namespace Cervantes.DAL;

public class DataInitializer
{
    public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        Contracts.IVulnCategoryManager vulnCategoryManager,
        Contracts.IOrganizationManager organizationManager, Contracts.IReportTemplateManager reportTemplateManager, ICweManager cweManager,
        IHostingEnvironment _appEnvironment)
    {
        SeedRoles(roleManager);
        SeedUsers(userManager);
        SeedVulnCategories(vulnCategoryManager);
        SeedOrganization(organizationManager);
        SeedTemplates(reportTemplateManager,userManager);
        SeedCwe(cweManager, _appEnvironment);
    }
    
    public static void SeedCwe(ICweManager cweManager, IHostingEnvironment _appEnvironment)
    {
        if (cweManager.GetAll().Count() == 0)
        {
            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.Load(_appEnvironment.WebRootPath+ "/Attachments/Templates/cwe.xml");

            XmlNodeList weaknessNodes = xmlDocument.GetElementsByTagName("Weakness");

            foreach (XmlNode weaknessNode in weaknessNodes)
            {
                string id = weaknessNode.Attributes["ID"].Value;

                string name = weaknessNode.Attributes["Name"].Value;

                string desc = weaknessNode.ChildNodes[0].InnerText;


                Cwe cwe = new Cwe();
                cwe.Id = Convert.ToInt32(id);
                cwe.Name = name;
                cwe.Description = desc;
                
                cweManager.Add(cwe);
                cweManager.Context.SaveChanges();
            }
        }
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

    private static void SeedVulnCategories(Contracts.IVulnCategoryManager vulnCategoryManager)
    {
        if (vulnCategoryManager.GetAll().Count() == 0)
        {
            var category = new VulnCategory();
            category.Name = "Access Controls";
            category.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category);
            vulnCategoryManager.Context.SaveChanges();

            var category2 = new VulnCategory();
            category2.Name = "Auditing and logging";
            category.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category2);
            vulnCategoryManager.Context.SaveChanges();

            var category3 = new VulnCategory();
            category3.Name = "Authentication";
            category.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category3);
            vulnCategoryManager.Context.SaveChanges();

            var category4 = new VulnCategory();
            category4.Name = "Authorization";
            category.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category4);
            vulnCategoryManager.Context.SaveChanges();

            var category5 = new VulnCategory();
            category5.Name = "Configuration";
            category.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category5);
            vulnCategoryManager.Context.SaveChanges();

            var category6 = new VulnCategory();
            category6.Name = "Cryptography";
            category.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category6);
            vulnCategoryManager.Context.SaveChanges();

            var category7 = new VulnCategory();
            category7.Name = "Data Exposure";
            category.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category7);
            vulnCategoryManager.Context.SaveChanges();

            var category8 = new VulnCategory();
            category8.Name = "Data Validation";
            category.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category8);
            vulnCategoryManager.Context.SaveChanges();

            var category9 = new VulnCategory();
            category9.Name = "Denial of Service";
            category.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category9);
            vulnCategoryManager.Context.SaveChanges();

            var category10 = new VulnCategory();
            category10.Name = "Error Reporting";
            category.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category10);
            vulnCategoryManager.Context.SaveChanges();

            var category11 = new VulnCategory();
            category11.Name = "Injection";
            category.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category11);
            vulnCategoryManager.Context.SaveChanges();

            var category12 = new VulnCategory();
            category12.Name = "Patching";
            category.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category12);
            vulnCategoryManager.Context.SaveChanges();

            var category13 = new VulnCategory();
            category13.Name = "Session Management";
            category.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category13);
            vulnCategoryManager.Context.SaveChanges();

            var category14 = new VulnCategory();
            category14.Name = "Timing";
            category.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category14);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp1 = new VulnCategory();
            owasp1.Name = "WSTG-INFO-01";
            owasp1.Description = "Conduct Search Engine Discovery Reconnaissance for Information Leakage";
            owasp1.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp1);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp2 = new VulnCategory();
            owasp2.Name = "WSTG-INFO-02";
            owasp2.Description = "Fingerprint Web Server";
            owasp2.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp2);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp3 = new VulnCategory();
            owasp3.Name = "WSTG-INFO-03";
            owasp3.Description = "Review Webserver Metafiles for Information Leakage";
            owasp3.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp3);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp4 = new VulnCategory();
            owasp4.Name = "WSTG-INFO-04";
            owasp4.Description = "Enumerate Applications on Webserver";
            owasp4.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp4);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp5 = new VulnCategory();
            owasp5.Name = "WSTG-INFO-05";
            owasp5.Description = "Review Webpage Content for Information Leakage";
            owasp5.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp5);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp6 = new VulnCategory();
            owasp6.Name = "WSTG-INFO-06";
            owasp6.Description = "Identify application entry points";
            owasp6.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp6);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp7 = new VulnCategory();
            owasp7.Name = "WSTG-INFO-07";
            owasp7.Description = "Map execution paths through application";
            owasp7.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp7);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp8 = new VulnCategory();
            owasp8.Name = "WSTG-INFO-08";
            owasp8.Description = "Fingerprint Web Application Framework";
            owasp8.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp8);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp9 = new VulnCategory();
            owasp9.Name = "WSTG-INFO-09";
            owasp9.Description = "Fingerprint Web Application";
            owasp9.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp9);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp10 = new VulnCategory();
            owasp10.Name = "WSTG-INFO-10";
            owasp10.Description = "Map Application Architecture";
            owasp10.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp10);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp11 = new VulnCategory();
            owasp11.Name = "WSTG-CONF-01";
            owasp11.Description = "Test Network Infrastructure Configuration";
            owasp11.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp11);
            vulnCategoryManager.Context.SaveChanges();
            
            
            var owasp13 = new VulnCategory();
            owasp13.Name = "WSTG-CONF-02";
            owasp13.Description = "Test Application Platform Configuration";
            owasp13.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp13);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp14 = new VulnCategory();
            owasp14.Name = "WSTG-CONF-03";
            owasp14.Description = "Test File Extensions Handling for Sensitive Information";
            owasp14.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp14);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp15 = new VulnCategory();
            owasp15.Name = "WSTG-CONF-04";
            owasp15.Description = "Review Old Backup and Unreferenced Files for Sensitive Information";
            owasp15.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp15);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp16 = new VulnCategory();
            owasp16.Name = "WSTG-CONF-05";
            owasp16.Description = "Enumerate Infrastructure and Application Admin Interfaces";
            owasp16.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp16);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp17 = new VulnCategory();
            owasp17.Name = "WSTG-CONF-06";
            owasp17.Description = "Test HTTP Methods";
            owasp17.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp17);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp18 = new VulnCategory();
            owasp18.Name = "WSTG-CONF-07";
            owasp18.Description = "Test HTTP Strict Transport Security";
            owasp18.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp18);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp19 = new VulnCategory();
            owasp19.Name = "WSTG-CONF-08";
            owasp19.Description = "Test RIA cross domain policy";
            owasp19.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp19);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp20 = new VulnCategory();
            owasp20.Name = "WSTG-CONF-09";
            owasp20.Description = "Test File Permission";
            owasp20.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp20);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp21 = new VulnCategory();
            owasp21.Name = "WSTG-CONF-10";
            owasp21.Description = "Test for Subdomain Takeover";
            owasp21.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp21);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp22 = new VulnCategory();
            owasp22.Name = "WSTG-CONF-11";
            owasp22.Description = "Test Cloud Storage";
            owasp22.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp22);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp23 = new VulnCategory();
            owasp23.Name = "WSTG-IDNT-01";
            owasp23.Description = "Test Role Definitions";
            owasp23.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp23);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp24 = new VulnCategory();
            owasp24.Name = "WSTG-IDNT-02";
            owasp24.Description = "Test User Registration Process";
            owasp24.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp24);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp25 = new VulnCategory();
            owasp25.Name = "WSTG-IDNT-03";
            owasp25.Description = "Test Account Provisioning Process";
            owasp25.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp25);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp26 = new VulnCategory();
            owasp26.Name = "WSTG-IDNT-04";
            owasp26.Description = "Testing for Account Enumeration and Guessable User Account";
            owasp26.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp26);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp27 = new VulnCategory();
            owasp27.Name = "WSTG-IDNT-05";
            owasp27.Description = "Testing for Weak or unenforced username policy";
            owasp27.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp27);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp28 = new VulnCategory();
            owasp28.Name = "WSTG-ATHN-01";
            owasp28.Description = "Testing for Credentials Transported over an Encrypted Channel";
            owasp28.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp28);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp29 = new VulnCategory();
            owasp29.Name = "WSTG-ATHN-02";
            owasp29.Description = "Testing for Default Credentials";
            owasp29.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp29);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp30 = new VulnCategory();
            owasp30.Name = "WSTG-ATHN-03";
            owasp30.Description = "Testing for Weak Lock Out Mechanism";
            owasp30.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp30);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp31 = new VulnCategory();
            owasp31.Name = "WSTG-ATHN-04";
            owasp31.Description = "Testing for Bypassing Authentication Schema";
            owasp31.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp31);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp32 = new VulnCategory();
            owasp32.Name = "WSTG-ATHN-05";
            owasp32.Description = "Testing for Vulnerable Remember Password";
            owasp32.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp32);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp33 = new VulnCategory();
            owasp33.Name = "WSTG-ATHN-06";
            owasp33.Description = "Testing for Browser Cache Weaknesses";
            owasp33.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp33);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp34 = new VulnCategory();
            owasp34.Name = "WSTG-ATHN-07";
            owasp34.Description = "Testing for Weak Password Policy";
            owasp34.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp34);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp35 = new VulnCategory();
            owasp35.Name = "WSTG-ATHN-08";
            owasp35.Description = "Testing for Weak Security Question Answer";
            owasp35.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp35);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp36 = new VulnCategory();
            owasp36.Name = "WSTG-ATHN-09";
            owasp36.Description = "Testing for Weak Password Change or Reset Functionalities";
            owasp36.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp36);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp37 = new VulnCategory();
            owasp37.Name = "WSTG-ATHN-10";
            owasp37.Description = "Testing for Weaker Authentication in Alternative Channel";
            owasp37.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp37);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp38 = new VulnCategory();
            owasp38.Name = "WSTG-ATHZ-01";
            owasp38.Description = "Testing Directory Traversal File Include";
            owasp38.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp38);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp39 = new VulnCategory();
            owasp39.Name = "WSTG-ATHZ-02";
            owasp39.Description = "Testing for Bypassing Authorization Schema";
            owasp39.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp39);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp40 = new VulnCategory();
            owasp40.Name = "WSTG-ATHZ-03";
            owasp40.Description = "Testing for Privilege Escalation";
            owasp40.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp40);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp41 = new VulnCategory();
            owasp41.Name = "WSTG-ATHZ-04";
            owasp41.Description = "Testing for Insecure Direct Object References";
            owasp41.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp41);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp42 = new VulnCategory();
            owasp42.Name = "WSTG-SESS-01";
            owasp42.Description = "Testing for Session Management Schema";
            owasp42.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp42);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp43 = new VulnCategory();
            owasp43.Name = "WSTG-SESS-02";
            owasp43.Description = "Testing for Cookies Attributes";
            owasp43.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp43);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp44 = new VulnCategory();
            owasp44.Name = "WSTG-SESS-03";
            owasp44.Description = "Testing for Session Fixation";
            owasp44.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp44);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp45 = new VulnCategory();
            owasp45.Name = "WSTG-SESS-04";
            owasp45.Description = "Testing for Exposed Session Variables";
            owasp45.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp45);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp46 = new VulnCategory();
            owasp46.Name = "WSTG-SESS-05";
            owasp46.Description = "Testing for Cross Site Request Forgery";
            owasp46.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp46);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp47 = new VulnCategory();
            owasp47.Name = "WSTG-SESS-06";
            owasp47.Description = "Testing for Logout Functionality";
            owasp47.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp47);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp48 = new VulnCategory();
            owasp48.Name = "WSTG-SESS-07";
            owasp48.Description = "Testing Session Timeout";
            owasp48.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp48);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp49 = new VulnCategory();
            owasp49.Name = "WSTG-SESS-08";
            owasp49.Description = "Testing for Session Puzzling";
            owasp49.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp49);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp50 = new VulnCategory();
            owasp50.Name = "WSTG-SESS-09";
            owasp50.Description = "Testing for Session Hijacking";
            owasp50.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp50);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp51 = new VulnCategory();
            owasp51.Name = "WSTG-INPV-01";
            owasp51.Description = "Testing for Reflected Cross Site Scripting";
            owasp51.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp51);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp52 = new VulnCategory();
            owasp52.Name = "WSTG-INPV-02";
            owasp52.Description = "Testing for Stored Cross Site Scripting";
            owasp52.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp52);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp53 = new VulnCategory();
            owasp53.Name = "WSTG-INPV-03";
            owasp53.Description = "Testing for HTTP Verb Tampering";
            owasp53.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp53);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp54 = new VulnCategory();
            owasp54.Name = "WSTG-INPV-04";
            owasp54.Description = "Testing for HTTP Parameter Pollution";
            owasp54.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp54);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp55 = new VulnCategory();
            owasp55.Name = "WSTG-INPV-05";
            owasp55.Description = "Testing for SQL Injection";
            owasp55.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp55);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp56 = new VulnCategory();
            owasp56.Name = "WSTG-INPV-06";
            owasp56.Description = "Testing for LDAP Injection";
            owasp56.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp56);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp57 = new VulnCategory();
            owasp57.Name = "WSTG-INPV-07";
            owasp57.Description = "Testing for XML Injection";
            owasp57.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp57);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp58 = new VulnCategory();
            owasp58.Name = "WSTG-INPV-08";
            owasp58.Description = "Testing for SSI Injection";
            owasp58.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp58);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp59 = new VulnCategory();
            owasp59.Name = "WSTG-INPV-09";
            owasp59.Description = "Testing for XPath Injection";
            owasp59.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp59);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp60 = new VulnCategory();
            owasp60.Name = "WSTG-INPV-10";
            owasp60.Description = "Testing for IMAP SMTP Injection";
            owasp60.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp60);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp61 = new VulnCategory();
            owasp61.Name = "WSTG-INPV-11";
            owasp61.Description = "Testing for Code Injection";
            owasp61.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp61);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp62 = new VulnCategory();
            owasp62.Name = "WSTG-INPV-12";
            owasp62.Description = "Testing for Command Injection";
            owasp62.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp62);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp63 = new VulnCategory();
            owasp63.Name = "WSTG-INPV-13";
            owasp63.Description = "Testing for Format String Injection";
            owasp63.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp63);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp64 = new VulnCategory();
            owasp64.Name = "WSTG-INPV-14";
            owasp64.Description = "Testing for Incubated Vulnerability";
            owasp64.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp64);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp65 = new VulnCategory();
            owasp65.Name = "WSTG-INPV-15";
            owasp65.Description = "Testing for HTTP Splitting Smuggling";
            owasp65.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp65);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp66 = new VulnCategory();
            owasp66.Name = "WSTG-INPV-16";
            owasp66.Description = "Testing for HTTP Incoming Requests";
            owasp66.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp66);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp67 = new VulnCategory();
            owasp67.Name = "WSTG-INPV-17";
            owasp67.Description = "Testing for Host Header Injection";
            owasp67.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp67);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp68 = new VulnCategory();
            owasp68.Name = "WSTG-INPV-18";
            owasp68.Description = "Testing for Server-side Template Injection";
            owasp68.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp68);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp69 = new VulnCategory();
            owasp69.Name = "WSTG-INPV-19";
            owasp69.Description = "Testing for Server-Side Request Forgery";
            owasp69.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp69);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp70 = new VulnCategory();
            owasp70.Name = "WSTG-ERRH-01";
            owasp70.Description = "Testing for Improper Error Handling";
            owasp70.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp70);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp71 = new VulnCategory();
            owasp71.Name = "WSTG-ERRH-02";
            owasp71.Description = "Testing for Stack Traces";
            owasp71.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp71);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp72 = new VulnCategory();
            owasp72.Name = "WSTG-CRYP-01";
            owasp72.Description = "Testing for Weak Transport Layer Security";
            owasp72.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp72);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp73 = new VulnCategory();
            owasp73.Name = "WSTG-CRYP-02";
            owasp73.Description = "Testing for Padding Oracle";
            owasp73.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp73);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp74 = new VulnCategory();
            owasp74.Name = "WSTG-CRYP-03";
            owasp74.Description = "Testing for Sensitive Information Sent via Unencrypted Channels";
            owasp74.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp74);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp75 = new VulnCategory();
            owasp75.Name = "WSTG-CRYP-04";
            owasp75.Description = "Testing for Weak Encryption";
            owasp75.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp75);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp76 = new VulnCategory();
            owasp76.Name = "WSTG-BUSL-01";
            owasp76.Description = "Test Business Logic Data Validation";
            owasp76.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp76);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp77 = new VulnCategory();
            owasp77.Name = "WSTG-BUSL-02";
            owasp77.Description = "Test Ability to Forge Requests";
            owasp77.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp77);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp78 = new VulnCategory();
            owasp78.Name = "WSTG-BUSL-03";
            owasp78.Description = "Test Integrity Checks";
            owasp78.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp78);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp79 = new VulnCategory();
            owasp79.Name = "WSTG-BUSL-04";
            owasp79.Description = "Test for Process Timing";
            owasp79.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp79);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp80 = new VulnCategory();
            owasp80.Name = "WSTG-BUSL-05";
            owasp80.Description = "Test Number of Times a Function Can be Used Limits";
            owasp80.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp80);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp81 = new VulnCategory();
            owasp81.Name = "WSTG-BUSL-06";
            owasp81.Description = "Testing for the Circumvention of Work Flows";
            owasp81.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp81);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp82 = new VulnCategory();
            owasp82.Name = "WSTG-BUSL-07";
            owasp82.Description = "Test Defenses Against Application Mis-use";
            owasp82.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp82);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp83 = new VulnCategory();
            owasp83.Name = "WSTG-BUSL-08";
            owasp83.Description = "Test Upload of Unexpected File Types";
            owasp83.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp83);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp84 = new VulnCategory();
            owasp84.Name = "WSTG-BUSL-09";
            owasp84.Description = "Test Upload of Malicious Files";
            owasp84.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp84);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp85 = new VulnCategory();
            owasp85.Name = "WSTG-CLNT-01";
            owasp85.Description = "Testing for DOM-Based Cross Site Scripting";
            owasp85.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp85);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp86 = new VulnCategory();
            owasp86.Name = "WSTG-CLNT-02";
            owasp86.Description = "Testing for JavaScript Execution";
            owasp86.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp86);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp87 = new VulnCategory();
            owasp87.Name = "WSTG-CLNT-03";
            owasp87.Description = "Testing for HTML Injection";
            owasp87.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp87);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp88 = new VulnCategory();
            owasp88.Name = "WSTG-CLNT-04";
            owasp88.Description = "Testing for Client Side URL Redirect";
            owasp88.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp88);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp89 = new VulnCategory();
            owasp89.Name = "WSTG-CLNT-05";
            owasp89.Description = "Testing for CSS Injection";
            owasp89.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp89);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp90 = new VulnCategory();
            owasp90.Name = "WSTG-CLNT-06";
            owasp90.Description = "Testing for Client Side Resource Manipulation";
            owasp90.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp90);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp91 = new VulnCategory();
            owasp91.Name = "WSTG-CLNT-07";
            owasp91.Description = "Test Cross Origin Resource Sharing";
            owasp91.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp91);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp92 = new VulnCategory();
            owasp92.Name = "WSTG-CLNT-08";
            owasp92.Description = "Testing for Cross Site Flashing";
            owasp92.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp92);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp93 = new VulnCategory();
            owasp93.Name = "WSTG-CLNT-09";
            owasp93.Description = "Testing for Clickjacking";
            owasp93.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp93);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp94 = new VulnCategory();
            owasp94.Name = "WSTG-CLNT-10";
            owasp94.Description = "Testing WebSockets";
            owasp94.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp94);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp95 = new VulnCategory();
            owasp95.Name = "WSTG-CLNT-11";
            owasp95.Description = "Test Web Messaging";
            owasp95.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp95);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp96 = new VulnCategory();
            owasp96.Name = "WSTG-CLNT-12";
            owasp96.Description = "Testing Browser Storage";
            owasp96.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp96);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp97 = new VulnCategory();
            owasp97.Name = "WSTG-CLNT-13";
            owasp97.Description = "Testing for Cross Site Script Inclusion";
            owasp97.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp97);
            vulnCategoryManager.Context.SaveChanges();
            
            var owasp98 = new VulnCategory();
            owasp98.Name = "WSTG-APIT-01";
            owasp98.Description = "Testing GraphQL";
            owasp98.Type = VulnCategoryType.OwaspWSTG;
            vulnCategoryManager.Add(owasp98);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas1 = new VulnCategory();
            mas1.Name = "MSTG-ARCH-1";
            mas1.Description = "All app components are identified and known to be needed.";
            mas1.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas1);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas2 = new VulnCategory();
            mas2.Name = "MSTG-ARCH-2";
            mas2.Description = "Security controls are never enforced only on the client side, but on the respective remote endpoints.";
            mas2.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas2);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas3 = new VulnCategory();
            mas3.Name = "MSTG-ARCH-3";
            mas3.Description = "A high-level architecture for the mobile app and all connected remote services has been defined and security has been addressed in that architecture.";
            mas3.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas3);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas4 = new VulnCategory();
            mas4.Name = "MSTG-ARCH-4";
            mas4.Description = "Data considered sensitive in the context of the mobile app is clearly identified.";
            mas4.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas4);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas5 = new VulnCategory();
            mas5.Name = "MSTG-ARCH-5";
            mas5.Description = "All app components are defined in terms of the business functions and/or security functions they provide.";
            mas5.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas5);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas6 = new VulnCategory();
            mas6.Name = "MSTG-ARCH-6";
            mas6.Description = "A threat model for the mobile app and the associated remote services has been produced that identifies potential threats and countermeasures.";
            mas6.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas6);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas7 = new VulnCategory();
            mas7.Name = "MSTG-ARCH-7";
            mas7.Description = "All security controls have a centralized implementation.";
            mas7.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas7);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas8 = new VulnCategory();
            mas8.Name = "MSTG-ARCH-8";
            mas8.Description = "There is an explicit policy for how cryptographic keys (if any) are managed, and the lifecycle of cryptographic keys is enforced. Ideally, follow a key management standard such as NIST SP 800-57.";
            mas8.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas8);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas9 = new VulnCategory();
            mas9.Name = "MSTG-ARCH-9";
            mas9.Description = "A mechanism for enforcing updates of the mobile app exists.";
            mas9.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas9);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas10 = new VulnCategory();
            mas10.Name = "MSTG-ARCH-10";
            mas10.Description = "Security is addressed within all parts of the software development lifecycle.";
            mas10.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas10);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas11 = new VulnCategory();
            mas11.Name = "MSTG-ARCH-11";
            mas11.Description = "A responsible disclosure policy is in place and effectively applied.";
            mas11.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas11);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas12 = new VulnCategory();
            mas12.Name = "MSTG-ARCH-12";
            mas12.Description = "The app should comply with privacy laws and regulations.";
            mas12.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas12);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas13 = new VulnCategory();
            mas13.Name = "MSTG-STORAGE-1";
            mas13.Description = "System credential storage facilities need to be used to store sensitive data, such as PII, user credentials or cryptographic keys.";
            mas13.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas13);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas14 = new VulnCategory();
            mas14.Name = "MSTG-STORAGE-2";
            mas14.Description = "No sensitive data should be stored outside of the app container or system credential storage facilities.";
            mas14.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas14);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas15 = new VulnCategory();
            mas15.Name = "MSTG-STORAGE-3";
            mas15.Description = "No sensitive data is written to application logs.";
            mas15.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas15);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas16 = new VulnCategory();
            mas16.Name = "MSTG-STORAGE-4";
            mas16.Description = "No sensitive data is shared with third parties unless it is a necessary part of the architecture.";
            mas16.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas16);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas17 = new VulnCategory();
            mas17.Name = "MSTG-STORAGE-5";
            mas17.Description = "The keyboard cache is disabled on text inputs that process sensitive data.";
            mas17.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas17);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas18 = new VulnCategory();
            mas18.Name = "MSTG-STORAGE-6";
            mas18.Description = "No sensitive data is exposed via IPC mechanisms.";
            mas18.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas18);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas19 = new VulnCategory();
            mas19.Name = "MSTG-STORAGE-7";
            mas19.Description = "No sensitive data, such as passwords or pins, is exposed through the user interface.";
            mas19.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas19);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas20 = new VulnCategory();
            mas20.Name = "MSTG-STORAGE-8";
            mas20.Description = "No sensitive data is included in backups generated by the mobile operating system.";
            mas20.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas20);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas21 = new VulnCategory();
            mas21.Name = "MSTG-STORAGE-9";
            mas21.Description = "The app removes sensitive data from views when moved to the background.";
            mas21.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas21);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas22 = new VulnCategory();
            mas22.Name = "MSTG-STORAGE-10";
            mas22.Description = "The app does not hold sensitive data in memory longer than necessary, and memory is cleared explicitly after use.";
            mas22.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas22);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas23 = new VulnCategory();
            mas23.Name = "MSTG-STORAGE-11";
            mas23.Description = "The app enforces a minimum device-access-security policy, such as requiring the user to set a device passcode.";
            mas23.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas23);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas24 = new VulnCategory();
            mas24.Name = "MSTG-STORAGE-12";
            mas24.Description = "The app educates the user about the types of personally identifiable information processed, as well as security best practices the user should follow in using the app.";
            mas24.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas24);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas25 = new VulnCategory();
            mas25.Name = "MSTG-STORAGE-13";
            mas25.Description = "No sensitive data should be stored locally on the mobile device. Instead, data should be retrieved from a remote endpoint when needed and only be kept in memory.";
            mas25.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas25);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas26 = new VulnCategory();
            mas26.Name = "MSTG-STORAGE-14";
            mas26.Description = "If sensitive data is still required to be stored locally, it should be encrypted using a key derived from hardware backed storage which requires authentication.";
            mas26.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas26);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas27 = new VulnCategory();
            mas27.Name = "MSTG-STORAGE-15";
            mas27.Description = "The app’s local storage should be wiped after an excessive number of failed authentication attempts.";
            mas27.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas27);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas28 = new VulnCategory();
            mas28.Name = "MSTG-CRYPTO-1";
            mas28.Description = "The app does not rely on symmetric cryptography with hardcoded keys as a sole method of encryption.";
            mas28.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas28);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas29 = new VulnCategory();
            mas29.Name = "MSTG-CRYPTO-2";
            mas29.Description = "The app uses proven implementations of cryptographic primitives.";
            mas29.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas29);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas30 = new VulnCategory();
            mas30.Name = "MSTG-CRYPTO-3";
            mas30.Description = "The app uses cryptographic primitives that are appropriate for the particular use-case, configured with parameters that adhere to industry best practices.";
            mas30.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas30);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas31 = new VulnCategory();
            mas31.Name = "MSTG-CRYPTO-4";
            mas31.Description = "The app does not use cryptographic protocols or algorithms that are widely considered deprecated for security purposes.";
            mas31.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas31);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas32 = new VulnCategory();
            mas32.Name = "MSTG-CRYPTO-5";
            mas32.Description = "The app doesn't re-use the same cryptographic key for multiple purposes.";
            mas32.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas32);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas33 = new VulnCategory();
            mas33.Name = "MSTG-CRYPTO-6";
            mas33.Description = "All random values are generated using a sufficiently secure random number generator.";
            mas33.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas33);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas34 = new VulnCategory();
            mas34.Name = "MSTG-AUTH-1";
            mas34.Description = "If the app provides users access to a remote service, some form of authentication, such as username/password authentication, is performed at the remote endpoint.";
            mas34.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas34);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas35 = new VulnCategory();
            mas35.Name = "MSTG-AUTH-2";
            mas35.Description = "If stateful session management is used, the remote endpoint uses randomly generated session identifiers to authenticate client requests without sending the user's credentials.";
            mas35.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas35);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas36 = new VulnCategory();
            mas36.Name = "MSTG-AUTH-3";
            mas36.Description = "If stateless token-based authentication is used, the server provides a token that has been signed using a secure algorithm.";
            mas36.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas36);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas37 = new VulnCategory();
            mas37.Name = "MSTG-AUTH-4";
            mas37.Description = "The remote endpoint terminates the existing session when the user logs out.";
            mas37.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas37);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas38 = new VulnCategory();
            mas38.Name = "MSTG-AUTH-5";
            mas38.Description = "A password policy exists and is enforced at the remote endpoint.";
            mas38.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas38);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas39 = new VulnCategory();
            mas39.Name = "MSTG-AUTH-6";
            mas39.Description = "The remote endpoint implements a mechanism to protect against the submission of credentials an excessive number of times.";
            mas39.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas39);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas40 = new VulnCategory();
            mas40.Name = "MSTG-AUTH-7";
            mas40.Description = "Sessions are invalidated at the remote endpoint after a predefined period of inactivity and access tokens expire.";
            mas40.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas40);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas41 = new VulnCategory();
            mas41.Name = "MSTG-AUTH-8";
            mas41.Description = "Biometric authentication, if any, is not event-bound (i.e. using an API that simply returns true or false). Instead, it is based on unlocking the keychain/keystore.";
            mas41.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas41);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas42 = new VulnCategory();
            mas42.Name = "MSTG-AUTH-9";
            mas42.Description = "A second factor of authentication exists at the remote endpoint and the 2FA requirement is consistently enforced.";
            mas42.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas42);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas43 = new VulnCategory();
            mas43.Name = "MSTG-AUTH-10";
            mas43.Description = "Sensitive transactions require step-up authentication.";
            mas43.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas43);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas44 = new VulnCategory();
            mas44.Name = "MSTG-AUTH-11";
            mas44.Description = "The app informs the user of all sensitive activities with their account. Users are able to view a list of devices, view contextual information (IP address, location, etc.), and to block specific devices.";
            mas44.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas44);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas45 = new VulnCategory();
            mas45.Name = "MSTG-AUTH-12";
            mas45.Description = "Authorization models should be defined and enforced at the remote endpoint.";
            mas45.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas45);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas46 = new VulnCategory();
            mas46.Name = "MSTG-NETWORK-1";
            mas46.Description = "Data is encrypted on the network using TLS. The secure channel is used consistently throughout the app.";
            mas46.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas46);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas47 = new VulnCategory();
            mas47.Name = "MSTG-NETWORK-2";
            mas47.Description = "The TLS settings are in line with current best practices, or as close as possible if the mobile operating system does not support the recommended standards.";
            mas47.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas47);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas48 = new VulnCategory();
            mas48.Name = "MSTG-NETWORK-3";
            mas48.Description = "The app verifies the X.509 certificate of the remote endpoint when the secure channel is established. Only certificates signed by a trusted CA are accepted.";
            mas48.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas48);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas49 = new VulnCategory();
            mas49.Name = "MSTG-NETWORK-4";
            mas49.Description = "The app either uses its own certificate store, or pins the endpoint certificate or public key, and subsequently does not establish connections with endpoints that offer a different certificate or key, even if signed by a trusted CA.";
            mas49.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas49);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas50 = new VulnCategory();
            mas50.Name = "MSTG-NETWORK-5";
            mas50.Description = "The app doesn't rely on a single insecure communication channel (email or SMS) for critical operations, such as enrollments and account recovery.";
            mas50.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas50);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas51 = new VulnCategory();
            mas51.Name = "MSTG-NETWORK-6";
            mas51.Description = "The app only depends on up-to-date connectivity and security libraries.";
            mas51.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas51);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas52 = new VulnCategory();
            mas52.Name = "MSTG-PLATFORM-1";
            mas52.Description = "The app only requests the minimum set of permissions necessary.";
            mas52.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas52);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas53 = new VulnCategory();
            mas53.Name = "MSTG-PLATFORM-2";
            mas53.Description = "All inputs from external sources and the user are validated and if necessary sanitized. This includes data received via the UI, IPC mechanisms such as intents, custom URLs, and network sources.";
            mas53.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas53);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas54 = new VulnCategory();
            mas54.Name = "MSTG-PLATFORM-3";
            mas54.Description = "The app does not export sensitive functionality via custom URL schemes, unless these mechanisms are properly protected.";
            mas54.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas54);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas55 = new VulnCategory();
            mas55.Name = "MSTG-PLATFORM-4";
            mas55.Description = "The app does not export sensitive functionality through IPC facilities, unless these mechanisms are properly protected.";
            mas55.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas55);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas56 = new VulnCategory();
            mas56.Name = "MSTG-PLATFORM-5";
            mas56.Description = "JavaScript is disabled in WebViews unless explicitly required.";
            mas56.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas56);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas57 = new VulnCategory();
            mas57.Name = "MSTG-PLATFORM-6";
            mas57.Description = "WebViews are configured to allow only the minimum set of protocol handlers required (ideally, only https is supported). Potentially dangerous handlers, such as file, tel and app-id, are disabled.";
            mas57.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas57);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas58 = new VulnCategory();
            mas58.Name = "MSTG-PLATFORM-7";
            mas58.Description = "If native methods of the app are exposed to a WebView, verify that the WebView only renders JavaScript contained within the app package.";
            mas58.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas58);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas59 = new VulnCategory();
            mas59.Name = "MSTG-PLATFORM-8";
            mas59.Description = "Object deserialization, if any, is implemented using safe serialization APIs.";
            mas59.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas59);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas60 = new VulnCategory();
            mas60.Name = "MSTG-PLATFORM-9";
            mas60.Description = "The app protects itself against screen overlay attacks. (Android only)";
            mas60.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas60);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas61 = new VulnCategory();
            mas61.Name = "MSTG-PLATFORM-10";
            mas61.Description = "A WebView's cache, storage, and loaded resources (JavaScript, etc.) should be cleared before the WebView is destroyed.";
            mas61.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas61);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas62 = new VulnCategory();
            mas62.Name = "MSTG-PLATFORM-11";
            mas62.Description = "Verify that the app prevents usage of custom third-party keyboards whenever sensitive data is entered (iOS only).";
            mas62.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas62);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas63 = new VulnCategory();
            mas63.Name = "MSTG-CODE-1";
            mas63.Description = "The app is signed and provisioned with a valid certificate, of which the private key is properly protected.";
            mas63.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas63);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas64 = new VulnCategory();
            mas64.Name = "MSTG-CODE-2";
            mas64.Description = "The app has been built in release mode, with settings appropriate for a release build (e.g. non-debuggable).";
            mas64.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas64);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas65 = new VulnCategory();
            mas65.Name = "MSTG-CODE-3";
            mas65.Description = "Debugging symbols have been removed from native binaries.";
            mas65.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas65);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas66 = new VulnCategory();
            mas66.Name = "MSTG-CODE-4";
            mas66.Description = "Debugging code and developer assistance code (e.g. test code, backdoors, hidden settings) have been removed. The app does not log verbose errors or debugging messages.";
            mas66.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas66);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas67 = new VulnCategory();
            mas67.Name = "MSTG-CODE-5";
            mas67.Description = "All third party components used by the mobile app, such as libraries and frameworks, are identified, and checked for known vulnerabilities.";
            mas67.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas67);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas68 = new VulnCategory();
            mas68.Name = "MSTG-CODE-6";
            mas68.Description = "The app catches and handles possible exceptions.";
            mas68.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas68);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas69 = new VulnCategory();
            mas69.Name = "MSTG-CODE-7";
            mas69.Description = "Error handling logic in security controls denies access by default.";
            mas69.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas69);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas70 = new VulnCategory();
            mas70.Name = "MSTG-CODE-8";
            mas70.Description = "In unmanaged code, memory is allocated, freed and used securely.";
            mas70.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas70);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas71 = new VulnCategory();
            mas71.Name = "MSTG-CODE-9";
            mas71.Description = "Free security features offered by the toolchain, such as byte-code minification, stack protection, PIE support and automatic reference counting, are activated.";
            mas71.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas71);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas72 = new VulnCategory();
            mas72.Name = "MSTG-RESILIENCE-1";
            mas72.Description = "The app detects, and responds to, the presence of a rooted or jailbroken device either by alerting the user or terminating the app.";
            mas72.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas72);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas73 = new VulnCategory();
            mas73.Name = "MSTG-RESILIENCE-2";
            mas73.Description = "The app prevents debugging and/or detects, and responds to, a debugger being attached. All available debugging protocols must be covered.";
            mas73.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas73);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas74 = new VulnCategory();
            mas74.Name = "MSTG-RESILIENCE-3";
            mas74.Description = "The app detects, and responds to, tampering with executable files and critical data within its own sandbox.";
            mas74.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas74);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas75 = new VulnCategory();
            mas75.Name = "MSTG-RESILIENCE-4";
            mas75.Description = "The app detects, and responds to, the presence of widely used reverse engineering tools and frameworks on the device.";
            mas75.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas75);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas76 = new VulnCategory();
            mas76.Name = "MSTG-RESILIENCE-5";
            mas76.Description = "The app detects, and responds to, being run in an emulator.";
            mas76.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas76);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas77 = new VulnCategory();
            mas77.Name = "MSTG-RESILIENCE-6";
            mas77.Description = "The app detects, and responds to, tampering the code and data in its own memory space.";
            mas77.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas77);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas78 = new VulnCategory();
            mas78.Name = "MSTG-RESILIENCE-7";
            mas78.Description = "The app implements multiple mechanisms in each defense category (8.1 to 8.6). Note that resiliency scales with the amount, diversity of the originality of the mechanisms used.";
            mas78.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas78);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas79 = new VulnCategory();
            mas79.Name = "MSTG-RESILIENCE-8";
            mas79.Description = "The detection mechanisms trigger responses of different types, including delayed and stealthy responses.";
            mas79.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas79);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas80 = new VulnCategory();
            mas80.Name = "MSTG-RESILIENCE-9";
            mas80.Description = "Obfuscation is applied to programmatic defenses, which in turn impede de-obfuscation via dynamic analysis.";
            mas80.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas80);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas81 = new VulnCategory();
            mas81.Name = "MSTG-RESILIENCE-10";
            mas81.Description = "The app implements a 'device binding' functionality using a device fingerprint derived from multiple properties unique to the device.";
            mas81.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas81);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas82 = new VulnCategory();
            mas82.Name = "MSTG-RESILIENCE-11";
            mas82.Description = "All executable files and libraries belonging to the app are either encrypted on the file level and/or important code and data segments inside the executables are encrypted or packed. Trivial static analysis does not reveal important code or data.";
            mas82.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas82);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas83 = new VulnCategory();
            mas83.Name = "MSTG-RESILIENCE-12";
            mas83.Description = "If the goal of obfuscation is to protect sensitive computations, an obfuscation scheme is used that is both appropriate for the particular task and robust against manual and automated de-obfuscation methods, considering currently published research. The effectiveness of the obfuscation scheme must be verified through manual testing. Note that hardware-based isolation features are preferred over obfuscation whenever possible.";
            mas83.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas83);
            vulnCategoryManager.Context.SaveChanges();
            
            var mas84 = new VulnCategory();
            mas84.Name = "MSTG-RESILIENCE-13";
            mas84.Description = "As a defense in depth, next to having solid hardening of the communicating parties, application level payload encryption can be applied to further impede eavesdropping.";
            mas84.Type = VulnCategoryType.OwaspMSTG;
            vulnCategoryManager.Add(mas84);
            vulnCategoryManager.Context.SaveChanges();
            
        }
    }

}