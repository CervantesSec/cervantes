using System;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class ClientEditViewModel
{
      public Guid Id { get; set; }
      
      
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
    
          public string? FileName { get; set; }
          public byte[]? FileContent { get; set; }
          
          /// <summary>
          /// Custom field values as key-value pairs (CustomFieldId -> Value)
          /// </summary>
          public Dictionary<Guid, string> CustomFieldValues { get; set; } = new Dictionary<Guid, string>();

}