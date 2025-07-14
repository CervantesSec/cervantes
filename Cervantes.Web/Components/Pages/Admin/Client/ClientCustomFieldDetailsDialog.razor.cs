using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;
using System.Security.Claims;
using AuthPermissions.AspNetCore;
using Microsoft.AspNetCore.Components.Authorization;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Client;

public partial class ClientCustomFieldDetailsDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public ClientCustomFieldViewModel customFieldSelected { get; set; }
    [Inject] private ClientCustomFieldController ClientCustomFieldController { get; set; }
    [Inject] private IStringLocalizer<Resource> localizer { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; }
    
    MudForm form;
    ClientCustomFieldEditValidator validator = new ClientCustomFieldEditValidator();
    
    private ClientCustomFieldEditViewModel model = new ClientCustomFieldEditViewModel();
    private string optionsText = string.Empty;
    private ClaimsPrincipal userAth;

    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        
        // Initialize model with selected custom field data
        model.Id = customFieldSelected.Id;
        model.Name = customFieldSelected.Name;
        model.Label = customFieldSelected.Label;
        model.Type = customFieldSelected.Type;
        model.IsRequired = customFieldSelected.IsRequired;
        model.IsUnique = customFieldSelected.IsUnique;
        model.IsSearchable = customFieldSelected.IsSearchable;
        model.IsVisible = customFieldSelected.IsVisible;
        model.Order = customFieldSelected.Order;
        model.DefaultValue = customFieldSelected.DefaultValue ?? string.Empty;
        model.Description = customFieldSelected.Description ?? string.Empty;
        model.IsActive = customFieldSelected.IsActive;
        
        // Convert JSON options to text format for editing
        if (customFieldSelected.Type == ClientCustomFieldType.Select && !string.IsNullOrEmpty(customFieldSelected.Options))
        {
            try
            {
                var options = System.Text.Json.JsonSerializer.Deserialize<string[]>(customFieldSelected.Options);
                model.Options = string.Join("\n", options);
            }
            catch
            {
                model.Options = customFieldSelected.Options;
            }
        }
        else
        {
            model.Options = customFieldSelected.Options ?? string.Empty;
        }
    }

    void Cancel() => MudDialog.Cancel();

    private async Task Submit()
    {
        await form.Validate();
        
        if (form.IsValid)
        {
            // Convert options text to JSON array for Select fields
            if (model.Type == ClientCustomFieldType.Select && !string.IsNullOrEmpty(model.Options))
            {
                var options = model.Options.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(o => o.Trim())
                                        .Where(o => !string.IsNullOrEmpty(o))
                                        .ToArray();
                model.Options = System.Text.Json.JsonSerializer.Serialize(options);
            }
            else if (model.Type != ClientCustomFieldType.Select)
            {
                model.Options = string.Empty;
            }
            
            try
            {
                var result = await ClientCustomFieldController.Put(model);
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
    
    private async Task Delete(ClientCustomFieldViewModel customField)
    {
        try
        {
            var result = await ClientCustomFieldController.Delete(customField.Id);
            if (result is Microsoft.AspNetCore.Mvc.OkObjectResult)
            {
                Snackbar.Add($"{localizer["customFieldDeleted"]}", MudBlazor.Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add($"{localizer["customFieldDeleteError"]}", MudBlazor.Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"{localizer["customFieldDeleteError"]}: {ex.Message}", MudBlazor.Severity.Error);
        }
    }
    
    private string GetFieldTypeDisplay(ClientCustomFieldType type)
    {
        return type switch
        {
            ClientCustomFieldType.Input => localizer["input"],
            ClientCustomFieldType.Textarea => localizer["textarea"],
            ClientCustomFieldType.Select => localizer["select"],
            ClientCustomFieldType.Number => localizer["number"],
            ClientCustomFieldType.Date => localizer["date"],
            ClientCustomFieldType.Boolean => localizer["boolean"],
            _ => type.ToString()
        };
    }
}

public class ClientCustomFieldEditValidator : AbstractValidator<ClientCustomFieldEditViewModel>
{
    public ClientCustomFieldEditValidator()
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
            .When(x => x.Type == ClientCustomFieldType.Select)
            .WithMessage("Options are required for Select fields");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.DefaultValue)
            .MaximumLength(200)
            .WithMessage("Default value cannot exceed 200 characters");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<ClientCustomFieldEditViewModel>.CreateWithOptions((ClientCustomFieldEditViewModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}