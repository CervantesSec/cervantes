using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Mvc;

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
            var deleted = 0;
            foreach (var user in users)
            {
                var response = await _UserController.Delete(user.Id);
                if (response is NoContentResult)
                {
                    deleted++;
                    Snackbar.Add(@localizer["userDeleted"], Severity.Success);
                }
                else if (response is ConflictObjectResult)
                {
                    Snackbar.Add($"{user.FullName}: {localizer["userHasContent"]}", Severity.Warning);
                }
                else
                {
                    Snackbar.Add(@localizer["userDeletedError"], Severity.Error);
                }
            }
            // The list only needs reloading when something was deleted; otherwise the dialog stays for another try.
            if (deleted > 0)
            {
                MudDialog.Close(DialogResult.Ok(true));
            }

            
            
        }
    }
}