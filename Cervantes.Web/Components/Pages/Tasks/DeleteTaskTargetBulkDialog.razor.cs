using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Tasks;

public partial class DeleteTaskTargetBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] TaskController _taskController { get; set; }
    MudForm form;


    [Parameter] public List<TaskTargets> targets { get; set; }
	 

    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {

            foreach (var target in targets)
            {
                var response = await _taskController.DeleteTarget(target.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
                {
                    Snackbar.Add(@localizer["targetDeleted"], Severity.Success);
                }
                else
                {
                    Snackbar.Add(@localizer["targetDeletedError"], Severity.Error);
                }
            }
            MudDialog.Close(DialogResult.Ok(true));

            
        }
    }
}
