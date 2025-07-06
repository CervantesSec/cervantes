using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;

namespace Cervantes.Web.Components.Pages.Admin.Vuln;

public partial class CreateVulnCustomFieldDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Inject] private VulnCustomFieldController VulnCustomFieldController { get; set; }
    [Inject] private IStringLocalizer<Resource> localizer { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    
    MudForm form;
    VulnCustomFieldValidator validator = new VulnCustomFieldValidator();
    
    private VulnCustomFieldCreateViewModel model = new VulnCustomFieldCreateViewModel();
    private string optionsText = string.Empty;

    protected override void OnInitialized()
    {
        // Set defaults for new field
        model.IsSearchable = true;
        model.IsVisible = true;
        model.Order = 1;
    }

    void Cancel() => MudDialog.Cancel();

    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();
        
        if (form.IsValid)
        {
            // Convert options text to JSON array for Select fields
            if (model.Type == VulnCustomFieldType.Select && !string.IsNullOrEmpty(optionsText))
            {
                var options = optionsText.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(o => o.Trim())
                                        .Where(o => !string.IsNullOrEmpty(o))
                                        .ToArray();
                model.Options = System.Text.Json.JsonSerializer.Serialize(options);
            }
            else
            {
                model.Options = null;
            }
            
            try
            {
                var response = await VulnCustomFieldController.Add(model);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.CreatedAtActionResult")
                {
                    Snackbar.Add(localizer["customFieldCreated"], MudBlazor.Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    Snackbar.Add(localizer["customFieldCreateError"], MudBlazor.Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error: {ex.Message}", MudBlazor.Severity.Error);
            }
        }
    }

    private string GetFieldTypeDisplay(VulnCustomFieldType type)
    {
        return type switch
        {
            VulnCustomFieldType.Input => localizer["fieldTypeInput"],
            VulnCustomFieldType.Textarea => localizer["fieldTypeTextarea"],
            VulnCustomFieldType.Select => localizer["fieldTypeSelect"],
            VulnCustomFieldType.Number => localizer["fieldTypeNumber"],
            VulnCustomFieldType.Date => localizer["fieldTypeDate"],
            VulnCustomFieldType.Boolean => localizer["fieldTypeBoolean"],
            _ => type.ToString()
        };
    }

    public class VulnCustomFieldValidator : AbstractValidator<VulnCustomFieldCreateViewModel>
    {
        public VulnCustomFieldValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .Length(1, 100).WithMessage("Name must be between 1 and 100 characters")
                .Matches(@"^[a-zA-Z0-9\s_]+$").WithMessage("Name can only contain alphanumeric characters, spaces, and underscores");
                
            RuleFor(x => x.Label)
                .NotEmpty().WithMessage("Label is required")
                .Length(1, 200).WithMessage("Label must be between 1 and 200 characters");
                
            RuleFor(x => x.Order)
                .GreaterThan(0).WithMessage("Order must be greater than 0");
        }

        public Func<object, string, System.Threading.Tasks.Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<VulnCustomFieldCreateViewModel>.CreateWithOptions((VulnCustomFieldCreateViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}