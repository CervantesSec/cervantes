using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Admin.Roles;

public partial class CreateRole : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
     void Cancel() => MudDialog.Cancel();
     
     [Inject] ISnackbar Snackbar { get; set; }
     [Inject] private UserController _UserController { get; set; }

    MudForm form;

    RoleModelFluentValidator clientValidator = new RoleModelFluentValidator();
    private long maxFileSize = 1024 * 1024 * 5;
    CreateRoleViewModel model = new CreateRoleViewModel();
    List<PermissionsViewModel> _permissionsViewModels = new List<PermissionsViewModel>();
	 private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
	        var response = await _UserController.AddRole(model);
	        if (response.ToString() == "Microsoft.AspNetCore.Mvc.CreatedAtActionResult")
	        {
		        Snackbar.Add(@localizer["roleCreated"], Severity.Success);
		        MudDialog.Close(DialogResult.Ok(true));
	        }
	        else
	        {
		        Snackbar.Add(@localizer["roleCreatedError"], Severity.Error);
	        }
            
        }
    }
	 
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_permissionsViewModels = _UserController.GetPermissions().ToList();
		StateHasChanged();
	}
    /// <summary>
    /// A standard AbstractValidator which contains multiple rules and can be shared with the back end API
    /// </summary>
    /// <typeparam name="ClientModel"></typeparam>
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