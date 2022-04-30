using Cervantes.CORE;
using System.Collections.Generic;

namespace Cervantes.Web.Models
{
    public class ClientDetailsViewModel
    {
        public Client Client { get; set; }
        public IEnumerable<Project> Project { get; set; }
    }
}
