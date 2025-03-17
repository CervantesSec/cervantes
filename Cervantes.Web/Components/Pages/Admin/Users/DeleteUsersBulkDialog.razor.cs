using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Users;

public partial class DeleteUsersBulkDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public List<UserViewModel> users { get; set; }
    [Inject] private UserController _UserController { get; set; }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            foreach (var user in users)
            {
                var response = await _UserController.Delete(user.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
                {
                    Snackbar.Add(@localizer["userDeleted"], Severity.Success);
                }
                else
                {
                    Snackbar.Add(@localizer["userDeletedError"], Severity.Error);
                }
            }
            MudDialog.Close(DialogResult.Ok(true));

            
            
        }
    }
}