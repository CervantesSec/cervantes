using System;
using Cervantes.CORE;

namespace Cervantes.Contracts;

public interface IJiraManager: IGenericManager<Jira>
{
    public Jira GetByJiraKey(string key);
    public Jira GetByVulnId(Guid id);
}