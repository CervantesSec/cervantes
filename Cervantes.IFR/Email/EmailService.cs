using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Cervantes.IFR.Email;

public class EmailService : IEmailService
{
    private readonly IEmailConfiguration _emailConfiguration;
    private readonly IUserManager userManager;
    private readonly IProjectManager projectManager;
    private readonly IClientManager clientManager;
    private readonly ITaskManager taskManager;

    public EmailService(IEmailConfiguration emailConfiguration, IUserManager userManager,
        IProjectManager projectManager,
        IClientManager clientManager, ITaskManager taskManager)
    {
        _emailConfiguration = emailConfiguration;
        this.userManager = userManager;
        this.projectManager = projectManager;
        this.clientManager = clientManager;
        this.taskManager = taskManager;
    }

    public bool IsEnabled()
    {
        return _emailConfiguration.Enabled;
    }

    public void SendWelcome(string userId, string link)
    {
        try
        {
            if (_emailConfiguration.Enabled == false)
            {
                return;
            }

            var user = userManager.GetByUserId(userId);
            if (user == null)
            {
                return;
            }

            var to = new List<EmailAddress>();
            to.Add(new EmailAddress
            {
                Address = user.Email,
                Name = user.FullName,
            });


            StreamReader sr =
                new StreamReader(Directory.GetCurrentDirectory() + "/wwwroot/Resources/Email/UserCreated.html");
            string s = sr.ReadToEnd();
            s = s.Replace("{UserName}", user.FullName).Replace("{CervantesLink}", link);
            sr.Close();

            EmailMessage message = new EmailMessage
            {
                ToAddresses = to,
                Subject = "Cervantes - Welcome to Cervantes " + user.FullName,
                Content = s
            };


            var mimeMessage = new MimeMessage();
            mimeMessage.To.AddRange(message.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            var from = new MailboxAddress(_emailConfiguration.Name, _emailConfiguration.From);
            mimeMessage.From.Add(from);
            mimeMessage.Subject = message.Subject;
            //We will say we are sending HTML. But there are options for plaintext etc. 
            mimeMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = message.Content
            };

            //Be careful that the SmtpClient class is the one from Mailkit not the framework!
            using (var emailClient = new SmtpClient())
            {
                //The last parameter here is to use SSL (Which you should!)
                emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort,
                    SecureSocketOptions.Auto);

                //Remove any OAuth functionality as we won't be using it. 
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

                emailClient.Send(mimeMessage);

                emailClient.Disconnect(true);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void SendAsignedProject(string userId, Guid projectId)
    {
        try
        {
            if (_emailConfiguration.Enabled == false)
            {
                return;
            }

            var user = userManager.GetByUserId(userId);
            var project = projectManager.GetById(projectId);
            var client = clientManager.GetById(project.ClientId);
            if (user == null || project == null)
            {
                return;
            }

            var to = new List<EmailAddress>();
            to.Add(new EmailAddress
            {
                Address = user.Email,
                Name = user.FullName,
            });


            StreamReader sr =
                new StreamReader(Directory.GetCurrentDirectory() + "/wwwroot/Resources/Email/AddedToProject.html");
            string s = sr.ReadToEnd();
            s = s.Replace("{UserName}", user.FullName).Replace("{Project}", project.Name)
                .Replace("{StartDate}", project.StartDate.ToShortDateString())
                .Replace("{EndDate}", project.EndDate.ToShortDateString()).Replace("{Client}", client.Name);
            sr.Close();

            EmailMessage message = new EmailMessage
            {
                ToAddresses = to,
                Subject = "Cervantes - " + @user.FullName + " you have been added to a Project",
                Content = s
            };


            var mimeMessage = new MimeMessage();
            mimeMessage.To.AddRange(message.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            var from = new MailboxAddress(_emailConfiguration.Name, _emailConfiguration.From);
            mimeMessage.From.Add(from);
            mimeMessage.Subject = message.Subject;
            //We will say we are sending HTML. But there are options for plaintext etc. 
            mimeMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = message.Content
            };

            //Be careful that the SmtpClient class is the one from Mailkit not the framework!
            using (var emailClient = new SmtpClient())
            {
                //The last parameter here is to use SSL (Which you should!)
                emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort,
                    SecureSocketOptions.Auto);

                //Remove any OAuth functionality as we won't be using it. 
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

                emailClient.Send(mimeMessage);

                emailClient.Disconnect(true);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void SendAsignedTask(string userId, Guid? projectId, Guid taskId)
    {
        try
        {
            if (_emailConfiguration.Enabled == false)
            {
                return;
            }

            var user = userManager.GetByUserId(userId);

            CORE.Entities.Project project = new Project();

            if (projectId != Guid.Empty || projectId != null)
            {
                project = projectManager.GetById(projectId.Value);
            }

            var task = taskManager.GetById(taskId);
            if (user == null || task == null)
            {
                return;
            }

            var to = new List<EmailAddress>();
            to.Add(new EmailAddress
            {
                Address = user.Email,
                Name = user.FullName,
            });


            StreamReader sr =
                new StreamReader(Directory.GetCurrentDirectory() + "/wwwroot/Resources/Email/AsignedTask.html");
            string s = sr.ReadToEnd();

            if (project != null)
            {
                s = s.Replace("{UserName}", user.FullName).Replace("{Project}", project.Name)
                    .Replace("{Task}", task.Name).Replace("{StartDate}", task.StartDate.ToShortDateString())
                    .Replace("{EndDate}", task.EndDate.ToShortDateString())
                    .Replace("{Description}", task.Description);
            }
            else
            {
                s = s.Replace("{UserName}", user.FullName).Replace("{Project}", "No Project")
                    .Replace("{Task}", task.Name).Replace("{StartDate}", task.StartDate.ToShortDateString())
                    .Replace("{EndDate}", task.EndDate.ToShortDateString())
                    .Replace("{Description}", task.Description);
            }

            sr.Close();

            EmailMessage message = new EmailMessage
            {
                ToAddresses = to,
                Subject = "Cervantes - " + @user.FullName + " you have a new Task assigned",
                Content = s
            };


            var mimeMessage = new MimeMessage();
            mimeMessage.To.AddRange(message.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            var from = new MailboxAddress(_emailConfiguration.Name, _emailConfiguration.From);
            mimeMessage.From.Add(from);
            mimeMessage.Subject = message.Subject;
            //We will say we are sending HTML. But there are options for plaintext etc. 
            mimeMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = message.Content
            };

            //Be careful that the SmtpClient class is the one from Mailkit not the framework!
            using (var emailClient = new SmtpClient())
            {
                //The last parameter here is to use SSL (Which you should!)
                emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort,
                    SecureSocketOptions.Auto);

                //Remove any OAuth functionality as we won't be using it. 
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

                emailClient.Send(mimeMessage);

                emailClient.Disconnect(true);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> SendCveNotificationAsync(CveNotification notification)
    {
        try
        {
            if (_emailConfiguration.Enabled == false)
            {
                return false;
            }

            if (notification.User == null || string.IsNullOrEmpty(notification.User.Email))
            {
                return false;
            }

            var to = new List<EmailAddress>();
            to.Add(new EmailAddress
            {
                Address = notification.User.Email,
                Name = notification.User.FullName,
            });

            var htmlBody = GenerateCveNotificationHtml(notification);

            EmailMessage message = new EmailMessage
            {
                ToAddresses = to,
                Subject = notification.Title ?? "CVE Alert",
                Content = htmlBody
            };

            var mimeMessage = new MimeMessage();
            mimeMessage.To.AddRange(message.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            var from = new MailboxAddress(_emailConfiguration.Name, _emailConfiguration.From);
            mimeMessage.From.Add(from);
            mimeMessage.Subject = message.Subject;

            // Create multipart/related for HTML with embedded images
            var multipart = new MimeKit.Multipart("related");
            
            // Add the HTML body
            var htmlPart = new TextPart(TextFormat.Html)
            {
                Text = message.Content
            };
            multipart.Add(htmlPart);

            // Add the logo image as embedded attachment
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "logo-horizontal2.png");
            if (System.IO.File.Exists(logoPath))
            {
                // Read the file into a memory stream to avoid disposal issues
                var logoBytes = System.IO.File.ReadAllBytes(logoPath);
                var memoryStream = new MemoryStream(logoBytes);
                
                var image = new MimePart("image", "png")
                {
                    Content = new MimeContent(memoryStream),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Inline),
                    ContentTransferEncoding = ContentEncoding.Base64
                };
                image.ContentId = "cervantes_logo";
                multipart.Add(image);
            }

            mimeMessage.Body = multipart;

            using (var emailClient = new SmtpClient())
            {
                emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort,
                    SecureSocketOptions.Auto);

                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

                await emailClient.SendAsync(mimeMessage);

                emailClient.Disconnect(true);
            }

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private string GenerateCveNotificationHtml(CveNotification notification)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset='utf-8'>");
        sb.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1'>");
        sb.AppendLine("<title>CVE Alert - Cervantes</title>");
        sb.AppendLine("<style>");
        sb.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }");
        sb.AppendLine(".container { max-width: 600px; margin: 0 auto; padding: 20px; }");
        sb.AppendLine(".header { background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin-bottom: 20px; border-left: 4px solid #007bff; }");
        sb.AppendLine(".alert-high { border-left-color: #dc3545; }");
        sb.AppendLine(".alert-medium { border-left-color: #ffc107; }");
        sb.AppendLine(".alert-low { border-left-color: #28a745; }");
        sb.AppendLine(".cve-info { background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 15px 0; }");
        sb.AppendLine(".severity-critical { color: #dc3545; font-weight: bold; }");
        sb.AppendLine(".severity-high { color: #fd7e14; font-weight: bold; }");
        sb.AppendLine(".severity-medium { color: #ffc107; font-weight: bold; }");
        sb.AppendLine(".severity-low { color: #28a745; font-weight: bold; }");
        sb.AppendLine(".kev-badge { background-color: #dc3545; color: white; padding: 3px 8px; border-radius: 3px; font-size: 12px; font-weight: bold; }");
        sb.AppendLine(".footer { margin-top: 30px; padding-top: 20px; border-top: 1px solid #dee2e6; font-size: 12px; color: #6c757d; text-align: center; }");
        sb.AppendLine(".logo { text-align: center; margin-bottom: 20px; }");
        sb.AppendLine(".logo h1 { color: #007bff; margin: 0; font-size: 24px; }");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("<div class='container'>");
        
        sb.AppendLine("<div class='logo'>");
        sb.AppendLine("<img src='cid:cervantes_logo' alt='Cervantes' style='max-width: 200px; height: auto; margin: 0 auto; display: block;'/>");
        sb.AppendLine("<p style='margin: 10px 0 0 0; color: #6c757d; text-align: center;'>CVE Management System</p>");
        sb.AppendLine("</div>");
        
        var priorityClass = notification.Priority?.ToLower() switch
        {
            "high" => "alert-high",
            "medium" => "alert-medium",
            "low" => "alert-low",
            _ => ""
        };
        
        sb.AppendLine($"<div class='header {priorityClass}'>");
        sb.AppendLine($"<h2 style='margin: 0 0 10px 0;'>{notification.Title}</h2>");
        sb.AppendLine($"<p style='margin: 5px 0;'><strong>Priority:</strong> {notification.Priority ?? "Normal"}</p>");
        sb.AppendLine($"<p style='margin: 5px 0;'><strong>Date:</strong> {notification.CreatedDate:yyyy-MM-dd HH:mm} UTC</p>");
        sb.AppendLine("</div>");
        
        if (notification.Cve != null)
        {
            sb.AppendLine("<div class='cve-info'>");
            sb.AppendLine("<h3 style='margin-top: 0;'>CVE Details</h3>");
            sb.AppendLine($"<p><strong>CVE ID:</strong> {notification.Cve.CveId}</p>");
            
            if (!string.IsNullOrEmpty(notification.Cve.Title))
            {
                sb.AppendLine($"<p><strong>Title:</strong> {notification.Cve.Title}</p>");
            }
            
            if (notification.Cve.CvssV3BaseScore.HasValue)
            {
                var severityClass = $"severity-{notification.Cve.CvssV3Severity?.ToLower() ?? "unknown"}";
                sb.AppendLine($"<p><strong>CVSS Score:</strong> {notification.Cve.CvssV3BaseScore:F1} (<span class='{severityClass}'>{notification.Cve.CvssV3Severity ?? "Unknown"}</span>)</p>");
            }
            
            if (notification.Cve.EpssScore.HasValue)
            {
                sb.AppendLine($"<p><strong>EPSS Score:</strong> {notification.Cve.EpssScore:F3} (Exploitation Probability)</p>");
            }
            
            if (notification.Cve.IsKnownExploited)
            {
                sb.AppendLine("<p style='margin: 10px 0;'><span class='kev-badge'>⚠️ KNOWN EXPLOITED VULNERABILITY</span></p>");
            }
            
            sb.AppendLine($"<p><strong>Published:</strong> {notification.Cve.PublishedDate:yyyy-MM-dd}</p>");
            
            sb.AppendLine($"<p><strong>Last Modified:</strong> {notification.Cve.LastModifiedDate:yyyy-MM-dd}</p>");
            
            if (!string.IsNullOrEmpty(notification.Cve.Description))
            {
                sb.AppendLine($"<p><strong>Description:</strong></p>");
                sb.AppendLine($"<p style='background-color: white; padding: 10px; border-radius: 3px; border-left: 3px solid #007bff;'>{notification.Cve.Description}</p>");
            }
            sb.AppendLine("</div>");
        }
        
        if (!string.IsNullOrEmpty(notification.Message))
        {
            sb.AppendLine("<div style='margin: 15px 0;'>");
            sb.AppendLine("<h3>Additional Information</h3>");
            sb.AppendLine($"<p>{notification.Message}</p>");
            sb.AppendLine("</div>");
        }
        
        if (notification.Subscription != null)
        {
            sb.AppendLine("<div style='margin: 15px 0; font-size: 14px; color: #6c757d;'>");
            sb.AppendLine($"<p><strong>Triggered by subscription:</strong> {notification.Subscription.Name}</p>");
            sb.AppendLine("</div>");
        }
        
        sb.AppendLine("<div class='footer'>");
        sb.AppendLine("<p><strong>Cervantes CVE Management System</strong></p>");
        sb.AppendLine("<p>This is an automated security notification. Please review and take appropriate action if necessary.</p>");
        sb.AppendLine("<p>If you no longer wish to receive these notifications, please update your CVE subscription settings in Cervantes.</p>");
        sb.AppendLine("</div>");
        
        sb.AppendLine("</div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }
}