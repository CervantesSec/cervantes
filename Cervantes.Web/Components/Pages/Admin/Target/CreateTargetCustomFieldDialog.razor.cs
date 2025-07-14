using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;

namespace Cervantes.Web.Components.Pages.Admin.Target;

public partial class CreateTargetCustomFieldDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Inject] private TargetCustomFieldController TargetCustomFieldController { get; set; }
    [Inject] private IStringLocalizer<Resource> localizer { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    
    MudForm form;
    TargetCustomFieldCreateValidator validator = new TargetCustomFieldCreateValidator();
    
    private TargetCustomFieldCreateViewModel model = new TargetCustomFieldCreateViewModel();

    protected override void OnInitialized()
    {
        // Set defaults for new field
        model.IsSearchable = true;
        model.IsVisible = true;
        model.Order = 1;
        model.Options = string.Empty;
        model.DefaultValue = string.Empty;
        model.Description = string.Empty;
    }

    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            // Convert options text to pipe-separated format for Select fields
            if (model.Type == TargetCustomFieldType.Select && !string.IsNullOrEmpty(model.Options))
            {
                var options = model.Options.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(o => o.Trim())
                                        .Where(o => !string.IsNullOrEmpty(o))
                                        .ToArray();
                model.Options = string.Join("|", options);
            }
            else if (model.Type != TargetCustomFieldType.Select)
            {
                model.Options = string.Empty;
            }
            
            try
            {
                var result = await TargetCustomFieldController.Post(model);
                if (result is Microsoft.AspNetCore.Mvc.CreatedAtActionResult)
                {
                    Snackbar.Add($"{localizer["customFieldCreated"]}", MudBlazor.Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    Snackbar.Add($"{localizer["customFieldCreateError"]}", MudBlazor.Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"{localizer["customFieldCreateError"]}: {ex.Message}", MudBlazor.Severity.Error);
            }
        }
    }
    
    private string GetFieldTypeDisplay(TargetCustomFieldType type)
    {
        return type switch
        {
            TargetCustomFieldType.Input => localizer["input"],
            TargetCustomFieldType.Textarea => localizer["textarea"],
            TargetCustomFieldType.Select => localizer["select"],
            TargetCustomFieldType.Number => localizer["number"],
            TargetCustomFieldType.Date => localizer["date"],
            TargetCustomFieldType.Boolean => localizer["boolean"],
            _ => type.ToString()
        };
    }
}

public class TargetCustomFieldCreateValidator : AbstractValidator<TargetCustomFieldCreateViewModel>
{
    public TargetCustomFieldCreateValidator()
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

        RuleFor(x => x.Options)
            .NotEmpty()
            .When(x => x.Type == TargetCustomFieldType.Select)
            .WithMessage("Options are required for Select fields");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.DefaultValue)
            .MaximumLength(200)
            .WithMessage("Default value cannot exceed 200 characters");
    }
    
    public Func<object, string, System.Threading.Tasks.Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<TargetCustomFieldCreateViewModel>.CreateWithOptions((TargetCustomFieldCreateViewModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}