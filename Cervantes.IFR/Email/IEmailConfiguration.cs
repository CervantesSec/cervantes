using System;

namespace Cervantes.IFR.Email;

public interface IEmailConfiguration
{
    string SmtpServer { get; }
    int SmtpPort { get; }
    string SmtpUsername { get; set; }
    string SmtpPassword { get; set; }
}