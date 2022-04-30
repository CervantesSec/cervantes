using Cervantes.CORE;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Cervantes.Web.Models
{
    public class ProjectDetailsViewModel
    {
        public Project Project { get; set; }
        public IEnumerable<ProjectUser> ProjectUsers { get; set; }
        public IEnumerable<Target> Targets { get; set; }
        public IEnumerable<CORE.Task> Tasks { get; set; }
        public IEnumerable<ProjectNote> ProjectNotes { get; set; }
        public IEnumerable<ProjectAttachment> ProjectAttachments { get; set; }

        public IEnumerable<Vuln> Vulns { get; set; }

        public IEnumerable<Report> Reports { get; set; }

        public IList<SelectListItem> Users { get; set; }

        public TargetType TargetType { get; set; }
        public Visibility Visibility { get; set; }
        
        public Language Language { get; set; }

        /// <summary>
        /// File Uploaded
        /// </summary>
        public IFormFile upload { get; set; }
    }
}
