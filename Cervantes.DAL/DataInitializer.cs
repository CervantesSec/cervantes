using System;
using Cervantes.CORE;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Xml;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Ganss.Xss;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.DAL;

public class DataInitializer
{
    public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        Contracts.IVulnCategoryManager vulnCategoryManager,
        Contracts.IOrganizationManager organizationManager, Contracts.IReportTemplateManager reportTemplateManager, 
        ICweManager cweManager, string cwe, IReportComponentsManager reportComponentsManager, IReportsPartsManager reportsPartsManager)
    {
        SeedRoles(roleManager);
        SeedUsers(userManager);
        SeedVulnCategories(vulnCategoryManager);
        SeedOrganization(organizationManager);
        SeedTemplates(reportTemplateManager,userManager,reportComponentsManager,reportsPartsManager);
        SeedCwe(cweManager,cwe);
    }
    
    public static void SeedCwe(ICweManager cweManager, string cweFile)
    {
        if (cweManager.GetAll().Count() == 0)
        {
            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.Load(cweFile);

            XmlNodeList weaknessNodes = xmlDocument.GetElementsByTagName("Weakness");
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedSchemes.Add("data");
            
            foreach (XmlNode weaknessNode in weaknessNodes)
            {
                string id = weaknessNode.Attributes["ID"].Value;

                string name = weaknessNode.Attributes["Name"].Value;

                string desc = weaknessNode.ChildNodes[0].InnerText;


                Cwe cwe = new Cwe();
                cwe.Id = Convert.ToInt32(id);
                cwe.Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(name));
                cwe.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(desc));
                
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
            user.Id = Guid.NewGuid().ToString();
            user.UserName = "admin@cervantes.local";
            user.Email = "admin@cervantes.local";
            user.FullName = "Administrator";
            user.Position = "Administrator";
            user.Avatar = "Attachments/Users/logo.png";
            user.Description = "Administrator";
            user.ExternalLogin = false;

            int length = 20;
            var randomNumber = new byte[32];
            string password;
            //string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            password = Convert.ToBase64String(randomNumber);
            Console.WriteLine("Your password for admin@cervantes.local is "+password);
            var result = userManager.CreateAsync(user, password).Result;

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
            org.ImagePath = "Attachments/Organization/cervantes.png";
            organizationManager.Add(org);
            organizationManager.Context.SaveChanges();
        }
    }

    public static void SeedTemplates(IReportTemplateManager reportTemplateManager, UserManager<ApplicationUser> userManager, IReportComponentsManager reportComponentsManager, IReportsPartsManager reportsPartsManager)
    {

        if (reportComponentsManager.GetAll().Count() == 0)
        {
            ApplicationUser admin =  userManager.FindByEmailAsync("admin@cervantes.local").Result;

            #region English
                #region Components general english
                ReportComponents coverGeneralComponent = new ReportComponents();
                coverGeneralComponent.Id = Guid.NewGuid();
                coverGeneralComponent.Name = "General - Cover";
                coverGeneralComponent.Language = Language.English;
                coverGeneralComponent.ComponentType = ReportPartType.Cover;
                coverGeneralComponent.Created = DateTime.Now.ToUniversalTime();
                coverGeneralComponent.Updated = DateTime.Now.ToUniversalTime();
                coverGeneralComponent.ContentCss = "";
                coverGeneralComponent.Content = @"<p>&nbsp;</p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <p style=""text-align: center;""><img src=""data:image/png;base64,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"" width=""350"" height=""350""></p>
                <p style=""text-align: center;"">&nbsp;</p>
                <p style=""text-align: center;"">&nbsp;</p>
                <p style=""text-align: center;"">&nbsp;</p>
                <h1 style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{ProjectName}}</span></h1>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <p style=""text-align: center;"">Prepared by {{OrganizationName}}</p>
                <p style=""text-align: center;"">Prepared for {{ClientName}}</p>
                <p style=""text-align: center;"">&nbsp;</p>
                <p style=""text-align: center;"">&nbsp;</p>
                <p style=""text-align: center;"">Start Date: {{StartDate}}</p>
                <p style=""text-align: center;"">End Date: {{EndDate}}</p>
                <p>&nbsp;</p>";
                reportComponentsManager.Add(coverGeneralComponent);
                reportComponentsManager.Context.SaveChanges();
                
                
                ReportComponents headerGeneralComponent = new ReportComponents();
                headerGeneralComponent.Id = Guid.NewGuid();
                headerGeneralComponent.Name = "General - Header";
                headerGeneralComponent.Language = Language.English;
                headerGeneralComponent.ComponentType = ReportPartType.Header;
                headerGeneralComponent.Created = DateTime.Now.ToUniversalTime();
                headerGeneralComponent.Updated = DateTime.Now.ToUniversalTime();
                headerGeneralComponent.ContentCss = "";
                headerGeneralComponent.Content = @"<table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none;"" border=""1""><colgroup><col style=""width: 33.2692%;""><col style=""width: 33.2692%;""><col style=""width: 33.2692%;""></colgroup>
                                                <tbody>
                                                <tr>
                                                <td style=""border-style: none;""><span style=""color: rgb(0, 0, 0);""><em>{{OrganizationName}}</em></span></td>
                                                <td style=""border-style: none; text-align: center;""><span style=""color: rgb(0, 0, 0);""><em>Privileged and Confidential&nbsp;</em></span></td>
                                                <td style=""border-style: none; text-align: right;""><span style=""color: rgb(0, 0, 0);""><em>{{ProjectName}}</em></span></td>
                                                </tr>
                                                </tbody>
                                                </table><hr>
                                                <p>&nbsp;</p>";
                reportComponentsManager.Add(headerGeneralComponent);
                reportComponentsManager.Context.SaveChanges();
                
                ReportComponents footerGeneralComponent = new ReportComponents();
                footerGeneralComponent.Id = Guid.NewGuid();
                footerGeneralComponent.Name = "General - Footer";
                footerGeneralComponent.Language = Language.English;
                footerGeneralComponent.ComponentType = ReportPartType.Footer;
                footerGeneralComponent.Created = DateTime.Now.ToUniversalTime();
                footerGeneralComponent.Updated = DateTime.Now.ToUniversalTime();
                footerGeneralComponent.ContentCss = "";
                footerGeneralComponent.Content = @"<hr>
                <table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none;"" border=""1""><colgroup><col style=""width: 33.2692%;""><col style=""width: 33.2692%;""><col style=""width: 33.2692%;""></colgroup>
                <tbody>
                <tr>
                <td style=""border-style: none;""><span style=""color: rgb(0, 0, 0);""><em>{{Year}}</em></span></td>
                <td style=""border-style: none;"">&nbsp;</td>
                <td style=""border-style: none; text-align: right;""><span style=""color: rgb(0, 0, 0);""><em>Prepared for {{ClientName}}</em></span></td>
                </tr>
                </tbody>
                </table>";
                reportComponentsManager.Add(footerGeneralComponent);
                reportComponentsManager.Context.SaveChanges();
                
                
                ReportComponents disclaimerGeneralComponent = new ReportComponents();
                disclaimerGeneralComponent.Id = Guid.NewGuid();
                disclaimerGeneralComponent.Name = "General - Disclaimer";
                disclaimerGeneralComponent.Language = Language.English;
                disclaimerGeneralComponent.ComponentType = ReportPartType.Body;
                disclaimerGeneralComponent.Created = DateTime.Now.ToUniversalTime();
                disclaimerGeneralComponent.Updated = DateTime.Now.ToUniversalTime();
                disclaimerGeneralComponent.ContentCss = "";
                disclaimerGeneralComponent.Content = @"<table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none;"" border=""1""><colgroup><col style=""width: 50%;""><col style=""width: 50%;""></colgroup>
                <tbody>
                <tr>
                <td style=""border-style: none;"">
                <p><span style=""color: rgb(0, 0, 0);""><strong>{{OrganizationName}}</strong></span></p>
                <p><span style=""color: rgb(0, 0, 0);"">{{OrganizationEmail}}</span></p>
                <p><span style=""color: rgb(0, 0, 0);"">{{OrganizationPhone}}</span></p>
                </td>
                <td style=""border-style: none;"">
                <p style=""text-align: right;""><span style=""color: rgb(0, 0, 0);""><strong>{{ClientName}}</strong></span></p>
                <p style=""text-align: right;""><span style=""color: rgb(0, 0, 0);"">{{ClientEmail}}</span></p>
                <p style=""text-align: right;""><span style=""color: rgb(0, 0, 0);"">{{ClientPhone}}</span></p>
                </td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <h2><span style=""color: rgb(0, 0, 0);"">&nbsp;</span><br><span style=""color: rgb(0, 0, 0);"">Disclaimer</span><br><span style=""color: rgb(0, 0, 0);"">&nbsp;</span></h2>
                <p><span style=""color: rgb(0, 0, 0);"">No warranties, express or implied are given by {{OrganizationName}} with respect to accuracy, reliability, quality, correctness, or freedom from error or omission of this work product, including any implied warranties of merchantability, fitness for a specific purpose or non-infringement. This document is delivered ""as is"", and {{OrganizationName}} shall not be liable for any inaccuracy thereof. {{OrganizationName}} does not warrant that all errors in this work product shall be corrected. Except as expressly set forth in any master services agreement or project assignment, {{OrganizationName}} is not assuming any obligations or liabilities including but not limited to direct, indirect, incidental or consequential, special or exemplary damages resulting from the use of or reliance upon any information in this document. This document does not imply an endorsement of any of the companies or products mentioned.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">&copy; {{Year}} {{OrganizationName}}. All rights reserved. No part of this document may be reproduced, copied or modified without the express written consent of the authors. Unless written permission is expressly granted for other purposes, this document shall be treated at all times as the confidential and proprietary material of {{OrganizationName}} and may not be distributed or published to any third-party.</span></p>";
                reportComponentsManager.Add(disclaimerGeneralComponent);
                
                ReportComponents purposeGeneralComponent = new ReportComponents();
                purposeGeneralComponent.Id = Guid.NewGuid();
                purposeGeneralComponent.Name = "General - Project Purpose";
                purposeGeneralComponent.Language = Language.English;
                purposeGeneralComponent.ComponentType = ReportPartType.Body;
                purposeGeneralComponent.Created = DateTime.Now.ToUniversalTime();
                purposeGeneralComponent.Updated = DateTime.Now.ToUniversalTime();
                purposeGeneralComponent.ContentCss = "";
                purposeGeneralComponent.Content = @"<h1>Purpose</h1>
                <p><span style=""color: rgb(0, 0, 0);"">{{ProjectDescription}}</span></p>
                <p>&nbsp;</p>";
                reportComponentsManager.Add(purposeGeneralComponent);
                
                ReportComponents documentControlGeneralComponent = new ReportComponents();
                documentControlGeneralComponent.Id = Guid.NewGuid();
                documentControlGeneralComponent.Name = "General - Document Control";
                documentControlGeneralComponent.Language = Language.English;
                documentControlGeneralComponent.ComponentType = ReportPartType.Body;
                documentControlGeneralComponent.Created = DateTime.Now.ToUniversalTime();
                documentControlGeneralComponent.Updated = DateTime.Now.ToUniversalTime();
                documentControlGeneralComponent.ContentCss = "";
                documentControlGeneralComponent.Content = @"<h1><span style=""color: rgb(0, 0, 0);"">Document Control</span></h1>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 33.3333%;""><col style=""width: 33.3333%;""><col style=""width: 33.3333%;""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">Name</span></strong></td>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">Version</span></strong></td>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">Description</span></strong></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{tablerow doc in Documents}}{{doc.DocumentName}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{doc.DocumentVersion}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{doc.DocumentDescription}}{{end}}</span></td>
                </tr>
                </tbody>
                </table>";
                reportComponentsManager.Add(documentControlGeneralComponent);
                
                ReportComponents introductionGeneralComponent = new ReportComponents();
                introductionGeneralComponent.Id = Guid.NewGuid();
                introductionGeneralComponent.Name = "General - Introduction";
                introductionGeneralComponent.Language = Language.English;
                introductionGeneralComponent.ComponentType = ReportPartType.Body;
                introductionGeneralComponent.Created = DateTime.Now.ToUniversalTime();
                introductionGeneralComponent.Updated = DateTime.Now.ToUniversalTime();
                introductionGeneralComponent.ContentCss = "";
                introductionGeneralComponent.Content = @"<h1><span style=""color: rgb(0, 0, 0);"">Introduction</span></h1>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">This document presents the results of a Security Review for {{ClientName}} . This engagement aimed to verify to discover security vulnerabilities that could negatively affect the {{ClientName}} networks or systems</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">{{ClientDescription}}</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">For each vulnerability discovered during the&nbsp;assessment, {{OrganizationName}} attributed a risk severity rating and, whenever possible, validated the existence of the vulnerability with a working exploit code.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">The main objectives of the assessment were the following:</span></p>
                <p>&nbsp;</p>
                <ul>
                <li style=""color: rgb(0, 0, 0);""><span style=""color: rgb(0, 0, 0);"">Identify the main security-related issues present.</span></li>
                <li style=""color: rgb(0, 0, 0);""><span style=""color: rgb(0, 0, 0);"">Assess the level of secure coding practices present in the project</span></li>
                <li style=""color: rgb(0, 0, 0);""><span style=""color: rgb(0, 0, 0);"">Obtain evidences for each vulnerability and, if possible, develop a working exploit</span></li>
                <li style=""color: rgb(0, 0, 0);""><span style=""color: rgb(0, 0, 0);"">Document, in a clear and easy to reproduce manner, all procedures used to replicate the issue</span></li>
                <li style=""color: rgb(0, 0, 0);""><span style=""color: rgb(0, 0, 0);"">Recommend mitigation factors and fixes for each defect identified in the analysis</span></li>
                <li style=""color: rgb(0, 0, 0);""><span style=""color: rgb(0, 0, 0);"">Provide context with a real risk scenario based on a realistic threat model</span></li>
                </ul>
                <p>&nbsp;</p>";
                
                reportComponentsManager.Add(introductionGeneralComponent);
                
                ReportComponents teamGeneralComponent = new ReportComponents();
                teamGeneralComponent.Id = Guid.NewGuid();
                teamGeneralComponent.Name = "General - Team";
                teamGeneralComponent.Language = Language.English;
                teamGeneralComponent.ComponentType = ReportPartType.Body;
                teamGeneralComponent.Created = DateTime.Now.ToUniversalTime();
                teamGeneralComponent.Updated = DateTime.Now.ToUniversalTime();
                teamGeneralComponent.ContentCss = "";
                teamGeneralComponent.Content = @"<h1><span style=""color: rgb(0, 0, 0);"">Team</span></h1>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 33.3333%;""><col style=""width: 33.3333%;""><col style=""width: 33.3333%;""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">Name</span></strong></td>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">Version</span></strong></td>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">Position</span></strong></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{tablerow user in Users}}{{user.UserFullName}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{user.UserEmail}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{user.UserPosition}}{{end}}</span></td>
                </tr>
                </tbody>
                </table>";
                reportComponentsManager.Add(teamGeneralComponent);
                
                ReportComponents executiveSummaryGeneralComponent = new ReportComponents();
                executiveSummaryGeneralComponent.Id = Guid.NewGuid();
                executiveSummaryGeneralComponent.Name = "General - Executive Summary";
                executiveSummaryGeneralComponent.Language = Language.English;
                executiveSummaryGeneralComponent.ComponentType = ReportPartType.Body;
                executiveSummaryGeneralComponent.Created = DateTime.Now.ToUniversalTime();
                executiveSummaryGeneralComponent.Updated = DateTime.Now.ToUniversalTime();
                executiveSummaryGeneralComponent.ContentCss = "";
                executiveSummaryGeneralComponent.Content = @"<h1><span style=""color: rgb(0, 0, 0);"">Executive Summary</span></h1>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">{{ProjectExecutiveSummary}}</span></p>";
                
                reportComponentsManager.Add(executiveSummaryGeneralComponent);
                
                ReportComponents scopeGeneralComponent = new ReportComponents();
                scopeGeneralComponent.Id = Guid.NewGuid();
                scopeGeneralComponent.Name = "General - Scope";
                scopeGeneralComponent.Language = Language.English;
                scopeGeneralComponent.ComponentType = ReportPartType.Body;
                scopeGeneralComponent.Created = DateTime.Now.ToUniversalTime();
                scopeGeneralComponent.Updated = DateTime.Now.ToUniversalTime();
                scopeGeneralComponent.ContentCss = "";
                scopeGeneralComponent.Content = @"<h1><span style=""color: rgb(0, 0, 0);"">Scope</span></h1>
                <p><span style=""color: rgb(0, 0, 0);"">In accordance with the contract signed between {{OrganizationName}} and {{ClientName}}, the penetration test was performed between {{StartDate}} and {{EndDate}}</span></p>
                <p><span style=""color: rgb(0, 0, 0);"">The scope of the test was limited to targets listed below.</span></p>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 33.3333%;""><col style=""width: 33.3333%;""><col style=""width: 33.3333%;""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">Target</span></strong></td>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">Description</span></strong></td>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">Type</span></strong></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{tablerow target in Targets}}{{target.TargetName}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{target.TargetDescription}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{target.TargetType}}{{end}}</span></td>
                </tr>
                </tbody>
                </table>";
                reportComponentsManager.Add(scopeGeneralComponent);
                
                ReportComponents findingsOverViewGeneralComponent = new ReportComponents();
                findingsOverViewGeneralComponent.Id = Guid.NewGuid();
                findingsOverViewGeneralComponent.Name = "General - Findings Overview (Vulnerabilities)";
                findingsOverViewGeneralComponent.Language = Language.English;
                findingsOverViewGeneralComponent.ComponentType = ReportPartType.Body;
                findingsOverViewGeneralComponent.Created = DateTime.Now.ToUniversalTime();
                findingsOverViewGeneralComponent.Updated = DateTime.Now.ToUniversalTime();
                findingsOverViewGeneralComponent.ContentCss = "";
                findingsOverViewGeneralComponent.Content = @"<h1><span style=""color: rgb(0, 0, 0);"">Findings Overview</span></h1>
                    <p>&nbsp;</p>
                    <p><span style=""color: rgb(0, 0, 0);"">The following sections list both vulnerabilities and implementation issues spotted during the testing period. Note that findings are listed by their degree of severity and impact. The aforementioned severity rank is simply given in brackets following the title heading for each vulnerability</span></p>
                    <p>&nbsp;</p>
                    <table style=""border-collapse: collapse; width: 100%; height: 134.344px;"" border=""1""><colgroup><col style=""width: 50%;""><col style=""width: 50%;""></colgroup>
                    <tbody>
                    <tr style=""height: 22.3906px;"">
                    <td style=""background-color: rgb(0, 0, 0); text-align: center; height: 22.3906px;""><strong><span style=""color: rgb(255, 255, 255);"">Severity</span></strong></td>
                    <td style=""background-color: rgb(0, 0, 0); text-align: center; height: 22.3906px;""><strong><span style=""color: rgb(255, 255, 255);"">Count</span></strong></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; background-color: rgb(186, 55, 42); height: 22.3906px;""><span style=""color: rgb(255, 255, 255);""><strong>Critical</strong></span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{VulnCriticalCount}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; background-color: rgb(224, 62, 45); height: 22.3906px;""><span style=""color: rgb(255, 255, 255);""><strong>High</strong></span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{VulnHighCount}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; background-color: rgb(230, 126, 35); height: 22.3906px;""><span style=""color: rgb(255, 255, 255);""><strong>Medium</strong></span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{VulnMediumCount}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; background-color: rgb(241, 196, 15); height: 22.3906px;""><span style=""color: rgb(255, 255, 255);""><strong>Low</strong></span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{VulnLowCount}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; background-color: rgb(53, 152, 219); height: 22.3906px;""><span style=""color: rgb(255, 255, 255);""><strong>Info</strong></span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{VulnInfoCount}}</span></td>
                    </tr>
                    </tbody>
                    </table>";
                reportComponentsManager.Add(findingsOverViewGeneralComponent);
                
                ReportComponents findingsClassificationGeneralComponent = new ReportComponents();
                findingsClassificationGeneralComponent.Id = Guid.NewGuid();
                findingsClassificationGeneralComponent.Name = "General - Findings Classification (Vulnerabilities)";
                findingsClassificationGeneralComponent.Language = Language.English;
                findingsClassificationGeneralComponent.ComponentType = ReportPartType.Body;
                findingsClassificationGeneralComponent.Created = DateTime.Now.ToUniversalTime();
                findingsClassificationGeneralComponent.Updated = DateTime.Now.ToUniversalTime();
                findingsClassificationGeneralComponent.ContentCss = "";
                findingsClassificationGeneralComponent.Content = @"<h1><span style=""color: rgb(0, 0, 0);"">Findings Classification</span></h1>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">Each vulnerability or risk identified has been labeled as a finding and categorized as a Critical, High, Medium, Low or Informational Risk which are defined as:</span></p>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 99.9444%;""></colgroup>
                <tbody>
                <tr>
                <td style=""text-align: center; background-color: rgb(186, 55, 42);""><strong><span style=""color: rgb(255, 255, 255);"">Critical</span></strong></td>
                </tr>
                <tr>
                <td>
                <p><span style=""color: rgb(0, 0, 0);"">These vulnerabilities should be addressed promptly as they may pose an immediate danger to the security of the networks, systems or data.</span></p>
                <p><span style=""color: rgb(0, 0, 0);"">Exploitation does not require advanced tools or techniques or special knowledge of the target.</span></p>
                </td>
                </tr>
                <tr>
                <td style=""text-align: center; background-color: rgb(224, 62, 45);""><strong><span style=""color: rgb(255, 255, 255);"">High</span></strong></td>
                </tr>
                <tr>
                <td>
                <p><span style=""color: rgb(0, 0, 0);"">These vulnerabilities should be addressed promptly as they may pose an immediate danger to the security of the networks, systems or data.</span></p>
                <p><span style=""color: rgb(0, 0, 0);"">The issue is commonly more difficult to exploit but could allow for elevated permissions, loss of data, or a system downtime.</span></p>
                </td>
                </tr>
                <tr>
                <td style=""text-align: center; background-color: rgb(230, 126, 35);""><strong><span style=""color: rgb(255, 255, 255);"">Medium</span></strong></td>
                </tr>
                <tr>
                <td>
                <p><span style=""color: rgb(0, 0, 0);"">These vulnerabilities should be addressed promptly in a timely manner.</span></p>
                <p><span style=""color: rgb(0, 0, 0);"">Exploitation is often difficult to exploit and requires social engineering, existing access, or special circumstances.</span></p>
                </td>
                </tr>
                <tr>
                <td style=""text-align: center; background-color: rgb(241, 196, 15);""><span style=""color: rgb(255, 255, 255);""><strong>Low</strong></span></td>
                </tr>
                <tr>
                <td>
                <p><span style=""color: rgb(0, 0, 0);"">The vulnerabilities should be noted and addressed at a later date.</span></p>
                <p><span style=""color: rgb(0, 0, 0);"">These issues offer very little opportunity or information to an attacker and may not pose an actual threat.</span></p>
                </td>
                </tr>
                <tr>
                <td style=""text-align: center; background-color: rgb(53, 152, 219);""><span style=""color: rgb(255, 255, 255);""><strong>Info</strong></span></td>
                </tr>
                <tr>
                <td><span style=""color: rgb(0, 0, 0);"">These issues are for informational purposes only and likely do not represent actual threat.</span></td>
                </tr>
                </tbody>
                </table>";
                reportComponentsManager.Add(findingsClassificationGeneralComponent);
                
                ReportComponents findingsDetailsGeneralComponent = new ReportComponents();
                findingsDetailsGeneralComponent.Id = Guid.NewGuid();
                findingsDetailsGeneralComponent.Name = "General - Findings Details (Vulnerabilities)";
                findingsDetailsGeneralComponent.Language = Language.English;
                findingsDetailsGeneralComponent.ComponentType = ReportPartType.Body;
                findingsDetailsGeneralComponent.Created = DateTime.Now.ToUniversalTime();
                findingsDetailsGeneralComponent.Updated = DateTime.Now.ToUniversalTime();
                findingsDetailsGeneralComponent.ContentCss = "";
                findingsDetailsGeneralComponent.Content = @"<h1><span style=""color: rgb(0, 0, 0);"">Findings</span></h1>
                <p>&nbsp;</p>
                <p>{{for vuln in Vulns}}</p>
                <h2><span style=""color: rgb(0, 0, 0);"">{{vuln.VulnFindingId}} - {{vuln.VulnName}}</span></h2>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 16.6263%;""><col style=""width: 16.6263%;""><col style=""width: 16.6263%;""><col style=""width: 16.6263%;""><col style=""width: 16.6263%;""><col style=""width: 16.6263%;""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">CVE</span></strong></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{vuln.VulnCve}}</span></td>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">Risk</span></strong></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{vuln.VulnRisk}}</span></td>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">CVSS</span></strong></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{vuln.VulnCvss}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 7.91599%;""><col style=""width: 42.1648%;""><col style=""width: 6.70273%;""><col style=""width: 43.2165%;""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">Category</span></strong></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{vuln.VulnCategory}}</span></td>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">CWE</span></strong></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{vuln.VulnCwes}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 99.9183%;""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">Description</span></strong></td>
                </tr>
                <tr>
                <td style=""text-align: justify;""><span style=""color: rgb(0, 0, 0);"">{{vuln.VulnDescription}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 99.9183%;""></colgroup>
                <tbody>
                <tr>
                <td style=""text-align: center; background-color: rgb(0, 0, 0);""><span style=""color: rgb(255, 255, 255);""><strong>Impact</strong></span></td>
                </tr>
                <tr>
                <td><span style=""color: rgb(0, 0, 0);"">{{vuln.VulnImpact}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 99.9183%;""></colgroup>
                <tbody>
                <tr>
                <td style=""text-align: center; background-color: rgb(0, 0, 0);""><span style=""color: rgb(255, 255, 255);""><strong>Proof of Concept</strong></span></td>
                </tr>
                <tr>
                <td><span style=""color: rgb(0, 0, 0);"">{{vuln.VulnPoc}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 99.9183%;""></colgroup>
                <tbody>
                <tr>
                <td style=""text-align: center; background-color: rgb(0, 0, 0);""><span style=""color: rgb(255, 255, 255);""><strong>Remediation</strong></span></td>
                </tr>
                <tr>
                <td><span style=""color: rgb(0, 0, 0);"">{{vuln.VulnRemediation}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 25%;""><col style=""width: 25%;""><col style=""width: 25%;""><col style=""width: 25%;""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">Complexity</span></strong></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{vuln.VulnComplexity}}</span></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0);""><span style=""color: rgb(255, 255, 255);""><strong>Priority</strong></span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{vuln.VulnPriority}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 17.116%;""><col style=""width: 82.9657%;""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgb(0, 0, 0); text-align: center;""><strong><span style=""color: rgb(255, 255, 255);"">Assets Affected</span></strong></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{vuln.VulnTargets}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <p>{{end}}</p>
                <p>&nbsp;</p>";
                reportComponentsManager.Add(findingsDetailsGeneralComponent);
                
                
                ReportComponents methodlogyGeneralComponent = new ReportComponents();
                methodlogyGeneralComponent.Id = Guid.NewGuid();
                methodlogyGeneralComponent.Name = "General - Methodology";
                methodlogyGeneralComponent.Language = Language.English;
                methodlogyGeneralComponent.ComponentType = ReportPartType.Body;
                methodlogyGeneralComponent.Created = DateTime.Now.ToUniversalTime();
                methodlogyGeneralComponent.Updated = DateTime.Now.ToUniversalTime();
                methodlogyGeneralComponent.ContentCss = "";
                methodlogyGeneralComponent.Content = @"<h2><span style=""color: rgb(0, 0, 0);"">Methodology</span></h2>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">The methodology consisted of 7 of steps beginning with the determination of test scope, and ending with reporting. These tests were performed by security experts using potential attackers&rsquo; modes of operation while controlling execution to prevent harm to the systems being tested. The approach included but is not limited to manual and automated vulnerability scans, verification of findings (automated and otherwise). This verification step and manual scanning process eliminated false positives and erroneous outputs, resulting in more efficient tests.</span></p>
                <p>&nbsp;</p>
                <ul>
                <li style=""color: rgb(0, 0, 0);""><span style=""color: rgb(0, 0, 0);"">Determining scope of the test</span></li>
                <li style=""color: rgb(0, 0, 0);""><span style=""color: rgb(0, 0, 0);"">Information Gathering / Reconnaissance</span></li>
                <li style=""color: rgb(0, 0, 0);""><span style=""color: rgb(0, 0, 0);"">Scanning</span></li>
                <li style=""color: rgb(0, 0, 0);""><span style=""color: rgb(0, 0, 0);"">Vulnerability Analysis</span></li>
                <li style=""color: rgb(0, 0, 0);""><span style=""color: rgb(0, 0, 0);"">Exploitation</span></li>
                <li style=""color: rgb(0, 0, 0);""><span style=""color: rgb(0, 0, 0);"">Post-Exploitation activities</span></li>
                <li style=""color: rgb(0, 0, 0);""><span style=""color: rgb(0, 0, 0);"">Reporting</span></li>
                </ul>";
                reportComponentsManager.Add(methodlogyGeneralComponent);
                reportComponentsManager.Context.SaveChanges();
                #endregion
                
                #region OWASP MASTG English
                
                 ReportComponents coverOwaspMastgComponent = new ReportComponents();
                 coverOwaspMastgComponent.Id = Guid.NewGuid();
                 coverOwaspMastgComponent.Name = "OWASP MASTG - Cover";
                 coverOwaspMastgComponent.Language = Language.English;
                 coverOwaspMastgComponent.ComponentType = ReportPartType.Cover;
                 coverOwaspMastgComponent.Created = DateTime.Now.ToUniversalTime();
                 coverOwaspMastgComponent.Updated = DateTime.Now.ToUniversalTime();
                 coverOwaspMastgComponent.ContentCss = "";
                 coverOwaspMastgComponent.Content = @"<p>&nbsp;</p>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none;"" border=""1""><colgroup><col style=""width: 51.8049%;""><col style=""width: 48.1951%;""></colgroup>
                <tbody>
                <tr>
                <td style=""border-style: none; text-align: center;""><img style=""float: left;"" src=""data:image/png;base64,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"" width=""250"" height=""87""></td>
                <td style=""border-style: none;""><img style=""float: right;"" src=""data:image/png;base64,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"" width=""270"" height=""54""></td>
                </tr>
                </tbody>
                </table>
                <h1 style=""text-align: center;"">&nbsp;</h1>
                <p>&nbsp;</p>
                <h1 style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">OWASP MASTG REPORT</span></h1>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none;"" border=""1""><colgroup><col style=""width: 50%;""><col style=""width: 50%;""></colgroup>
                <tbody>
                <tr>
                <td style=""border-style: none;"">
                <p><span style=""color: rgb(0, 0, 0);""><strong>Organization: {{OrganizationName}}</strong></span></p>
                <p><span style=""color: rgb(0, 0, 0);"">Contact Email: {{OrganizationEmail}}</span></p>
                <p><span style=""color: rgb(0, 0, 0);"">Contact Phone: {{OrganizationPhone}}</span></p>
                </td>
                <td style=""border-style: none;"">
                <p style=""text-align: right;""><span style=""color: rgb(0, 0, 0);""><strong>Client: {{ClientName}}</strong></span></p>
                <p style=""text-align: right;""><span style=""color: rgb(0, 0, 0);"">Client Email: {{ClientEmail}}</span></p>
                <p style=""text-align: right;""><span style=""color: rgb(0, 0, 0);"">Client Phone: {{ClientPhone}}</span></p>
                </td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none;"" border=""1""><colgroup><col style=""width: 50%;""><col style=""width: 50%;""></colgroup>
                <tbody>
                <tr>
                <td style=""text-align: center; border-style: none;""><span style=""color: rgb(0, 0, 0);""><strong>Url</strong>: {{TargetName}}</span></td>
                <td style=""text-align: center; border-style: none;""><span style=""color: rgb(0, 0, 0);""><strong>Date</strong>: {{CreatedDate}}</span></td>
                </tr>
                </tbody>
                </table>
                <div style=""break-after: page;"">&nbsp;</div>";
                reportComponentsManager.Add(coverOwaspMastgComponent);

                ReportComponents headerOwaspMastgComponent = new ReportComponents();
                headerOwaspMastgComponent.Id = Guid.NewGuid();
                headerOwaspMastgComponent.Name = "OWASP MASTG - Header";
                headerOwaspMastgComponent.Language = Language.English;
                headerOwaspMastgComponent.ComponentType = ReportPartType.Header;
                headerOwaspMastgComponent.Created = DateTime.Now.ToUniversalTime();
                headerOwaspMastgComponent.Updated = DateTime.Now.ToUniversalTime();
                headerOwaspMastgComponent.ContentCss = "";
                headerOwaspMastgComponent.Content = @"<table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none;"" border=""1""><colgroup><col style=""width: 33.2692%;""><col style=""width: 33.2692%;""><col style=""width: 33.2692%;""></colgroup>
                <tbody>
                <tr>
                <td style=""border-style: none;""><em>Privileged and Confidential&nbsp;</em></td>
                <td style=""border-style: none;"">&nbsp;</td>
                <td style=""border-style: none; text-align: right;""><em>OWASP MASTG Report</em></td>
                </tr>
                </tbody>
                </table>
                <hr>
                <p>&nbsp;</p>";
                reportComponentsManager.Add(headerOwaspMastgComponent);

                
                ReportComponents introOwaspMastgComponent = new ReportComponents();
                introOwaspMastgComponent.Id = Guid.NewGuid();
                introOwaspMastgComponent.Name = "OWASP MASTG - Intro";
                introOwaspMastgComponent.Language = Language.English;
                introOwaspMastgComponent.ComponentType = ReportPartType.Body;
                introOwaspMastgComponent.Created = DateTime.Now.ToUniversalTime();
                introOwaspMastgComponent.Updated = DateTime.Now.ToUniversalTime();
                introOwaspMastgComponent.ContentCss = "";
                introOwaspMastgComponent.Content = @"<h2><span style=""color: rgb(0, 0, 0);"">Introduction</span></h2>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">{{ClientName}} we are pleased to present you with the Open Web Application Security Project (OWASP) Mobile Application Security Testing Guid (MASTG) report as a result of our recent penetration testing engagement on your digital infrastructure. The OWASP MASTG report provides a comprehensive examination of your app from a security perspective.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">Over the course of our engagement, we have utilized the OWASP MASTG, an industry-standard methodology for robust web application security testing. Our team of highly skilled penetration testers has conducted a systematic assessment of your web applications to identify any existing vulnerabilities, threats or weaknesses that could potentially be exploited by malevolent parties.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">It's important to note that this report provides an overview of the identified vulnerabilities. The recommendations given in this report are designed to assist your organization in minimizing the risk of any future security breaches, in light of ever-evolving cyber threats.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">Please review this report thoroughly, and let us know if there are any areas that you would like to discuss further or need additional clarification on.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">We look forward to your feedback, and we are eager to assist you in strengthening your IT security profile. Cybersecurity is a journey, not a destination, and we are committed to staying your trusted partner in this journey.</span></p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>";
                reportComponentsManager.Add(introOwaspMastgComponent);

                ReportComponents resultsAndroidOwaspMastgComponent = new ReportComponents();
                resultsAndroidOwaspMastgComponent.Id = Guid.NewGuid();
                resultsAndroidOwaspMastgComponent.Name = "OWASP MASTG - Results (Android)";
                resultsAndroidOwaspMastgComponent.Language = Language.English;
                resultsAndroidOwaspMastgComponent.ComponentType = ReportPartType.Body;
                resultsAndroidOwaspMastgComponent.Created = DateTime.Now.ToUniversalTime();
                resultsAndroidOwaspMastgComponent.Updated = DateTime.Now.ToUniversalTime();
                resultsAndroidOwaspMastgComponent.ContentCss = "";
                resultsAndroidOwaspMastgComponent.Content = @"<h2>Results</h2>
                    <p>&nbsp;</p>
                    <table style=""border-collapse: collapse; width: 100%; height: 268.687px;"" border=""1""><colgroup><col style=""width: 11.9632%;""><col style=""width: 45.5419%;""><col style=""width: 4.45814%;""><col style=""width: 4.21779%;""><col style=""width: 5.06135%;""><col style=""width: 14.6472%;""><col style=""width: 14.1104%;""></colgroup>
                    <tbody>
                    <tr style=""height: 44.7812px;"">
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Information Gathering</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">L1</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">L2</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">R</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;"" rowspan=""3""><span style=""color: rgb(0, 0, 0);"">MASVS-STORAGE-1</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">The app securely stores sensitive data.</span></td>
                    <td style=""height: 22.3906px; text-align: left; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage1Status}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage1Note}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Local Storage for Sensitive Data</span></td>
                    <td style=""height: 22.3906px; text-align: left; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage1Status1}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage1Note1}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing the Device-Access-Security Policy</span></td>
                    <td style=""height: 22.3906px; text-align: left;"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage1Status2}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage1Note2}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 156.734px;"" rowspan=""7""><span style=""color: rgb(0, 0, 0);"">MASVS-STORAGE-2</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app prevents leakage of sensitive data.</span></td>
                    <td style=""height: 22.3906px; text-align: left; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Status}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Note}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Memory for Sensitive Data</span></td>
                    <td style=""height: 22.3906px; text-align: left;"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Status1}}&nbsp;</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Note1}}&nbsp;</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Backups for Sensitive Data</span></td>
                    <td style=""height: 22.3906px; text-align: left;"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Status2}}&nbsp;&nbsp;</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Note2}}&nbsp;&nbsp;</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Logs for Sensitive Data</span></td>
                    <td style=""height: 22.3906px; text-align: left; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Status3}}&nbsp;&nbsp;</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Note3}}&nbsp;&nbsp;</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Determining Whether Sensitive Data Is Shared with Third Parties via Notifications</span></td>
                    <td style=""height: 22.3906px; text-align: left; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">&nbsp;{{Storage2Status4}}&nbsp;</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">&nbsp;{{Storage2Note4}}&nbsp;</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Determining Whether the Keyboard Cache Is Disabled for Text Input Fields</span></td>
                    <td style=""height: 22.3906px; text-align: left; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Status5}}&nbsp;&nbsp;</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">&nbsp;{{Storage2Note5}}&nbsp;</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Determining Whether Sensitive Data Is Shared with Third Parties via Embedded Services</span></td>
                    <td style=""height: 22.3906px; text-align: left; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: left; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">&nbsp;{{Storage2Status6}}&nbsp;</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Note6}}&nbsp;&nbsp;</span></td>
                    </tr>
                    </tbody>
                    </table>
                    <p>&nbsp;</p>
                    <table style=""border-collapse: collapse; width: 100%; height: 297px;"" border=""1""><colgroup><col style=""width: 14.2638%;""><col style=""width: 43.0215%;""><col style=""width: 4.29448%;""><col style=""width: 4.52454%;""><col style=""width: 4.83164%;""><col style=""width: 14.7236%;""><col style=""width: 14.2638%;""></colgroup>
                    <tbody>
                    <tr style=""height: 44.75px;"">
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Crypthography</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">L1</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">L2</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">R</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 89.5624px; text-align: center;"" rowspan=""4""><span style=""color: rgb(0, 0, 0);"">MASVS-CRYPTO-1</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">The app employs current strong cryptography and uses it according to industry best practices.</span></td>
                    <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto1Status}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto1Note}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing Symmetric Cryptography</span></td>
                    <td style=""height: 22.3906px; background-color: rgb(50, 153, 255); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(11, 186, 131); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto1Status1}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto1Note1}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing Random Number Generation</span></td>
                    <td style=""height: 22.3906px; background-color: rgb(50, 153, 255); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(11, 186, 131); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto1Status2}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto1Note2}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing the Configuration of Cryptographic Standard Algorithms</span></td>
                    <td style=""height: 22.3906px; background-color: rgb(50, 153, 255); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(11, 186, 131); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto1Status3}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto1Note3}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 44.7812px; text-align: center;"" rowspan=""2""><span style=""color: rgb(0, 0, 0);"">MASVS-CRYPTO-2</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">The app performs key management according to industry best practices.</span></td>
                    <td style=""height: 22.3906px; background-color: rgb(206, 212, 217); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(206, 212, 217); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(206, 212, 217); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto2Status}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto2Note}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing the Purposes of Keys</span></td>
                    <td style=""height: 22.3906px; background-color: rgb(50, 153, 255); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(11, 186, 131); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto2Status1}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto2Note1}}</span></td>
                    </tr>
                    </tbody>
                    </table>
                    <p>&nbsp;</p>
                    <table style=""border-collapse: collapse; width: 100%; height: 161.016px;"" border=""1""><colgroup><col style=""width: 14.2638%;""><col style=""width: 43.0215%;""><col style=""width: 4.29448%;""><col style=""width: 4.52454%;""><col style=""width: 4.83129%;""><col style=""width: 14.7239%;""><col style=""width: 14.2638%;""></colgroup>
                    <tbody>
                    <tr style=""height: 49.0625px;"">
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Authentication &amp; Authorization</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L1</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L2</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">R</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">MASVS-AUTH-1</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app uses secure authentication and authorization protocols and follows the relevant best practices.</span></td>
                    <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth1Status}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth1Note}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 67.1718px;"" rowspan=""3""><span style=""color: rgb(0, 0, 0);"">MASVS-AUTH-2</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app performs local authentication securely according to the platform best practices.</span></td>
                    <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(206, 212, 217); height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth2Status}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth2Note}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Confirm Credentials</span></td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""background-color: rgb(11, 186, 131); text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth2Status1}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth2Note1}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Biometric Authentication</span></td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""background-color: rgb(11, 186, 131); text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth2Status2}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth2Note2}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">MASVS-AUTH-3</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app secures sensitive operations with additional authentication.</span></td>
                    <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth3Status}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth3Note}}</span></td>
                    </tr>
                    </tbody>
                    </table>
                    <p>&nbsp;</p>
                    <table style=""border-collapse: collapse; width: 100%; height: 250.578px;"" border=""1""><colgroup><col style=""width: 14.2638%;""><col style=""width: 43.0215%;""><col style=""width: 4.29448%;""><col style=""width: 4.52454%;""><col style=""width: 4.83129%;""><col style=""width: 14.7239%;""><col style=""width: 14.2638%;""></colgroup>
                    <tbody>
                    <tr style=""height: 49.0625px;"">
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Network Communications</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L1</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L2</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">R</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                    </tr>
                    <tr style=""height: 44.7812px;"">
                    <td style=""text-align: center; height: 134.344px;"" rowspan=""5""><span style=""color: rgb(0, 0, 0);"">MASVS-NETWORK-1</span></td>
                    <td style=""text-align: center; height: 44.7812px;""><span style=""color: rgb(0, 0, 0);"">The app performs identity pinning for all remote endpoints under the developer's control.</span></td>
                    <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 44.7812px;"">&nbsp;</td>
                    <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 44.7812px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 44.7812px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 44.7812px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Status}}</span></td>
                    <td style=""text-align: center; height: 44.7812px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Note}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing the Security Provider</span></td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Status1}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Note1}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Data Encryption on the Network</span></td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Status2}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Note2}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing the TLS Settings</span></td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Status3}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Note3}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Endpoint Identify Verification</span></td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Status4}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Note4}}</span></td>
                    </tr>
                    <tr style=""height: 44.7812px;"">
                    <td style=""text-align: center; height: 67.1718px;"" rowspan=""2""><span style=""color: rgb(0, 0, 0);"">MASVS-NETWORK-2</span></td>
                    <td style=""text-align: center; height: 44.7812px;""><span style=""color: rgb(0, 0, 0);"">The app performs identity pinning for all remote endpoints under the developer's control.</span></td>
                    <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 44.7812px;"">&nbsp;</td>
                    <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 44.7812px;"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(206, 212, 217); height: 44.7812px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 44.7812px;""><span style=""color: rgb(0, 0, 0);"">{{Network2Status}}</span></td>
                    <td style=""text-align: center; height: 44.7812px;""><span style=""color: rgb(0, 0, 0);"">{{Network2Note}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Custom Certificate Stores and Certificate Pinning</span></td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network2Status1}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network2Note1}}</span></td>
                    </tr>
                    </tbody>
                    </table>
                    <p>&nbsp;</p>
                    <table style=""border-collapse: collapse; width: 100%; height: 250.578px;"" border=""1""><colgroup><col style=""width: 14.2638%;""><col style=""width: 43.0215%;""><col style=""width: 4.29448%;""><col style=""width: 4.52454%;""><col style=""width: 4.83129%;""><col style=""width: 14.7239%;""><col style=""width: 14.2638%;""></colgroup>
                    <tbody>
                    <tr style=""height: 49.0625px;"">
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Platform Interaction</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L1</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L2</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">R</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 67.1718px;"" rowspan=""6""><span style=""color: rgb(0, 0, 0);"">MASVS-PLATFORM-1</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app uses IPC mechanisms securely.</span></td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Status}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Note}}</span></td>
                    </tr>
                    <tr>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing for App Permissions</span></td>
                    <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center;"">&nbsp;</td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Status1}}</span></td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Note1}}</span></td>
                    </tr>
                    <tr>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing for Sensitive Functionality Exposure Through IPC</span></td>
                    <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center;"">&nbsp;</td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Status2}}</span></td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Note2}}</span></td>
                    </tr>
                    <tr>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing Deep Links</span></td>
                    <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center;"">&nbsp;</td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Status3}}</span></td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Note3}}</span></td>
                    </tr>
                    <tr>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing for Vulnerable Implementation of PendingIntent</span></td>
                    <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center;"">&nbsp;</td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Status4}}</span></td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Note4}}</span></td>
                    </tr>
                    <tr>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Determining Whether Sensitive Stored Data Has Been Exposed via IPC Mechanisms</span></td>
                    <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center;"">&nbsp;</td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Status5}}</span></td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Note5}}</span></td>
                    </tr>
                    <tr>
                    <td style=""text-align: center;"" rowspan=""5""><span style=""color: rgb(0, 0, 0);"">MASVS-PLATFORM-2</span></td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">The app uses WebViews securely.</span></td>
                    <td style=""text-align: center; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Status}}</span></td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Note}}</span></td>
                    </tr>
                    <tr>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing WebView Protocol Handlers</span></td>
                    <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center;"">&nbsp;</td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Status1}}</span></td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Note1}}</span></td>
                    </tr>
                    <tr>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing JavaScript Execution in WebViews</span></td>
                    <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center;"">&nbsp;</td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Status2}}</span></td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Note2}}</span></td>
                    </tr>
                    <tr>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing WebViews Cleanup</span></td>
                    <td style=""text-align: center;"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center;"">&nbsp;</td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Status3}}</span></td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Note3}}</span></td>
                    </tr>
                    <tr>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing for Java Objects Exposed Through WebViews</span></td>
                    <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center;"">&nbsp;</td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Status4}}</span></td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Note4}}</span></td>
                    </tr>
                    <tr>
                    <td style=""text-align: center;"" rowspan=""4""><span style=""color: rgb(0, 0, 0);"">MASVS-PLATFORM-3</span></td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">The app uses the user interface securely.</span></td>
                    <td style=""text-align: center; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform3Status}}</span></td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform3Note}}</span></td>
                    </tr>
                    <tr>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Checking for Sensitive Data Disclosure Through the User Interface</span></td>
                    <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center;"">&nbsp;</td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform3Status1}}</span></td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform3Note1}}</span></td>
                    </tr>
                    <tr>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Finding Sensitive Information in Auto-Generated Screenshots</span></td>
                    <td style=""text-align: center;"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center;"">&nbsp;</td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform3Status2}}</span></td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform3Note2}}</span></td>
                    </tr>
                    <tr>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing for Overlay Attacks</span></td>
                    <td style=""text-align: center;"">&nbsp;</td>
                    <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center;"">&nbsp;</td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform3Status3}}</span></td>
                    <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform3Note3}}</span></td>
                    </tr>
                    </tbody>
                    </table>
                    <p>&nbsp;</p>
                    <table style=""border-collapse: collapse; width: 100%; height: 340.14px;"" border=""1""><colgroup><col style=""width: 14.2638%;""><col style=""width: 43.0215%;""><col style=""width: 4.29448%;""><col style=""width: 4.52454%;""><col style=""width: 4.83129%;""><col style=""width: 14.7239%;""><col style=""width: 14.2638%;""></colgroup>
                    <tbody>
                    <tr style=""height: 49.0625px;"">
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Code Quality</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L1</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L2</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">R</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">MASVS-CODE-1</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app requires an up-to-date platform version.</span></td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code1Status}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code1Note}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 44.7812px;"" rowspan=""2""><span style=""color: rgb(0, 0, 0);"">MASVS-CODE-2</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app has a mechanism for enforcing app updates.</span></td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code2Status}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code2Note}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Enforced Updating</span></td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code2Status1}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code2Note1}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 44.7812px;"" rowspan=""2""><span style=""color: rgb(0, 0, 0);"">MASVS-CODE-3</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app only uses software components without known vulnerabilities.</span></td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code3Status}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code3Note}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Checking for Weaknesses in Third Party Libraries</span></td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code3Status1}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code3Note1}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 179.125px;"" rowspan=""8""><span style=""color: rgb(0, 0, 0);"">MASVS-CODE-4</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app only uses software components without known vulnerabilities.</span></td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Status}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Note}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Object Persistence</span></td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Status1}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Note1}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Make Sure That Free Security Features Are Activated</span></td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Status2}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Note2}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Local Storage for Input Validation</span></td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Status3}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Note3}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing for Injection Flaws</span></td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Status4}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Note4}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing for URL Loading in WebViews</span></td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Status5}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Note5}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Memory Corruption Bugs</span></td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Status6}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Note6}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Implicit Intents</span></td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Status7}}</span></td>
                    <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Note7}}</span></td>
                    </tr>
                    </tbody>
                    </table>
                    <p>&nbsp;</p>
                    <p>&nbsp;</p>
                    <table style=""border-collapse: collapse; width: 100%; height: 384.921px;"" border=""1""><colgroup><col style=""width: 14.2638%;""><col style=""width: 43.0215%;""><col style=""width: 4.29448%;""><col style=""width: 4.52454%;""><col style=""width: 4.83129%;""><col style=""width: 14.7239%;""><col style=""width: 14.2638%;""></colgroup>
                    <tbody>
                    <tr style=""height: 49.0625px;"">
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Resilience against reverse</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L1</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L2</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">R</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                    <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 67.1718px; text-align: center;"" rowspan=""3""><span style=""color: rgb(0, 0, 0);"">MASVS-RESILIENCE-1</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">The app validates the integrity of the platform.</span></td>
                    <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience1Status}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience1Note}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing Emulator Detection</span></td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(255, 168, 0); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience1Status1}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience1Note1}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing Root Detection</span></td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(255, 168, 0); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience1Status2}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience1Note2}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 89.5624px; text-align: center;"" rowspan=""4""><span style=""color: rgb(0, 0, 0);"">MASVS-RESILIENCE-2</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">The app implements anti-tampering mechanisms.</span></td>
                    <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience2Status}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience2Note}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing Runtime Integrity Checks</span></td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(255, 168, 0); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience2Status1}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience2Note1}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing File Integrity Checks</span></td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(255, 168, 0); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience2Status2}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience2Note2}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Making Sure that the App is Properly Signed</span></td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(255, 168, 0); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience2Status3}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience2Note3}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 89.5624px; text-align: center;"" rowspan=""4""><span style=""color: rgb(0, 0, 0);"">MASVS-RESILIENCE-3</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">The app implements anti-static analysis mechanisms.</span></td>
                    <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience3Status}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience3Note}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing for Debugging Symbols</span></td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(255, 168, 0); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience3Status1}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience3Note1}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing Obfuscation</span></td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(255, 168, 0); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience3Status2}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience3Note2}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing for Debugging Code and Verbose Error Logging</span></td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(255, 168, 0); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience3Status3}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience3Note3}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 89.5624px; text-align: center;"" rowspan=""4""><span style=""color: rgb(0, 0, 0);"">MASVS-RESILIENCE-4</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">The app implements anti-dynamic analysis techniques.</span></td>
                    <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience4Status}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience4Note}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing Anti-Debugging Detection</span></td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(255, 168, 0); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience4Status1}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience4Note1}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing whether the App is Debuggable</span></td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(255, 168, 0); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience4Status2}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience4Note2}}</span></td>
                    </tr>
                    <tr style=""height: 22.3906px;"">
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing Reverse Engineering Tools Detection</span></td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; background-color: rgb(255, 168, 0); text-align: center;"">&nbsp;</td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience4Status3}}</span></td>
                    <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Resilience4Note3}}</span></td>
                    </tr>
                    </tbody>
                    </table>
                    <p style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">L1 - Standard Security / L2 - Defense-in-Depth / R - Resilience Against Reverse Engineering and Tampering</span><br><span style=""color: rgb(53, 152, 219);""><a style=""color: rgb(53, 152, 219);"" href=""https://mas.owasp.org/"" target=""_blank"" rel=""noopener"">Based on OWASP MASTG v1.6.0 &amp; OWASP MASVS 2.0.0</a></span></p>";
                reportComponentsManager.Add(resultsAndroidOwaspMastgComponent);
                
                ReportComponents resultsIosOwaspMastgComponent = new ReportComponents();
                resultsIosOwaspMastgComponent.Id = Guid.NewGuid();
                resultsIosOwaspMastgComponent.Name = "OWASP MASTG - Results (iOS)";
                resultsIosOwaspMastgComponent.Language = Language.English;
                resultsIosOwaspMastgComponent.ComponentType = ReportPartType.Body;
                resultsIosOwaspMastgComponent.Created = DateTime.Now.ToUniversalTime();
                resultsIosOwaspMastgComponent.Updated = DateTime.Now.ToUniversalTime();
                resultsIosOwaspMastgComponent.ContentCss = "";
                resultsIosOwaspMastgComponent.Content = @"<h2>Results</h2>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; height: 223.906px;"" border=""1""><colgroup><col style=""width: 11.9632%;""><col style=""width: 45.5419%;""><col style=""width: 4.45814%;""><col style=""width: 4.21779%;""><col style=""width: 5.06135%;""><col style=""width: 14.6472%;""><col style=""width: 14.1104%;""></colgroup>
                <tbody>
                <tr style=""height: 44.7812px;"">
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Information Gathering</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">L1</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">L2</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">R</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 44.7812px;"" rowspan=""2""><span style=""color: rgb(0, 0, 0);"">MASVS-STORAGE-1</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">The app securely stores sensitive data.</span></td>
                <td style=""height: 22.3906px; text-align: left; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: left; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: left; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage1Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage1Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Local Storage for Sensitive Data</span></td>
                <td style=""height: 22.3906px; text-align: left; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: left; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: left; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage1Status3}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage1Note3}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 134.344px;"" rowspan=""6""><span style=""color: rgb(0, 0, 0);"">MASVS-STORAGE-2</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app prevents leakage of sensitive data.</span></td>
                <td style=""height: 22.3906px; text-align: left; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: left; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: left; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Finding Sensitive Data in the Keyboard Cache</span></td>
                <td style=""height: 22.3906px; text-align: left; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: left; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: left; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Status7}}&nbsp;</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Note7}}&nbsp;</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Backups for Sensitive Data</span></td>
                <td style=""height: 22.3906px; text-align: left;"">&nbsp;</td>
                <td style=""text-align: left; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: left; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Status8}}&nbsp;&nbsp;</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Note8}}&nbsp;&nbsp;</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Checking Logs for Sensitive Data</span></td>
                <td style=""height: 22.3906px; text-align: left; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: left; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: left; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Status9}}&nbsp;&nbsp;</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Note9}}&nbsp;&nbsp;</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Determining Whether Sensitive Data Is Shared with Third Parties</span></td>
                <td style=""height: 22.3906px; text-align: left; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: left; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: left; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Status10}}&nbsp;&nbsp;</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">&nbsp;{{Storage2Note10}}&nbsp;</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Memory for Sensitive Data</span></td>
                <td style=""height: 22.3906px; text-align: left;"">&nbsp;</td>
                <td style=""text-align: left; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: left; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">&nbsp;{{Storage2Status11}}&nbsp;</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Storage2Note11}}&nbsp;&nbsp;</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; height: 297px;"" border=""1""><colgroup><col style=""width: 14.2638%;""><col style=""width: 43.0215%;""><col style=""width: 4.29448%;""><col style=""width: 4.52454%;""><col style=""width: 4.83164%;""><col style=""width: 14.7236%;""><col style=""width: 14.2638%;""></colgroup>
                <tbody>
                <tr style=""height: 44.75px;"">
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Crypthography</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">L1</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">L2</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">R</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 89.5624px; text-align: center;"" rowspan=""3""><span style=""color: rgb(0, 0, 0);"">MASVS-CRYPTO-1</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">The app employs current strong cryptography and uses it according to industry best practices.</span></td>
                <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                <td style=""background-color: rgb(206, 212, 217); height: 22.3906px; text-align: center;"">&nbsp;</td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto1Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto1Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Verifying the Configuration of Cryptographic Standard Algorithms</span></td>
                <td style=""height: 22.3906px; background-color: rgb(50, 153, 255); text-align: center;"">&nbsp;</td>
                <td style=""height: 22.3906px; background-color: rgb(11, 186, 131); text-align: center;"">&nbsp;</td>
                <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto1Status4}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto1Note4}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing Random Number Generation</span></td>
                <td style=""height: 22.3906px; background-color: rgb(50, 153, 255); text-align: center;"">&nbsp;</td>
                <td style=""height: 22.3906px; background-color: rgb(11, 186, 131); text-align: center;"">&nbsp;</td>
                <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto1Status5}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto1Note5}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 44.7812px; text-align: center;"" rowspan=""2""><span style=""color: rgb(0, 0, 0);"">MASVS-CRYPTO-2</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">The app performs key management according to industry best practices.</span></td>
                <td style=""height: 22.3906px; background-color: rgb(206, 212, 217); text-align: center;"">&nbsp;</td>
                <td style=""height: 22.3906px; background-color: rgb(206, 212, 217); text-align: center;"">&nbsp;</td>
                <td style=""height: 22.3906px; background-color: rgb(206, 212, 217); text-align: center;"">&nbsp;</td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto2Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto2Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing Key Management</span></td>
                <td style=""height: 22.3906px; background-color: rgb(50, 153, 255); text-align: center;"">&nbsp;</td>
                <td style=""height: 22.3906px; background-color: rgb(11, 186, 131); text-align: center;"">&nbsp;</td>
                <td style=""height: 22.3906px; text-align: center;"">&nbsp;</td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto2Status2}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Crypto2Note2}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; height: 161.016px;"" border=""1""><colgroup><col style=""width: 14.2638%;""><col style=""width: 43.0215%;""><col style=""width: 4.29448%;""><col style=""width: 4.52454%;""><col style=""width: 4.83129%;""><col style=""width: 14.7239%;""><col style=""width: 14.2638%;""></colgroup>
                <tbody>
                <tr style=""height: 49.0625px;"">
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Authentication &amp; Authorization</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L1</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L2</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">R</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">MASVS-AUTH-1</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app uses secure authentication and authorization protocols and follows the relevant best practices.</span></td>
                <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth1Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth1Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 67.1718px;"" rowspan=""2""><span style=""color: rgb(0, 0, 0);"">MASVS-AUTH-2</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app performs local authentication securely according to the platform best practices.</span></td>
                <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217); height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth2Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth2Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Local Authentication</span></td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""background-color: rgb(11, 186, 131); text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth2Status3}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth2Note3}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">MASVS-AUTH-3</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app secures sensitive operations with additional authentication.</span></td>
                <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth3Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Auth3Note}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; height: 250.578px;"" border=""1""><colgroup><col style=""width: 14.2638%;""><col style=""width: 43.0215%;""><col style=""width: 4.29448%;""><col style=""width: 4.52454%;""><col style=""width: 4.83129%;""><col style=""width: 14.7239%;""><col style=""width: 14.2638%;""></colgroup>
                <tbody>
                <tr style=""height: 49.0625px;"">
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Network Communications</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L1</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L2</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">R</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr style=""height: 44.7812px;"">
                <td style=""text-align: center; height: 134.344px;"" rowspan=""4""><span style=""color: rgb(0, 0, 0);"">MASVS-NETWORK-1</span></td>
                <td style=""text-align: center; height: 44.7812px;""><span style=""color: rgb(0, 0, 0);"">The app secures all network traffic according to the current best practices.</span></td>
                <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 44.7812px;"">&nbsp;</td>
                <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 44.7812px;"">&nbsp;</td>
                <td style=""text-align: center; height: 44.7812px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 44.7812px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Status}}</span></td>
                <td style=""text-align: center; height: 44.7812px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Endpoint Identity Verification</span></td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Status5}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Note5}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing the TLS Settings</span></td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Status6}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Note6}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Data Encryption on the Network</span></td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Status7}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network1Note7}}</span></td>
                </tr>
                <tr style=""height: 44.7812px;"">
                <td style=""text-align: center; height: 67.1718px;"" rowspan=""2""><span style=""color: rgb(0, 0, 0);"">MASVS-NETWORK-2</span></td>
                <td style=""text-align: center; height: 44.7812px;""><span style=""color: rgb(0, 0, 0);"">The app performs identity pinning for all remote endpoints under the developer's control.</span></td>
                <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 44.7812px;"">&nbsp;</td>
                <td style=""background-color: rgb(206, 212, 217); text-align: center; height: 44.7812px;"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217); height: 44.7812px;"">&nbsp;</td>
                <td style=""text-align: center; height: 44.7812px;""><span style=""color: rgb(0, 0, 0);"">{{Network2Status}}</span></td>
                <td style=""text-align: center; height: 44.7812px;""><span style=""color: rgb(0, 0, 0);"">{{Network2Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Custom Certificate Stores and Certificate Pinning</span></td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network2Status2}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Network2Note2}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; height: 250.578px;"" border=""1""><colgroup><col style=""width: 14.2638%;""><col style=""width: 43.0215%;""><col style=""width: 4.29448%;""><col style=""width: 4.52454%;""><col style=""width: 4.83129%;""><col style=""width: 14.7239%;""><col style=""width: 14.2638%;""></colgroup>
                <tbody>
                <tr style=""height: 49.0625px;"">
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Platform Interaction</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L1</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L2</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">R</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 67.1718px;"" rowspan=""9""><span style=""color: rgb(0, 0, 0);"">MASVS-PLATFORM-1</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app uses IPC mechanisms securely.</span></td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing UIActivity Sharing</span></td>
                <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center;"">&nbsp;</td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Status6}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Note6}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing App Permissions</span></td>
                <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center;"">&nbsp;</td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Status7}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Note7}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing Universal Links</span></td>
                <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center;"">&nbsp;</td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Status8}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Note8}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Determining Whether Sensitive Data Is Exposed via IPC Mechanisms</span></td>
                <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center;"">&nbsp;</td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Status9}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Note9}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing Custom URL Schemes</span></td>
                <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center;"">&nbsp;</td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Status10}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Note10}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing for Sensitive Functionality Exposure Through IPC</span></td>
                <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center;"">&nbsp;</td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Status11}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Note11}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing App Extensions</span></td>
                <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center;"">&nbsp;</td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Status12}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Note12}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing UIPasteboard</span></td>
                <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center;"">&nbsp;</td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Status13}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform1Note13}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;"" rowspan=""4""><span style=""color: rgb(0, 0, 0);"">MASVS-PLATFORM-2</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">The app uses WebViews securely.</span></td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing iOS WebViews</span></td>
                <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center;"">&nbsp;</td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Status5}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Note5}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Determining Whether Native Methods Are Exposed Through WebViews</span></td>
                <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center;"">&nbsp;</td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Status6}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Note6}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing WebView Protocol Handlers</span></td>
                <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center;"">&nbsp;</td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Status7}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform2Note7}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;"" rowspan=""3""><span style=""color: rgb(0, 0, 0);"">MASVS-PLATFORM-3</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">The app uses the user interface securely.</span></td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform3Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform3Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Testing Auto-Generated Screenshots for Sensitive Information</span></td>
                <td style=""text-align: center;"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center;"">&nbsp;</td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform3Status4}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform3Note4}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Checking for Sensitive Data Disclosed Through the User Interface</span></td>
                <td style=""text-align: center; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center;"">&nbsp;</td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform3Status5}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Platform3Note5}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; height: 340.14px;"" border=""1""><colgroup><col style=""width: 14.2638%;""><col style=""width: 43.0215%;""><col style=""width: 4.29448%;""><col style=""width: 4.52454%;""><col style=""width: 4.83129%;""><col style=""width: 14.7239%;""><col style=""width: 14.2638%;""></colgroup>
                <tbody>
                <tr style=""height: 49.0625px;"">
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Code Quality</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L1</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L2</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">R</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">MASVS-CODE-1</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app requires an up-to-date platform version.</span></td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code1Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code1Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 44.7812px;"" rowspan=""2""><span style=""color: rgb(0, 0, 0);"">MASVS-CODE-2</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app has a mechanism for enforcing app updates.</span></td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code2Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code2Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Enforced Updating</span></td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code2Status2}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code2Note2}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 44.7812px;"" rowspan=""2""><span style=""color: rgb(0, 0, 0);"">MASVS-CODE-3</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app only uses software components without known vulnerabilities.</span></td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code3Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code3Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Checking for Weaknesses in Third Party Libraries</span></td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code3Status2}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code3Note2}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 179.125px;"" rowspan=""4""><span style=""color: rgb(0, 0, 0);"">MASVS-CODE-4</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app only uses software components without known vulnerabilities.</span></td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(206, 212, 217);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Object Persistence</span></td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Status8}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Note8}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Make Sure That Free Security Features Are Activated</span></td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Status9}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Note9}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Memory Corruption Bugs</span></td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(50, 153, 255);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(11, 186, 131);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Status10}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Code4Note10}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; height: 384.921px;"" border=""1""><colgroup><col style=""width: 14.2638%;""><col style=""width: 43.0215%;""><col style=""width: 4.29448%;""><col style=""width: 4.52454%;""><col style=""width: 4.83129%;""><col style=""width: 14.7239%;""><col style=""width: 14.2638%;""></colgroup>
                <tbody>
                <tr style=""height: 49.0625px;"">
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Resilience against reverse</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L1</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">L2</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">R</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 49.0625px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;"" rowspan=""3""><span style=""color: rgb(0, 0, 0);"">MASVS-RESILIENCE-1</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app validates the integrity of the platform.</span></td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217); height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217); height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217); height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience1Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience1Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Emulator Detection</span></td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(255, 168, 0);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience1Status3}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience1Note3}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Jailbreak Detection</span></td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(255, 168, 0);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience1Status4}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience1Note4}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;"" rowspan=""3""><span style=""color: rgb(0, 0, 0);"">MASVS-RESILIENCE-2</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app implements anti-tampering mechanisms.</span></td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217); height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217); height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217); height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience2Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience2Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Making Sure that the App Is Properly Signed</span></td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(255, 168, 0);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience2Status4}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience2Note4}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing File Integrity Checks</span></td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(255, 168, 0);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience2Status5}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience2Note5}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;"" rowspan=""4""><span style=""color: rgb(0, 0, 0);"">MASVS-RESILIENCE-3</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app implements anti-static analysis mechanisms.</span></td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217); height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217); height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217); height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience3Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience3Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing for Debugging Symbols</span></td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(255, 168, 0);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience3Status4}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience3Note4}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Obfuscation</span></td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(255, 168, 0);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience3Status5}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience3Note5}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing for Debugging Code and Verbose Error Logging</span></td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(255, 168, 0);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience3Status6}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience3Note6}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;"" rowspan=""4""><span style=""color: rgb(0, 0, 0);"">MASVS-RESILIENCE-4</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">The app implements anti-dynamic analysis techniques.</span></td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217); height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217); height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; background-color: rgb(206, 212, 217); height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience4Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience4Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Reverse Engineering Tools Detection</span></td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(255, 168, 0);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience4Status4}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience4Note4}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing whether the App is Debuggable</span></td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(255, 168, 0);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience4Status5}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience4Note5}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">Testing Anti-Debugging Detection</span></td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px; background-color: rgb(255, 168, 0);"">&nbsp;</td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience4Status6}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Resilience4Note6}}</span></td>
                </tr>
                </tbody>
                </table>
                <p style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">L1 - Standard Security / L2 - Defense-in-Depth / R - Resilience Against Reverse Engineering and Tampering</span><br><span style=""color: rgb(53, 152, 219);""><a style=""color: rgb(53, 152, 219);"" href=""https://mas.owasp.org/"" target=""_blank"" rel=""noopener"">Based on OWASP MASTG v1.6.0 &amp; OWASP MASVS 2.0.0</a></span></p>";
                reportComponentsManager.Add(resultsIosOwaspMastgComponent);
                reportComponentsManager.Context.SaveChanges();
                #endregion
                
                #region OWASP WSTG English
                ReportComponents coverOwaspWstgComponent = new ReportComponents();
                coverOwaspWstgComponent.Id = Guid.NewGuid();
                coverOwaspWstgComponent.Name = "OWASP WSTG - Cover";
                coverOwaspWstgComponent.Language = Language.English;
                coverOwaspWstgComponent.ComponentType = ReportPartType.Cover;
                coverOwaspWstgComponent.Created = DateTime.Now.ToUniversalTime();
                coverOwaspWstgComponent.Updated = DateTime.Now.ToUniversalTime();
                coverOwaspWstgComponent.ContentCss = "";
                coverOwaspWstgComponent.Content = @"<p>&nbsp;</p>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none;"" border=""1""><colgroup><col style=""width: 51.8049%;""><col style=""width: 48.1951%;""></colgroup>
                <tbody>
                <tr>
                <td style=""border-style: none; text-align: center;""><img style=""float: left;"" src=""data:image/png;base64,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"" width=""250"" height=""87""></td>
                <td style=""border-style: none;""><img style=""float: right;"" src=""data:image/png;base64,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"" width=""270"" height=""54""></td>
                </tr>
                </tbody>
                </table>
                <h1 style=""text-align: center;"">&nbsp;</h1>
                <p>&nbsp;</p>
                <h1 style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">OWASP WSTG REPORT</span></h1>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none;"" border=""1""><colgroup><col style=""width: 50%;""><col style=""width: 50%;""></colgroup>
                <tbody>
                <tr>
                <td style=""border-style: none;"">
                <p><span style=""color: rgb(0, 0, 0);""><strong>Organization: {{OrganizationName}}</strong></span></p>
                <p><span style=""color: rgb(0, 0, 0);"">Contact Email: {{OrganizationEmail}}</span></p>
                <p><span style=""color: rgb(0, 0, 0);"">Contact Phone: {{OrganizationPhone}}</span></p>
                </td>
                <td style=""border-style: none;"">
                <p style=""text-align: right;""><span style=""color: rgb(0, 0, 0);""><strong>Client: {{ClientName}}</strong></span></p>
                <p style=""text-align: right;""><span style=""color: rgb(0, 0, 0);"">Client Email: {{ClientEmail}}</span></p>
                <p style=""text-align: right;""><span style=""color: rgb(0, 0, 0);"">Client Phone: {{ClientPhone}}</span></p>
                </td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none;"" border=""1""><colgroup><col style=""width: 50%;""><col style=""width: 50%;""></colgroup>
                <tbody>
                <tr>
                <td style=""text-align: center; border-style: none;""><span style=""color: rgb(0, 0, 0);""><strong>Url</strong>: {{TargetUrl}}</span></td>
                <td style=""text-align: center; border-style: none;""><span style=""color: rgb(0, 0, 0);""><strong>Date</strong>: {{CreatedDate}}</span></td>
                </tr>
                </tbody>
                </table>
                <div style=""break-after: page;"">&nbsp;</div>";
                reportComponentsManager.Add(coverOwaspWstgComponent);
                
                ReportComponents headerOwaspWstgComponent = new ReportComponents();
                headerOwaspWstgComponent.Id = Guid.NewGuid();
                headerOwaspWstgComponent.Name = "OWASP WSTG - Header";
                headerOwaspWstgComponent.Language = Language.English;
                headerOwaspWstgComponent.ComponentType = ReportPartType.Header;
                headerOwaspWstgComponent.Created = DateTime.Now.ToUniversalTime();
                headerOwaspWstgComponent.Updated = DateTime.Now.ToUniversalTime();
                headerOwaspWstgComponent.ContentCss = "";
                headerOwaspWstgComponent.Content = @"<table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none;"" border=""1""><colgroup><col style=""width: 33.2692%;""><col style=""width: 33.2692%;""><col style=""width: 33.2692%;""></colgroup>
                <tbody>
                <tr>
                <td style=""border-style: none;""><em>Privileged and Confidential&nbsp;</em></td>
                <td style=""border-style: none;"">&nbsp;</td>
                <td style=""border-style: none; text-align: right;""><em>OWASP WSTG Report</em></td>
                </tr>
                </tbody>
                </table>
                <hr>
                <p>&nbsp;</p>";
                reportComponentsManager.Add(headerOwaspWstgComponent);
                
                ReportComponents introOwaspWstgComponent = new ReportComponents();
                introOwaspWstgComponent.Id = Guid.NewGuid();
                introOwaspWstgComponent.Name = "OWASP WSTG - Intro";
                introOwaspWstgComponent.Language = Language.English;
                introOwaspWstgComponent.ComponentType = ReportPartType.Body;
                introOwaspWstgComponent.Created = DateTime.Now.ToUniversalTime();
                introOwaspWstgComponent.Updated = DateTime.Now.ToUniversalTime();
                introOwaspWstgComponent.ContentCss = "";
                introOwaspWstgComponent.Content = @"<h2><span style=""color: rgb(0, 0, 0);"">Introduction</span></h2>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">{{ClientName}} we are pleased to present you with the Open Web Application Security Project (OWASP) Web Security Testing Guide (WSTG) report as a result of our recent penetration testing engagement on your digital infrastructure. The OWASP WSTG report provides a comprehensive examination of your system from a security perspective.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">Over the course of our engagement, we have utilized the OWASP WSTG, an industry-standard methodology for robust web application security testing. Our team of highly skilled penetration testers has conducted a systematic assessment of your web applications to identify any existing vulnerabilities, threats or weaknesses that could potentially be exploited by malevolent parties.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">It's important to note that this report provides an overview of the identified vulnerabilities. The recommendations given in this report are designed to assist your organization in minimizing the risk of any future security breaches, in light of ever-evolving cyber threats.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">Please review this report thoroughly, and let us know if there are any areas that you would like to discuss further or need additional clarification on.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgb(0, 0, 0);"">We look forward to your feedback, and we are eager to assist you in strengthening your IT security profile. Cybersecurity is a journey, not a destination, and we are committed to staying your trusted partner in this journey.</span></p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>";
                reportComponentsManager.Add(introOwaspWstgComponent);
                
                ReportComponents resultsOwaspWstgComponent = new ReportComponents();
                resultsOwaspWstgComponent.Id = Guid.NewGuid();
                resultsOwaspWstgComponent.Name = "OWASP WSTG - Results";
                resultsOwaspWstgComponent.Language = Language.English;
                resultsOwaspWstgComponent.ComponentType = ReportPartType.Body;
                resultsOwaspWstgComponent.Created = DateTime.Now.ToUniversalTime();
                resultsOwaspWstgComponent.Updated = DateTime.Now.ToUniversalTime();
                resultsOwaspWstgComponent.ContentCss = "";
                resultsOwaspWstgComponent.Content = @"
                <h2><span style=""color: rgb(0, 0, 0);"">Results</span></h2>
                <table style=""border-collapse: collapse; width: 100%; height: 246.297px;"" border=""1""><colgroup><col style=""width: 13.4188%;""><col style=""width: 26.6118%;""><col style=""width: 29.2178%;""><col style=""width: 10.8129%;""><col style=""width: 19.9387%;""></colgroup>
                <tbody>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 22.3906px;""><strong><span style=""color: rgb(255, 255, 255);"">Information Gathering</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 22.3906px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 22.3906px;""><strong><span style=""color: rgb(255, 255, 255);"">Objectives</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 22.3906px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 22.3906px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">WSTG-INFO-01</span></td>
                <td style=""height: 22.3906px; text-align: left;"">
                <p style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">Conduct Search Engine Discovery Reconnaissance for Information Leakage</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify what sensitive design and configuration information of the application, system, or organization is exposed directly (on the organization&rsquo;s website) or indirectly (via third-party services).</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info01Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info01Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">WSTG-INFO-02</span></td>
                <td style=""text-align: center; height: 22.3906px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Fingerprint Web Server</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Determine the version and type of a running web server to enable further discovery of any known vulnerabilities.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info02Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info02Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">WSTG-INFO-03</span></td>
                <td style=""text-align: center; height: 22.3906px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Review Webserver Metafiles for Information Leakage</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify hidden or obfuscated paths and functionality through the analysis of metadata files.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Extract and map other information that could lead to better understanding of the systems at hand.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info03Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info03Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">WSTG-INFO-04</span></td>
                <td style=""text-align: center; height: 22.3906px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Enumerate Applications on Webserver</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Enumerate the applications within scope that exist on a web server.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info04Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info04Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">WSTG-INFO-05</span></td>
                <td style=""text-align: center; height: 22.3906px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Review Webpage Content for Information Leakage</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Review webpage comments and metadata to find any information leakage.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Gather JavaScript files and review the JS code to better understand the application and to find any information leakage.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Identify if source map files or other front-end debug files exist.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info05Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info05Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">WSTG-INFO-06</span></td>
                <td style=""text-align: center; height: 22.3906px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Identify Application Entry Points</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify possible entry and injection points through request and response analysis.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info06Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info06Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">WSTG-INFO-07</span></td>
                <td style=""text-align: center; height: 22.3906px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Map Execution Paths Through Application</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Map the target application and understand the principal workflows.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info07Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info07Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">WSTG-INFO-08</span></td>
                <td style=""text-align: center; height: 22.3906px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Fingerprint Web Application Framework</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Fingerprint the components being used by the web applications.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info08Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info08Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">WSTG-INFO-09</span></td>
                <td style=""text-align: center; height: 22.3906px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Fingerprint Web Application</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Fingerprint the components being used by the web applications.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info09Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info09Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">WSTG-INFO-10</span></td>
                <td style=""text-align: center; height: 22.3906px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Map Application Architecture</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Generate a map of the application at hand based on the research conducted.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info10Status}}</span></td>
                <td style=""text-align: center; height: 22.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Info10Note}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; height: 268.687px;"" border=""1""><colgroup><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""></colgroup>
                <tbody>
                <tr style=""height: 44.7812px;"">
                <td style=""background-color: rgb(0, 0, 0); text-align: center; height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Configuration and Deployment Management Testing</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Objectives</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CONF-01</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test Network Infrastructure Configuration</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Review the applications&rsquo; configurations set across the network and validate that they are not vulnerable.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Validate that used frameworks and systems are secure and not susceptible to known vulnerabilities due to unmaintained software or default settings and credentials.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf01Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf01Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CONF-02</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test Application Platform Configuration</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Ensure that defaults and known files have been removed.</span></li>
                <li><span style=""font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif; color: rgb(0, 0, 0);"">Validate that no debugging code or extensions are left in the production environments.</span></li>
                <li><span style=""font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif; color: rgb(0, 0, 0);"">Review the logging mechanisms set in place for the application.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf02Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf02Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CONF-03</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test File Extensions Handling for Sensitive Information</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Dirbust sensitive file extensions, or extensions that might contain raw data (e.g. scripts, raw data, credentials, etc.).</span></li>
                <li><span style=""font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif; color: rgb(0, 0, 0);"">Validate that no system framework bypasses exist on the rules set.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf03Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf03Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CONF-04</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Review Old Backup and Unreferenced Files for Sensitive Information</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Find and analyse unreferenced files that might contain sensitive information.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf04Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf04Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CONF-05</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Enumerate Infrastructure and Application Admin Interfaces</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify hidden administrator interfaces and functionality.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf05Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf05Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CONF-06</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test HTTP Methods</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Enumerate supported HTTP methods.</span></li>
                <li><span style=""font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif; color: rgb(0, 0, 0);"">Test for access control bypass.</span></li>
                <li><span style=""font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif; color: rgb(0, 0, 0);"">Test XST vulnerabilities.</span></li>
                <li><span style=""font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif; color: rgb(0, 0, 0);"">Test HTTP method overriding techniques.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf06Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf06Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CONF-07</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test HTTP Strict Transport Security</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Review the HSTS header and its validity.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf07Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf07Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CONF-08</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test RIA Cross Domain Policy</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Review and validate the policy files.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf08Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf08Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CONF-09</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test File Permission</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Review and identify any rogue file permissions.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf09Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf09Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CONF-10</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test for Subdomain Takeover</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Enumerate all possible domains (previous and current).</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Identify forgotten or misconfigured domains.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf10Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf10Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CONF-11</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test Cloud Storage</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Assess that the access control configuration for the storage services is properly in place.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf11Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Conf11Note}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgb(0, 0, 0); text-align: center; height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Identity Management Testing</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Objectives</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7812px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-IDNT-01</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test Role Definitions</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify and document roles used by the application.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Attempt to switch, change, or access another role.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Review the granularity of the roles and the needs behind the permissions given.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Idnt01Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Idnt01Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-IDNT-02</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test User Registration Process</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Verify that the identity requirements for user registration are aligned with business and security requirements.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Validate the registration process.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Idnt02Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Idnt02Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-IDNT-03</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test Account Provisioning Process</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Verify which accounts may provision other accounts and of what type.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Idnt03Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Idnt03Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-IDNT-04</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Account Enumeration and Guessable User Account</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Review processes that pertain to user identification (*e.g.* registration, login, etc.).</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Enumerate users where possible through response analysis.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Idnt04Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Idnt04Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-IDNT-05</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Weak or unenforced username policy</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Determine whether a consistent account name structure renders the application vulnerable to account enumeration.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Determine whether the application's error messages permit account enumeration.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Idnt05Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Idnt05Note}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; height: 246.281px;"" border=""1""><colgroup><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""></colgroup>
                <tbody>
                <tr style=""height: 44.7656px;"">
                <td style=""background-color: rgb(0, 0, 0); text-align: center; height: 44.7656px;""><strong><span style=""color: rgb(255, 255, 255);"">Authentication Testing</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7656px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7656px;""><strong><span style=""color: rgb(255, 255, 255);"">Objectives</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7656px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7656px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-ATHN-01</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Credentials Transported over an Encrypted Channel</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Assess whether any use case of the web site or application causes the server or the client to exchange credentials without encryption.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn01Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn01Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-ATHN-02</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Default Credentials</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Enumerate the applications for default credentials and validate if they still exist.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Review and assess new user accounts and if they are created with any defaults or identifiable patterns.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn02Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn02Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-ATHN-03</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Weak Lock Out Mechanism</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Evaluate the account lockout mechanism's ability to mitigate brute force password guessing.- Evaluate the unlock mechanism's resistance to unauthorized account unlocking.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn03Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn03Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-ATHN-04</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Bypassing Authentication Schema</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Ensure that authentication is applied across all services that require it.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn04Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn04Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-ATHN-05</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Vulnerable Remember Password</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Validate that the generated session is managed securely and do not put the user's credentials in danger.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn05Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn05Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-ATHN-06</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Browser Cache Weaknesses</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Review if the application stores sensitive information on the client side.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Review if access can occur without authorization.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn06Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn06Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-ATHN-07</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Weak Password Policy</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Determine the resistance of the application against brute force password guessing using available password dictionaries by evaluating the length, complexity, reuse, and aging requirements of passwords.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn07Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn07Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-ATHN-08</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Weak Security Question Answer</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Determine the complexity and how straight-forward the questions are.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess possible user answers and brute force capabilities.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn08Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn08Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-ATHN-09</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Weak Password Change or Reset Functionalities</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Determine the resistance of the application to subversion of the account change process allowing someone to change the password of an account.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Determine the resistance of the passwords reset functionality against guessing or bypassing.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn09Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn09Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-ATHN-10</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Weaker Authentication in Alternative Channel</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify alternative authentication channels.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the security measures used and if any bypasses exists on the alternative channels.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn10Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athn10Note}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgb(0, 0, 0); text-align: center; height: 44.7656px;""><strong><span style=""color: rgb(255, 255, 255);"">Authorization Testing&nbsp;</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7656px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7656px;""><strong><span style=""color: rgb(255, 255, 255);"">Objectives</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7656px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.7656px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-ATHZ-01</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing Directory Traversal File Include</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify injection points that pertain to path traversal.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess bypassing techniques and identify the extent of path traversal.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athz01Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athz01Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-ATHZ-02</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Bypassing Authorization Schema</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Assess if horizontal or vertical access is possible.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athz02Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athz02Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-ATHZ-03</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Privilege Escalation</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify injection points related to privilege manipulation.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Fuzz or otherwise attempt to bypass security measures.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athz03Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athz03Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-ATHZ-04</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Insecure Direct Object References</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify points where object references may occur.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the access control measures and if they're vulnerable to IDOR.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athz04Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Athz04Note}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; height: 1001.2px;"" border=""1""><colgroup><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""></colgroup>
                <tbody>
                <tr style=""height: 44.75px;"">
                <td style=""background-color: rgb(0, 0, 0); text-align: center; height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Session Management Testing</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Objectives</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr style=""height: 201.516px;"">
                <td style=""text-align: center; height: 201.516px;""><span style=""color: rgb(0, 0, 0);"">WSTG-SESS-01</span></td>
                <td style=""text-align: center; height: 201.516px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Session Management Schema</span></p>
                </td>
                <td style=""height: 201.516px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Gather session tokens, for the same user and for different users where possible.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Analyze and ensure that enough randomness exists to stop session forging attacks.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Modify cookies that are not signed and contain information that can be manipulated.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 201.516px;""><span style=""color: rgb(0, 0, 0);"">{{Sess01Status}}</span></td>
                <td style=""text-align: center; height: 201.516px;""><span style=""color: rgb(0, 0, 0);"">{{Sess01Note}}</span></td>
                </tr>
                <tr style=""height: 76.7812px;"">
                <td style=""text-align: center; height: 76.7812px;""><span style=""color: rgb(0, 0, 0);"">WSTG-SESS-02</span></td>
                <td style=""text-align: center; height: 76.7812px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Cookies Attributes</span></p>
                </td>
                <td style=""height: 76.7812px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Ensure that the proper security configuration is set for cookies.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 76.7812px;""><span style=""color: rgb(0, 0, 0);"">{{Sess02Status}}</span></td>
                <td style=""text-align: center; height: 76.7812px;""><span style=""color: rgb(0, 0, 0);"">{{Sess02Note}}</span></td>
                </tr>
                <tr style=""height: 89.5625px;"">
                <td style=""text-align: center; height: 89.5625px;""><span style=""color: rgb(0, 0, 0);"">WSTG-SESS-03</span></td>
                <td style=""text-align: center; height: 89.5625px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Session Fixation</span></p>
                </td>
                <td style=""height: 89.5625px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Analyze the authentication mechanism and its flow.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Force cookies and assess the impact.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 89.5625px;""><span style=""color: rgb(0, 0, 0);"">{{Sess03Status}}</span></td>
                <td style=""text-align: center; height: 89.5625px;""><span style=""color: rgb(0, 0, 0);"">{{Sess03Note}}</span></td>
                </tr>
                <tr style=""height: 134.344px;"">
                <td style=""text-align: center; height: 134.344px;""><span style=""color: rgb(0, 0, 0);"">WSTG-SESS-04</span></td>
                <td style=""text-align: center; height: 134.344px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Exposed Session Variables</span></p>
                </td>
                <td style=""height: 134.344px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Ensure that proper encryption is implemented.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Review the caching configuration.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the channel and methods' security.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 134.344px;""><span style=""color: rgb(0, 0, 0);"">{{Sess04Status}}</span></td>
                <td style=""text-align: center; height: 134.344px;""><span style=""color: rgb(0, 0, 0);"">{{Sess04Note}}</span></td>
                </tr>
                <tr style=""height: 89.5625px;"">
                <td style=""text-align: center; height: 89.5625px;""><span style=""color: rgb(0, 0, 0);"">WSTG-SESS-05</span></td>
                <td style=""text-align: center; height: 89.5625px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Cross Site Request Forgery</span></p>
                </td>
                <td style=""height: 89.5625px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Determine whether it is possible to initiate requests on a user's behalf that are not initiated by the user.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 89.5625px;""><span style=""color: rgb(0, 0, 0);"">{{Sess05Status}}</span></td>
                <td style=""text-align: center; height: 89.5625px;""><span style=""color: rgb(0, 0, 0);"">{{Sess05Note}}</span></td>
                </tr>
                <tr style=""height: 89.5625px;"">
                <td style=""text-align: center; height: 89.5625px;""><span style=""color: rgb(0, 0, 0);"">WSTG-SESS-06</span></td>
                <td style=""text-align: center; height: 89.5625px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Logout Functionality</span></p>
                </td>
                <td style=""height: 89.5625px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the logout UI.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Analyze the session timeout and if the session is properly killed after logout.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 89.5625px;""><span style=""color: rgb(0, 0, 0);"">{{Sess06Status}}</span></td>
                <td style=""text-align: center; height: 89.5625px;""><span style=""color: rgb(0, 0, 0);"">{{Sess06Note}}</span></td>
                </tr>
                <tr style=""height: 54.3906px;"">
                <td style=""text-align: center; height: 54.3906px;""><span style=""color: rgb(0, 0, 0);"">WSTG-SESS-07</span></td>
                <td style=""text-align: center; height: 54.3906px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing Session Timeout</span></p>
                </td>
                <td style=""height: 54.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Validate that a hard session timeout exists.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 54.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Sess07Status}}</span></td>
                <td style=""text-align: center; height: 54.3906px;""><span style=""color: rgb(0, 0, 0);"">{{Sess07Note}}</span></td>
                </tr>
                <tr style=""height: 99.1719px;"">
                <td style=""text-align: center; height: 99.1719px;""><span style=""color: rgb(0, 0, 0);"">WSTG-SESS-08</span></td>
                <td style=""text-align: center; height: 99.1719px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Session Puzzling</span></p>
                </td>
                <td style=""height: 99.1719px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify all session variables.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Break the logical flow of session generation.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 99.1719px;""><span style=""color: rgb(0, 0, 0);"">{{Sess08Status}}</span></td>
                <td style=""text-align: center; height: 99.1719px;""><span style=""color: rgb(0, 0, 0);"">{{Sess08Note}}</span></td>
                </tr>
                <tr style=""height: 121.562px;"">
                <td style=""text-align: center; height: 121.562px;""><span style=""color: rgb(0, 0, 0);"">WSTG-SESS-09</span></td>
                <td style=""text-align: center; height: 121.562px;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Session Hijacking</span></p>
                </td>
                <td style=""height: 121.562px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify vulnerable session cookies.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Hijack vulnerable cookies and assess the risk level.</span></li>
                </ul>
                </td>
                <td style=""text-align: center; height: 121.562px;""><span style=""color: rgb(0, 0, 0);"">{{Sess09Status}}</span></td>
                <td style=""text-align: center; height: 121.562px;""><span style=""color: rgb(0, 0, 0);"">{{Sess09Note}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; height: 2981.37px;"" border=""1""><colgroup><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 20.0153%;""><col style=""width: 19.9387%;""></colgroup>
                <tbody>
                <tr style=""height: 44.75px;"">
                <td style=""background-color: rgb(0, 0, 0); text-align: center; height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Data Validation Testing</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Objectives</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr style=""height: 166.344px;"">
                <td style=""height: 166.344px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-01</span></td>
                <td style=""height: 166.344px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Reflected Cross Site Scripting</span></p>
                </td>
                <td style=""height: 166.344px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify variables that are reflected in responses.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the input they accept and the encoding that gets applied on return (if any).</span></li>
                </ul>
                </td>
                <td style=""height: 166.344px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv01Status}}</span></td>
                <td style=""height: 166.344px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv01Note}}</span></td>
                </tr>
                <tr style=""height: 166.344px;"">
                <td style=""height: 166.344px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-02</span></td>
                <td style=""height: 166.344px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Stored Cross Site Scripting</span></p>
                </td>
                <td style=""height: 166.344px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify stored input that is reflected on the client-side.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the input they accept and the encoding that gets applied on return (if any).</span></li>
                </ul>
                </td>
                <td style=""height: 166.344px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv02Status}}</span></td>
                <td style=""height: 166.344px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv02Note}}</span></td>
                </tr>
                <tr style=""height: 188.734px;"">
                <td style=""height: 188.734px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-03</span></td>
                <td style=""height: 188.734px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for HTTP Verb Tampering</span></p>
                </td>
                <td style=""height: 188.734px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Enumerate supported HTTP methods.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Test for access control bypass.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Test XST vulnerabilities.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Test HTTP method overriding techniques.</span></li>
                </ul>
                </td>
                <td style=""height: 188.734px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv03Status}}</span></td>
                <td style=""height: 188.734px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv03Note}}</span></td>
                </tr>
                <tr style=""height: 143.953px;"">
                <td style=""height: 143.953px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-04</span></td>
                <td style=""height: 143.953px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for HTTP Parameter Pollution</span></p>
                </td>
                <td style=""height: 143.953px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify the backend and the parsing method used.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess injection points and try bypassing input filters using HPP.</span></li>
                </ul>
                </td>
                <td style=""height: 143.953px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv04Status}}</span></td>
                <td style=""height: 143.953px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv04Note}}</span></td>
                </tr>
                <tr style=""height: 166.344px;"">
                <td style=""height: 166.344px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-05</span></td>
                <td style=""height: 166.344px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for SQL Injection</span></p>
                </td>
                <td style=""height: 166.344px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify SQL injection points.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the severity of the injection and the level of access that can be achieved through it.</span></li>
                </ul>
                </td>
                <td style=""height: 166.344px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv05Status}}</span></td>
                <td style=""height: 166.344px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv05Note}}</span></td>
                </tr>
                <tr style=""height: 121.562px;"">
                <td style=""height: 121.562px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-06</span></td>
                <td style=""height: 121.562px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for LDAP Injection</span></p>
                </td>
                <td style=""height: 121.562px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify LDAP injection points.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the severity of the injection.</span></li>
                </ul>
                </td>
                <td style=""height: 121.562px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv06Status}}</span></td>
                <td style=""height: 121.562px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv06Note}}</span></td>
                </tr>
                <tr style=""height: 143.953px;"">
                <td style=""height: 143.953px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-07</span></td>
                <td style=""height: 143.953px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for XML Injection</span></p>
                </td>
                <td style=""height: 143.953px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify XML injection points.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the types of exploits that can be attained and their severities.</span></li>
                </ul>
                </td>
                <td style=""height: 143.953px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv07Status}}</span></td>
                <td style=""height: 143.953px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv07Note}}</span></td>
                </tr>
                <tr style=""height: 99.1719px;"">
                <td style=""height: 99.1719px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-08</span></td>
                <td style=""height: 99.1719px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for SSI Injection</span></p>
                </td>
                <td style=""height: 99.1719px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify SSI injection points.</span></li>
                <li><span style=""font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif; color: rgb(0, 0, 0);"">Assess the severity of the injection.</span></li>
                </ul>
                </td>
                <td style=""height: 99.1719px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv08Status}}</span></td>
                <td style=""height: 99.1719px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv08Note}}</span></td>
                </tr>
                <tr style=""height: 76.7812px;"">
                <td style=""height: 76.7812px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-09</span></td>
                <td style=""height: 76.7812px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for XPath Injection</span></p>
                </td>
                <td style=""height: 76.7812px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify XPATH injection points.</span></li>
                </ul>
                </td>
                <td style=""height: 76.7812px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv09Status}}</span></td>
                <td style=""height: 76.7812px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv09Note}}</span></td>
                </tr>
                <tr style=""height: 188.734px;"">
                <td style=""height: 188.734px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-10</span></td>
                <td style=""height: 188.734px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for IMAP SMTP Injection</span></p>
                </td>
                <td style=""height: 188.734px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify IMAP/SMTP injection points.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Understand the data flow and deployment structure of the system.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the injection impacts.</span></li>
                </ul>
                </td>
                <td style=""height: 188.734px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv10Status}}</span></td>
                <td style=""height: 188.734px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv10Note}}</span></td>
                </tr>
                <tr style=""height: 143.953px;"">
                <td style=""height: 143.953px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-11</span></td>
                <td style=""height: 143.953px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Code Injection</span></p>
                </td>
                <td style=""height: 143.953px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify injection points where you can inject code into the application.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the injection severity.</span></li>
                </ul>
                </td>
                <td style=""height: 143.953px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv11Status}}</span></td>
                <td style=""height: 143.953px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv11Note}}</span></td>
                </tr>
                <tr style=""height: 76.7812px;"">
                <td style=""height: 76.7812px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-12</span></td>
                <td style=""height: 76.7812px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Command Injection</span></p>
                </td>
                <td style=""height: 76.7812px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify and assess the command injection points.</span></li>
                </ul>
                </td>
                <td style=""height: 76.7812px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv12Status}}</span></td>
                <td style=""height: 76.7812px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv12Note}}</span></td>
                </tr>
                <tr style=""height: 166.344px;"">
                <td style=""height: 166.344px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-13</span></td>
                <td style=""height: 166.344px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Format String Injection</span></p>
                </td>
                <td style=""height: 166.344px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Assess whether injecting format string conversion specifiers into user-controlled fields causes undesired behaviour from the application.</span></li>
                </ul>
                </td>
                <td style=""height: 166.344px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv13Status}}</span></td>
                <td style=""height: 166.344px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv13Note}}</span></td>
                </tr>
                <tr style=""height: 188.734px;"">
                <td style=""height: 188.734px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-14</span></td>
                <td style=""height: 188.734px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Incubated Vulnerability</span></p>
                </td>
                <td style=""height: 188.734px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify injections that are stored and require a recall step to the stored injection.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Understand how a recall step could occur.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Set listeners or activate the recall step if possible.</span></li>
                </ul>
                </td>
                <td style=""height: 188.734px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv14Status}}</span></td>
                <td style=""height: 188.734px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv14Note}}</span></td>
                </tr>
                <tr style=""height: 233.516px;"">
                <td style=""height: 233.516px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-15</span></td>
                <td style=""height: 233.516px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for HTTP Splitting Smuggling</span></p>
                </td>
                <td style=""height: 233.516px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Assess if the application is vulnerable to splitting, identifying what possible attacks are achievable.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess if the chain of communication is vulnerable to smuggling, identifying what possible attacks are achievable.</span></li>
                </ul>
                </td>
                <td style=""height: 233.516px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv15Status}}</span></td>
                <td style=""height: 233.516px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv15Note}}</span></td>
                </tr>
                <tr style=""height: 211.125px;"">
                <td style=""height: 211.125px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-16</span></td>
                <td style=""height: 211.125px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for HTTP Incoming Requests</span></p>
                </td>
                <td style=""height: 211.125px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Monitor all incoming and outgoing HTTP requests to the Web Server to inspect any suspicious requests.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Monitor HTTP traffic without changes of end user Browser proxy or client-side application.</span></li>
                </ul>
                </td>
                <td style=""height: 211.125px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv16Status}}</span></td>
                <td style=""height: 211.125px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv16Note}}</span></td>
                </tr>
                <tr style=""height: 143.953px;"">
                <td style=""height: 143.953px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-17</span></td>
                <td style=""height: 143.953px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Host Header Injection</span></p>
                </td>
                <td style=""height: 143.953px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Assess if the Host header is being parsed dynamically in the application.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Bypass security controls that rely on the header.</span></li>
                </ul>
                </td>
                <td style=""height: 143.953px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv17Status}}</span></td>
                <td style=""height: 143.953px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv17Note}}</span></td>
                </tr>
                <tr style=""height: 143.953px;"">
                <td style=""height: 143.953px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-18</span></td>
                <td style=""height: 143.953px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Server-side Template Injection</span></p>
                </td>
                <td style=""height: 143.953px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Detect template injection vulnerability points.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Identify the templating engine.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Build the exploit.</span></li>
                </ul>
                </td>
                <td style=""height: 143.953px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv18Status}}</span></td>
                <td style=""height: 143.953px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv18Note}}</span></td>
                </tr>
                <tr style=""height: 166.344px;"">
                <td style=""height: 166.344px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-INPV-19</span></td>
                <td style=""height: 166.344px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Server-Side Request Forgery</span></p>
                </td>
                <td style=""height: 166.344px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify SSRF injection points.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Test if the injection points are exploitable.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the severity of the vulnerability.</span></li>
                </ul>
                </td>
                <td style=""height: 166.344px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv19Status}}</span></td>
                <td style=""height: 166.344px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Inpv19Note}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 19.9889%;""><col style=""width: 19.9889%;""><col style=""width: 19.9889%;""><col style=""width: 19.9889%;""><col style=""width: 19.9889%;""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgb(0, 0, 0); text-align: center; height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Error Handling</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Objectives</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-ERRH-01</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Improper Error Handling</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify existing error output.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Analyze the different output returned.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Errh01Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Errh01Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-ERRH-02</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Stack Traces</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify existing error output.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Analyze the different output returned.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Errh02Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Errh02Note}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 19.9889%;""><col style=""width: 19.9889%;""><col style=""width: 19.9889%;""><col style=""width: 19.9889%;""><col style=""width: 19.9889%;""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgb(0, 0, 0); text-align: center; height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Cryptography</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Objectives</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CRYP-01</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Weak Transport Layer Security</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Validate the service configuration.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Review the digital certificate's cryptographic strength and validity.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Ensure that the TLS security is not bypassable and is properly implemented across the application.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Cryp01Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Cryp01Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CRYP-02</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Padding Oracle</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify encrypted messages that rely on padding.- Attempt to break the padding of the encrypted messages and analyze the returned error messages for further analysis.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Cryp02Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Cryp02Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CRYP-03</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Sensitive Information Sent via Unencrypted Channels</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify sensitive information transmitted through the various channels.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the privacy and security of the channels used.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Cryp03Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Cryp03Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CRYP-04</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Weak Encryption</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Provide a guideline for the identification weak encryption or hashing uses and implementations.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Cryp04Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Cryp04Note}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 19.9889%;""><col style=""width: 19.9889%;""><col style=""width: 19.9889%;""><col style=""width: 19.9889%;""><col style=""width: 19.9889%;""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgb(0, 0, 0); text-align: center; height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Business logic Testing</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Objectives</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-BUSL-01</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test Business Logic Data Validation</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify data injection points.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Validate that all checks are occurring on the back end and can't be bypassed.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Attempt to break the format of the expected data and analyze how the application is handling it.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl01Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl01Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-BUSL-02</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test Ability to Forge Requests</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Review the project documentation looking for guessable, predictable, or hidden functionality of fields.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Insert logically valid data in order to bypass normal business logic workflow.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl02Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl02Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-BUSL-03</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test Integrity Checks</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Review the project documentation for components of the system that move, store, or handle data.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Determine what type of data is logically acceptable by the component and what types the system should guard against.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Determine who should be allowed to modify or read that data in each component.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Attempt to insert, update, or delete data values used by each component that should not be allowed per the business logic workflow.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl03Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl03Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-BUSL-04</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test for Process Timing</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Review the project documentation for system functionality that may be impacted by time.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Develop and execute misuse cases.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl04Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl04Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-BUSL-05</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test Number of Times a Function Can be Used Limits</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify functions that must set limits to the times they can be called.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess if there is a logical limit set on the functions and if it is properly validated.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl05Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl05Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-BUSL-06</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for the Circumvention of Work Flows</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Review the project documentation for methods to skip or go through steps in the application process in a different order from the intended business logic flow.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Develop a misuse case and try to circumvent every logic flow identified.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl06Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl06Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-BUSL-07</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test Defenses Against Application Mis-use</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Generate notes from all tests conducted against the system.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Review which tests had a different functionality based on aggressive input.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Understand the defenses in place and verify if they are enough to protect the system against bypassing techniques.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl07Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl07Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-BUSL-08</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test Upload of Unexpected File Types</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Review the project documentation for file types that are rejected by the system.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Verify that the unwelcomed file types are rejected and handled safely.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Verify that file batch uploads are secure and do not allow any bypass against the set security measures.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl08Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl08Note}}</span></td>
                </tr>
                <tr>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-BUSL-09</span></td>
                <td style=""text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test Upload of Malicious Files</span></p>
                </td>
                <td style=""text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify the file upload functionality.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Review the project documentation to identify what file types are considered acceptable, and what types would be considered dangerous or malicious.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Determine how the uploaded files are processed.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Obtain or create a set of malicious files for testing.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Try to upload the malicious files to the application and determine whether it is accepted and processed.</span></li>
                </ul>
                </td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl09Status}}</span></td>
                <td style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Busl09Note}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; height: 470.172px;"" border=""1""><colgroup><col style=""width: 19.9889%;""><col style=""width: 19.9889%;""><col style=""width: 19.9889%;""><col style=""width: 19.9889%;""><col style=""width: 19.9889%;""></colgroup>
                <tbody>
                <tr style=""height: 44.75px;"">
                <td style=""background-color: rgb(0, 0, 0); text-align: center; height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Client Side Testing</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Objectives</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr style=""height: 67.1719px;"">
                <td style=""height: 67.1719px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CLNT-01</span></td>
                <td style=""height: 67.1719px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for DOM-Based Cross Site Scripting</span></p>
                </td>
                <td style=""height: 67.1719px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify DOM sinks.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Build payloads that pertain to every sink type.</span></li>
                </ul>
                </td>
                <td style=""height: 67.1719px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt01Status}}</span></td>
                <td style=""height: 67.1719px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt01Note}}</span></td>
                </tr>
                <tr style=""height: 44.7812px;"">
                <td style=""height: 44.7812px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CLNT-02</span></td>
                <td style=""height: 44.7812px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for JavaScript Execution</span></p>
                </td>
                <td style=""height: 44.7812px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify sinks and possible JavaScript injection points.</span></li>
                </ul>
                </td>
                <td style=""height: 44.7812px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt02Status}}</span></td>
                <td style=""height: 44.7812px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt02Note}}</span></td>
                </tr>
                <tr style=""height: 67.1719px;"">
                <td style=""height: 67.1719px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CLNT-03</span></td>
                <td style=""height: 67.1719px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for HTML Injection</span></p>
                </td>
                <td style=""height: 67.1719px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify HTML injection points and assess the severity of the injected content.</span></li>
                </ul>
                </td>
                <td style=""height: 67.1719px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt03Status}}</span></td>
                <td style=""height: 67.1719px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt03Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CLNT-04</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Client Side URL Redirect</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify injection points that handle URLs or paths.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the locations that the system could redirect to.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt04Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt04Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CLNT-05</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for CSS Injection</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify CSS injection points.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the impact of the injection.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt05Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt05Note}}</span></td>
                </tr>
                <tr style=""height: 44.7812px;"">
                <td style=""height: 44.7812px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CLNT-06</span></td>
                <td style=""height: 44.7812px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Client Side Resource Manipulation</span></p>
                </td>
                <td style=""height: 44.7812px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify sinks with weak input validation.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the impact of the resource manipulation.</span></li>
                </ul>
                </td>
                <td style=""height: 44.7812px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt06Status}}</span></td>
                <td style=""height: 44.7812px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt06Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CLNT-07</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test Cross Origin Resource Sharing</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify endpoints that implement CORS.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Ensure that the CORS configuration is secure or harmless.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt07Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt07Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CLNT-08</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Cross Site Flashing</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Decompile and analyze the application's code.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess sinks inputs and unsafe method usages.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt08Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt08Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CLNT-09</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Clickjacking</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Understand security measures in place.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess how strict the security measures are and if they are bypassable.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt09Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt09Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CLNT-10</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing WebSockets</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Identify the usage of WebSockets.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess its implementation by using the same tests on normal HTTP channels.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt10Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt10Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CLNT-11</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Test Web Messaging</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the security of the message's origin.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Validate that it's using safe methods and validating its input.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt11Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt11Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CLNT-12</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing Browser Storage</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Determine whether the website is storing sensitive data in client-side storage.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">The code handling of the storage objects should be examined for possibilities of injection attacks, such as utilizing unvalidated input or vulnerable libraries.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt12Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt12Note}}</span></td>
                </tr>
                <tr style=""height: 22.3906px;"">
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">WSTG-CLNT-13</span></td>
                <td style=""height: 22.3906px; text-align: center;"">
                <p><span style=""color: rgb(0, 0, 0);"">Testing for Cross Site Script Inclusion</span></p>
                </td>
                <td style=""height: 22.3906px; text-align: left;"">
                <ul>
                <li><span style=""color: rgb(0, 0, 0);"">Locate sensitive data across the system.</span></li>
                <li><span style=""color: rgb(0, 0, 0);"">Assess the leakage of sensitive data through various techniques.</span></li>
                </ul>
                </td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt13Status}}</span></td>
                <td style=""height: 22.3906px; text-align: center;""><span style=""color: rgb(0, 0, 0);"">{{Clnt13Note}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%;"" border=""1""><colgroup><col style=""width: 19.9873%;""><col style=""width: 19.9873%;""><col style=""width: 19.9873%;""><col style=""width: 19.9873%;""><col style=""width: 19.9873%;""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgb(0, 0, 0); text-align: center; height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">API Testing</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Test Name</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Objectives</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Status</span></strong></td>
                <td style=""text-align: center; background-color: rgb(0, 0, 0); height: 44.75px;""><strong><span style=""color: rgb(255, 255, 255);"">Notes</span></strong></td>
                </tr>
                <tr>
                <td style=""text-align: center;"">WSTG-APIT-01</td>
                <td style=""text-align: center;"">
                <p>Testing GraphQL</p>
                </td>
                <td style=""text-align: center;"">
                <ul>
                <li style=""text-align: left;"">Assess that a secure and production-ready configuration is deployed.</li>
                <li style=""text-align: left;"">Validate all input fields against generic attacks.</li>
                <li style=""text-align: left;"">Ensure that proper access controls are applied.</li>
                </ul>
                </td>
                <td style=""text-align: center;"">{{Apit01Status}}</td>
                <td style=""text-align: center;"">{{Apit01Note}}</td>
                </tr>
                </tbody>
                </table>
                <h6 style=""text-align: center;""><span style=""color: rgb(0, 0, 0);"">OWASP Testing Checklist The following is the list of items to test during the assessment: (The Status column is set for the values ""Pass"", ""Issues"" and ""N/A"")</span></h6>";
                reportComponentsManager.Add(resultsOwaspWstgComponent);
                reportComponentsManager.Context.SaveChanges();
                #endregion

                #region Create Templates

                var templateWstgAndroid = new ReportTemplate();
                templateWstgAndroid.Id = Guid.NewGuid();
                templateWstgAndroid.Name = "OWASP MASTG - Checklist Report (iOS)";
                templateWstgAndroid.Description = "OWASP MASTG - Checklist Report (iOS)";
                templateWstgAndroid.Language = Language.English;
                templateWstgAndroid.CreatedDate = DateTime.Now.ToUniversalTime();
                templateWstgAndroid.ReportType = ReportType.MASTG;
                templateWstgAndroid.UserId = admin.Id;
                reportTemplateManager.Add(templateWstgAndroid);
                reportTemplateManager.Context.SaveChanges();

                var templateWstgAndroidPart1 = new ReportParts();
                templateWstgAndroidPart1.Id = Guid.NewGuid();
                templateWstgAndroidPart1.TemplateId = templateWstgAndroid.Id;
                templateWstgAndroidPart1.PartType = ReportPartType.Cover;
                templateWstgAndroidPart1.ComponentId = coverOwaspWstgComponent.Id;
                templateWstgAndroidPart1.Order = 1;
                reportsPartsManager.Add(templateWstgAndroidPart1);
                
                var templateWstgAndroidPart2 = new ReportParts();
                templateWstgAndroidPart2.Id = Guid.NewGuid();
                templateWstgAndroidPart2.TemplateId = templateWstgAndroid.Id;
                templateWstgAndroidPart2.PartType = ReportPartType.Header;
                templateWstgAndroidPart2.ComponentId = headerOwaspMastgComponent.Id;
                templateWstgAndroidPart2.Order = 1;
                reportsPartsManager.Add(templateWstgAndroidPart2);
                
                var templateWstgAndroidPart3 = new ReportParts();
                templateWstgAndroidPart3.Id = Guid.NewGuid();
                templateWstgAndroidPart3.TemplateId = templateWstgAndroid.Id;
                templateWstgAndroidPart3.PartType = ReportPartType.Body;
                templateWstgAndroidPart3.ComponentId = introOwaspMastgComponent.Id;
                templateWstgAndroidPart3.Order = 1;
                reportsPartsManager.Add(templateWstgAndroidPart3);
                
                var templateWstgAndroidPart4 = new ReportParts();
                templateWstgAndroidPart4.Id = Guid.NewGuid();
                templateWstgAndroidPart4.TemplateId = templateWstgAndroid.Id;
                templateWstgAndroidPart4.PartType = ReportPartType.Body;
                templateWstgAndroidPart4.ComponentId = resultsAndroidOwaspMastgComponent.Id;
                templateWstgAndroidPart4.Order = 2;
                reportsPartsManager.Add(templateWstgAndroidPart4);
                
                var templateWstgAndroidPart5 = new ReportParts();
                templateWstgAndroidPart5.Id = Guid.NewGuid();
                templateWstgAndroidPart5.TemplateId = templateWstgAndroid.Id;
                templateWstgAndroidPart5.PartType = ReportPartType.Footer;
                templateWstgAndroidPart5.ComponentId = footerGeneralComponent.Id;
                templateWstgAndroidPart5.Order = 1;
                reportsPartsManager.Add(templateWstgAndroidPart5);
                reportsPartsManager.Context.SaveChanges();
                
                
                var templateWstgIos = new ReportTemplate();
                templateWstgIos.Id = Guid.NewGuid();
                templateWstgIos.Name = "OWASP MASTG - Checklist Report (Android)";
                templateWstgIos.Description = "OWASP MASTG - Checklist Report (Android)";
                templateWstgIos.Language = Language.English;
                templateWstgIos.CreatedDate = DateTime.Now.ToUniversalTime();
                templateWstgIos.ReportType = ReportType.MASTG;
                templateWstgIos.UserId = admin.Id;
                reportTemplateManager.Add(templateWstgIos);
                reportTemplateManager.Context.SaveChanges();
                
                var templateWstgIosPart1 = new ReportParts();
                templateWstgIosPart1.Id = Guid.NewGuid();
                templateWstgIosPart1.TemplateId = templateWstgIos.Id;
                templateWstgIosPart1.PartType = ReportPartType.Cover;
                templateWstgIosPart1.ComponentId = coverOwaspMastgComponent.Id;
                templateWstgIosPart1.Order = 1;
                reportsPartsManager.Add(templateWstgIosPart1);
                
                var templateWstgIosPart2 = new ReportParts();
                templateWstgIosPart2.Id = Guid.NewGuid();
                templateWstgIosPart2.TemplateId = templateWstgIos.Id;
                templateWstgIosPart2.PartType = ReportPartType.Header;
                templateWstgIosPart2.ComponentId = headerOwaspMastgComponent.Id;
                templateWstgIosPart2.Order = 1;
                reportsPartsManager.Add(templateWstgIosPart2);

                var templateWstgIosPart3 = new ReportParts();
                templateWstgIosPart3.Id = Guid.NewGuid();
                templateWstgIosPart3.TemplateId = templateWstgIos.Id;
                templateWstgIosPart3.PartType = ReportPartType.Body;
                templateWstgIosPart3.ComponentId = introOwaspMastgComponent.Id;
                templateWstgIosPart3.Order = 1;
                reportsPartsManager.Add(templateWstgIosPart3);
                
                var templateWstgIosPart4 = new ReportParts();
                templateWstgIosPart4.Id = Guid.NewGuid();
                templateWstgIosPart4.TemplateId = templateWstgIos.Id;
                templateWstgIosPart4.PartType = ReportPartType.Body;
                templateWstgIosPart4.ComponentId = resultsIosOwaspMastgComponent.Id;
                templateWstgIosPart4.Order = 2;
                reportsPartsManager.Add(templateWstgIosPart4);
                
                var templateWstgIosPart5 = new ReportParts();
                templateWstgIosPart5.Id = Guid.NewGuid();
                templateWstgIosPart5.TemplateId = templateWstgIos.Id;
                templateWstgIosPart5.PartType = ReportPartType.Footer;
                templateWstgIosPart5.ComponentId = footerGeneralComponent.Id;
                templateWstgIosPart5.Order = 1;
                reportsPartsManager.Add(templateWstgIosPart5);
                
                var templateWstg = new ReportTemplate();
                templateWstg.Id = Guid.NewGuid();
                templateWstg.Name = "OWASP WSTG - Checklist Report";
                templateWstg.Description = "OWASP WSTG - Checklist Report";
                templateWstg.Language = Language.English;
                templateWstg.CreatedDate = DateTime.Now.ToUniversalTime();
                templateWstg.ReportType = ReportType.WSTG;
                templateWstg.UserId = admin.Id;
                reportTemplateManager.Add(templateWstg);
                reportTemplateManager.Context.SaveChanges();
                
                var templateWstgPart1= new ReportParts();
                templateWstgPart1.Id = Guid.NewGuid();
                templateWstgPart1.TemplateId = templateWstg.Id;
                templateWstgPart1.PartType = ReportPartType.Cover;
                templateWstgPart1.ComponentId = coverOwaspWstgComponent.Id;
                templateWstgPart1.Order = 1;
                reportsPartsManager.Add(templateWstgPart1);
                
                var templateWstgPart2= new ReportParts();
                templateWstgPart2.Id = Guid.NewGuid();
                templateWstgPart2.TemplateId = templateWstg.Id;
                templateWstgPart2.PartType = ReportPartType.Header;
                templateWstgPart2.ComponentId = headerOwaspWstgComponent.Id;
                templateWstgPart2.Order = 1;
                reportsPartsManager.Add(templateWstgPart2);
                
                var templateWstgPart3= new ReportParts();
                templateWstgPart3.Id = Guid.NewGuid();
                templateWstgPart3.TemplateId = templateWstg.Id;
                templateWstgPart3.PartType = ReportPartType.Header;
                templateWstgPart3.ComponentId = footerGeneralComponent.Id;
                templateWstgPart3.Order = 1;
                reportsPartsManager.Add(templateWstgPart3);
                
                var templateWstgPart4= new ReportParts();
                templateWstgPart4.Id = Guid.NewGuid();
                templateWstgPart4.TemplateId = templateWstg.Id;
                templateWstgPart4.PartType = ReportPartType.Body;
                templateWstgPart4.ComponentId = introOwaspWstgComponent.Id;
                templateWstgPart4.Order = 1;
                reportsPartsManager.Add(templateWstgPart4);
                
                var templateWstgPart5= new ReportParts();
                templateWstgPart5.Id = Guid.NewGuid();
                templateWstgPart5.TemplateId = templateWstg.Id;
                templateWstgPart5.PartType = ReportPartType.Body;
                templateWstgPart5.ComponentId = resultsOwaspWstgComponent.Id;
                templateWstgPart5.Order = 2;
                reportsPartsManager.Add(templateWstgPart5);
                reportsPartsManager.Context.SaveChanges();
                
                var templateGeneralEnglish = new ReportTemplate();
                templateGeneralEnglish.Id = Guid.NewGuid();
                templateGeneralEnglish.Name = "Default English Report Template";
                templateGeneralEnglish.Description = "Default English Report Template";
                templateGeneralEnglish.Language = Language.English;
                templateGeneralEnglish.CreatedDate = DateTime.Now.ToUniversalTime();
                templateGeneralEnglish.ReportType = ReportType.General;
                templateGeneralEnglish.UserId = admin.Id;
                reportTemplateManager.Add(templateGeneralEnglish);
                
                var templateGeneralEnglishPart1= new ReportParts();
                templateGeneralEnglishPart1.Id = Guid.NewGuid();
                templateGeneralEnglishPart1.TemplateId = templateGeneralEnglish.Id;
                templateGeneralEnglishPart1.PartType = ReportPartType.Cover;
                templateGeneralEnglishPart1.ComponentId = coverGeneralComponent.Id;
                templateGeneralEnglishPart1.Order = 1;
                reportsPartsManager.Add(templateGeneralEnglishPart1);

                var templateGeneralEnglishPart2= new ReportParts();
                templateGeneralEnglishPart2.Id = Guid.NewGuid();
                templateGeneralEnglishPart2.TemplateId = templateGeneralEnglish.Id;
                templateGeneralEnglishPart2.PartType = ReportPartType.Header;
                templateGeneralEnglishPart2.ComponentId = headerGeneralComponent.Id;
                templateGeneralEnglishPart2.Order = 1;
                reportsPartsManager.Add(templateGeneralEnglishPart2);

                var templateGeneralEnglishPart3= new ReportParts();
                templateGeneralEnglishPart3.Id = Guid.NewGuid();
                templateGeneralEnglishPart3.TemplateId = templateGeneralEnglish.Id;
                templateGeneralEnglishPart3.PartType = ReportPartType.Footer;
                templateGeneralEnglishPart3.ComponentId = footerGeneralComponent.Id;
                templateGeneralEnglishPart3.Order = 1;
                reportsPartsManager.Add(templateGeneralEnglishPart3);

                var templateGeneralEnglishPart4= new ReportParts();
                templateGeneralEnglishPart4.Id = Guid.NewGuid();
                templateGeneralEnglishPart4.TemplateId = templateGeneralEnglish.Id;
                templateGeneralEnglishPart4.PartType = ReportPartType.Body;
                templateGeneralEnglishPart4.ComponentId = disclaimerGeneralComponent.Id;
                templateGeneralEnglishPart4.Order = 1;
                reportsPartsManager.Add(templateGeneralEnglishPart4);

                var templateGeneralEnglishPart5= new ReportParts();
                templateGeneralEnglishPart5.Id = Guid.NewGuid();
                templateGeneralEnglishPart5.TemplateId = templateGeneralEnglish.Id;
                templateGeneralEnglishPart5.PartType = ReportPartType.Body;
                templateGeneralEnglishPart5.ComponentId = documentControlGeneralComponent.Id;
                templateGeneralEnglishPart5.Order = 2;
                reportsPartsManager.Add(templateGeneralEnglishPart5);

                var templateGeneralEnglishPart6= new ReportParts();
                templateGeneralEnglishPart6.Id = Guid.NewGuid();
                templateGeneralEnglishPart6.TemplateId = templateGeneralEnglish.Id;
                templateGeneralEnglishPart6.PartType = ReportPartType.Body;
                templateGeneralEnglishPart6.ComponentId = teamGeneralComponent.Id;
                templateGeneralEnglishPart6.Order = 3;
                reportsPartsManager.Add(templateGeneralEnglishPart6);

                var templateGeneralEnglishPart7= new ReportParts();
                templateGeneralEnglishPart7.Id = Guid.NewGuid();
                templateGeneralEnglishPart7.TemplateId = templateGeneralEnglish.Id;
                templateGeneralEnglishPart7.PartType = ReportPartType.Body;
                templateGeneralEnglishPart7.ComponentId = introductionGeneralComponent.Id;
                templateGeneralEnglishPart7.Order = 4;
                reportsPartsManager.Add(templateGeneralEnglishPart7);

                var templateGeneralEnglishPart8= new ReportParts();
                templateGeneralEnglishPart8.Id = Guid.NewGuid();
                templateGeneralEnglishPart8.TemplateId = templateGeneralEnglish.Id;
                templateGeneralEnglishPart8.PartType = ReportPartType.Body;
                templateGeneralEnglishPart8.ComponentId = purposeGeneralComponent.Id;
                templateGeneralEnglishPart8.Order = 5;
                reportsPartsManager.Add(templateGeneralEnglishPart8);

                
                var templateGeneralEnglishPart9= new ReportParts();
                templateGeneralEnglishPart9.Id = Guid.NewGuid();
                templateGeneralEnglishPart9.TemplateId = templateGeneralEnglish.Id;
                templateGeneralEnglishPart9.PartType = ReportPartType.Body;
                templateGeneralEnglishPart9.ComponentId = executiveSummaryGeneralComponent.Id;
                templateGeneralEnglishPart9.Order = 6;
                reportsPartsManager.Add(templateGeneralEnglishPart9);

                var templateGeneralEnglishPart10= new ReportParts();
                templateGeneralEnglishPart10.Id = Guid.NewGuid();
                templateGeneralEnglishPart10.TemplateId = templateGeneralEnglish.Id;
                templateGeneralEnglishPart10.PartType = ReportPartType.Body;
                templateGeneralEnglishPart10.ComponentId = scopeGeneralComponent.Id;
                templateGeneralEnglishPart10.Order = 7;
                reportsPartsManager.Add(templateGeneralEnglishPart10);

                var templateGeneralEnglishPart10Bis= new ReportParts();
                templateGeneralEnglishPart10Bis.Id = Guid.NewGuid();
                templateGeneralEnglishPart10Bis.TemplateId = templateGeneralEnglish.Id;
                templateGeneralEnglishPart10Bis.PartType = ReportPartType.Body;
                templateGeneralEnglishPart10Bis.ComponentId = methodlogyGeneralComponent.Id;
                templateGeneralEnglishPart10Bis.Order = 8;
                reportsPartsManager.Add(templateGeneralEnglishPart10Bis);

                
                var templateGeneralEnglishPart11= new ReportParts();
                templateGeneralEnglishPart11.Id = Guid.NewGuid();
                templateGeneralEnglishPart11.TemplateId = templateGeneralEnglish.Id;
                templateGeneralEnglishPart11.PartType = ReportPartType.Body;
                templateGeneralEnglishPart11.ComponentId = findingsOverViewGeneralComponent.Id;
                templateGeneralEnglishPart11.Order = 9;
                reportsPartsManager.Add(templateGeneralEnglishPart11);

                var templateGeneralEnglishPart12= new ReportParts();
                templateGeneralEnglishPart12.Id = Guid.NewGuid();
                templateGeneralEnglishPart12.TemplateId = templateGeneralEnglish.Id;
                templateGeneralEnglishPart12.PartType = ReportPartType.Body;
                templateGeneralEnglishPart12.ComponentId = findingsClassificationGeneralComponent.Id;
                templateGeneralEnglishPart12.Order = 10;
                reportsPartsManager.Add(templateGeneralEnglishPart12);

                
                var templateGeneralEnglishPart13= new ReportParts();
                templateGeneralEnglishPart13.Id = Guid.NewGuid();
                templateGeneralEnglishPart13.TemplateId = templateGeneralEnglish.Id;
                templateGeneralEnglishPart13.PartType = ReportPartType.Body;
                templateGeneralEnglishPart13.ComponentId = findingsDetailsGeneralComponent.Id;
                templateGeneralEnglishPart13.Order = 11;
                reportsPartsManager.Add(templateGeneralEnglishPart13);
                
                reportsPartsManager.Context.SaveChanges();

                #endregion
            #endregion

            #region Español

                #region Componentes General
                ReportComponents portadaGeneral = new ReportComponents();
                portadaGeneral.Id = Guid.NewGuid();
                portadaGeneral.Name = "General - Portada";
                portadaGeneral.Language = Language.Español;
                portadaGeneral.ComponentType = ReportPartType.Cover;
                portadaGeneral.Created = DateTime.Now.ToUniversalTime();
                portadaGeneral.Updated = DateTime.Now.ToUniversalTime();
                portadaGeneral.ContentCss = "";
                portadaGeneral.Content = @"<p>&nbsp;</p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <p style=""text-align: center""><img src=""data:image/png;base64,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"" width=""350"" height=""350""></p>
                <p style=""text-align: center"">&nbsp;</p>
                <p style=""text-align: center"">&nbsp;</p>
                <p style=""text-align: center"">&nbsp;</p>
                <h1 style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{ProjectName}}</span></h1>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <p style=""text-align: center"">Preparado por {{OrganizationName}}</p>
                <p style=""text-align: center"">Preparado para {{ClientName}}</p>
                <p style=""text-align: center"">&nbsp;</p>
                <p style=""text-align: center"">&nbsp;</p>
                <p style=""text-align: center"">Fecha de Inicio: {{StartDate}}</p>
                <p style=""text-align: center"">Fecha de Fin: {{EndDate}}</p>
                <p>&nbsp;</p>";
                reportComponentsManager.Add(portadaGeneral);
                reportComponentsManager.Context.SaveChanges();
                
                ReportComponents cabeceraGeneral = new ReportComponents();
                cabeceraGeneral.Id = Guid.NewGuid();
                cabeceraGeneral.Name = "General - Cabecera";
                cabeceraGeneral.Language = Language.Español;
                cabeceraGeneral.ComponentType = ReportPartType.Header;
                cabeceraGeneral.Created = DateTime.Now.ToUniversalTime();
                cabeceraGeneral.Updated = DateTime.Now.ToUniversalTime();
                cabeceraGeneral.ContentCss = "";
                cabeceraGeneral.Content = @"<table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none"" border=""1""><colgroup><col style=""width: 33.2692%""><col style=""width: 33.2692%""><col style=""width: 33.2692%""></colgroup>
                <tbody>
                <tr>
                <td style=""border-style: none""><span style=""color: rgba(0, 0, 0, 1)""><em>{{OrganizationName}}</em></span></td>
                <td style=""border-style: none; text-align: center""><span style=""color: rgba(0, 0, 0, 1)""><em>Confidencial&nbsp;</em></span></td>
                <td style=""border-style: none; text-align: right""><span style=""color: rgba(0, 0, 0, 1)""><em>{{ProjectName}}</em></span></td>
                </tr>
                </tbody>
                </table>
                <hr>
                <p>&nbsp;</p>";
                reportComponentsManager.Add(cabeceraGeneral);
                reportComponentsManager.Context.SaveChanges();

                ReportComponents pieGeneral = new ReportComponents();
                pieGeneral.Id = Guid.NewGuid();
                pieGeneral.Name = "General - Pie de Página";
                pieGeneral.Language = Language.Español;
                pieGeneral.ComponentType = ReportPartType.Footer;
                pieGeneral.Created = DateTime.Now.ToUniversalTime();
                pieGeneral.Updated = DateTime.Now.ToUniversalTime();
                pieGeneral.ContentCss = "";
                pieGeneral.Content = @"<hr>
                <table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none"" border=""1""><colgroup><col style=""width: 33.2692%""><col style=""width: 33.2692%""><col style=""width: 33.2692%""></colgroup>
                <tbody>
                <tr>
                <td style=""border-style: none""><span style=""color: rgba(0, 0, 0, 1)""><em>{{Year}}</em></span></td>
                <td style=""border-style: none"">&nbsp;</td>
                <td style=""border-style: none; text-align: right""><span style=""color: rgba(0, 0, 0, 1)""><em>Preparado para {{ClientName}}</em></span></td>
                </tr>
                </tbody>
                </table>";
                reportComponentsManager.Add(pieGeneral);
                reportComponentsManager.Context.SaveChanges();
                
                ReportComponents introGeneral = new ReportComponents();
                introGeneral.Id = Guid.NewGuid();
                introGeneral.Name = "General - Introducción";
                introGeneral.Language = Language.Español;
                introGeneral.ComponentType = ReportPartType.Body;
                introGeneral.Created = DateTime.Now.ToUniversalTime();
                introGeneral.Updated = DateTime.Now.ToUniversalTime();
                introGeneral.ContentCss = "";
                introGeneral.Content = @"<h1><span style=""font-family: -apple-system, BlinkMacSystemFont, &quot;Segoe UI&quot;, Roboto, Oxygen, Ubuntu, Cantarell, &quot;Open Sans&quot;, &quot;Helvetica Neue&quot;, sans-serif; color: rgba(0, 0, 0, 1)"">Introducción</span></h1>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">Este documento presenta los resultados de una Auditoría de Seguridad para {{ClientName}}. Este compromiso tenía como objetivo descubrir vulnerabilidades de seguridad que pudieran afectar negativamente a las redes o sistemas de {{ClientName}}.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">{{ClientDescription}}</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">Para cada vulnerabilidad descubierta durante la evaluación, {{OrganizationName}} asignó una calificación de gravedad de riesgo y, siempre que fuera posible, validó la existencia de la vulnerabilidad con un código de explotación funcional.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">Los objetivos principales de la evaluación fueron los siguientes:</span></p>
                <p>&nbsp;</p>
                <ul>
                <li>Identificar los principales problemas relacionados con la seguridad presentes.</li>
                <li>Evaluar el nivel de prácticas de codificación segura en el proyecto.</li>
                <li>Obtener evidencias para cada vulnerabilidad y, si es posible, desarrollar un código de explotación funcional.</li>
                <li>Documentar, de manera clara y fácil de reproducir, todos los procedimientos utilizados para replicar el problema.</li>
                <li>Recomendar factores de mitigación y soluciones para cada defecto identificado en el análisis.</li>
                <li>Proporcionar contexto con un escenario de riesgo real basado en un modelo de amenazas realista.</li>
                </ul>
                <p>&nbsp;</p>";
                reportComponentsManager.Add(introGeneral);
                reportComponentsManager.Context.SaveChanges();
                
                ReportComponents equipoGeneral = new ReportComponents();
                equipoGeneral.Id = Guid.NewGuid();
                equipoGeneral.Name = "General - Equipo";
                equipoGeneral.Language = Language.Español;
                equipoGeneral.ComponentType = ReportPartType.Body;
                equipoGeneral.Created = DateTime.Now.ToUniversalTime();
                equipoGeneral.Updated = DateTime.Now.ToUniversalTime();
                equipoGeneral.ContentCss = "";
                equipoGeneral.Content = @"<h1><span style=""color: rgba(0, 0, 0, 1)"">Team</span></h1>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 33.3333%""><col style=""width: 33.3333%""><col style=""width: 33.3333%""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nombre</span></strong></td>
                <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">Versión</span></strong></td>
                <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">Posición</span></strong></td>
                </tr>

                <tr>
                <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{tablerow user in Users}}{{user.UserFullName}}</span></td>
                <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{user.UserEmail}}</span></td>
                <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{user.UserPosition}}{{end}}</span></td>
                </tr>
                </tbody>
                </table>";
                reportComponentsManager.Add(equipoGeneral);
                reportComponentsManager.Context.SaveChanges();
                
                ReportComponents metodologiaGeneral = new ReportComponents();
                metodologiaGeneral.Id = Guid.NewGuid();
                metodologiaGeneral.Name = "General - Metodología";
                metodologiaGeneral.Language = Language.Español;
                metodologiaGeneral.ComponentType = ReportPartType.Body;
                metodologiaGeneral.Created = DateTime.Now.ToUniversalTime();
                metodologiaGeneral.Updated = DateTime.Now.ToUniversalTime();
                metodologiaGeneral.ContentCss = "";
                metodologiaGeneral.Content = @"<h1><span style=""color: rgba(0, 0, 0, 1)"">Metodología</span></h1>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">La metodología consistió en 7 pasos que comenzaron con la determinación del alcance de la prueba y finalizaron con la elaboración del informe. Estas pruebas fueron realizadas por expertos en seguridad utilizando modos de operación de posibles atacantes, controlando la ejecución para evitar daños a los sistemas probados. El enfoque incluyó, pero no se limitó a, escaneos de vulnerabilidades manuales y automáticos, verificación de hallazgos (tanto automatizados como otros). Este paso de verificación y el proceso de escaneo manual eliminaron falsos positivos y salidas erróneas, lo que resultó en pruebas más eficientes.</span></p>
                <p>&nbsp;</p>
                <ul>
                <li style=""color: rgba(0, 0, 0, 1)""><span style=""color: rgba(0, 0, 0, 1)"">Determinación del alcance de la prueba</span></li>
                <li style=""color: rgba(0, 0, 0, 1)""><span style=""color: rgba(0, 0, 0, 1)"">Recopilación de información / Reconocimiento</span></li>
                <li style=""color: rgba(0, 0, 0, 1)""><span style=""color: rgba(0, 0, 0, 1)"">Escaneo</span></li>
                <li style=""color: rgba(0, 0, 0, 1)""><span style=""color: rgba(0, 0, 0, 1)"">Análisis de vulnerabilidades</span></li>
                <li style=""color: rgba(0, 0, 0, 1)""><span style=""color: rgba(0, 0, 0, 1)"">Explotación</span></li>
                <li style=""color: rgba(0, 0, 0, 1)""><span style=""color: rgba(0, 0, 0, 1)"">Actividades post-explotación</span></li>
                <li style=""color: rgba(0, 0, 0, 1)""><span style=""color: rgba(0, 0, 0, 1)"">Elaboración del informe</span></li>
                </ul>";
                reportComponentsManager.Add(metodologiaGeneral);
                reportComponentsManager.Context.SaveChanges();
                
                
                ReportComponents alcanceGeneral = new ReportComponents();
                alcanceGeneral.Id = Guid.NewGuid();
                alcanceGeneral.Name = "General - Alcance";
                alcanceGeneral.Language = Language.Español;
                alcanceGeneral.ComponentType = ReportPartType.Body;
                alcanceGeneral.Created = DateTime.Now.ToUniversalTime();
                alcanceGeneral.Updated = DateTime.Now.ToUniversalTime();
                alcanceGeneral.ContentCss = "";
                alcanceGeneral.Content = @"<h1><span style=""color: rgba(0, 0, 0, 1)"">Alcance</span></h1>
                    <p>De acuerdo con el acuerdo firmado entre {{OrganizationName}} y {{ClientName}}, la prueba de penetración se llevó a cabo entre {{StartDate}} y {{EndDate}}.</p>
                    <p>El alcance de la prueba se limitó a los objetivos enumerados a continuación.</p>
                    <p>&nbsp;</p>
                    <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 33.3333%""><col style=""width: 33.3333%""><col style=""width: 33.3333%""></colgroup>
                    <tbody>
                    <tr>
                    <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">Objetivo</span></strong></td>
                    <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">Descripción</span></strong></td>
                    <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">Tipo</span></strong></td>
                    </tr>

                    <tr>
                    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{tablerow target in Targets}}{{target.TargetName}}</span></td>
                    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{target.TargetDescription}}</span></td>
                    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{target.TargetType}}{{end}}</span></td>
                    </tr>
                    </tbody>
                    </table>";
                reportComponentsManager.Add(alcanceGeneral);
                reportComponentsManager.Context.SaveChanges();
                
                ReportComponents resumenHallazgosGeneral = new ReportComponents();
                resumenHallazgosGeneral.Id = Guid.NewGuid();
                resumenHallazgosGeneral.Name = "General - Resumen de Hallazgos (Vulnerabilidades)";
                resumenHallazgosGeneral.Language = Language.Español;
                resumenHallazgosGeneral.ComponentType = ReportPartType.Body;
                resumenHallazgosGeneral.Created = DateTime.Now.ToUniversalTime();
                resumenHallazgosGeneral.Updated = DateTime.Now.ToUniversalTime();
                resumenHallazgosGeneral.ContentCss = "";
                resumenHallazgosGeneral.Content = @"<h1>Resumen de Hallazgos</h1>
                <p>&nbsp;</p>
                <p>Las siguientes secciones enumeran tanto vulnerabilidades como problemas de implementación identificados durante el período de prueba. Tenga en cuenta que los hallazgos se enumeran según su grado de gravedad e impacto. El rango de gravedad mencionado anteriormente se proporciona simplemente entre corchetes después del encabezado del título para cada vulnerabilidad.</p>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; height: 134.344px"" border=""1""><colgroup><col style=""width: 50%""><col style=""width: 50%""></colgroup>
                <tbody>
                <tr style=""height: 22.3906px"">
                <td style=""background-color: rgba(0, 0, 0, 1); text-align: center; height: 22.3906px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Severidad</span></strong></td>
                <td style=""background-color: rgba(0, 0, 0, 1); text-align: center; height: 22.3906px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Número</span></strong></td>
                </tr>
                <tr style=""height: 22.3906px"">
                <td style=""text-align: center; background-color: rgba(186, 55, 42, 1); height: 22.3906px""><span style=""color: rgba(255, 255, 255, 1)""><strong>Crítica</strong></span></td>
                <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{VulnCriticalCount}}</span></td>
                </tr>
                <tr style=""height: 22.3906px"">
                <td style=""text-align: center; background-color: rgba(224, 62, 45, 1); height: 22.3906px""><span style=""color: rgba(255, 255, 255, 1)""><strong>Alta</strong></span></td>
                <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{VulnHighCount}}</span></td>
                </tr>
                <tr style=""height: 22.3906px"">
                <td style=""text-align: center; background-color: rgba(230, 126, 35, 1); height: 22.3906px""><span style=""color: rgba(255, 255, 255, 1)""><strong>Media</strong></span></td>
                <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{VulnMediumCount}}</span></td>
                </tr>
                <tr style=""height: 22.3906px"">
                <td style=""text-align: center; background-color: rgba(241, 196, 15, 1); height: 22.3906px""><span style=""color: rgba(255, 255, 255, 1)""><strong>Baja</strong></span></td>
                <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{VulnLowCount}}</span></td>
                </tr>
                <tr style=""height: 22.3906px"">
                <td style=""text-align: center; background-color: rgba(53, 152, 219, 1); height: 22.3906px""><span style=""color: rgba(255, 255, 255, 1)""><strong>Informativa</strong></span></td>
                <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{VulnInfoCount}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <p>&nbsp;</p>";
                reportComponentsManager.Add(resumenHallazgosGeneral);
                reportComponentsManager.Context.SaveChanges();
                
                ReportComponents clasHallazgosGeneral = new ReportComponents();
                clasHallazgosGeneral.Id = Guid.NewGuid();
                clasHallazgosGeneral.Name = "General - Clasificación de Hallazgos (Vulnerabilidades)";
                clasHallazgosGeneral.Language = Language.Español;
                clasHallazgosGeneral.ComponentType = ReportPartType.Body;
                clasHallazgosGeneral.Created = DateTime.Now.ToUniversalTime();
                clasHallazgosGeneral.Updated = DateTime.Now.ToUniversalTime();
                clasHallazgosGeneral.ContentCss = "";
                clasHallazgosGeneral.Content = @"
                <h1>Clasificación de Hallazgos</h1>
                <p>&nbsp;</p>
                <p>Cada vulnerabilidad o riesgo identificado se ha etiquetado como un hallazgo y se ha categorizado como Riesgo Crítico, Alto, Medio, Bajo o Informativo, que se definen de la siguiente manera:</p>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; height: 482.062px"" border=""1""><colgroup><col style=""width: 99.9444%""></colgroup>
                <tbody>
                <tr style=""height: 22.3906px"">
                <td style=""text-align: center; background-color: rgba(186, 55, 42, 1); height: 22.3906px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Crítica</span></strong></td>
                </tr>
                <tr style=""height: 92.7812px"">
                <td style=""height: 92.7812px"">
                <p>&nbsp;</p>
                <p>Estas vulnerabilidades deben abordarse de inmediato, ya que pueden representar un peligro inmediato para la seguridad de las redes, sistemas o datos.</p>
                <p>La explotación no requiere herramientas o técnicas avanzadas ni conocimientos especiales del objetivo.</p>
                <p>&nbsp;</p>
                </td>
                </tr>
                <tr style=""height: 22.3906px"">
                <td style=""text-align: center; background-color: rgba(224, 62, 45, 1); height: 22.3906px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Alta</span></strong></td>
                </tr>
                <tr style=""height: 92.7812px"">
                <td style=""height: 92.7812px"">
                <p>&nbsp;</p>
                <p>Estas vulnerabilidades deben abordarse de inmediato, ya que pueden representar un peligro inmediato para la seguridad de las redes, sistemas o datos.</p>
                <p>El problema suele ser más difícil de explotar, pero podría permitir permisos elevados, pérdida de datos o tiempo de inactividad del sistema.</p>
                <p>&nbsp;</p>
                </td>
                </tr>
                <tr style=""height: 22.3906px"">
                <td style=""text-align: center; background-color: rgba(230, 126, 35, 1); height: 22.3906px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Media</span></strong></td>
                </tr>
                <tr style=""height: 92.7812px"">
                <td style=""height: 92.7812px"">
                <p>&nbsp;</p>
                <p>Estas vulnerabilidades deben abordarse de manera oportuna.</p>
                <p>La explotación suele ser difícil y requiere ingeniería social, acceso existente o circunstancias especiales.</p>
                <p>&nbsp;</p>
                </td>
                </tr>
                <tr style=""height: 22.3906px"">
                <td style=""text-align: center; background-color: rgba(241, 196, 15, 1); height: 22.3906px""><span style=""color: rgba(255, 255, 255, 1)""><strong>Baja</strong></span></td>
                </tr>
                <tr style=""height: 54.3906px"">
                <td style=""height: 54.3906px"">
                <p>&nbsp;</p>
                Estos problemas ofrecen muy pocas oportunidades o información a un atacante y es posible que no representen una amenaza real.
                <p>&nbsp;</p>
                </td>
                </tr>
                <tr style=""height: 22.3906px"">
                <td style=""text-align: center; background-color: rgba(53, 152, 219, 1); height: 22.3906px""><span style=""color: rgba(255, 255, 255, 1)""><strong>Informativa</strong></span></td>
                </tr>
                <tr style=""height: 37.375px"">
                <td style=""height: 37.375px""><span style=""color: rgba(0, 0, 0, 1)"">Estos problemas son únicamente con fines informativos y probablemente no representan una amenaza real.</span></td>
                </tr>
                </tbody>
                </table>";
                reportComponentsManager.Add(clasHallazgosGeneral);
                reportComponentsManager.Context.SaveChanges();
                
                ReportComponents controlDocGeneral = new ReportComponents();
                controlDocGeneral.Id = Guid.NewGuid();
                controlDocGeneral.Name = "General - Control del Documento";
                controlDocGeneral.Language = Language.Español;
                controlDocGeneral.ComponentType = ReportPartType.Body;
                controlDocGeneral.Created = DateTime.Now.ToUniversalTime();
                controlDocGeneral.Updated = DateTime.Now.ToUniversalTime();
                controlDocGeneral.ContentCss = "";
                controlDocGeneral.Content = @"<h1><span style=""color: rgba(0, 0, 0, 1)"">Control del Documento</span></h1>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 33.3333%""><col style=""width: 33.3333%""><col style=""width: 33.3333%""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nombre</span></strong></td>
                <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">Versión</span></strong></td>
                <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">Descripción</span></strong></td>
                </tr>

                <tr>
                <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{tablerow doc in Documents}}{{doc.DocumentName}}</span></td>
                <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{doc.DocumentVersion}}</span></td>
                <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{doc.DocumentDescription}}{{end}}</span></td>
                </tr>
                </tbody>
                </table>";
                reportComponentsManager.Add(controlDocGeneral);
                reportComponentsManager.Context.SaveChanges();
                
                ReportComponents descargoResponsabilidadGeneral = new ReportComponents();
                descargoResponsabilidadGeneral.Id = Guid.NewGuid();
                descargoResponsabilidadGeneral.Name = "General - Descargo de responsabilidad";
                descargoResponsabilidadGeneral.Language = Language.Español;
                descargoResponsabilidadGeneral.ComponentType = ReportPartType.Body;
                descargoResponsabilidadGeneral.Created = DateTime.Now.ToUniversalTime();
                descargoResponsabilidadGeneral.Updated = DateTime.Now.ToUniversalTime();
                descargoResponsabilidadGeneral.ContentCss = "";
                descargoResponsabilidadGeneral.Content = @"<table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none"" border=""1""><colgroup><col style=""width: 50%""><col style=""width: 50%""></colgroup>
