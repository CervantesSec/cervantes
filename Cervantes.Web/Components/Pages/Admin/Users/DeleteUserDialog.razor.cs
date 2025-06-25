using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Users;

public partial class DeleteUserDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public UserViewModel user { get; set; } = new UserViewModel();
	 [Inject] private UserController _UserController { get; set; }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await _UserController.Delete(user.Id);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
            {
                Snackbar.Add(@localizer["userDeleted"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["userDeletedError"], Severity.Error);
            }
            
        }
    }
}