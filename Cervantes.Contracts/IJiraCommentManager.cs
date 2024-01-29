using Cervantes.CORE;
using Cervantes.CORE.Entities;

namespace Cervantes.Contracts;

public interface IJiraCommentManager: IGenericManager<JiraComments>
{
    public JiraComments GetByJiraId(string id);
}