<tbody>
<tr>
<td style=""border-style: none"">
<p><span style=""color: rgba(0, 0, 0, 1)""><strong>{{OrganizationName}}</strong></span></p>
<p><span style=""color: rgba(0, 0, 0, 1)"">{{OrganizationEmail}}</span></p>
<p><span style=""color: rgba(0, 0, 0, 1)"">{{OrganizationPhone}}</span></p>
</td>
<td style=""border-style: none"">
<p style=""text-align: right""><span style=""color: rgba(0, 0, 0, 1)""><strong>{{ClientName}}</strong></span></p>
<p style=""text-align: right""><span style=""color: rgba(0, 0, 0, 1)"">{{ClientEmail}}</span></p>
<p style=""text-align: right""><span style=""color: rgba(0, 0, 0, 1)"">{{ClientPhone}}</span></p>
</td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<h2><span style=""color: rgba(0, 0, 0, 1)"">&nbsp;</span><br><span style=""color: rgba(0, 0, 0, 1)"">Descargo de responsabilidad</span><br><span style=""color: rgba(0, 0, 0, 1)"">&nbsp;</span></h2>
<p><span style=""color: rgba(0, 0, 0, 1)"">No se ofrecen garantías, expresas o implícitas, por parte de {{OrganizationName}} con respecto a la precisión, confiabilidad, calidad, corrección o ausencia de errores u omisiones de este producto de trabajo, incluyendo cualquier garantía implícita de comerciabilidad, idoneidad para un propósito específico o no infracción. Este documento se entrega ""tal cual"", y {{OrganizationName}} no será responsable de ninguna inexactitud del mismo. {{OrganizationName}} no garantiza que se corrijan todos los errores en este producto de trabajo. Excepto según se establezca expresamente en cualquier acuerdo de servicios maestros o asignación de proyecto, {{OrganizationName}} no asume ninguna obligación o responsabilidad, incluyendo, entre otros, daños directos, indirectos, incidentales o consecuentes, especiales o ejemplares, resultantes del uso o confianza en cualquier información de este documento. Este documento no implica un respaldo de ninguna de las empresas o productos mencionados.</span></p>
<p>&nbsp;</p>
<p><span style=""color: rgba(0, 0, 0, 1)"">© {{Year}} {{OrganizationName}}. Todos los derechos reservados. Ninguna parte de este documento puede ser reproducida, copiada o modificada sin el consentimiento expreso por escrito de los autores. A menos que se otorgue permiso por escrito de manera expresa para otros fines, este documento se considerará en todo momento como material confidencial y propiedad de {{OrganizationName}} y no podrá ser distribuido o publicado a terceros.</span></p>";
                reportComponentsManager.Add(descargoResponsabilidadGeneral);
                reportComponentsManager.Context.SaveChanges();
                
                ReportComponents detallesHallazgosGeneral = new ReportComponents();
                detallesHallazgosGeneral.Id = Guid.NewGuid();
                detallesHallazgosGeneral.Name = "General - Detalles de Hallazgos (Vulnerabilidades)";
                detallesHallazgosGeneral.Language = Language.Español;
                detallesHallazgosGeneral.ComponentType = ReportPartType.Body;
                detallesHallazgosGeneral.Created = DateTime.Now.ToUniversalTime();
                detallesHallazgosGeneral.Updated = DateTime.Now.ToUniversalTime();
                detallesHallazgosGeneral.ContentCss = "";
                detallesHallazgosGeneral.Content = @"<h1><span style=""color: rgba(0, 0, 0, 1)"">Hallazgos</span></h1>
                <p>&nbsp;</p>
                <p>{{for vuln in Vulns}}</p>    
                <p>&nbsp;</p>
                <h2><span style=""color: rgba(0, 0, 0, 1)"">{{vuln.VulnFindingId}} - {{vuln.VulnName}}</span></h2>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 16.6263%""><col style=""width: 16.6263%""><col style=""width: 16.6263%""><col style=""width: 16.6263%""><col style=""width: 16.6263%""><col style=""width: 16.6263%""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">CVE</span></strong></td>
                <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{vuln.VulnCve}}</span></td>
                <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">Riesgo</span></strong></td>
                <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{vuln.VulnRisk}}</span></td>
                <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">CVSS</span></strong></td>
                <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{vuln.VulnCvss}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 7.91599%""><col style=""width: 42.1648%""><col style=""width: 6.70273%""><col style=""width: 43.2165%""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">Categoría</span></strong></td>
                <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{vuln.VulnCategory}}</span></td>
                <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">CWE</span></strong></td>
                <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{vuln.VulnCwes}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 99.9183%""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">Descripción</span></strong></td>
                </tr>
                <tr>
                <td style=""text-align: justify""><span style=""color: rgba(0, 0, 0, 1)"">{{vuln.VulnDescription}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 99.9183%""></colgroup>
                <tbody>
                <tr>
                <td style=""text-align: center; background-color: rgba(0, 0, 0, 1)""><span style=""color: rgba(255, 255, 255, 1)""><strong>Impacto</strong></span></td>
                </tr>
                <tr>
                <td><span style=""color: rgba(0, 0, 0, 1)"">{{vuln.VulnImpact}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 99.9183%""></colgroup>
                <tbody>
                <tr>
                <td style=""text-align: center; background-color: rgba(0, 0, 0, 1)""><span style=""color: rgba(255, 255, 255, 1)""><strong>Prueba de Concepto</strong></span></td>
                </tr>
                <tr>
                <td><span style=""color: rgba(0, 0, 0, 1)"">{{vuln.VulnPoc}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 99.9183%""></colgroup>
                <tbody>
                <tr>
                <td style=""text-align: center; background-color: rgba(0, 0, 0, 1)""><span style=""color: rgba(255, 255, 255, 1)""><strong>Remediación</strong></span></td>
                </tr>
                <tr>
                <td><span style=""color: rgba(0, 0, 0, 1)"">{{vuln.VulnRemediation}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 25%""><col style=""width: 25%""><col style=""width: 25%""><col style=""width: 25%""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">Complejidad</span></strong></td>
                <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{vuln.VulnComplexity}}</span></td>
                <td style=""text-align: center; background-color: rgba(0, 0, 0, 1)""><span style=""color: rgba(255, 255, 255, 1)""><strong>Prioridad</strong></span></td>
                <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{vuln.VulnPriority}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 17.116%""><col style=""width: 82.9657%""></colgroup>
                <tbody>
                <tr>
                <td style=""background-color: rgba(0, 0, 0, 1); text-align: center""><strong><span style=""color: rgba(255, 255, 255, 1)"">Activos Afectados</span></strong></td>
                <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{vuln.VulnTargets}}</span></td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <p>{{end}}</p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>";
                reportComponentsManager.Add(detallesHallazgosGeneral);
                reportComponentsManager.Context.SaveChanges();
                
                ReportComponents resumenEjecutivoGeneral = new ReportComponents();
                resumenEjecutivoGeneral.Id = Guid.NewGuid();
                resumenEjecutivoGeneral.Name = "General - Resumen Ejecutivo";
                resumenEjecutivoGeneral.Language = Language.Español;
                resumenEjecutivoGeneral.ComponentType = ReportPartType.Body;
                resumenEjecutivoGeneral.Created = DateTime.Now.ToUniversalTime();
                resumenEjecutivoGeneral.Updated = DateTime.Now.ToUniversalTime();
                resumenEjecutivoGeneral.ContentCss = "";
                resumenEjecutivoGeneral.Content = @"<h1><span style=""color: rgba(0, 0, 0, 1)"">Resumen Ejecutivo</span></h1>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">{{ProjectExecutiveSummary}}</span></p>";
                reportComponentsManager.Add(resumenEjecutivoGeneral);
                reportComponentsManager.Context.SaveChanges();
                
                ReportComponents propositoGeneral = new ReportComponents();
                propositoGeneral.Id = Guid.NewGuid();
                propositoGeneral.Name = "General - Propósito del Proyecto";
                propositoGeneral.Language = Language.Español;
                propositoGeneral.ComponentType = ReportPartType.Body;
                propositoGeneral.Created = DateTime.Now.ToUniversalTime();
                propositoGeneral.Updated = DateTime.Now.ToUniversalTime();
                propositoGeneral.ContentCss = "";
                propositoGeneral.Content = @"<h1><span style=""color: rgba(0, 0, 0, 1)"">Propósito</span></h1>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">{{ProjectDescription}}</span></p>";
                reportComponentsManager.Add(propositoGeneral);
                reportComponentsManager.Context.SaveChanges();
                #endregion
                
                #region OWASP WSTG
                ReportComponents wstgPortada = new ReportComponents();
                wstgPortada.Id = Guid.NewGuid();
                wstgPortada.Name = "OWASP WSTG - Portada";
                wstgPortada.Language = Language.Español;
                wstgPortada.ComponentType = ReportPartType.Cover;
                wstgPortada.Created = DateTime.Now.ToUniversalTime();
                wstgPortada.Updated = DateTime.Now.ToUniversalTime();
                wstgPortada.ContentCss = "";
                wstgPortada.Content = @"<p>&nbsp;</p>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none"" border=""1""><colgroup><col style=""width: 51.8049%""><col style=""width: 48.1951%""></colgroup>
                <tbody>
                <tr>
                <td style=""border-style: none; text-align: center""><img style=""float: left"" src=""data:image/png;base64,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"" width=""250"" height=""87""></td>
                <td style=""border-style: none""><img style=""float: right"" src=""data:image/png;base64,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"" width=""270"" height=""54""></td>
                </tr>
                </tbody>
                </table>
                <h1 style=""text-align: center"">&nbsp;</h1>
                <p>&nbsp;</p>
                <h1 style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Informe OWASP WSTG</span></h1>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none"" border=""1""><colgroup><col style=""width: 50%""><col style=""width: 50%""></colgroup>
                <tbody>
                <tr>
                <td style=""border-style: none"">
                <p><span style=""color: rgba(0, 0, 0, 1)""><strong>Organización: {{OrganizationName}}</strong></span></p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">Email de Contacto: {{OrganizationEmail}}</span></p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">Teléfono de Contacto: {{OrganizationPhone}}</span></p>
                </td>
                <td style=""border-style: none"">
                <p style=""text-align: right""><span style=""color: rgba(0, 0, 0, 1)""><strong>Cliente: {{ClientName}}</strong></span></p>
                <p style=""text-align: right""><span style=""color: rgba(0, 0, 0, 1)"">Email del Cliente: {{ClientEmail}}</span></p>
                <p style=""text-align: right""><span style=""color: rgba(0, 0, 0, 1)"">Teléfono del Cliente: {{ClientPhone}}</span></p>
                </td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none"" border=""1""><colgroup><col style=""width: 50%""><col style=""width: 50%""></colgroup>
                <tbody>
                <tr>
                <td style=""text-align: center; border-style: none""><span style=""color: rgba(0, 0, 0, 1)""><strong>Url</strong>: {{TargetUrl}}</span></td>
                <td style=""text-align: center; border-style: none""><span style=""color: rgba(0, 0, 0, 1)""><strong>Fecha de Creación</strong>: {{CreatedDate}}</span></td>
                </tr>
                </tbody>
                </table>
                <div style=""break-after: page"">&nbsp;</div>";
                reportComponentsManager.Add(wstgPortada);
                reportComponentsManager.Context.SaveChanges();
                
                ReportComponents wstgCabecera = new ReportComponents();
                wstgCabecera.Id = Guid.NewGuid();
                wstgCabecera.Name = "OWASP WSTG - Cabecera";
                wstgCabecera.Language = Language.Español;
                wstgCabecera.ComponentType = ReportPartType.Header;
                wstgCabecera.Created = DateTime.Now.ToUniversalTime();
                wstgCabecera.Updated = DateTime.Now.ToUniversalTime();
                wstgCabecera.ContentCss = "";
                wstgCabecera.Content = @"<table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none"" border=""1""><colgroup><col style=""width: 33.3655%""><col style=""width: 33.3655%""><col style=""width: 33.3655%""></colgroup>
                <tbody>
                <tr>
                <td style=""border-style: none""><span style=""color: rgba(0, 0, 0, 1)""><em>Confidencial&nbsp;</em></span></td>
                <td style=""border-style: none"">&nbsp;</td>
                <td style=""border-style: none; text-align: right""><span style=""color: rgba(0, 0, 0, 1)""><em>Informe OWASP WSTG</em></span></td>
                </tr>
                </tbody>
                </table>
                <hr>
                <p>&nbsp;</p>";
                reportComponentsManager.Add(wstgCabecera);
                reportComponentsManager.Context.SaveChanges();

                ReportComponents wstgIntroduccion = new ReportComponents();
                wstgIntroduccion.Id = Guid.NewGuid();
                wstgIntroduccion.Name = "OWASP WSTG - Introducción";
                wstgIntroduccion.Language = Language.Español;
                wstgIntroduccion.ComponentType = ReportPartType.Body;
                wstgIntroduccion.Created = DateTime.Now.ToUniversalTime();
                wstgIntroduccion.Updated = DateTime.Now.ToUniversalTime();
                wstgIntroduccion.ContentCss = "";
                wstgIntroduccion.Content = @"<h1><span style=""color: rgba(0, 0, 0, 1)"">Introducción</span></h1>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">{{ClientName}}, nos complace presentarle el informe del Proyecto de Pruebas de Seguridad de Aplicaciones Web (WSTG) del Proyecto de Seguridad de Aplicaciones Web de Código Abierto (OWASP), como resultado de nuestro reciente compromiso de pruebas de penetración en su infraestructura digital. El informe WSTG de OWASP proporciona un examen integral de su sistema desde una perspectiva de seguridad.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">A lo largo de nuestro compromiso, hemos utilizado el WSTG de OWASP, una metodología estándar de la industria para pruebas robustas de seguridad de aplicaciones web. Nuestro equipo de probadores de penetración altamente capacitados ha llevado a cabo una evaluación sistemática de sus aplicaciones web para identificar cualquier vulnerabilidad existente, amenaza o debilidad que pueda ser explotada por partes malintencionadas.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">Es importante tener en cuenta que este informe proporciona una visión general de las vulnerabilidades identificadas. Las recomendaciones dadas en este informe están diseñadas para ayudar a su organización a minimizar el riesgo de futuros incidentes de seguridad, en vista de las amenazas cibernéticas en constante evolución.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">Le pedimos que revise este informe detenidamente y nos informe si hay áreas que le gustaría discutir más a fondo o si necesita aclaraciones adicionales.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">Esperamos sus comentarios y estamos ansiosos por ayudarle a fortalecer su perfil de seguridad informática. La ciberseguridad es un viaje, no un destino, y estamos comprometidos a ser su socio de confianza en este camino.</span></p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>";
                reportComponentsManager.Add(wstgIntroduccion);
                reportComponentsManager.Context.SaveChanges();
                
                ReportComponents wstgResultados = new ReportComponents();
                wstgResultados.Id = Guid.NewGuid();
                wstgResultados.Name = "OWASP WSTG - Resultados";
                wstgResultados.Language = Language.Español;
                wstgResultados.ComponentType = ReportPartType.Body;
                wstgResultados.Created = DateTime.Now.ToUniversalTime();
                wstgResultados.Updated = DateTime.Now.ToUniversalTime();
                wstgResultados.ContentCss = "";
                wstgResultados.Content = @"<h2><span style=""color: rgba(0, 0, 0, 1)"">Resultados</span></h2>
    <table style=""border-collapse: collapse; width: 100%; height: 1058.89px"" border=""1""><colgroup><col style=""width: 13.4188%""><col style=""width: 26.6118%""><col style=""width: 29.2178%""><col style=""width: 10.8129%""><col style=""width: 19.9387%""></colgroup>
    <tbody>
    <tr style=""height: 44.7812px"">
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Recopilación de Información<br></span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Objetivos</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr style=""height: 143.953px"">
    <td style=""text-align: center; height: 143.953px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INFO-01</span></td>
    <td style=""height: 143.953px; text-align: left"">
    <p style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Realizar un reconocimiento de filtración de información mediante la búsqueda en motores de búsqueda.</span></p>
    </td>
    <td style=""height: 143.953px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar qué información sensible de diseño y configuración de la aplicación, sistema u organización se expone directamente (en el sitio web de la organización) o indirectamente (a través de servicios de terceros).</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 143.953px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info01Status}}</span></td>
    <td style=""text-align: center; height: 143.953px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info01Note}}</span></td>
    </tr>
    <tr style=""height: 99.1719px"">
    <td style=""text-align: center; height: 99.1719px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INFO-02</span></td>
    <td style=""text-align: center; height: 99.1719px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Huellas del Servidor Web</span></p>
    </td>
    <td style=""height: 99.1719px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Determinar la versión y tipo de un servidor web en ejecución para permitir una mayor exploración de posibles vulnerabilidades conocidas.&nbsp;</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 99.1719px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info02Status}}</span></td>
    <td style=""text-align: center; height: 99.1719px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info02Note}}</span></td>
    </tr>
    <tr style=""height: 121.562px"">
    <td style=""text-align: center; height: 121.562px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INFO-03</span></td>
    <td style=""text-align: center; height: 121.562px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Revisar Metarchivos del Servidor Web en busca de Fugas de Información</span></p>
    </td>
    <td style=""height: 121.562px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">&nbsp;Identificar rutas y funcionalidades ocultas u ofuscadas mediante el análisis de archivos de metadatos.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Extraer y mapear otra información que pueda conducir a una mejor comprensión de los sistemas en cuestión.</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 121.562px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info03Status}}</span></td>
    <td style=""text-align: center; height: 121.562px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info03Note}}</span></td>
    </tr>
    <tr style=""height: 76.7812px"">
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INFO-04</span></td>
    <td style=""text-align: center; height: 76.7812px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Enumerar Aplicaciones en el Servidor Web</span></p>
    </td>
    <td style=""height: 76.7812px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Enumerar las aplicaciones dentro del alcance que existen en un servidor web.</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info04Status}}</span></td>
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info04Note}}</span></td>
    </tr>
    <tr style=""height: 188.734px"">
    <td style=""text-align: center; height: 188.734px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INFO-05</span></td>
    <td style=""text-align: center; height: 188.734px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Revisar Contenido de la Página Web en busca de Fugas de Información</span></p>
    </td>
    <td style=""height: 188.734px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar comentarios y metadatos de la página web en busca de posibles fugas de información.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Recopilar archivos JavaScript y revisar el código JS para entender mejor la aplicación y encontrar posibles fugas de información.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar si existen archivos de mapas de origen u otros archivos de depuración de front-end.</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 188.734px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info05Status}}</span></td>
    <td style=""text-align: center; height: 188.734px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info05Note}}</span></td>
    </tr>
    <tr style=""height: 76.7812px"">
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INFO-06</span></td>
    <td style=""text-align: center; height: 76.7812px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Identificar Puntos de Entrada de la Aplicación</span></p>
    </td>
    <td style=""height: 76.7812px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar posibles puntos de entrada e inyección mediante el análisis de solicitudes y respuestas.</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info06Status}}</span></td>
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info06Note}}</span></td>
    </tr>
    <tr style=""height: 76.7812px"">
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INFO-07</span></td>
    <td style=""text-align: center; height: 76.7812px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Mapear Rutas de Ejecución a través de la Aplicación</span></p>
    </td>
    <td style=""height: 76.7812px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Mapear la aplicación objetivo y comprender los flujos de trabajo principales.</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info07Status}}</span></td>
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info07Note}}</span></td>
    </tr>
    <tr style=""height: 76.7812px"">
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INFO-08</span></td>
    <td style=""text-align: center; height: 76.7812px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Huellas del Marco de Aplicación Web</span></p>
    </td>
    <td style=""height: 76.7812px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Huellas de los componentes utilizados por las aplicaciones web.</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info08Status}}</span></td>
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info08Note}}</span></td>
    </tr>
    <tr style=""height: 76.7812px"">
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INFO-09</span></td>
    <td style=""text-align: center; height: 76.7812px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Fingerprint Web Application</span></p>
    </td>
    <td style=""height: 76.7812px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Huellas de los componentes utilizados por las aplicaciones web.</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info09Status}}</span></td>
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info09Note}}</span></td>
    </tr>
    <tr style=""height: 76.7812px"">
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INFO-10</span></td>
    <td style=""text-align: center; height: 76.7812px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Mapear Arquitectura de la Aplicación<br></span></p>
    </td>
    <td style=""height: 76.7812px; text-align: left"">
    <ul>
    <li>
    <div>
    <div>
    <div>
    <div>
    <div>
    <div>
    <div>
    <div>
    <div>
    <div>
    <div>
    <div>
    <p><span style=""color: rgba(0, 0, 0, 1)"">Generar un mapa de la aplicación en cuestión basado en la investigación realizada.</span></p>
    </div>
    </div>
    </div>
    <div>&nbsp;</div>
    </div>
    </div>
    </div>
    </div>
    </div>
    </div>
    </div>
    </div>
    </div>
    <div>&nbsp;</div>
    </li>
    </ul>
    </td>
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info10Status}}</span></td>
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Info10Note}}</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%; height: 268.687px"" border=""1""><colgroup><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 19.9605%""></colgroup>
    <tbody>
    <tr style=""height: 44.7812px"">
    <td style=""background-color: rgba(0, 0, 0, 1); text-align: center; height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Pruebas de Gestión de Configuración e Implementación<br></span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Objetivos</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CONF-01</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Configuración de Infraestructura de Red&nbsp;</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">&nbsp;Revisar las configuraciones de las aplicaciones establecidas en toda la red y validar que no sean vulnerables.&nbsp;</span></li>
    </ul>
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">&nbsp;Asegurarse de que los marcos y sistemas utilizados sean seguros y no susceptibles a vulnerabilidades conocidas debido a software no mantenido o configuraciones y credenciales predeterminadas.</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf01Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf01Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CONF-02</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Configuración de Plataforma de Aplicación&nbsp;</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Garantizar que se hayan eliminado las configuraciones predeterminadas y los archivos conocidos.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Validar que no quede código de depuración ni extensiones en los entornos de producción.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar los mecanismos de registro establecidos para la aplicación.&nbsp;</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf02Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf02Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CONF-03</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Manejo de Extensiones de Archivos para Información Sensible&nbsp;</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Realizar pruebas de directorios para extensiones de archivos sensibles o extensiones que puedan contener datos en bruto (por ejemplo, scripts, datos en bruto, credenciales, etc.).</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Validar que no existan omisiones de framework del sistema en las reglas establecidas.</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf03Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf03Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CONF-04</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Revisar Antiguas Copias de Seguridad y Archivos no Referenciados en busca de Información Sensible&nbsp;</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Encontrar y analizar archivos no referenciados que puedan contener información sensible.</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf04Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf04Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CONF-05</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Enumerar Interfaces de Administración de Infraestructura y Aplicación&nbsp;</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar interfaces y funcionalidades de administrador ocultas.&nbsp;</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf05Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf05Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CONF-06</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Métodos HTTP&nbsp;</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Enumerar los métodos HTTP admitidos.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Probar el bypass del control de acceso.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Probar vulnerabilidades de XST</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Probar técnicas de anulación de métodos HTTP.</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf06Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf06Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CONF-07</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Strict Transport Security (HSTS) HTTP&nbsp;</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar la cabecera HSTS y su validez.&nbsp;</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf07Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf07Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CONF-08</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Política RIA Cross Domain&nbsp;</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar y validar los archivos de directivas</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf08Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf08Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CONF-09</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Permisos de Archivos&nbsp;</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar e identificar permisos de archivos no autorizados.</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf09Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf09Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CONF-10</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Subdomain Takeover</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Enumerar todos los dominios posibles (anteriores y actuales).</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar dominios olvidados o mal configurados.</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf10Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf10Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CONF-11</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Almacenamiento en la Nube<br></span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar que la configuración de control de acceso para los servicios de almacenamiento esté correctamente establecida.</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf11Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Conf11Note}}</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 19.9605%""></colgroup>
    <tbody>
    <tr>
    <td style=""background-color: rgba(0, 0, 0, 1); text-align: center; height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Identity Management Testing</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Objetivos</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-IDNT-01</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Probar Definiciones de Roles&nbsp;</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar y documentar los roles utilizados por la aplicación.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Intentar cambiar, modificar o acceder a otro rol.&nbsp;</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar la granularidad de los roles y las necesidades detrás de los permisos otorgados.</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Idnt01Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Idnt01Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-IDNT-02</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Probar Proceso de Registro de Usuarios&nbsp;</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Verificar que los requisitos de identidad para el registro de usuarios estén alineados con los requisitos comerciales y de seguridad</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Validar el proceso de registro</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Idnt02Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Idnt02Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-IDNT-03</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Probar Proceso de Provisionamiento de Cuentas&nbsp;</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Determinar qué cuentas pueden aprovisionar otras cuentas y de qué tipo son.</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Idnt03Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Idnt03Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-IDNT-04</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Enumeración de Cuentas y Cuentas de Usuario Adivinables&nbsp;</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar los procesos relacionados con la identificación del usuario (por ejemplo, registro, inicio de sesión, etc.)</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">&nbsp;Enumerar usuarios cuando sea posible mediante análisis de respuestas.</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Idnt04Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Idnt04Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-IDNT-05</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Política de Nombre de Usuario Débil o No Aplicada<br></span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Determinar si una estructura de nombres de cuenta consistente hace que la aplicación sea vulnerable a la enumeración de cuentas</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Determinar si los mensajes de error de la aplicación permiten la enumeración de cuentas</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Idnt05Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Idnt05Note}}</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%; height: 246.281px"" border=""1""><colgroup><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 19.9605%""></colgroup>
    <tbody>
    <tr style=""height: 44.7656px"">
    <td style=""background-color: rgba(0, 0, 0, 1); text-align: center; height: 44.7656px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Authentication Testing</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Objetivos</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-ATHN-01</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Credenciales Transportadas a través de un Canal Cifrado</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar si algún caso de uso del sitio web o aplicación provoca que el servidor o el cliente intercambien credenciales sin cifrado</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn01Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn01Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-ATHN-02</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Credenciales Predeterminadas</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Enumerar las aplicaciones en busca de credenciales predeterminadas y validar si aún existen.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar y evaluar las cuentas de usuario nuevas y si se crean con algún valor predeterminado o patrones identificables</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn02Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn02Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-ATHN-03</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Mecanismo de Bloqueo Débil</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar la resistencia del mecanismo de desbloqueo a desbloqueos no autorizados de cuentas. Asegurarse de que la autenticación se aplique en todos los servicios que la requieran.</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn03Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn03Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-ATHN-04</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de bypass del Esquema de Autenticación</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Asegurarse de que la autenticación se aplique en todos los servicios que la requieran.</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn04Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn04Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-ATHN-05</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Recordar Contraseña Vulnerable</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Validar que la sesión generada se gestione de manera segura y no ponga en peligro las credenciales del usuario</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn05Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn05Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-ATHN-06</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Debilidades en la Caché del Navegador</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar si la aplicación almacena información sensible en el lado del cliente</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar si el acceso puede ocurrir sin autorización</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn06Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn06Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-ATHN-07</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Política de Contraseña Débil</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Determinar la resistencia de la aplicación contra el adivinado de contraseñas mediante fuerza bruta utilizando diccionarios de contraseñas disponibles, evaluando la longitud, complejidad, reutilización y requisitos de envejecimiento de las contraseñas</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn07Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn07Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-ATHN-08</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Respuestas Débiles a Preguntas de Seguridad</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Determinar la complejidad y lo directas que son las preguntas</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar posibles respuestas del usuario y capacidades de fuerza bruta</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn08Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn08Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-ATHN-09</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Funcionalidades Débiles de Cambio o Restablecimiento de Contraseñas</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Determinar la resistencia de la aplicación a la subversión del proceso de cambio de cuenta que permita a alguien cambiar la contraseña de una cuenta</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Determinar la resistencia de la funcionalidad de restablecimiento de contraseñas contra el adivinado o eludirlo</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn09Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn09Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-ATHN-10</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Autenticación Más Débil en un Canal Alternativo</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar canales de autenticación alternativos</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar las medidas de seguridad utilizadas y si existen elusiones en los canales alternativos.</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn10Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athn10Note}}</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 19.9605%""></colgroup>
    <tbody>
    <tr>
    <td style=""background-color: rgba(0, 0, 0, 1); text-align: center; height: 44.7656px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Authorization Testing&nbsp;</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Objetivos</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-ATHZ-01</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Inclusión de archivos y transversal de directorios</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar puntos de inyección relacionados&nbsp;</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar técnicas de elusión e identificar la extensión de la travesía de directorios</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athz01Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athz01Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-ATHZ-02</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Elusión de Esquema de Autorización</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar si es posible el acceso horizontal o vertical</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athz02Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athz02Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-ATHZ-03</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Elevación de Privilegios</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar puntos de inyección relacionados con la manipulación de privilegios</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Realizar pruebas de fuzzing u otros intentos para eludir medidas de seguridad</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athz03Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athz03Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-ATHZ-04</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Referencias Directas a Objetos No Seguras</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar puntos donde pueden ocurrir referencias a objetos.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar las medidas de control de acceso y si son vulnerables a IDOR (Referencias Directas a Objetos No Seguras)</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athz04Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Athz04Note}}</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%; height: 1001.2px"" border=""1""><colgroup><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 19.9605%""></colgroup>
    <tbody>
    <tr style=""height: 44.75px"">
    <td style=""background-color: rgba(0, 0, 0, 1); text-align: center; height: 44.75px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Session Management Testing</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Objetivos</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr style=""height: 201.516px"">
    <td style=""text-align: center; height: 201.516px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-SESS-01</span></td>
    <td style=""text-align: center; height: 201.516px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Gestión de Sesiones</span></p>
    </td>
    <td style=""height: 201.516px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Recopilar tokens de sesión, tanto para el mismo usuario como para diferentes usuarios cuando sea posible.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Analizar y asegurarse de que exista suficiente aleatoriedad para evitar ataques de falsificación de sesiones</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Modificar cookies que no estén firmadas y que contengan información que pueda manipularse</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 201.516px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess01Status}}</span></td>
    <td style=""text-align: center; height: 201.516px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess01Note}}</span></td>
    </tr>
    <tr style=""height: 76.7812px"">
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-SESS-02</span></td>
    <td style=""text-align: center; height: 76.7812px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Atributos de Cookies</span></p>
    </td>
    <td style=""height: 76.7812px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Asegurarse de que la configuración de seguridad adecuada esté establecida para las cookies.</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess02Status}}</span></td>
    <td style=""text-align: center; height: 76.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess02Note}}</span></td>
    </tr>
    <tr style=""height: 89.5625px"">
    <td style=""text-align: center; height: 89.5625px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-SESS-03</span></td>
    <td style=""text-align: center; height: 89.5625px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Fijación de Sesiones</span></p>
    </td>
    <td style=""height: 89.5625px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Analizar el mecanismo de autenticación y su flujo</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Forzar cookies y evaluar el impacto</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 89.5625px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess03Status}}</span></td>
    <td style=""text-align: center; height: 89.5625px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess03Note}}</span></td>
    </tr>
    <tr style=""height: 134.344px"">
    <td style=""text-align: center; height: 134.344px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-SESS-04</span></td>
    <td style=""text-align: center; height: 134.344px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Variables de Sesión Expuestas</span></p>
    </td>
    <td style=""height: 134.344px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Asegurarse de que se implemente el cifrado adecuado.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar la configuración de almacenamiento en caché</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar la seguridad del canal y los métodos</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 134.344px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess04Status}}</span></td>
    <td style=""text-align: center; height: 134.344px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess04Note}}</span></td>
    </tr>
    <tr style=""height: 89.5625px"">
    <td style=""text-align: center; height: 89.5625px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-SESS-05</span></td>
    <td style=""text-align: center; height: 89.5625px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Cross Site Request Forgery (CSRF)</span></p>
    </td>
    <td style=""height: 89.5625px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Determinar si es posible iniciar solicitudes en nombre de un usuario que no fueron iniciadas por el usuario</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 89.5625px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess05Status}}</span></td>
    <td style=""text-align: center; height: 89.5625px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess05Note}}</span></td>
    </tr>
    <tr style=""height: 89.5625px"">
    <td style=""text-align: center; height: 89.5625px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-SESS-06</span></td>
    <td style=""text-align: center; height: 89.5625px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Funcionalidad de Cierre de Sesión</span></p>
    </td>
    <td style=""height: 89.5625px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar la interfaz de cierre de sesión</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Analizar el tiempo de expiración de la sesión y si la sesión se elimina correctamente después del cierre de sesión</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 89.5625px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess06Status}}</span></td>
    <td style=""text-align: center; height: 89.5625px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess06Note}}</span></td>
    </tr>
    <tr style=""height: 54.3906px"">
    <td style=""text-align: center; height: 54.3906px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-SESS-07</span></td>
    <td style=""text-align: center; height: 54.3906px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Tiempo de Expiración de Sesión</span></p>
    </td>
    <td style=""height: 54.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Validar que exista un tiempo de expiración de sesión estricto</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 54.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess07Status}}</span></td>
    <td style=""text-align: center; height: 54.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess07Note}}</span></td>
    </tr>
    <tr style=""height: 99.1719px"">
    <td style=""text-align: center; height: 99.1719px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-SESS-08</span></td>
    <td style=""text-align: center; height: 99.1719px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Puzzling de Sesiones</span></p>
    </td>
    <td style=""height: 99.1719px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar todas las variables de sesión</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Romper el flujo lógico de generación de sesión</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 99.1719px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess08Status}}</span></td>
    <td style=""text-align: center; height: 99.1719px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess08Note}}</span></td>
    </tr>
    <tr style=""height: 121.562px"">
    <td style=""text-align: center; height: 121.562px""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-SESS-09</span></td>
    <td style=""text-align: center; height: 121.562px"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Secuestro de Sesiones</span></p>
    </td>
    <td style=""height: 121.562px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar cookies de sesión vulnerables.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Secuestrar cookies vulnerables y evaluar el nivel de riesgo.</span></li>
    </ul>
    </td>
    <td style=""text-align: center; height: 121.562px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess09Status}}</span></td>
    <td style=""text-align: center; height: 121.562px""><span style=""color: rgba(0, 0, 0, 1)"">{{Sess09Note}}</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%; height: 2981.37px"" border=""1""><colgroup><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 19.9605%""></colgroup>
    <tbody>
    <tr style=""height: 44.75px"">
    <td style=""background-color: rgba(0, 0, 0, 1); text-align: center; height: 44.75px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Data Validation Testing</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Objetivos</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr style=""height: 166.344px"">
    <td style=""height: 166.344px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-01</span></td>
    <td style=""height: 166.344px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Cross Site Scripting Reflejado (Reflected XSS)</span></p>
    </td>
    <td style=""height: 166.344px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar variables que se reflejan en las respuestas.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar la entrada que aceptan y la codificación que se aplica en el retorno (si la hay)</span></li>
    </ul>
    </td>
    <td style=""height: 166.344px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv01Status}}</span></td>
    <td style=""height: 166.344px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv01Note}}</span></td>
    </tr>
    <tr style=""height: 166.344px"">
    <td style=""height: 166.344px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-02</span></td>
    <td style=""height: 166.344px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Cross Site Scripting Almacenado</span></p>
    </td>
    <td style=""height: 166.344px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar la entrada almacenada que se refleja en el lado del cliente</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar la entrada que aceptan y la codificación que se aplica en el retorno (si la hay)</span></li>
    </ul>
    </td>
    <td style=""height: 166.344px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv02Status}}</span></td>
    <td style=""height: 166.344px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv02Note}}</span></td>
    </tr>
    <tr style=""height: 188.734px"">
    <td style=""height: 188.734px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-03</span></td>
    <td style=""height: 188.734px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Manipulación de HTTP verbs</span></p>
    </td>
    <td style=""height: 188.734px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Enumerar los métodos HTTP admitidos</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Probar el bypass del control de acceso</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Probar vulnerabilidades de XST.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Probar técnicas de anulación de métodos HTTP</span></li>
    </ul>
    </td>
    <td style=""height: 188.734px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv03Status}}</span></td>
    <td style=""height: 188.734px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv03Note}}</span></td>
    </tr>
    <tr style=""height: 143.953px"">
    <td style=""height: 143.953px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-04</span></td>
    <td style=""height: 143.953px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Contaminación de Parámetros HTTP</span></p>
    </td>
    <td style=""height: 143.953px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar el backend y el método de parseo utilizado</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar los puntos de inyección y tratar de eludir los filtros de entrada utilizando HPP (HTTP Parameter Pollution).</span></li>
    </ul>
    </td>
    <td style=""height: 143.953px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv04Status}}</span></td>
    <td style=""height: 143.953px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv04Note}}</span></td>
    </tr>
    <tr style=""height: 166.344px"">
    <td style=""height: 166.344px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-05</span></td>
    <td style=""height: 166.344px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Inyección SQL</span></p>
    </td>
    <td style=""height: 166.344px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar puntos de inyección SQL</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar la gravedad de la inyección y el nivel de acceso que se puede lograr a través de ella</span></li>
    </ul>
    </td>
    <td style=""height: 166.344px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv05Status}}</span></td>
    <td style=""height: 166.344px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv05Note}}</span></td>
    </tr>
    <tr style=""height: 121.562px"">
    <td style=""height: 121.562px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-06</span></td>
    <td style=""height: 121.562px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Inyección LDAP</span></p>
    </td>
    <td style=""height: 121.562px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar puntos de inyección LDAP</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar la gravedad de la inyección</span></li>
    </ul>
    </td>
    <td style=""height: 121.562px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv06Status}}</span></td>
    <td style=""height: 121.562px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv06Note}}</span></td>
    </tr>
    <tr style=""height: 143.953px"">
    <td style=""height: 143.953px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-07</span></td>
    <td style=""height: 143.953px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Inyección XML</span></p>
    </td>
    <td style=""height: 143.953px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar puntos de inyección XML</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar los tipos de exploits que se pueden lograr y sus gravedades</span></li>
    </ul>
    </td>
    <td style=""height: 143.953px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv07Status}}</span></td>
    <td style=""height: 143.953px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv07Note}}</span></td>
    </tr>
    <tr style=""height: 99.1719px"">
    <td style=""height: 99.1719px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-08</span></td>
    <td style=""height: 99.1719px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Inyección SSI</span></p>
    </td>
    <td style=""height: 99.1719px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar puntos de inyección SSI</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar la gravedad de la inyección.</span></li>
    </ul>
    </td>
    <td style=""height: 99.1719px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv08Status}}</span></td>
    <td style=""height: 99.1719px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv08Note}}</span></td>
    </tr>
    <tr style=""height: 76.7812px"">
    <td style=""height: 76.7812px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-09</span></td>
    <td style=""height: 76.7812px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Inyección XPath</span></p>
    </td>
    <td style=""height: 76.7812px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar puntos de inyección XPATH.</span></li>
    </ul>
    </td>
    <td style=""height: 76.7812px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv09Status}}</span></td>
    <td style=""height: 76.7812px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv09Note}}</span></td>
    </tr>
    <tr style=""height: 188.734px"">
    <td style=""height: 188.734px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-10</span></td>
    <td style=""height: 188.734px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Inyección IMAP SMTP</span></p>
    </td>
    <td style=""height: 188.734px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar puntos de inyección IMAP/SMTP.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Comprender el flujo de datos y la estructura de implementación del sistema</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar los impactos de la inyección</span></li>
    </ul>
    </td>
    <td style=""height: 188.734px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv10Status}}</span></td>
    <td style=""height: 188.734px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv10Note}}</span></td>
    </tr>
    <tr style=""height: 143.953px"">
    <td style=""height: 143.953px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-11</span></td>
    <td style=""height: 143.953px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Inyección de Código</span></p>
    </td>
    <td style=""height: 143.953px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar puntos de inyección donde se puede insertar código en la aplicación</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar la gravedad de la inyección</span></li>
    </ul>
    </td>
    <td style=""height: 143.953px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv11Status}}</span></td>
    <td style=""height: 143.953px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv11Note}}</span></td>
    </tr>
    <tr style=""height: 76.7812px"">
    <td style=""height: 76.7812px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-12</span></td>
    <td style=""height: 76.7812px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Inyección de Comandos</span></p>
    </td>
    <td style=""height: 76.7812px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar y evaluar los puntos de inyección de comandos</span></li>
    </ul>
    </td>
    <td style=""height: 76.7812px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv12Status}}</span></td>
    <td style=""height: 76.7812px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv12Note}}</span></td>
    </tr>
    <tr style=""height: 166.344px"">
    <td style=""height: 166.344px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-13</span></td>
    <td style=""height: 166.344px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Inyección de Cadenas de Formato</span></p>
    </td>
    <td style=""height: 166.344px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar si la inyección de especificadores de conversión de cadena de formato en campos controlados por el usuario provoca un comportamiento no deseado de la aplicación.</span></li>
    </ul>
    </td>
    <td style=""height: 166.344px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv13Status}}</span></td>
    <td style=""height: 166.344px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv13Note}}</span></td>
    </tr>
    <tr style=""height: 188.734px"">
    <td style=""height: 188.734px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-14</span></td>
    <td style=""height: 188.734px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Vulnerabilidad Incubada</span></p>
    </td>
    <td style=""height: 188.734px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar inyecciones almacenadas que requieren un paso de recuperación de la inyección almacenada.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Comprender cómo podría ocurrir un paso de recuperación</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Establecer escuchas o activar el paso de recuperación si es posible.</span></li>
    </ul>
    </td>
    <td style=""height: 188.734px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv14Status}}</span></td>
    <td style=""height: 188.734px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv14Note}}</span></td>
    </tr>
    <tr style=""height: 233.516px"">
    <td style=""height: 233.516px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-15</span></td>
    <td style=""height: 233.516px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Splitting y Smuggling HTTP</span></p>
    </td>
    <td style=""height: 233.516px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar si la aplicación es vulnerable a splitting, identificando qué posibles ataques son alcanzables.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar si la cadena de comunicación es vulnerable a smuggling, identificando qué posibles ataques son alcanzables.</span></li>
    </ul>
    </td>
    <td style=""height: 233.516px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv15Status}}</span></td>
    <td style=""height: 233.516px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv15Note}}</span></td>
    </tr>
    <tr style=""height: 211.125px"">
    <td style=""height: 211.125px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-16</span></td>
    <td style=""height: 211.125px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Solicitudes HTTP Entrantes</span></p>
    </td>
    <td style=""height: 211.125px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Monitorear todas las solicitudes HTTP entrantes y salientes al servidor web para inspeccionar cualquier solicitud sospechosa.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Monitorear el tráfico HTTP sin cambios en el proxy del navegador del usuario final o en la aplicación del lado del cliente</span></li>
    </ul>
    </td>
    <td style=""height: 211.125px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv16Status}}</span></td>
    <td style=""height: 211.125px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv16Note}}</span></td>
    </tr>
    <tr style=""height: 143.953px"">
    <td style=""height: 143.953px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-17</span></td>
    <td style=""height: 143.953px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Inyección de Cabecera de Host</span></p>
    </td>
    <td style=""height: 143.953px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar si el encabezado Host se analiza dinámicamente en la aplicación.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Eludir controles de seguridad que dependen del encabezado.</span></li>
    </ul>
    </td>
    <td style=""height: 143.953px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv17Status}}</span></td>
    <td style=""height: 143.953px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv17Note}}</span></td>
    </tr>
    <tr style=""height: 143.953px"">
    <td style=""height: 143.953px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-18</span></td>
    <td style=""height: 143.953px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Inyección de Plantillas del Lado del Servidor</span></p>
    </td>
    <td style=""height: 143.953px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Detectar puntos de vulnerabilidad de inyección de plantillas.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar el motor de plantillas.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Construir el exploit</span></li>
    </ul>
    </td>
    <td style=""height: 143.953px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv18Status}}</span></td>
    <td style=""height: 143.953px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv18Note}}</span></td>
    </tr>
    <tr style=""height: 166.344px"">
    <td style=""height: 166.344px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-INPV-19</span></td>
    <td style=""height: 166.344px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Falsificación de Solicitudes del Lado del Servidor (SSRF)</span></p>
    </td>
    <td style=""height: 166.344px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar puntos de inyección SSRF</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Probar si los puntos de inyección son explotables</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar la gravedad de la vulnerabilidad</span></li>
    </ul>
    </td>
    <td style=""height: 166.344px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv19Status}}</span></td>
    <td style=""height: 166.344px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Inpv19Note}}</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""></colgroup>
    <tbody>
    <tr>
    <td style=""background-color: rgba(0, 0, 0, 1); text-align: center; height: 44.75px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Error Handling</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Objetivos</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-ERRH-01</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Manejo Incorrecto de Errores</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar la salida de errores existente</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Analizar la variedad de salidas devueltas.</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Errh01Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Errh01Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-ERRH-02</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Rastros de Pila (Stack Traces)</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar la salida de errores existente</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Analizar la variedad de salidas devueltas.</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Errh02Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Errh02Note}}</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""></colgroup>
    <tbody>
    <tr>
    <td style=""background-color: rgba(0, 0, 0, 1); text-align: center; height: 44.75px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Cryptography</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Objetivos</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CRYP-01</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Seguridad Débil en la Capa de Transporte (TLS)</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Validar la configuración del servicio.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar la fuerza criptográfica y validez del certificado digital</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Asegurarse de que la seguridad de TLS no se pueda eludir y esté implementada correctamente en toda la aplicación</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Cryp01Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Cryp01Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CRYP-02</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Padding Oracle</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">IIdentificar mensajes cifrados que dependan del relleno</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Intentar romper el relleno de los mensajes cifrados y analizar los mensajes de error devueltos para un análisis adicional</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Cryp02Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Cryp02Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CRYP-03</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Envío de Información Sensible a través de Canales no Cifrados</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar información sensible transmitida a través de varios canales</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar la privacidad y seguridad de los canales utilizados</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Cryp03Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Cryp03Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CRYP-04</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Cifrado Débil</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Proporcionar una guía para la identificación de usos e implementaciones débiles de cifrado o hash</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Cryp04Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Cryp04Note}}</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""></colgroup>
    <tbody>
    <tr>
    <td style=""background-color: rgba(0, 0, 0, 1); text-align: center; height: 44.75px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Business logic Testing</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Objetivos</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-BUSL-01</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Validación de Datos de la Lógica de Negocio</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar puntos de inyección de datos</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Validar que todas las comprobaciones se realizan en el backend y no pueden eludirse</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Intentar romper el formato de los datos esperados y analizar cómo la aplicación lo maneja</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl01Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl01Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-BUSL-02</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Capacidad para Falsificar Peticiones</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar la documentación del proyecto en busca de funcionalidades adivinables, predecibles o ocultas en los campos</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Insertar datos lógicamente válidos para eludir el flujo de trabajo normal de la lógica de negocios</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl02Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl02Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-BUSL-03</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Controles de Integridad</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar la documentación del proyecto para identificar componentes del sistema que mueven, almacenan o manipulan datos</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Determinar qué tipo de datos es lógicamente aceptable para el componente y contra qué tipos debería protegerse el sistema</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Determinar quién debería estar autorizado para modificar o leer esos datos en cada componente</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Intentar insertar, actualizar o eliminar valores de datos utilizados por cada componente que no deberían permitirse según el flujo de trabajo de la lógica de negocios.</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl03Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl03Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-BUSL-04</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Temporización de Procesos</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar la documentación del proyecto para identificar funcionalidades del sistema que pueden verse afectadas por el tiempo</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Desarrollar y ejecutar casos de mal uso.</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl04Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl04Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-BUSL-05</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Límites en la Cantidad de Veces que una Función Puede Ser Utilizada</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar funciones que deben establecer límites en las veces que pueden ser llamadas.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar si hay un límite lógico establecido en las funciones y si se valida correctamente.</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl05Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl05Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-BUSL-06</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de la Circunvención de Flujos de Trabajo</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar la documentación del proyecto en busca de métodos para omitir o pasar por alto pasos en el proceso de la aplicación en un orden diferente al flujo lógico de la lógica de negocios previsto</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Desarrollar un caso de mal uso e intentar eludir cada flujo lógico identificado</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl06Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl06Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-BUSL-07</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Defensas contra el Mal Uso de la Aplicación</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Generar notas de todas las pruebas realizadas contra el sistema</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar qué pruebas tuvieron una funcionalidad diferente basada en una entrada agresiva</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Comprender las defensas implementadas y verificar si son suficientes para proteger el sistema contra técnicas de elusión.</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl07Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl07Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-BUSL-08</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Subida de Tipos de Archivos Inesperados</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar la documentación del proyecto para identificar los tipos de archivos que son rechazados por el sistema.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Verificar que los tipos de archivos no deseados sean rechazados y manejados de manera segura.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Verificar que las cargas masivas de archivos sean seguras y no permitan eludir las medidas de seguridad establecidas.</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl08Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl08Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-BUSL-09</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Subida de Archivos Maliciosos</span></p>
    </td>
    <td style=""text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar la funcionalidad de carga de archivos</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Revisar la documentación del proyecto para identificar qué tipos de archivos se consideran aceptables y cuáles se considerarían peligrosos o maliciosos.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Determinar cómo se procesan los archivos cargados.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Obtener o crear un conjunto de archivos maliciosos para las pruebas.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Intentar cargar los archivos maliciosos en la aplicación y determinar si se aceptan y procesan</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl09Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Busl09Note}}</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%; height: 470.172px"" border=""1""><colgroup><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""></colgroup>
    <tbody>
    <tr style=""height: 44.75px"">
    <td style=""background-color: rgba(0, 0, 0, 1); text-align: center; height: 44.75px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Client Side Testing</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Objetivos</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr style=""height: 67.1719px"">
    <td style=""height: 67.1719px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CLNT-01</span></td>
    <td style=""height: 67.1719px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de DOM-Based Cross Site Scripting</span></p>
    </td>
    <td style=""height: 67.1719px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar fuentes de DOM (DOM sinks)</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Crear payloads que se relacionen con cada tipo de fuente.&nbsp;</span></li>
    </ul>
    </td>
    <td style=""height: 67.1719px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt01Status}}</span></td>
    <td style=""height: 67.1719px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt01Note}}</span></td>
    </tr>
    <tr style=""height: 44.7812px"">
    <td style=""height: 44.7812px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CLNT-02</span></td>
    <td style=""height: 44.7812px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Ejecución de JavaScript</span></p>
    </td>
    <td style=""height: 44.7812px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar fuentes y posibles puntos de inyección de JavaScript</span></li>
    </ul>
    </td>
    <td style=""height: 44.7812px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt02Status}}</span></td>
    <td style=""height: 44.7812px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt02Note}}</span></td>
    </tr>
    <tr style=""height: 67.1719px"">
    <td style=""height: 67.1719px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CLNT-03</span></td>
    <td style=""height: 67.1719px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Inyección HTML</span></p>
    </td>
    <td style=""height: 67.1719px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar puntos de inyección de HTML y evaluar la gravedad del contenido inyectado.&nbsp;</span></li>
    </ul>
    </td>
    <td style=""height: 67.1719px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt03Status}}</span></td>
    <td style=""height: 67.1719px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt03Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CLNT-04</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Redirección de URL del Lado del Cliente</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar puntos de inyección que manejen URL o rutas.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar las ubicaciones a las que el sistema podría redirigir</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt04Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt04Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CLNT-05</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Inyección de CSS</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar puntos de inyección de CSS.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar el impacto de la inyección</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt05Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt05Note}}</span></td>
    </tr>
    <tr style=""height: 44.7812px"">
    <td style=""height: 44.7812px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CLNT-06</span></td>
    <td style=""height: 44.7812px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Manipulación de Recursos del Lado del Cliente</span></p>
    </td>
    <td style=""height: 44.7812px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar fuentes con validación débil de la entrada</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar el impacto de la manipulación de recursos.</span></li>
    </ul>
    </td>
    <td style=""height: 44.7812px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt06Status}}</span></td>
    <td style=""height: 44.7812px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt06Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CLNT-07</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Cross Origin Resource Sharing</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">dentificar puntos finales que implementen CORS</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Asegurarse de que la configuración de CORS sea segura</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt07Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt07Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CLNT-08</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Cross Site Flashing</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Descompilar y analizar el código de la aplicación</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar las entradas de las fuentes y los usos de métodos no seguros.</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt08Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt08Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CLNT-09</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Clickjacking</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Comprender las medidas de seguridad implementadas</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar qué tan estrictas son las medidas de seguridad y si se pueden eludir.</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt09Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt09Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CLNT-10</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de WebSockets</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Identificar el uso de WebSockets</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar su implementación utilizando las mismas pruebas en canales HTTP normales</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt10Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt10Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CLNT-11</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Mensajería Web</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar la seguridad del origen del mensaje.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Validar que esté utilizando métodos seguros y validando su entrada.</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt11Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt11Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CLNT-12</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Almacenamiento del Navegador</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Determinar si el sitio web almacena datos sensibles en el almacenamiento del lado del cliente.</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">El código que maneja los objetos de almacenamiento debe examinarse en busca de posibilidades de ataques de inyección, como utilizar entrada no validada o bibliotecas vulnerables.</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt12Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt12Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-CLNT-13</span></td>
    <td style=""height: 22.3906px; text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Cross Site Script Inclusion</span></p>
    </td>
    <td style=""height: 22.3906px; text-align: left"">
    <ul>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Localizar datos sensibles en todo el sistema</span></li>
    <li><span style=""color: rgba(0, 0, 0, 1)"">Evaluar la filtración de datos sensibles a través de diversas técnicas.</span></li>
    </ul>
    </td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt13Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Clnt13Note}}</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%"" border=""1""><colgroup><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""><col style=""width: 20.0264%""></colgroup>
    <tbody>
    <tr>
    <td style=""background-color: rgba(0, 0, 0, 1); text-align: center; height: 44.75px""><strong><span style=""color: rgba(255, 255, 255, 1)"">API Testing</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Objetivos</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">WSTG-APIT-01</span></td>
    <td style=""text-align: center"">
    <p><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de GraphQL</span></p>
    </td>
    <td style=""text-align: center"">
    <ul>
    <li style=""text-align: left""><span style=""color: rgba(0, 0, 0, 1)"">Evaluar que se haya implementado una configuración segura y lista para producción</span></li>
    <li style=""text-align: left""><span style=""color: rgba(0, 0, 0, 1)"">Validar todos los campos de entrada contra ataques genéricos.</span></li>
    <li style=""text-align: left""><span style=""color: rgba(0, 0, 0, 1)"">Asegurarse de que se apliquen controles de acceso adecuados</span></li>
    </ul>
    </td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Apit01Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Apit01Note}}</span></td>
    </tr>
    </tbody>
    </table>
    <h6 style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Lista de verificación de pruebas de OWASP. A continuación se presenta la lista de elementos para evaluar durante la evaluación: (La columna de Estado se establece en los valores ""Éxito"", ""Problemas"" y ""N/A"")</span><br>&nbsp;<br>&nbsp;<br>&nbsp;</h6>";
                reportComponentsManager.Add(wstgResultados);
                reportComponentsManager.Context.SaveChanges();
                #endregion
                
                #region OWASP MASTG

                ReportComponents mastgPortada = new ReportComponents();
                mastgPortada.Id = Guid.NewGuid();
                mastgPortada.Name = "OWASP MASTG - Portada";
                mastgPortada.Language = Language.Español;
                mastgPortada.ComponentType = ReportPartType.Cover;
                mastgPortada.Created = DateTime.Now.ToUniversalTime();
                mastgPortada.Updated = DateTime.Now.ToUniversalTime();
                mastgPortada.ContentCss = "";
                mastgPortada.Content = @"<p>&nbsp;</p>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none"" border=""1""><colgroup><col style=""width: 51.8049%""><col style=""width: 48.1951%""></colgroup>
                <tbody>
                <tr>
                <td style=""border-style: none; text-align: center""><img style=""float: left"" src=""data:image/png;base64,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"" width=""250"" height=""87""></td>
                <td style=""border-style: none""><img style=""float: right"" src=""data:image/png;base64,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"" width=""270"" height=""54""></td>
                </tr>
                </tbody>
                </table>
                <h1 style=""text-align: center"">&nbsp;</h1>
                <p>&nbsp;</p>
                <h1 style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Informe OWASP MASTG</span></h1>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none"" border=""1""><colgroup><col style=""width: 50%""><col style=""width: 50%""></colgroup>
                <tbody>
                <tr>
                <td style=""border-style: none"">
                <p><span style=""color: rgba(0, 0, 0, 1)""><strong>Organización: {{OrganizationName}}</strong></span></p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">Email de Contacto: {{OrganizationEmail}}</span></p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">Teléfono de Contacto: {{OrganizationPhone}}</span></p>
                </td>
                <td style=""border-style: none"">
                <p style=""text-align: right""><span style=""color: rgba(0, 0, 0, 1)""><strong>Cliente: {{ClientName}}</strong></span></p>
                <p style=""text-align: right""><span style=""color: rgba(0, 0, 0, 1)"">Email del Cliente: {{ClientEmail}}</span></p>
                <p style=""text-align: right""><span style=""color: rgba(0, 0, 0, 1)"">Teléfono del Cliente: {{ClientPhone}}</span></p>
                </td>
                </tr>
                </tbody>
                </table>
                <p>&nbsp;</p>
                <table style=""border-collapse: collapse; width: 100%; border-width: 1px; border-style: none"" border=""1""><colgroup><col style=""width: 50%""><col style=""width: 50%""></colgroup>
                <tbody>
                <tr>
                <td style=""text-align: center; border-style: none""><span style=""color: rgba(0, 0, 0, 1)""><strong>Url</strong>: {{TargetName}}</span></td>
                <td style=""text-align: center; border-style: none""><span style=""color: rgba(0, 0, 0, 1)""><strong>Fecha de Creación</strong>: {{CreatedDate}}</span></td>
                </tr>
                </tbody>
                </table>
                <div style=""break-after: page"">&nbsp;</div>";
                reportComponentsManager.Add(mastgPortada);
                reportComponentsManager.Context.SaveChanges();

                ReportComponents mastgCabecera = new ReportComponents();
                mastgCabecera.Id = Guid.NewGuid();
                mastgCabecera.Name = "OWASP MASTG - Cabecera";
                mastgCabecera.Language = Language.Español;
                mastgCabecera.ComponentType = ReportPartType.Header;
                mastgCabecera.Created = DateTime.Now.ToUniversalTime();
                mastgCabecera.Updated = DateTime.Now.ToUniversalTime();
                mastgCabecera.ContentCss = "";
                mastgCabecera.Content = @"";
                reportComponentsManager.Add(mastgCabecera);
                reportComponentsManager.Context.SaveChanges();
                
                ReportComponents mastgIntroduccion = new ReportComponents();
                mastgIntroduccion.Id = Guid.NewGuid();
                mastgIntroduccion.Name = "OWASP MASTG - Introducción";
                mastgIntroduccion.Language = Language.Español;
                mastgIntroduccion.ComponentType = ReportPartType.Body;
                mastgIntroduccion.Created = DateTime.Now.ToUniversalTime();
                mastgIntroduccion.Updated = DateTime.Now.ToUniversalTime();
                mastgIntroduccion.ContentCss = "";
                mastgIntroduccion.Content = @"<h1><span style=""color: rgba(0, 0, 0, 1)"">Introducción</span></h1>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">{{ClientName}}, nos complace presentarle el informe del Proyecto de Pruebas de Seguridad de Aplicaciones Móviles (MASTG) del Proyecto de Seguridad de Aplicaciones Web de Código Abierto (OWASP) como resultado de nuestro reciente compromiso de pruebas de penetración en su infraestructura digital. El informe MASTG de OWASP proporciona un examen integral de su aplicación desde una perspectiva de seguridad.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">A lo largo de nuestro compromiso, hemos utilizado el MASTG de OWASP, una metodología estándar de la industria para pruebas robustas de seguridad de aplicaciones web. Nuestro equipo de probadores de penetración altamente capacitados ha llevado a cabo una evaluación sistemática de sus aplicaciones web para identificar cualquier vulnerabilidad existente, amenaza o debilidad que pueda ser explotada por partes malintencionadas.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">Es importante tener en cuenta que este informe proporciona una visión general de las vulnerabilidades identificadas. Las recomendaciones dadas en este informe están diseñadas para ayudar a su organización a minimizar el riesgo de futuros incidentes de seguridad, en vista de las amenazas cibernéticas en constante evolución.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">Le pedimos que revise este informe detenidamente y nos informe si hay áreas que le gustaría discutir más a fondo o si necesita aclaraciones adicionales.</span></p>
                <p>&nbsp;</p>
                <p><span style=""color: rgba(0, 0, 0, 1)"">Esperamos sus comentarios y estamos ansiosos por ayudarle a fortalecer su perfil de seguridad informática. La ciberseguridad es un viaje, no un destino, y estamos comprometidos a ser su socio de confianza en este camino.</span></p>
                <p>&nbsp;</p>
                <p>&nbsp;</p>";
                reportComponentsManager.Add(mastgIntroduccion);
                reportComponentsManager.Context.SaveChanges();
                
                 ReportComponents mastgAndroid = new ReportComponents();
                 mastgAndroid.Id = Guid.NewGuid();
                 mastgAndroid.Name = "OWASP MASTG - Resultados (Android)";
                 mastgAndroid.Language = Language.Español;
                 mastgAndroid.ComponentType = ReportPartType.Body;
                 mastgAndroid.Created = DateTime.Now.ToUniversalTime();
                 mastgAndroid.Updated = DateTime.Now.ToUniversalTime();
                    mastgAndroid.ContentCss = "";
                 mastgAndroid.Content = @"<h2>Resultados</h2>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%; height: 268.687px"" border=""1""><colgroup><col style=""width: 11.9895%""><col style=""width: 45.5204%""><col style=""width: 4.47958%""><col style=""width: 4.21607%""><col style=""width: 5.07246%""><col style=""width: 14.6245%""><col style=""width: 14.0975%""></colgroup>
    <tbody>
    <tr style=""height: 44.7812px"">
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Almacenamiento</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L1</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L2</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">R</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px"" rowspan=""3""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-STORAGE-1</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación almacena de manera segura datos sensibles.</span></td>
    <td style=""height: 22.3906px; text-align: left; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage1Status}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage1Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas en el Almacenamiento Local (Local Storage) en busca de Datos Sensibles.</span></td>
    <td style=""height: 22.3906px; text-align: left; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage1Status1}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage1Note1}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de la Política de Seguridad de Acceso al Dispositivo</span></td>
    <td style=""height: 22.3906px; text-align: left"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage1Status2}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage1Note2}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 156.734px"" rowspan=""7""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-STORAGE-2</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación previene la fuga de datos sensibles.</span></td>
    <td style=""height: 22.3906px; text-align: left; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Status}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas en la Memoria en busca de Datos Sensibles.</span></td>
    <td style=""height: 22.3906px; text-align: left"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Status1}}&nbsp;</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Note1}}&nbsp;</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas en Copias de Seguridad en busca de Datos Sensibles.</span></td>
    <td style=""height: 22.3906px; text-align: left"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Status2}}&nbsp;&nbsp;</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Note2}}&nbsp;&nbsp;</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Revisar Registros en busca de Datos Sensibles.</span></td>
    <td style=""height: 22.3906px; text-align: left; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Status3}}&nbsp;&nbsp;</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Note3}}&nbsp;&nbsp;</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Determinar si Datos Sensibles se Comparten con Terceros a través de Notificaciones.</span></td>
    <td style=""height: 22.3906px; text-align: left; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">&nbsp;{{Storage2Status4}}&nbsp;</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">&nbsp;{{Storage2Note4}}&nbsp;</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Determinar si la Caché del Teclado está Desactivada para Campos de Entrada de Texto.</span></td>
    <td style=""height: 22.3906px; text-align: left; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Status5}}&nbsp;&nbsp;</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">&nbsp;{{Storage2Note5}}&nbsp;</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Determinar si Datos Sensibles se Comparten con Terceros a través de Servicios Integrados</span></td>
    <td style=""height: 22.3906px; text-align: left; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: left; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">&nbsp;{{Storage2Status6}}&nbsp;</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Note6}}&nbsp;&nbsp;</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%; height: 297px"" border=""1""><colgroup><col style=""width: 14.2951%""><col style=""width: 43.083%""><col style=""width: 4.28195%""><col style=""width: 4.54545%""><col style=""width: 4.80896%""><col style=""width: 14.7563%""><col style=""width: 14.2951%""></colgroup>
    <tbody>
    <tr style=""height: 44.75px"">
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.75px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Criptografía</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L1</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L2</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">R</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 89.5624px; text-align: center"" rowspan=""4""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-CRYPTO-1</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación utiliza criptografía actual fuerte y la aplica según las mejores prácticas de la industria.</span></td>
    <td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto1Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto1Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Criptografía Simétrica.</span></td>
    <td style=""height: 22.3906px; background-color: rgba(50, 153, 255, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(11, 186, 131, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto1Status1}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto1Note1}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Generación de Números Aleatorios.</span></td>
    <td style=""height: 22.3906px; background-color: rgba(50, 153, 255, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(11, 186, 131, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto1Status2}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto1Note2}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Configuración de Algoritmos Criptográficos Estándar</span></td>
    <td style=""height: 22.3906px; background-color: rgba(50, 153, 255, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(11, 186, 131, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto1Status3}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto1Note3}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 44.7812px; text-align: center"" rowspan=""2""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-CRYPTO-2</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación realiza la gestión de claves según las mejores prácticas de la industria.</span></td>
    <td style=""height: 22.3906px; background-color: rgba(206, 212, 217, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(206, 212, 217, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(206, 212, 217, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto2Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto2Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Propósitos de las Claves</span></td>
    <td style=""height: 22.3906px; background-color: rgba(50, 153, 255, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(11, 186, 131, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto2Status1}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto2Note1}}</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%; height: 161.016px"" border=""1""><colgroup><col style=""width: 14.2951%""><col style=""width: 43.083%""><col style=""width: 4.28195%""><col style=""width: 4.54545%""><col style=""width: 4.80896%""><col style=""width: 14.7563%""><col style=""width: 14.2951%""></colgroup>
    <tbody>
    <tr style=""height: 49.0625px"">
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 49.0625px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Autenticación y Autorización</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L1</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L2</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">R</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-AUTH-1</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación utiliza protocolos de autenticación y autorización seguros y sigue las mejores prácticas relevantes</span></td>
    <td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth1Status}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth1Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 67.1718px"" rowspan=""3""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-AUTH-2</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación asegura la autenticación local de manera segura según las mejores prácticas de la plataforma</span></td>
    <td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(206, 212, 217, 1); height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth2Status}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth2Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Confirmación de Credenciales.</span></td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""background-color: rgba(11, 186, 131, 1); text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth2Status1}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth2Note1}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Autenticación Biométrica</span></td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""background-color: rgba(11, 186, 131, 1); text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth2Status2}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth2Note2}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-AUTH-3</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación asegura operaciones sensibles con autenticación adicional</span></td>
    <td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth3Status}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth3Note}}</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%; height: 250.578px"" border=""1""><colgroup><col style=""width: 14.2951%""><col style=""width: 43.083%""><col style=""width: 4.28195%""><col style=""width: 4.54545%""><col style=""width: 4.80896%""><col style=""width: 14.7563%""><col style=""width: 14.2951%""></colgroup>
    <tbody>
    <tr style=""height: 49.0625px"">
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 49.0625px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Comunicaciones de Red</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L1</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L2</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">R</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr style=""height: 44.7812px"">
    <td style=""text-align: center; height: 134.344px"" rowspan=""5""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-NETWORK-1</span></td>
    <td style=""text-align: center; height: 44.7812px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación realiza la fijación de identidad para todos los puntos finales remotos bajo el control del desarrollador.</span></td>
    <td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 44.7812px"">&nbsp;</td>
    <td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 44.7812px"">&nbsp;</td>
    <td style=""text-align: center; height: 44.7812px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 44.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Status}}</span></td>
    <td style=""text-align: center; height: 44.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas del Proveedor de Seguridad.</span></td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Status1}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Note1}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Encriptación de Datos en la Red.</span></td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Status2}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Note2}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Configuración de TLS.</span></td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Status3}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Note3}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Verificación de Identidad del Punto Final</span></td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Status4}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Note4}}</span></td>
    </tr>
    <tr style=""height: 44.7812px"">
    <td style=""text-align: center; height: 67.1718px"" rowspan=""2""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-NETWORK-2</span></td>
    <td style=""text-align: center; height: 44.7812px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación realiza la fijación de identidad para todos los puntos finales remotos bajo el control del desarrollador.</span></td>
    <td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 44.7812px"">&nbsp;</td>
    <td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 44.7812px"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(206, 212, 217, 1); height: 44.7812px"">&nbsp;</td>
    <td style=""text-align: center; height: 44.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network2Status}}</span></td>
    <td style=""text-align: center; height: 44.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network2Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Almacenes de Certificados Personalizados y Fijación de Certificados</span></td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network2Status1}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network2Note1}}</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%; height: 250.578px"" border=""1""><colgroup><col style=""width: 14.2951%""><col style=""width: 43.083%""><col style=""width: 4.28195%""><col style=""width: 4.54545%""><col style=""width: 4.80896%""><col style=""width: 14.7563%""><col style=""width: 14.2951%""></colgroup>
    <tbody>
    <tr style=""height: 49.0625px"">
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 49.0625px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Interacción con la Plataforma</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L1</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L2</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">R</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 67.1718px"" rowspan=""6""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-PLATFORM-1</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación utiliza mecanismos IPC de manera segura.</span></td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Status}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Permisos de la Aplicación.</span></td>
    <td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center"">&nbsp;</td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Status1}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Note1}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Exposición de Funcionalidades Sensibles a través de IPC.</span></td>
    <td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center"">&nbsp;</td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Status2}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Note2}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Deep Links.</span></td>
    <td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center"">&nbsp;</td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Status3}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Note3}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Implementación Vulnerable de PendingIntent.</span></td>
    <td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center"">&nbsp;</td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Status4}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Note4}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Determinar si Datos Sensibles Almacenados se Han Expuesto a través de Mecanismos IPC</span></td>
    <td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center"">&nbsp;</td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Status5}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Note5}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center"" rowspan=""5""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-PLATFORM-2</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación utiliza WebViews de manera segura</span></td>
    <td style=""text-align: center; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Manejadores de Protocolo de WebViews</span></td>
    <td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center"">&nbsp;</td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Status1}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Note1}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Ejecución de JavaScript en WebViews</span></td>
    <td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center"">&nbsp;</td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Status2}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Note2}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Limpieza de WebViews</span></td>
    <td style=""text-align: center"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center"">&nbsp;</td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Status3}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Note3}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Objetos Java Expuestos a través de WebViews.</span></td>
    <td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center"">&nbsp;</td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Status4}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Note4}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center"" rowspan=""4""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-PLATFORM-3</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación utiliza la interfaz de usuario de manera segura.</span></td>
    <td style=""text-align: center; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform3Status}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform3Note}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Revisar si se Produce la Divulgación de Datos Sensibles a través de la Interfaz de Usuario.</span></td>
    <td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center"">&nbsp;</td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform3Status1}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform3Note1}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Encontrar Información Sensible en Capturas de Pantalla Generadas Automáticamente.</span></td>
    <td style=""text-align: center"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center"">&nbsp;</td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform3Status2}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform3Note2}}</span></td>
    </tr>
    <tr>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Ataques de Superposición</span></td>
    <td style=""text-align: center"">&nbsp;</td>
    <td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center"">&nbsp;</td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform3Status3}}</span></td>
    <td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform3Note3}}</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%; height: 340.14px"" border=""1""><colgroup><col style=""width: 14.2951%""><col style=""width: 43.083%""><col style=""width: 4.28195%""><col style=""width: 4.54545%""><col style=""width: 4.80896%""><col style=""width: 14.7563%""><col style=""width: 14.2951%""></colgroup>
    <tbody>
    <tr style=""height: 49.0625px"">
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 49.0625px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Calidad del Código</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L1</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L2</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">R</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-CODE-1</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación requiere una versión de plataforma actualizada.</span></td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code1Status}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code1Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 44.7812px"" rowspan=""2""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-CODE-2</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación tiene un mecanismo para hacer cumplir las actualizaciones de la aplicación.</span></td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code2Status}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code2Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Actualización Forzada</span></td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code2Status1}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code2Note1}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 44.7812px"" rowspan=""2""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-CODE-3</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación solo utiliza componentes de software sin vulnerabilidades conocidas.</span></td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code3Status}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code3Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Revisar Debilidades en Bibliotecas de Terceros.</span></td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code3Status1}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code3Note1}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 179.125px"" rowspan=""8""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-CODE-4</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación solo utiliza componentes de software sin vulnerabilidades conocidas.</span></td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Status}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Persistencia de Objetos.</span></td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Status1}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Note1}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Asegurarse de que las Funciones de Seguridad Gratuitas estén Activadas.</span></td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Status2}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Note2}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Almacenamiento Local para la Validación de Entradas.</span></td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Status3}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Note3}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Vulnerabilidades de Inyección.</span></td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Status4}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Note4}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Carga de URL en WebViews.</span></td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Status5}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Note5}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Bugs de Corrupción de Memoria.</span></td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Status6}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Note6}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Intenciones Implícitas</span></td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Status7}}</span></td>
    <td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Note7}}</span></td>
    </tr>
    </tbody>
    </table>
    <p>&nbsp;</p>
    <p>&nbsp;</p>
    <table style=""border-collapse: collapse; width: 100%; height: 384.921px"" border=""1""><colgroup><col style=""width: 14.2951%""><col style=""width: 43.083%""><col style=""width: 4.28195%""><col style=""width: 4.54545%""><col style=""width: 4.80896%""><col style=""width: 14.7563%""><col style=""width: 14.2951%""></colgroup>
    <tbody>
    <tr style=""height: 49.0625px"">
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 49.0625px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Resiliencia contra ingeniería inversa</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L1</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L2</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">R</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
    <td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 67.1718px; text-align: center"" rowspan=""3""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-RESILIENCE-1</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación valida la integridad de la plataforma.</span></td>
    <td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience1Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience1Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Detección de Emuladores.</span></td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(255, 168, 0, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience1Status1}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience1Note1}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Detección de Root</span></td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(255, 168, 0, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience1Status2}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience1Note2}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 89.5624px; text-align: center"" rowspan=""4""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-RESILIENCE-2</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación implementa mecanismos contra el manipuleo.</span></td>
    <td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience2Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience2Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Verificación de Integridad en Tiempo de Ejecución.</span></td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(255, 168, 0, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience2Status1}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience2Note1}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Verificación de Integridad de Archivos.</span></td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(255, 168, 0, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience2Status2}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience2Note2}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Asegurarse de que la Aplicación esté Firmada Correctamente</span></td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(255, 168, 0, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience2Status3}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience2Note3}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 89.5624px; text-align: center"" rowspan=""4""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-RESILIENCE-3</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación implementa mecanismos contra el análisis estático.</span></td>
    <td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience3Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience3Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Símbolos de Depuración.</span></td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(255, 168, 0, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience3Status1}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience3Note1}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Ofuscación.</span></td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(255, 168, 0, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience3Status2}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience3Note2}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Código de Depuración y Registro de Errores Detallado</span></td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(255, 168, 0, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience3Status3}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience3Note3}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 89.5624px; text-align: center"" rowspan=""4""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-RESILIENCE-4</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación implementa técnicas contra el análisis dinámico.</span></td>
    <td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience4Status}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience4Note}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Detección de Anti-Depuración.</span></td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(255, 168, 0, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience4Status1}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience4Note1}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de si la Aplicación es Depurable.</span></td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(255, 168, 0, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience4Status2}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience4Note2}}</span></td>
    </tr>
    <tr style=""height: 22.3906px"">
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Detección de Herramientas de Ingeniería Inversa</span></td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; background-color: rgba(255, 168, 0, 1); text-align: center"">&nbsp;</td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience4Status3}}</span></td>
    <td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience4Note3}}</span></td>
    </tr>
    </tbody>
    </table>
    <p style=""text-align: center"">L1 - Seguridad Estándar / L2 - Defensa en Profundidad / R - Resiliencia contra Ingeniería Inversa y Manipulación<br><span style=""color: rgba(53, 152, 219, 1)""><a style=""color: rgba(53, 152, 219, 1)"" href=""https://mas.owasp.org/"" target=""_blank"" rel=""noopener"">Basado en OWASP MASTG v1.6.0 &amp; OWASP MASVS 2.0.0</a></span></p>";
                reportComponentsManager.Add(mastgAndroid);
                reportComponentsManager.Context.SaveChanges();
                
                ReportComponents mastgIos = new ReportComponents();
                mastgIos.Id = Guid.NewGuid();
                mastgIos.Name = "OWASP MASTG - Resultados (iOS)";
                mastgIos.Language = Language.Español;
                mastgIos.ComponentType = ReportPartType.Body;
                mastgIos.Created = DateTime.Now.ToUniversalTime();
                mastgIos.Updated = DateTime.Now.ToUniversalTime();
                mastgIos.ContentCss = "";
                mastgIos.Content = @"<h2>Resultados</h2>
