using Cervantes.CORE;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Cervantes.Web.Models
{
    public class UserViewModel : IdentityUser
    {
        /// <summary>
        /// User full name
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// User avatar
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// user Bio description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// User Position
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// User Position
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// User Position
        /// </summary>

        public string ConfirmPassword { get; set; }

        /// <summary>
        /// File Uploaded
        /// </summary>
        public IFormFile upload { get; set; }

        /// <summary>
        /// Client Image
        /// </summary>
        public string ImagePath { get; set; }

        public RoleList Role { get; set; }
        public IList<SelectListItem> ItemList { get; set; }
        public string Option { get; set; }

        public IEnumerable<Project> Project { get; set; }
    }
}
