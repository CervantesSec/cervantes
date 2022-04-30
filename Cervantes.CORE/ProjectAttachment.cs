using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.CORE
{
    public class ProjectAttachment
    {
        /// <summary>
        /// Project Attachment Id
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// ProjectAttachment Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ProjectAttachment Path
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// User who created the attachment
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
    }
}
