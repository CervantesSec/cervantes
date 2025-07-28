using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Contracts;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Cervantes.Web.Localization;
using MudBlazor;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Cervantes.Web.Components.Pages.Cve;

public partial class CveSubscriptionDialog
{
    [CascadingParameter] IDialogReference? MudDialog { get; set; } = default!;
    [Parameter] public CveSubscription? ExistingSubscription { get; set; }
    [Inject] private IConfiguration Configuration { get; set; } = default!;
    

    private MudForm? form;
    private CveSubscriptionViewModel model = new();
    private bool loading = false;
    private string newKeyword = "";
    private ICollection<string> selectedKeywords = new HashSet<string>();

    private readonly List<string> commonKeywords = new()
    {
        "privilege escalation", "remote code execution", "injection", "authentication bypass",
        "denial of service", "cross-site scripting", "sql injection", "buffer overflow",
        "arbitrary code", "memory corruption", "information disclosure", "backdoor"
    };

    private const string Immediate = "Immediate";
    private const string Daily = "Daily";
    private const string Weekly = "Weekly";
    private const string Email = "Email";
    private const string InApp = "InApp";

    private bool IsEditMode => ExistingSubscription != null;
    
    private bool IsEmailEnabled => Configuration.GetValue<bool>("EmailConfiguration:Enabled");
    
    private List<string> AvailableNotificationMethods
    {
        get
        {
            var methods = new List<string> { "InApp" }; // InApp is always available
            
            if (IsEmailEnabled)
                methods.Add("Email");
                
            return methods;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (IsEditMode && ExistingSubscription != null)
        {
            model = CveSubscriptionViewModel.FromEntity(ExistingSubscription);
            // Load existing keywords
            if (!string.IsNullOrEmpty(ExistingSubscription.Keywords))
            {
                try
                {
                    var keywords = System.Text.Json.JsonSerializer.Deserialize<List<string>>(ExistingSubscription.Keywords);
                    selectedKeywords = new HashSet<string>(keywords ?? new List<string>());
                }
                catch
                {
                    // Ignore JSON parsing errors
                }
            }
        }
        else
        {
            model.SetDefaults();
            // Set default notification method to first available option
            model.NotificationMethod = AvailableNotificationMethods.First();
        }
        
        // Validate that the current notification method is available
        if (!AvailableNotificationMethods.Contains(model.NotificationMethod))
        {
            model.NotificationMethod = AvailableNotificationMethods.First();
        }
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await AddKeyword();
        }
    }

    private async Task AddKeyword()
    {
        if (!string.IsNullOrWhiteSpace(newKeyword) && !selectedKeywords.Contains(newKeyword))
        {
            selectedKeywords.Add(newKeyword);
            newKeyword = "";
            StateHasChanged();
        }
    }

    private void ToggleKeyword(string keyword)
    {
        if (selectedKeywords.Contains(keyword))
        {
            selectedKeywords.Remove(keyword);
        }
        else
        {
            selectedKeywords.Add(keyword);
        }
        StateHasChanged();
    }

    private async Task SaveSubscription()
    {
        if (form == null) return;
        
        loading = true;
        try
        {
            await form.Validate();
            if (!form.IsValid)
            {
                return;
            }

            // Set keywords from selected chips
            model.Keywords = selectedKeywords.ToList();

            CveSubscription subscription;
            if (IsEditMode && ExistingSubscription != null)
            {
                subscription = model.ToEntity(ExistingSubscription.Id);
                subscription.UserId = ExistingSubscription.UserId;
                subscription.CreatedDate = ExistingSubscription.CreatedDate;
                
                subscription = await CveController.UpdateSubscriptionForComponentsAsync(subscription);
                Snackbar.Add($"CVE subscription {subscription.Name} updated successfully", Severity.Success);
            }
            else
            {
                subscription = model.ToEntity();
                subscription = await CveController.CreateSubscriptionForComponentsAsync(subscription);
                Snackbar.Add($"CVE subscription {subscription.Name} created successfully", Severity.Success);
            }

            MudDialog?.Close(DialogResult.Ok(subscription));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error saving subscription: {ex.Message}", Severity.Error);
        }
        finally
        {
            loading = false;
        }
    }

    private void Cancel()
    {
        MudDialog?.Close();
    }

    private Func<object, string, Task<IEnumerable<string>>> ValidateModel => async (model, propertyName) =>
    {
        var subscriptionModel = (CveSubscriptionViewModel)model;
        var errors = new List<string>();

        if (propertyName == nameof(CveSubscriptionViewModel.Name))
        {
            if (string.IsNullOrEmpty(subscriptionModel.Name))
            {
                errors.Add("Name is required");
            }
        }

        if (propertyName == nameof(CveSubscriptionViewModel.MinCvssScore) || propertyName == nameof(CveSubscriptionViewModel.MaxCvssScore))
        {
            if (subscriptionModel.MinCvssScore.HasValue && subscriptionModel.MaxCvssScore.HasValue)
            {
                if (subscriptionModel.MinCvssScore > subscriptionModel.MaxCvssScore)
                {
                    errors.Add("Minimum CVSS score cannot be greater than maximum CVSS score");
                }
            }
        }

        if (propertyName == nameof(CveSubscriptionViewModel.NotificationMethod))
        {
            if (!AvailableNotificationMethods.Contains(subscriptionModel.NotificationMethod))
            {
                errors.Add("Invalid notification method selected");
            }
        }

        return errors;
    };
}