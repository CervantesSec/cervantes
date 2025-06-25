using System.Security.Claims;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Components.Pages.Admin.Users;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Admin.Roles;

public partial class RoleDialog : ComponentBase
{
    [Parameter] public RolesViewModel role { get; set; } 
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    private bool editMode = false;
    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private UserController _UserController { get; set; }

    MudForm form;
    private CreateRoleViewModel model { get; set; } = new CreateRoleViewModel();
    ClaimsPrincipal userAth;
    List<PermissionsViewModel> _permissionsViewModels = new List<PermissionsViewModel>();
    HashSet<PermissionsViewModel> selectedItems = new HashSet<PermissionsViewModel>();
    RoleModelFluentValidator clientValidator = new RoleModelFluentValidator();
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
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
    }
    
    async Task DeleteDialog(RolesViewModel roles,DialogOptions options)
    {
        var parameters = new DialogParameters { ["role"]=roles };
        IMudExDialogReference<DeleteRoleDialog>? dlgReference = await Dialog.ShowExAsync<DeleteRoleDialog>("Simple Dialog", parameters, middleWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            MudDialog.Close();
        }
    }

    private async Task EditMode()
    {
        if (editMode)
        {
            editMode = false;
            
        }
        else
        {
            editMode = true;
            _permissionsViewModels = new List<PermissionsViewModel>();
            _permissionsViewModels = _UserController.GetPermissions().ToList();
            model.Name = role.Name;
            model.Permissions = role.PermissionNames;
            model.Description = role.Description;
            selectedItems = _permissionsViewModels.Where(x => model.Permissions.Contains(x.Name)).ToHashSet();
            
        }
        MudDialog.StateHasChanged();
    }
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {

            var response = await _UserController.EditRole(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
            {
                Snackbar.Add(localizer["roleEdited"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(localizer["roleEditedError"], Severity.Error);
                StateHasChanged();
            }
            
        }
    }
    public class RoleModelFluentValidator : AbstractValidator<CreateRoleViewModel>
    {
        public RoleModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);
            RuleFor(x => x.Permissions)
                .NotEmpty();
   
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<CreateRoleViewModel>.CreateWithOptions((CreateRoleViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    void SelectedItemsChanged(HashSet<PermissionsViewModel> items)
    {
        model.Permissions = items.Select(item => item.Name).ToList();
    }
    
    private string searchString = "";
    private Func<PermissionsViewModel, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    };
}