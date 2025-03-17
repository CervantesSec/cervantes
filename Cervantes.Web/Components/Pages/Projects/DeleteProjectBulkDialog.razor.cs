using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class DeleteProjectBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public List<CORE.Entities.Project> projects { get; set; }
    [Inject] private ProjectController _ProjectController { get; set; }
    
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            foreach (var project in projects)
            {
                var response = await _ProjectController.Delete(project.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
                {
                    Snackbar.Add(@localizer["projectDeleted"], Severity.Success);
                }
                else
                {
                    Snackbar.Add(@localizer["projectDeletedError"], Severity.Error);
                }
            }
            MudDialog.Close(DialogResult.Ok(true));

            
            
        }
    }
}