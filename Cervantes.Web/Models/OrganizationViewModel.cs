using Microsoft.AspNetCore.Http;

namespace Cervantes.Web.Models
{
    public class OrganizationViewModel
    {
        /// <summary>
        /// Organization Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Organization Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Organization Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Organization Url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Organization Contact Name
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// Organization contact email
        /// </summary>
        public string ContactEmail { get; set; }
        /// <summary>
        /// Organization contact phone
        /// </summary>
        public string ContactPhone { get; set; }
        /// <summary>
        /// File Uploaded
        /// </summary>
        public IFormFile upload { get; set; }

        /// <summary>
        /// Organization Image
        /// </summary>
        public string ImagePath { get; set; }

    }
}
