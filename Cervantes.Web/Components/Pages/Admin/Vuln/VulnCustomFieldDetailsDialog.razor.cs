using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Components.Pages.Documents;
using Cervantes.Web.Controllers;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Task = DocumentFormat.OpenXml.Office2021.DocumentTasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Vuln;

public partial class VulnCustomFieldDetailsDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public VulnCustomFieldViewModel customFieldSelected { get; set; }
    [Inject] private VulnCustomFieldController VulnCustomFieldController { get; set; }
    [Inject] private IStringLocalizer<Resource> localizer { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    
    MudForm form;
    VulnCustomFieldValidator validator = new VulnCustomFieldValidator();
    
    private VulnCustomFieldEditViewModel model = new VulnCustomFieldEditViewModel();
    private string optionsText = string.Empty;

    protected override void OnInitialized()
    {
        if (customFieldSelected != null)
        {
            // Populate model from existing custom field
            model.Id = customFieldSelected.Id;
            model.Name = customFieldSelected.Name;
            model.Label = customFieldSelected.Label;
            model.Type = customFieldSelected.Type;
            model.IsRequired = customFieldSelected.IsRequired;
            model.IsUnique = customFieldSelected.IsUnique;
            model.IsSearchable = customFieldSelected.IsSearchable;
            model.IsVisible = customFieldSelected.IsVisible;
            model.Order = customFieldSelected.Order;
            model.Options = customFieldSelected.Options;
            model.DefaultValue = customFieldSelected.DefaultValue;
            model.Description = customFieldSelected.Description;
            model.IsActive = customFieldSelected.IsActive;
            
            // Convert JSON options to text for editing
            if (!string.IsNullOrEmpty(customFieldSelected.Options))
            {
                try
                {
                    var options = System.Text.Json.JsonSerializer.Deserialize<string[]>(customFieldSelected.Options);
                    optionsText = string.Join("\n", options);
                }
                catch
                {
                    optionsText = customFieldSelected.Options;
                }
            }
        }
    }

    DialogOptionsEx middleWidthEx = new DialogOptionsEx() 
    {
        MaximizeButton = true,
        CloseButton = true,
        FullHeight = false,
        CloseOnEscapeKey = true,
        MaxWidth = MaxWidth.Medium,
        MaxHeight = MaxHeight.False,
        FullWidth = true,
        DragMode = MudDialogDragMode.Simple,
        Animations = new[] { AnimationType.SlideIn },
        Position = DialogPosition.Center,
        DisableSizeMarginY = true,
        DisablePositionMargin = true,
        BackdropClick = false,
        Resizeable = true,
    };
    
    private async System.Threading.Tasks.Task Delete(VulnCustomFieldViewModel cfv)
    {
        var parameters = new DialogParameters { ["customField"]=cfv };
        IMudExDialogReference<DeleteVulnCustomFieldDialog>? dlgReference = await DialogService.ShowExAsync<DeleteVulnCustomFieldDialog>("Simple Dialog", parameters, middleWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            MudDialog.Close(DialogResult.Ok(true));
            StateHasChanged();
        }
    }
    
    void Cancel() => MudDialog.Cancel();
    private ClaimsPrincipal userAth;
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
                var response = await VulnCustomFieldController.Edit(model);
                if (response != null)
                {
                    Snackbar.Add(localizer["customFieldUpdated"], MudBlazor.Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    Snackbar.Add(localizer["customFieldUpdateError"], MudBlazor.Severity.Error);
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

    
    
    public class VulnCustomFieldValidator : AbstractValidator<VulnCustomFieldEditViewModel>
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
            var result = await ValidateAsync(ValidationContext<VulnCustomFieldEditViewModel>.CreateWithOptions((VulnCustomFieldEditViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    
}