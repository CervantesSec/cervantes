using Cervantes.CORE;
using Atlassian;
using Atlassian.Jira;

namespace Cervantes.IFR.Jira;

public class JiraService: IJIraService
{
    private readonly IJiraConfiguration _jiraConfiguration;

    public JiraService(IJiraConfiguration jiraConfiguration)
    {
        _jiraConfiguration = jiraConfiguration;
    }

    public void CreateIssue(Vuln vuln)
    {
        if (_jiraConfiguration.Enabled == false)
        {
            return;
        }

        var jira = Atlassian.Jira.Jira.CreateRestClient(_jiraConfiguration.Url, _jiraConfiguration.User,
            _jiraConfiguration.Password);
        
        var issue = jira.CreateIssue("My Project");
        issue.Type = "Bug";
        issue.Priority = "Major";
        issue.Summary = "Issue Summary";

        issue.SaveChangesAsync().Wait();
        

    }
}