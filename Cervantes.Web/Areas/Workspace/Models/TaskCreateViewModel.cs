using Cervantes.CORE;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Cervantes.Web.Areas.Workspace.Models
{
    public class TaskCreateViewModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Is vuln template
        /// </summary>
        public bool Template { get; set; }
        /// <summary>
        /// User who created project
        /// </summary>
        public virtual ApplicationUser User { get; set; }

        /// <summary>
        /// task created by
        /// </summary>
        public string CreatedUserId { get; set; }
        /// <summary>
        /// User asigned project
        /// </summary>
        public virtual ApplicationUser User2 { get; set; }
        /// <summary>
        /// Asined user id
        /// </summary>
        public string AsignedUserId { get; set; }

        /// <summary>
        /// Project Associated
        /// </summary>
        public virtual Project Project { get; set; }

        /// <summary>
        /// Id Project
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Task Start Date
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Task End Date
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Task Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Task Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Target Associated
        /// </summary>
        public virtual Target Target { get; set; }

        /// <summary>
        /// Id Target
        /// </summary>
        public int TargetId { get; set; }

        /// <summary>
        /// Task status
        /// </summary>
        public TaskStatus Status { get; set; }

        public string TargetName { get; set; }
        public IList<SelectListItem> TargetList { get; set; }
        public IList<SelectListItem> UsersList { get; set; }

    }
}
