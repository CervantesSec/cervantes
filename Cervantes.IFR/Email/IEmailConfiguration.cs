using System;

namespace Cervantes.IFR.Email;

public interface IEmailConfiguration
{
    
    public bool Enabled { get; set; }
    public string Name { get; set; }
    
    public string From { get; set; }
    string SmtpServer { get; }
    int SmtpPort { get; }
    string SmtpUsername { get; set; }
    string SmtpPassword { get; set; }
}