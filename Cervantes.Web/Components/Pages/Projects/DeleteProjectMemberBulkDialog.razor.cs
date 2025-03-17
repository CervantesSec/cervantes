using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class DeleteProjectMemberBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public List<ProjectUser> users { get; set; } 
    [Inject] private ProjectController _projectController { get; set; }
	 

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            foreach (var user in users)
            {
                var response = await _projectController.DeleteMember(user.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")            
                {
                    Snackbar.Add(@localizer["memberDeleted"], Severity.Success);
                }
                else
                {
                    Snackbar.Add(@localizer["memberDeletedError"], Severity.Error);
                }
            }
            MudDialog.Close(DialogResult.Ok(true));
            
        }
    }
}