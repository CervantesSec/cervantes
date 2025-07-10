using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Project;

public partial class ProjectCustomFieldDetailsDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public ProjectCustomFieldViewModel customFieldSelected { get; set; }
    [Inject] private ProjectCustomFieldController ProjectCustomFieldController { get; set; }
    [Inject] private IStringLocalizer<Resource> localizer { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; }
    
    MudForm form;
    ProjectCustomFieldEditValidator validator = new ProjectCustomFieldEditValidator();
    
    private ProjectCustomFieldEditViewModel model = new ProjectCustomFieldEditViewModel();
    private string optionsText = string.Empty;
    private ClaimsPrincipal userAth;

    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        
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

    void Cancel() => MudDialog.Cancel();

    private async Task Submit()
    {
        await form.Validate();
        
        if (form.IsValid)
        {
            // Convert options text to JSON array for Select fields
            if (model.Type == ProjectCustomFieldType.Select && !string.IsNullOrEmpty(optionsText))
            {
                var options = optionsText.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(o => o.Trim())
                                        .Where(o => !string.IsNullOrEmpty(o))
                                        .ToArray();
                model.Options = System.Text.Json.JsonSerializer.Serialize(options);
            }
            else if (model.Type != ProjectCustomFieldType.Select)
            {
                model.Options = null;
            }
            
            try
            {
                var result = await ProjectCustomFieldController.Put(model);
                if (result is Microsoft.AspNetCore.Mvc.OkObjectResult)
                {
                    Snackbar.Add($"{localizer["customFieldUpdated"]}", MudBlazor.Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    Snackbar.Add($"{localizer["customFieldUpdateError"]}", MudBlazor.Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"{localizer["customFieldUpdateError"]}: {ex.Message}", MudBlazor.Severity.Error);
            }
        }
    }
    
    private async Task Delete(ProjectCustomFieldViewModel customField)
    {
        var parameters = new DialogParameters { ["customField"] = customField };
        IMudExDialogReference<DeleteProjectCustomFieldDialog>? dlgReference = await DialogService.ShowExAsync<DeleteProjectCustomFieldDialog>(localizer["delete"], parameters, middleWidthEx);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
    }
    
    private string GetFieldTypeDisplay(ProjectCustomFieldType type)
    {
        return type switch
        {
            ProjectCustomFieldType.Input => localizer["input"],
            ProjectCustomFieldType.Textarea => localizer["textarea"],
            ProjectCustomFieldType.Select => localizer["select"],
            ProjectCustomFieldType.Number => localizer["number"],
            ProjectCustomFieldType.Date => localizer["date"],
            ProjectCustomFieldType.Boolean => localizer["boolean"],
            _ => type.ToString()
        };
    }
}

public class ProjectCustomFieldEditValidator : AbstractValidator<ProjectCustomFieldEditViewModel>
{
    public ProjectCustomFieldEditValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Label)
            .NotEmpty()
            .WithMessage("Label is required")
            .MaximumLength(200)
            .WithMessage("Label cannot exceed 200 characters");

        RuleFor(x => x.Type)
            .NotNull()
            .WithMessage("Type is required");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Order must be greater than or equal to 0");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.DefaultValue)
            .MaximumLength(200)
            .WithMessage("Default value cannot exceed 200 characters");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<ProjectCustomFieldEditViewModel>.CreateWithOptions((ProjectCustomFieldEditViewModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}