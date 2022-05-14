using Cervantes.CORE;
using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.Application;

public class NoteManager : GenericManager<Note>, INoteManager
{
    /// <summary>
    /// Note Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public NoteManager(IApplicationDbContext context) : base(context)
    {
    }
}