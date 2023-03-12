namespace Cervantes.IFR.Jira;

public interface IJiraConfiguration
{
    public bool Enabled { get; set; }
    public string Url { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public string Project { get; set; }
    public string Auth { get; set; }
    public string ConsumerKey { get; set; }
    public string ConsumerSecret { get; set; }
    public string OAuthAccessToken { get; set; }
    public string OAuthTokenSecret { get; set; }
 
}