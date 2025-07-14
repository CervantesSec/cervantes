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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;

namespace Cervantes.Web.Components.Pages.Admin.Target;

public partial class TargetCustomFieldDetailsDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public TargetCustomFieldViewModel customFieldSelected { get; set; }
    [Inject] private TargetCustomFieldController TargetCustomFieldController { get; set; }
    [Inject] private IStringLocalizer<Resource> localizer { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private IAuthorizationService AuthorizationService { get; set; }
    [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; }
    [Inject] private IDialogService Dialog { get; set; }
    
    MudForm form;
    TargetCustomFieldEditValidator validator = new TargetCustomFieldEditValidator();
    
    private TargetCustomFieldEditViewModel model = new TargetCustomFieldEditViewModel();
    private string optionsText = string.Empty;
    private ClaimsPrincipal userAth;

    DialogOptionsEx centerWidthEx = new DialogOptionsEx() 
    {
        MaximizeButton = true,
        CloseButton = true,
        FullHeight = true,
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
    
    protected override async System.Threading.Tasks.Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        
        // Load the custom field data into the model
        model.Id = customFieldSelected.Id;
        model.Name = customFieldSelected.Name;
        model.Label = customFieldSelected.Label;
        model.Type = customFieldSelected.Type;
        model.Description = customFieldSelected.Description;
        model.DefaultValue = customFieldSelected.DefaultValue;
        model.Options = customFieldSelected.Options;
        model.IsRequired = customFieldSelected.IsRequired;
        model.IsUnique = customFieldSelected.IsUnique;
        model.IsSearchable = customFieldSelected.IsSearchable;
        model.IsVisible = customFieldSelected.IsVisible;
        model.Order = customFieldSelected.Order;
        
        // Convert options to text for editing
        if (!string.IsNullOrEmpty(customFieldSelected.Options))
        {
            optionsText = customFieldSelected.Options.Replace("|", "\n");
        }
    }
    
    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            // Convert options text back to pipe-separated format
            if (model.Type == TargetCustomFieldType.Select && !string.IsNullOrEmpty(optionsText))
            {
                var options = optionsText.Split('\n', StringSplitOptions.RemoveEmptyEntries)
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
                var response = await TargetCustomFieldController.Put(model);
                if (response is Microsoft.AspNetCore.Mvc.NoContentResult)
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
                Snackbar.Add($"{localizer["customFieldUpdateError"]}: {ex.Message}", MudBlazor.Severity.Error);
            }
        }
    }
    
    private async System.Threading.Tasks.Task Delete(TargetCustomFieldViewModel customField)
    {
        var parameters = new DialogParameters { ["customField"] = customField };
        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
        
        var dialog = await Dialog.ShowAsync<DeleteTargetCustomFieldDialog>(localizer["deleteCustomField"], parameters, options);
        var result = await dialog.Result;
        
        if (!result.Canceled)
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
    }
}

public class TargetCustomFieldEditValidator : AbstractValidator<TargetCustomFieldEditViewModel>
{
    public TargetCustomFieldEditValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(1, 100);
            
        RuleFor(x => x.Label)
            .NotEmpty()
            .Length(1, 100);
            
        RuleFor(x => x.Order)
            .GreaterThan(0);
    }
    
    public Func<object, string, System.Threading.Tasks.Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<TargetCustomFieldEditViewModel>.CreateWithOptions((TargetCustomFieldEditViewModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}