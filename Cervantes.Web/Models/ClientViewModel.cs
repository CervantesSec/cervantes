using Microsoft.AspNetCore.Http;
using System;

namespace Cervantes.Web.Models
{
    public class ClientViewModel
    {
        /// <summary>
        /// Porject Note Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Note Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Note description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Note Name
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Note description
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// Note description
        /// </summary>
        public string ContactEmail { get; set; }
        /// <summary>
        /// Note description
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// File Uploaded
        /// </summary>
        public IFormFile upload { get; set; }

        /// <summary>
        /// Client Image
        /// </summary>
        public string ImagePath { get; set; }
        /// <summary>
        /// Client created by
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Creation Date
        /// </summary>
        public DateTime CreatedDate { get; set; }


    }
}
