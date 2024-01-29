using System.Linq;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class JiraCommentManager : GenericManager<JiraComments>, IJiraCommentManager
{
    /// <summary>
    /// Client Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public JiraCommentManager(IApplicationDbContext context) : base(context)
    {
    }
    
    public JiraComments GetByJiraId(string id)
    {
        return Context.Set<CORE.Entities.JiraComments>().Where(e => e.JiraIdComment == id).FirstOrDefault();
    }
}