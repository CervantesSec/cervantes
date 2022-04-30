﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE
{
    public class Report
    {

        /// <summary>
        /// Porject Note Id
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

        public virtual Project Project { get; set; }

        public int ProjectId { get; set; }

        /// <summary>
        /// Created Date
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Note Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Note description
        /// </summary>
        public string Description { get; set; }

        public string Version { get; set; }

        public string FilePath { get; set; }
        
        public Language Language { get; set; }
    }
}