<p>&nbsp;</p>
<table style=""border-collapse: collapse; width: 100%; height: 223.906px"" border=""1""><colgroup><col style=""width: 11.9632%""><col style=""width: 45.5419%""><col style=""width: 4.45814%""><col style=""width: 4.21779%""><col style=""width: 5.06135%""><col style=""width: 14.6472%""><col style=""width: 14.1104%""></colgroup>
<tbody>
<tr style=""height: 44.7812px"">
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Almacenamiento</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L1</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L2</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">R</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 44.7812px"" rowspan=""2""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-STORAGE-1</span></td>
<td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación almacena de manera segura datos sensibles.</span></td>
<td style=""height: 22.3906px; text-align: left; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: left; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: left; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage1Status}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage1Note}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas en el Almacenamiento Local (Local Storage) en busca de Datos Sensibles.</span></td>
<td style=""height: 22.3906px; text-align: left; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: left; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: left; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage1Status3}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage1Note3}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 134.344px"" rowspan=""6""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-STORAGE-2</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación previene la fuga de datos sensibles.</span></td>
<td style=""height: 22.3906px; text-align: left; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: left; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: left; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Status}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Note}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Encontrar Datos Sensibles en la Caché del Teclado.</span></td>
<td style=""height: 22.3906px; text-align: left; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: left; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: left; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Status7}}&nbsp;</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Note7}}&nbsp;</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas en Copias de Seguridad en busca de Datos Sensibles.</span></td>
<td style=""height: 22.3906px; text-align: left"">&nbsp;</td>
<td style=""text-align: left; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: left; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Status8}}&nbsp;&nbsp;</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Note8}}&nbsp;&nbsp;</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Revisar Registros en busca de Datos Sensibles.</span></td>
<td style=""height: 22.3906px; text-align: left; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: left; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: left; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Status9}}&nbsp;&nbsp;</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Note9}}&nbsp;&nbsp;</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Determinar si los Datos Sensibles se Comparten con Terceros.</span></td>
<td style=""height: 22.3906px; text-align: left; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: left; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: left; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Status10}}&nbsp;&nbsp;</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">&nbsp;{{Storage2Note10}}&nbsp;</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas en Memoria en busca de Datos Sensibles</span></td>
<td style=""height: 22.3906px; text-align: left"">&nbsp;</td>
<td style=""text-align: left; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: left; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">&nbsp;{{Storage2Status11}}&nbsp;</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Storage2Note11}}&nbsp;&nbsp;</span></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<table style=""border-collapse: collapse; width: 100%; height: 297px"" border=""1""><colgroup><col style=""width: 14.2951%""><col style=""width: 43.083%""><col style=""width: 4.28195%""><col style=""width: 4.54545%""><col style=""width: 4.80896%""><col style=""width: 14.7563%""><col style=""width: 14.2951%""></colgroup>
<tbody>
<tr style=""height: 44.75px"">
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.75px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Criptografía</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L1</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L2</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">R</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""height: 89.5624px; text-align: center"" rowspan=""3""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-CRYPTO-1</span></td>
<td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación utiliza criptografía actual fuerte y la aplica según las mejores prácticas de la industria.</span></td>
<td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
<td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
<td style=""background-color: rgba(206, 212, 217, 1); height: 22.3906px; text-align: center"">&nbsp;</td>
<td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto1Status}}</span></td>
<td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto1Note}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Verificación de la Configuración de Algoritmos Criptográficos Estándar.</span></td>
<td style=""height: 22.3906px; background-color: rgba(50, 153, 255, 1); text-align: center"">&nbsp;</td>
<td style=""height: 22.3906px; background-color: rgba(11, 186, 131, 1); text-align: center"">&nbsp;</td>
<td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
<td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto1Status4}}</span></td>
<td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto1Note4}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Generación de Números Aleatorios</span></td>
<td style=""height: 22.3906px; background-color: rgba(50, 153, 255, 1); text-align: center"">&nbsp;</td>
<td style=""height: 22.3906px; background-color: rgba(11, 186, 131, 1); text-align: center"">&nbsp;</td>
<td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
<td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto1Status5}}</span></td>
<td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto1Note5}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""height: 44.7812px; text-align: center"" rowspan=""2""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-CRYPTO-2</span></td>
<td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación realiza la gestión de claves según las mejores prácticas de la industria.</span></td>
<td style=""height: 22.3906px; background-color: rgba(206, 212, 217, 1); text-align: center"">&nbsp;</td>
<td style=""height: 22.3906px; background-color: rgba(206, 212, 217, 1); text-align: center"">&nbsp;</td>
<td style=""height: 22.3906px; background-color: rgba(206, 212, 217, 1); text-align: center"">&nbsp;</td>
<td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto2Status}}</span></td>
<td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto2Note}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas en la Gestión de Claves</span></td>
<td style=""height: 22.3906px; background-color: rgba(50, 153, 255, 1); text-align: center"">&nbsp;</td>
<td style=""height: 22.3906px; background-color: rgba(11, 186, 131, 1); text-align: center"">&nbsp;</td>
<td style=""height: 22.3906px; text-align: center"">&nbsp;</td>
<td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto2Status2}}</span></td>
<td style=""height: 22.3906px; text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Crypto2Note2}}</span></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<table style=""border-collapse: collapse; width: 100%; height: 161.016px"" border=""1""><colgroup><col style=""width: 14.2951%""><col style=""width: 43.083%""><col style=""width: 4.28195%""><col style=""width: 4.54545%""><col style=""width: 4.80896%""><col style=""width: 14.7563%""><col style=""width: 14.2951%""></colgroup>
<tbody>
<tr style=""height: 49.0625px"">
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 49.0625px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Autenticación y Autorización</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L1</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L2</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">R</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-AUTH-1</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación utiliza protocolos seguros de autenticación y autorización y sigue las mejores prácticas relevantes.</span></td>
<td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth1Status}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth1Note}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 67.1718px"" rowspan=""2""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-AUTH-2</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación realiza la autenticación local de manera segura según las mejores prácticas de la plataforma.</span></td>
<td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1); height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth2Status}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth2Note}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Autenticación Local.</span></td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""background-color: rgba(11, 186, 131, 1); text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth2Status3}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth2Note3}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-AUTH-3</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación asegura operaciones sensibles con autenticación adicional</span></td>
<td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth3Status}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Auth3Note}}</span></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<table style=""border-collapse: collapse; width: 100%; height: 250.578px"" border=""1""><colgroup><col style=""width: 14.2951%""><col style=""width: 43.083%""><col style=""width: 4.28195%""><col style=""width: 4.54545%""><col style=""width: 4.80896%""><col style=""width: 14.7563%""><col style=""width: 14.2951%""></colgroup>
<tbody>
<tr style=""height: 49.0625px"">
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 49.0625px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Comunicaciones de Red</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L1</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L2</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">R</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
</tr>
<tr style=""height: 44.7812px"">
<td style=""text-align: center; height: 134.344px"" rowspan=""4""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-NETWORK-1</span></td>
<td style=""text-align: center; height: 44.7812px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación asegura todo el tráfico de red según las mejores prácticas actuales.</span></td>
<td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 44.7812px"">&nbsp;</td>
<td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 44.7812px"">&nbsp;</td>
<td style=""text-align: center; height: 44.7812px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 44.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Status}}</span></td>
<td style=""text-align: center; height: 44.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Note}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Verificación de la Identidad del Punto de Extremo.</span></td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Status5}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Note5}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Configuración de TLS.</span></td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Status6}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Note6}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Encriptación de Datos en la Red.</span></td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Status7}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network1Note7}}</span></td>
</tr>
<tr style=""height: 44.7812px"">
<td style=""text-align: center; height: 67.1718px"" rowspan=""2""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-NETWORK-2</span></td>
<td style=""text-align: center; height: 44.7812px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación realiza el pinning de identidad para todos los puntos de extermo remotos bajo el control del desarrollador.</span></td>
<td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 44.7812px"">&nbsp;</td>
<td style=""background-color: rgba(206, 212, 217, 1); text-align: center; height: 44.7812px"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1); height: 44.7812px"">&nbsp;</td>
<td style=""text-align: center; height: 44.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network2Status}}</span></td>
<td style=""text-align: center; height: 44.7812px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network2Note}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Almacenamiento de Certificados Personalizados y Pinning de Certificados</span></td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network2Status2}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Network2Note2}}</span></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<table style=""border-collapse: collapse; width: 100%; height: 250.578px"" border=""1""><colgroup><col style=""width: 14.2951%""><col style=""width: 43.083%""><col style=""width: 4.28195%""><col style=""width: 4.54545%""><col style=""width: 4.80896%""><col style=""width: 14.7563%""><col style=""width: 14.2951%""></colgroup>
<tbody>
<tr style=""height: 49.0625px"">
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 49.0625px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Interacción con la Plataforma</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L1</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L2</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">R</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 67.1718px"" rowspan=""9""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-PLATFORM-1</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación utiliza mecanismos de Comunicación entre Procesos (IPC) de manera segura.</span></td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Status}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Note}}</span></td>
</tr>
<tr>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Compartir Actividades de la Interfaz de Usuario (UIActivity Sharing).</span></td>
<td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center"">&nbsp;</td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Status6}}</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Note6}}</span></td>
</tr>
<tr>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Permisos de la Aplicación (App Permissions).</span></td>
<td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center"">&nbsp;</td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Status7}}</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Note7}}</span></td>
</tr>
<tr>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Enlaces Universales (Universal Links).</span></td>
<td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center"">&nbsp;</td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Status8}}</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Note8}}</span></td>
</tr>
<tr>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Determinar si Datos Sensibles se Exponen a través de Mecanismos IPC.</span></td>
<td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center"">&nbsp;</td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Status9}}</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Note9}}</span></td>
</tr>
<tr>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Esquemas de URL Personalizados (Custom URL Schemes).</span></td>
<td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center"">&nbsp;</td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Status10}}</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Note10}}</span></td>
</tr>
<tr>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Exposición de Funcionalidad Sensible a través de IPC.</span></td>
<td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center"">&nbsp;</td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Status11}}</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Note11}}</span></td>
</tr>
<tr>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Extensiones de Aplicaciones (App Extensions).</span></td>
<td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center"">&nbsp;</td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Status12}}</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Note12}}</span></td>
</tr>
<tr>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Portapapeles de Usuario (UIPasteboard)</span></td>
<td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center"">&nbsp;</td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Status13}}</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform1Note13}}</span></td>
</tr>
<tr>
<td style=""text-align: center"" rowspan=""4""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-PLATFORM-2</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación utiliza WebViews de manera segura.</span></td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Status}}</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Note}}</span></td>
</tr>
<tr>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de WebViews en iOS.</span></td>
<td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center"">&nbsp;</td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Status5}}</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Note5}}</span></td>
</tr>
<tr>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Determinar si los Métodos Nativos se Exponen a través de WebViews.</span></td>
<td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center"">&nbsp;</td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Status6}}</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Note6}}</span></td>
</tr>
<tr>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Manipuladores de Protocolos en WebViews.</span></td>
<td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center"">&nbsp;</td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Status7}}</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform2Note7}}</span></td>
</tr>
<tr>
<td style=""text-align: center"" rowspan=""3""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-PLATFORM-3</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación utiliza la interfaz de usuario de manera segura.</span></td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform3Status}}</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform3Note}}</span></td>
</tr>
<tr>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Capturas de Pantalla Generadas Automáticamente en busca de Información Sensible.</span></td>
<td style=""text-align: center"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center"">&nbsp;</td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform3Status4}}</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform3Note4}}</span></td>
</tr>
<tr>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">Verificación de Datos Sensibles Revelados a través de la Interfaz de Usuario</span></td>
<td style=""text-align: center; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center"">&nbsp;</td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform3Status5}}</span></td>
<td style=""text-align: center""><span style=""color: rgba(0, 0, 0, 1)"">{{Platform3Note5}}</span></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<table style=""border-collapse: collapse; width: 100%; height: 340.14px"" border=""1""><colgroup><col style=""width: 14.2951%""><col style=""width: 43.083%""><col style=""width: 4.28195%""><col style=""width: 4.54545%""><col style=""width: 4.80896%""><col style=""width: 14.7563%""><col style=""width: 14.2951%""></colgroup>
<tbody>
<tr style=""height: 49.0625px"">
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 49.0625px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Calidad del Código</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L1</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L2</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">R</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-CODE-1</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación requiere una versión actualizada de la plataforma.</span></td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code1Status}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code1Note}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 44.7812px"" rowspan=""2""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-CODE-2</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación tiene un mecanismo para hacer cumplir las actualizaciones de la aplicación</span></td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code2Status}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code2Note}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Actualización Forzada (Enforced Updating).</span></td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code2Status2}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code2Note2}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 44.7812px"" rowspan=""2""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-CODE-3</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación solo utiliza componentes de software sin vulnerabilidades conocidas.</span></td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code3Status}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code3Note}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Verificación de Debilidades en Bibliotecas de Terceros.</span></td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code3Status2}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code3Note2}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 179.125px"" rowspan=""4""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-CODE-4</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación solo utiliza componentes de software sin vulnerabilidades conocidas.</span></td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(206, 212, 217, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Status}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Note}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Persistencia de Objetos.</span></td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Status8}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Note8}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Asegurarse de que las Funciones de Seguridad Gratuitas estén Activadas.</span></td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Status9}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Note9}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Bugs de Corrupción de Memoria</span></td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(50, 153, 255, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(11, 186, 131, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Status10}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Code4Note10}}</span></td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<p>&nbsp;</p>
<table style=""border-collapse: collapse; width: 100%; height: 384.921px"" border=""1""><colgroup><col style=""width: 14.2951%""><col style=""width: 43.083%""><col style=""width: 4.28195%""><col style=""width: 4.54545%""><col style=""width: 4.80896%""><col style=""width: 14.7563%""><col style=""width: 14.2951%""></colgroup>
<tbody>
<tr style=""height: 49.0625px"">
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 49.0625px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Resiliencia contra ingeniería inversa</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Test</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L1</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">L2</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">R</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Estado</span></strong></td>
<td style=""text-align: center; background-color: rgba(0, 0, 0, 1); height: 44.7812px""><strong><span style=""color: rgba(255, 255, 255, 1)"">Nota</span></strong></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px"" rowspan=""3""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-RESILIENCE-1</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación valida la integridad de la plataforma.</span></td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1); height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1); height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1); height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience1Status}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience1Note}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Detección de Emuladores.</span></td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(255, 168, 0, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience1Status3}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience1Note3}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Detección de Jailbreak</span></td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(255, 168, 0, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience1Status4}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience1Note4}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px"" rowspan=""3""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-RESILIENCE-2</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación implementa mecanismos contra manipulaciones.</span></td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1); height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1); height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1); height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience2Status}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience2Note}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Asegurarse de que la aplicación esté debidamente firmada.</span></td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(255, 168, 0, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience2Status4}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience2Note4}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Verificación de Integridad de Archivos</span></td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(255, 168, 0, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience2Status5}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience2Note5}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px"" rowspan=""4""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-RESILIENCE-3</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación implementa mecanismos contra análisis estáticos.</span></td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1); height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1); height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1); height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience3Status}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience3Note}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Detección de Símbolos de Depuración.</span></td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(255, 168, 0, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience3Status4}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience3Note4}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Ofuscación.</span></td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(255, 168, 0, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience3Status5}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience3Note5}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Código de Depuración y Registro de Errores con Verbose</span></td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(255, 168, 0, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience3Status6}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience3Note6}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px"" rowspan=""4""><span style=""color: rgba(0, 0, 0, 1)"">MASVS-RESILIENCE-4</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">La aplicación implementa técnicas contra análisis dinámicos.</span></td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1); height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1); height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; background-color: rgba(206, 212, 217, 1); height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience4Status}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience4Note}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Detección de Herramientas de Ingeniería Inversa.</span></td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(255, 168, 0, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience4Status4}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience4Note4}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas para determinar si la aplicación es depurable.</span></td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(255, 168, 0, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience4Status5}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience4Note5}}</span></td>
</tr>
<tr style=""height: 22.3906px"">
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">Pruebas de Detección de Antidepuración</span></td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px; background-color: rgba(255, 168, 0, 1)"">&nbsp;</td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience4Status6}}</span></td>
<td style=""text-align: center; height: 22.3906px""><span style=""color: rgba(0, 0, 0, 1)"">{{Resilience4Note6}}</span></td>
</tr>
</tbody>
</table>
<p style=""text-align: center"">L1 - Seguridad Estándar / L2 - Defensa en Profundidad / R - Resiliencia contra Ingeniería Inversa y Manipulación<br><span style=""color: rgba(53, 152, 219, 1)""><a style=""color: rgba(53, 152, 219, 1)"" href=""https://mas.owasp.org/"" target=""_blank"" rel=""noopener"">Basado en OWASP MASTG v1.6.0 &amp; OWASP MASVS 2.0.0</a></span></p>";
                reportComponentsManager.Add(mastgIos);
                reportComponentsManager.Context.SaveChanges();
                #endregion

                #region Plantillas

                #region General
                var plantillaGeneral= new ReportTemplate();
                plantillaGeneral.Id = Guid.NewGuid();
                plantillaGeneral.Name = "Default Spanish Report Template";
                plantillaGeneral.Description = "Default Spanish Report Template";
                plantillaGeneral.Language = Language.Español;
                plantillaGeneral.CreatedDate = DateTime.Now.ToUniversalTime();
                plantillaGeneral.ReportType = ReportType.General;
                plantillaGeneral.UserId = admin.Id;
                reportTemplateManager.Add(plantillaGeneral);
                reportTemplateManager.Context.SaveChanges();

                var plantillaGeneralPart1 = new ReportParts();
                plantillaGeneralPart1.Id = Guid.NewGuid();
                plantillaGeneralPart1.TemplateId = plantillaGeneral.Id;
                plantillaGeneralPart1.PartType = ReportPartType.Cover;
                plantillaGeneralPart1.ComponentId = portadaGeneral.Id;
                plantillaGeneralPart1.Order = 1;
                reportsPartsManager.Add(plantillaGeneralPart1);
                
                var plantillaGeneralPart2 = new ReportParts();
                plantillaGeneralPart2.Id = Guid.NewGuid();
                plantillaGeneralPart2.TemplateId = plantillaGeneral.Id;
                plantillaGeneralPart2.PartType = ReportPartType.Header;
                plantillaGeneralPart2.ComponentId = cabeceraGeneral.Id;
                plantillaGeneralPart2.Order = 1;
                reportsPartsManager.Add(plantillaGeneralPart2);
                
                var plantillaGeneralPart3 = new ReportParts();
                plantillaGeneralPart3.Id = Guid.NewGuid();
                plantillaGeneralPart3.TemplateId = plantillaGeneral.Id;
                plantillaGeneralPart3.PartType = ReportPartType.Footer;
                plantillaGeneralPart3.ComponentId = pieGeneral.Id;
                plantillaGeneralPart3.Order = 1;
                reportsPartsManager.Add(plantillaGeneralPart3);
                
                var plantillaGeneralPart4 = new ReportParts();
                plantillaGeneralPart4.Id = Guid.NewGuid();
                plantillaGeneralPart4.TemplateId = plantillaGeneral.Id;
                plantillaGeneralPart4.PartType = ReportPartType.Body;
                plantillaGeneralPart4.ComponentId = descargoResponsabilidadGeneral.Id;
                plantillaGeneralPart4.Order = 1;
                reportsPartsManager.Add(plantillaGeneralPart4);

                var plantillaGeneralPart5 = new ReportParts();
                plantillaGeneralPart5.Id = Guid.NewGuid();
                plantillaGeneralPart5.TemplateId = plantillaGeneral.Id;
                plantillaGeneralPart5.PartType = ReportPartType.Body;
                plantillaGeneralPart5.ComponentId = controlDocGeneral.Id;
                plantillaGeneralPart5.Order = 2;
                reportsPartsManager.Add(plantillaGeneralPart4);
                
                var plantillaGeneralPart6 = new ReportParts();
                plantillaGeneralPart6.Id = Guid.NewGuid();
                plantillaGeneralPart6.TemplateId = plantillaGeneral.Id;
                plantillaGeneralPart6.PartType = ReportPartType.Body;
                plantillaGeneralPart6.ComponentId = equipoGeneral.Id;
                plantillaGeneralPart6.Order = 3;
                reportsPartsManager.Add(plantillaGeneralPart6);
                
                var plantillaGeneralPart7 = new ReportParts();
                plantillaGeneralPart7.Id = Guid.NewGuid();
                plantillaGeneralPart7.TemplateId = plantillaGeneral.Id;
                plantillaGeneralPart7.PartType = ReportPartType.Body;
                plantillaGeneralPart7.ComponentId = introGeneral.Id;
                plantillaGeneralPart7.Order = 4;
                reportsPartsManager.Add(plantillaGeneralPart7);
                
                var plantillaGeneralPart8 = new ReportParts();
                plantillaGeneralPart8.Id = Guid.NewGuid();
                plantillaGeneralPart8.TemplateId = plantillaGeneral.Id;
                plantillaGeneralPart8.PartType = ReportPartType.Body;
                plantillaGeneralPart8.ComponentId = propositoGeneral.Id;
                plantillaGeneralPart8.Order = 5;
                reportsPartsManager.Add(plantillaGeneralPart8);
                
                var plantillaGeneralPart13 = new ReportParts();
                plantillaGeneralPart13.Id = Guid.NewGuid();
                plantillaGeneralPart13.TemplateId = plantillaGeneral.Id;
                plantillaGeneralPart13.PartType = ReportPartType.Body;
                plantillaGeneralPart13.ComponentId = alcanceGeneral.Id;
                plantillaGeneralPart13.Order = 6;
                reportsPartsManager.Add(plantillaGeneralPart13);
                
                var plantillaGeneralPart14 = new ReportParts();
                plantillaGeneralPart14.Id = Guid.NewGuid();
                plantillaGeneralPart14.TemplateId = plantillaGeneral.Id;
                plantillaGeneralPart14.PartType = ReportPartType.Body;
                plantillaGeneralPart14.ComponentId = metodologiaGeneral.Id;
                plantillaGeneralPart14.Order = 7;
                reportsPartsManager.Add(plantillaGeneralPart14);
                
                var plantillaGeneralPart9 = new ReportParts();
                plantillaGeneralPart9.Id = Guid.NewGuid();
                plantillaGeneralPart9.TemplateId = plantillaGeneral.Id;
                plantillaGeneralPart9.PartType = ReportPartType.Body;
                plantillaGeneralPart9.ComponentId = resumenEjecutivoGeneral.Id;
                plantillaGeneralPart9.Order = 8;
                reportsPartsManager.Add(plantillaGeneralPart9);
                
                var plantillaGeneralPart10 = new ReportParts();
                plantillaGeneralPart10.Id = Guid.NewGuid();
                plantillaGeneralPart10.TemplateId = plantillaGeneral.Id;
                plantillaGeneralPart10.PartType = ReportPartType.Body;
                plantillaGeneralPart10.ComponentId = clasHallazgosGeneral.Id;
                plantillaGeneralPart10.Order = 9;
                reportsPartsManager.Add(plantillaGeneralPart10);
                
                var plantillaGeneralPart11 = new ReportParts();
                plantillaGeneralPart11.Id = Guid.NewGuid();
                plantillaGeneralPart11.TemplateId = plantillaGeneral.Id;
                plantillaGeneralPart11.PartType = ReportPartType.Body;
                plantillaGeneralPart11.ComponentId = resumenHallazgosGeneral.Id;
                plantillaGeneralPart11.Order = 10;
                reportsPartsManager.Add(plantillaGeneralPart11);
                
                var plantillaGeneralPart12 = new ReportParts();
                plantillaGeneralPart12.Id = Guid.NewGuid();
                plantillaGeneralPart12.TemplateId = plantillaGeneral.Id;
                plantillaGeneralPart12.PartType = ReportPartType.Body;
                plantillaGeneralPart12.ComponentId = detallesHallazgosGeneral.Id;
                plantillaGeneralPart12.Order = 11;
                reportsPartsManager.Add(plantillaGeneralPart12);
                
                reportsPartsManager.Context.SaveChanges();
                #endregion

                #region OWASP WSTG

                var plantillaWstg= new ReportTemplate();
                plantillaWstg.Id = Guid.NewGuid();
                plantillaWstg.Name = "OWASP WSTG - Lista de Verificación";
                plantillaWstg.Description = "OWASP - Lista de Verificación";
                plantillaWstg.Language = Language.Español;
                plantillaWstg.CreatedDate = DateTime.Now.ToUniversalTime();
                plantillaWstg.ReportType = ReportType.WSTG;
                plantillaWstg.UserId = admin.Id;
                reportTemplateManager.Add(plantillaWstg);
                reportTemplateManager.Context.SaveChanges();

                var wstgPart1 = new ReportParts();
                wstgPart1.Id = Guid.NewGuid();
                wstgPart1.TemplateId = plantillaWstg.Id;
                wstgPart1.PartType = ReportPartType.Cover;
                wstgPart1.ComponentId = wstgPortada.Id;
                wstgPart1.Order = 1;
                reportsPartsManager.Add(wstgPart1);
                
                var wstgPart2 = new ReportParts();
                wstgPart2.Id = Guid.NewGuid();
                wstgPart2.TemplateId = plantillaWstg.Id;
                wstgPart2.PartType = ReportPartType.Header;
                wstgPart2.ComponentId = wstgCabecera.Id;
                wstgPart2.Order = 1;
                reportsPartsManager.Add(wstgPart2);

                var wstgPart3 = new ReportParts();
                wstgPart3.Id = Guid.NewGuid();
                wstgPart3.TemplateId = plantillaWstg.Id;
                wstgPart3.PartType = ReportPartType.Footer;
                wstgPart3.ComponentId = pieGeneral.Id;
                wstgPart3.Order = 1;
                reportsPartsManager.Add(wstgPart3);
                
                var wstgPart4 = new ReportParts();
                wstgPart4.Id = Guid.NewGuid();
                wstgPart4.TemplateId = plantillaWstg.Id;
                wstgPart4.PartType = ReportPartType.Body;
                wstgPart4.ComponentId = wstgIntroduccion.Id;
                wstgPart4.Order = 1;
                reportsPartsManager.Add(wstgPart4);
                
                var wstgPart5 = new ReportParts();
                wstgPart5.Id = Guid.NewGuid();
                wstgPart5.TemplateId = plantillaWstg.Id;
                wstgPart5.PartType = ReportPartType.Body;
                wstgPart5.ComponentId = wstgResultados.Id;
                wstgPart5.Order = 2;
                reportsPartsManager.Add(wstgPart5);
                
                reportsPartsManager.Context.SaveChanges();
                #endregion

                #region Mastg Android
                var plantillaAndroid= new ReportTemplate();
                plantillaAndroid.Id = Guid.NewGuid();
                plantillaAndroid.Name = "OWASP MASTG - Lista de Verificación (Android)";
                plantillaAndroid.Description = "OWASP MASTG - Lista de Verificación (Android)";
                plantillaAndroid.Language = Language.Español;
                plantillaAndroid.CreatedDate = DateTime.Now.ToUniversalTime();
                plantillaAndroid.ReportType = ReportType.MASTG;
                plantillaAndroid.UserId = admin.Id;
                reportTemplateManager.Add(plantillaAndroid);
                reportTemplateManager.Context.SaveChanges();

                var androidPart1 = new ReportParts();
                androidPart1.Id = Guid.NewGuid();
                androidPart1.TemplateId = plantillaAndroid.Id;
                androidPart1.PartType = ReportPartType.Cover;
                androidPart1.ComponentId = mastgPortada.Id;
                androidPart1.Order = 1;
                reportsPartsManager.Add(androidPart1);
                
                var androidPart2 = new ReportParts();
                androidPart2.Id = Guid.NewGuid();
                androidPart2.TemplateId = plantillaAndroid.Id;
                androidPart2.PartType = ReportPartType.Header;
                androidPart2.ComponentId = mastgCabecera.Id;
                androidPart2.Order = 1;
                reportsPartsManager.Add(androidPart2);
                
                var androidPart3 = new ReportParts();
                androidPart3.Id = Guid.NewGuid();
                androidPart3.TemplateId = plantillaAndroid.Id;
                androidPart3.PartType = ReportPartType.Footer;
                androidPart3.ComponentId = pieGeneral.Id;
                androidPart3.Order = 1;
                reportsPartsManager.Add(androidPart3);
                
                var androidPart4 = new ReportParts();
                androidPart4.Id = Guid.NewGuid();
                androidPart4.TemplateId = plantillaAndroid.Id;
                androidPart4.PartType = ReportPartType.Body;
                androidPart4.ComponentId = mastgIntroduccion.Id;
                androidPart4.Order = 1;
                reportsPartsManager.Add(androidPart4);

                var androidPart5 = new ReportParts();
                androidPart5.Id = Guid.NewGuid();
                androidPart5.TemplateId = plantillaAndroid.Id;
                androidPart5.PartType = ReportPartType.Body;
                androidPart5.ComponentId = mastgAndroid.Id;
                androidPart5.Order = 2;
                reportsPartsManager.Add(androidPart5);
                reportsPartsManager.Context.SaveChanges();
                #endregion

                #region MASTG iOS
                
                var plantillaIos= new ReportTemplate();
                plantillaIos.Id = Guid.NewGuid();
                plantillaIos.Name = "OWASP MASTG - Lista de Verificación (iOS)";
                plantillaIos.Description = "OWASP MASTG - Lista de Verificación (iOS)";
                plantillaIos.Language = Language.Español;
                plantillaIos.CreatedDate = DateTime.Now.ToUniversalTime();
                plantillaIos.ReportType = ReportType.MASTG;
                plantillaIos.UserId = admin.Id;
                reportTemplateManager.Add(plantillaIos);
                reportTemplateManager.Context.SaveChanges();

                var iosPart1 = new ReportParts();
                iosPart1.Id = Guid.NewGuid();
                iosPart1.TemplateId = plantillaIos.Id;
                iosPart1.PartType = ReportPartType.Cover;
                iosPart1.ComponentId = mastgPortada.Id;
                iosPart1.Order = 1;
                reportsPartsManager.Add(iosPart1);

                var iosPart2 = new ReportParts();
                iosPart2.Id = Guid.NewGuid();
                iosPart2.TemplateId = plantillaIos.Id;
                iosPart2.PartType = ReportPartType.Header;
                iosPart2.ComponentId = mastgCabecera.Id;
                iosPart2.Order = 1;
                reportsPartsManager.Add(iosPart2);
                
                var iosPart3 = new ReportParts();
                iosPart3.Id = Guid.NewGuid();
                iosPart3.TemplateId = plantillaIos.Id;
                iosPart3.PartType = ReportPartType.Footer;
                iosPart3.ComponentId = pieGeneral.Id;
                iosPart3.Order = 1;
                reportsPartsManager.Add(iosPart3);
                
                var iosPart4 = new ReportParts();
                iosPart4.Id = Guid.NewGuid();
                iosPart4.TemplateId = plantillaIos.Id;
                iosPart4.PartType = ReportPartType.Body;
                iosPart4.ComponentId = mastgIntroduccion.Id;
                iosPart4.Order = 1;
                reportsPartsManager.Add(iosPart4);
                
                var iosPart5 = new ReportParts();
                iosPart5.Id = Guid.NewGuid();
                iosPart5.TemplateId = plantillaIos.Id;
                iosPart5.PartType = ReportPartType.Body;
                iosPart5.ComponentId = mastgIos.Id;
                iosPart5.Order = 2;
                reportsPartsManager.Add(iosPart5);
                reportsPartsManager.Context.SaveChanges();
                #endregion
                #endregion

            #endregion
            
        }
        
    }

    private static void SeedVulnCategories(Contracts.IVulnCategoryManager vulnCategoryManager)
    {
        if (vulnCategoryManager.GetAll().Count() == 0)
        {
            var category = new VulnCategory();
            category.Name = "Access Controls";
            category.Description = "Access Controls";
            category.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category);
            vulnCategoryManager.Context.SaveChanges();

            var category2 = new VulnCategory();
            category2.Name = "Auditing and logging";
            category2.Description = "Auditing and logging";
            category2.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category2);
            vulnCategoryManager.Context.SaveChanges();

            var category3 = new VulnCategory();
            category3.Name = "Authentication";
            category3.Description = "Authentication";
            category3.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category3);
            vulnCategoryManager.Context.SaveChanges();

            var category4 = new VulnCategory();
            category4.Name = "Authorization";
            category4.Description = "Authorization";
            category4.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category4);
            vulnCategoryManager.Context.SaveChanges();

            var category5 = new VulnCategory();
            category5.Name = "Configuration";
            category5.Description = "Configuration";
            category5.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category5);
            vulnCategoryManager.Context.SaveChanges();

            var category6 = new VulnCategory();
            category6.Name = "Cryptography";
            category6.Description = "Cryptography";
            category6.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category6);
            vulnCategoryManager.Context.SaveChanges();

            var category7 = new VulnCategory();
            category7.Name = "Data Exposure";
            category7.Description = "Data Exposure";
            category7.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category7);
            vulnCategoryManager.Context.SaveChanges();

            var category8 = new VulnCategory();
            category8.Name = "Data Validation";
            category8.Description = "Data Validation";
            category8.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category8);
            vulnCategoryManager.Context.SaveChanges();

            var category9 = new VulnCategory();
            category9.Name = "Denial of Service";
            category9.Description = "Denial of Service";
            category9.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category9);
            vulnCategoryManager.Context.SaveChanges();

            var category10 = new VulnCategory();
            category10.Name = "Error Reporting";
            category10.Description = "Error Reporting";
            category10.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category10);
            vulnCategoryManager.Context.SaveChanges();

            var category11 = new VulnCategory();
            category11.Name = "Injection";
            category11.Description = "Injection";
            category11.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category11);
            vulnCategoryManager.Context.SaveChanges();

            var category12 = new VulnCategory();
            category12.Name = "Patching";
            category12.Description = "Patching";
            category12.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category12);
            vulnCategoryManager.Context.SaveChanges();

            var category13 = new VulnCategory();
            category13.Name = "Session Management";
            category13.Description = "Session Management";
            category13.Type = VulnCategoryType.General;
            vulnCategoryManager.Add(category13);
            vulnCategoryManager.Context.SaveChanges();

            var category14 = new VulnCategory();
            category14.Name = "Timing";
            category14.Description = "Timing Attacks";
            category14.Type = VulnCategoryType.General;
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