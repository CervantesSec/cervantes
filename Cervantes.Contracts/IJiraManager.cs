using System;
using Cervantes.CORE;
using Cervantes.CORE.Entities;

namespace Cervantes.Contracts;

public interface IJiraManager: IGenericManager<Jira>
{
    public Jira GetByJiraKey(string key);
    public Jira GetByVulnId(Guid id);
}