using System;
using Cervantes.CORE;

namespace Cervantes.IFR.Jira;

public interface IJIraService
{
    public bool CreateIssue(Guid vuln, string user);
    public bool JiraEnabled();
    public bool DeleteIssue(string key);

    public void UpdateIssue(string key);
    public Task<bool> AddCommentAsync(string key, string comment);
}