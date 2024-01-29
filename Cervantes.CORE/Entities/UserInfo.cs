using System;
using System.Collections.Generic;
using System.Text;

namespace Cervantes.CORE.Entities
{
    public class UserInfo
    {
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public Dictionary<string, string> ExposedClaims { get; set; }
    }
}
