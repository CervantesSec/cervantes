using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class CveSubscriptionViewModel
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";
    
    [StringLength(500)]
    public string Description { get; set; } = "";
    
    public bool IsActive { get; set; } = true;
    
    [StringLength(100)]
    public string Vendor { get; set; } = "";
    
    [StringLength(100)]
    public string Product { get; set; } = "";
    
    [Range(0.0, 10.0)]
    public double? MinCvssScore { get; set; }
    
    [Range(0.0, 10.0)]
    public double? MaxCvssScore { get; set; }
    
    [Range(0.0, 1.0)]
    public double? MinEpssScore { get; set; }
    
    public bool OnlyKnownExploited { get; set; } = false;
    
    public List<string> Keywords { get; set; } = new();
    
    public List<string> CweFilter { get; set; } = new();
    
    [Required]
    public string NotificationFrequency { get; set; } = "Daily";
    
    [Required]
    public string NotificationMethod { get; set; } = "Email";
    
    [Url]
    public string WebhookUrl { get; set; } = "";
    
    public Guid? ProjectId { get; set; }

    public void SetDefaults()
    {
        Name = "";
        Description = "";
        IsActive = true;
        Vendor = "";
        Product = "";
        MinCvssScore = null;
        MaxCvssScore = null;
        MinEpssScore = null;
        OnlyKnownExploited = false;
        Keywords = new List<string>();
        CweFilter = new List<string>();
        NotificationFrequency = "Daily";
        NotificationMethod = "Email";
        WebhookUrl = "";
        ProjectId = null;
    }

    public CveSubscription ToEntity(Guid? id = null)
    {
        return new CveSubscription
        {
            Id = id ?? Guid.NewGuid(),
            Name = Name ?? "",
            Description = Description ?? "",
            IsActive = IsActive,
            Vendor = Vendor ?? "",
            Product = Product ?? "",
            MinCvssScore = MinCvssScore,
            MaxCvssScore = MaxCvssScore,
            MinEpssScore = MinEpssScore,
            OnlyKnownExploited = OnlyKnownExploited,
            Keywords = Keywords.Any() ? JsonSerializer.Serialize(Keywords) : "[]",
            CweFilter = CweFilter.Any() ? JsonSerializer.Serialize(CweFilter) : "[]",
            NotificationFrequency = NotificationFrequency ?? "Daily",
            NotificationMethod = NotificationMethod ?? "Email",
            WebhookUrl = WebhookUrl ?? "",
            ProjectId = ProjectId,
            ModifiedDate = DateTime.UtcNow
        };
    }

    public static CveSubscriptionViewModel FromEntity(CveSubscription entity)
    {
        var viewModel = new CveSubscriptionViewModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive,
            Vendor = entity.Vendor ?? "",
            Product = entity.Product ?? "",
            MinCvssScore = entity.MinCvssScore,
            MaxCvssScore = entity.MaxCvssScore,
            MinEpssScore = entity.MinEpssScore,
            OnlyKnownExploited = entity.OnlyKnownExploited,
            NotificationFrequency = entity.NotificationFrequency,
            NotificationMethod = entity.NotificationMethod,
            WebhookUrl = entity.WebhookUrl ?? "",
            ProjectId = entity.ProjectId
        };

        // Deserialize keywords
        if (!string.IsNullOrEmpty(entity.Keywords))
        {
            try
            {
                viewModel.Keywords = JsonSerializer.Deserialize<List<string>>(entity.Keywords) ?? new List<string>();
            }
            catch
            {
                viewModel.Keywords = new List<string>();
            }
        }

        // Deserialize CWE filter
        if (!string.IsNullOrEmpty(entity.CweFilter))
        {
            try
            {
                viewModel.CweFilter = JsonSerializer.Deserialize<List<string>>(entity.CweFilter) ?? new List<string>();
            }
            catch
            {
                viewModel.CweFilter = new List<string>();
            }
        }

        return viewModel;
    }

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(Name))
        {
            errors.Add("Name is required");
        }

        if (MinCvssScore.HasValue && MaxCvssScore.HasValue && MinCvssScore > MaxCvssScore)
        {
            errors.Add("Minimum CVSS score cannot be greater than maximum CVSS score");
        }

        if (MinCvssScore.HasValue && (MinCvssScore < 0 || MinCvssScore > 10))
        {
            errors.Add("Minimum CVSS score must be between 0.0 and 10.0");
        }

        if (MaxCvssScore.HasValue && (MaxCvssScore < 0 || MaxCvssScore > 10))
        {
            errors.Add("Maximum CVSS score must be between 0.0 and 10.0");
        }

        if (MinEpssScore.HasValue && (MinEpssScore < 0 || MinEpssScore > 1))
        {
            errors.Add("Minimum EPSS score must be between 0.0 and 1.0");
        }

        if (NotificationMethod == "Webhook" && string.IsNullOrEmpty(WebhookUrl))
        {
            errors.Add("Webhook URL is required when notification method is Webhook");
        }

        if (!string.IsNullOrEmpty(WebhookUrl) && !Uri.IsWellFormedUriString(WebhookUrl, UriKind.Absolute))
        {
            errors.Add("Invalid webhook URL format");
        }

        return errors;
    }
}