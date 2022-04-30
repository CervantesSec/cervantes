using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.CORE
{
    public class Target
    {
        /// <summary>
        /// Id Vuln
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// User who created project
        /// </summary>
        public virtual ApplicationUser User { get; set; }

        /// <summary>
        /// Id user
        /// </summary>
        [ForeignKey("User")]
        public string UserId { get; set; }

        /// <summary>
        /// Project Associated
        /// </summary>
        public virtual Project Project { get; set; }

        /// <summary>
        /// Id Project
        /// </summary>
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        /// <summary>
        /// Target Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Target Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Target Type
        /// </summary>
        public TargetType Type { get; set; }
        
    }
}
