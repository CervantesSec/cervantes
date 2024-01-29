using System;
using System.Collections.Generic;

using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace Cervantes.CORE.ViewModels;

public class ClientViewModel
{
  /// <summary>
      /// Client Id
      /// </summary>

      public Guid Id { get; set; }
  
      /// <summary>
      /// User who created project
      /// </summary>
      public virtual ApplicationUser User { get; set; }
  
      /// <summary>
      /// Id user
      /// </summary>
      public string UserId { get; set; }
  
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
      /// Client Image
      /// </summary>
      public string ImagePath { get; set; }
      
      public IBrowserFile File { get; set; }
      public string FileName { get; set; }
      public byte[] FileContent { get; set; }
}