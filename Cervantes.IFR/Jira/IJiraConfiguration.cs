namespace Cervantes.IFR.Jira;

public interface IJiraConfiguration
{
    public bool Enabled { get; set; }
    public string Url { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
 
}