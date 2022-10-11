using System;
using Cervantes.CORE;

namespace Cervantes.IFR.Jira;

public interface IJIraService
{
    public void CreateIssue(Guid vuln, string user);
    public bool JiraEnabled();

    public void Issue(string key);
}