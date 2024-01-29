using System;
using System.Collections.Generic;
using System.Linq;
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
}