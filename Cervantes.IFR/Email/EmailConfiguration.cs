using System;

namespace Cervantes.IFR.Email;

public class EmailConfiguration : IEmailConfiguration
{
    public bool Enabled { get; set; }
    public string Name { get; set; }
    
    public string From { get; set; }
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }
}