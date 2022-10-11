using Cervantes.CORE;

namespace Cervantes.Contracts;

public interface IJiraCommentManager: IGenericManager<JiraComments>
{
    public JiraComments GetByJiraId(string id);
}