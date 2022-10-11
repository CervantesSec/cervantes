using System;
using System.Linq;
using Cervantes.Contracts;
using Cervantes.CORE;

namespace Cervantes.Application;

public class JiraManager : GenericManager<Jira>, IJiraManager
{
    /// <summary>
    /// Client Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public JiraManager(IApplicationDbContext context) : base(context)
    {
    }
    
    public Jira GetByJiraKey(string key)
    {
        return Context.Set<CORE.Jira>().Where(e => e.JiraKey == key).FirstOrDefault();
    }

    public Jira GetByVulnId(Guid id)
    {
        return Context.Set<CORE.Jira>().Where(e => e.VulnId == id).FirstOrDefault();
    }
    
}