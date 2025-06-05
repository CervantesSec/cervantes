using System.Security.Claims;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.Jira;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class VulnCategoryDialog: ComponentBase
{
     [Inject] private VulnController _vulnController { get; set; }
    [Parameter] public CORE.Entities.VulnCategory category { get; set; }

    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    
    private VulnCategoryEdit model = new VulnCategoryEdit();
    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool editMode = false;
    ClaimsPrincipal userAth;
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
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
    }
    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {

            var response = await _vulnController.EditCategory(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
            {

                Snackbar.Add(@localizer["categoryEdited"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["categoryEditedError"], Severity.Error);
            }

        }
    }
    
    void EditMode()
    {
        if (editMode)
        {
            editMode = false;
        }
        else
        {
            editMode = true;
            model = new VulnCategoryEdit();
            model.Id = category.Id;
            model.Name = category.Name;
            model.Description = category.Description;
            model.Type = category.Type;

        }
        MudDialog.StateHasChanged();
    }
    
    private async Task DeleteVulnCategoryDialog(CORE.Entities.VulnCategory category,DialogOptions options)
    {
        var parameters = new DialogParameters { ["category"]=category };
        IMudExDialogReference<DeleteVulnCategoryDialog>? dlgReference = await Dialog.ShowExAsync<DeleteVulnCategoryDialog>("Simple Dialog", parameters, centerWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            MudDialog.Close(DialogResult.Ok(true));
            StateHasChanged();
        }
    }
    
    VulnCategoryModelFluentValidator vulnValidator = new VulnCategoryModelFluentValidator();

    public class VulnCategoryModelFluentValidator : AbstractValidator<VulnCategoryEdit>
    {
        public VulnCategoryModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<VulnCategoryEdit>.CreateWithOptions((VulnCategoryEdit)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}