using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class DeleteProjectMemberDialog: ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public ProjectUser user { get; set; } = new ProjectUser();
    [Inject] private ProjectController _projectController { get; set; }
	 

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await _projectController.DeleteMember(user.Id);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")            
            {
                Snackbar.Add(@localizer["memberDeleted"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["memberDeletedError"], Severity.Error);
            }
            
        }
    }
}