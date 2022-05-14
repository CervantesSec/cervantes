using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.CORE;

public enum VulnStatus
{
    Open = 0,
    Confirmed = 1,
    Accepted = 2,
    Resolved = 3,
    OutOfScope = 4,
    Invalid = 5
}