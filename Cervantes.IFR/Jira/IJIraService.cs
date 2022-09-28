using Cervantes.CORE;

namespace Cervantes.IFR.Jira;

public interface IJIraService
{
    public void CreateIssue(string vuln);
    public bool JiraEnabled();
}