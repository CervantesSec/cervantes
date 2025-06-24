using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class DeleteChecklistBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public List<ChecklistViewModel> checklists { get; set; }
    [Inject] private ChecklistController _ChecklistController { get; set; }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            foreach (var checklist in checklists)
            {
                if (checklist.Type == ChecklistType.OWASPWSTG)
                {
                    var response = await _ChecklistController.DeleteWstg(checklist.Id);
                    if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
                    {
                        Snackbar.Add(@localizer["deletedChecklist"], Severity.Success);
                    }
                    else
                    {
                        Snackbar.Add(@localizer["deletedChecklistError"], Severity.Error);
                    }
                }
                else
                {
                    var response = await _ChecklistController.DeleteMastg(checklist.Id);
                    if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
                    {
                        Snackbar.Add(@localizer["deletedChecklist"], Severity.Success);
                    }
                    else
                    {
                        Snackbar.Add(@localizer["deletedChecklistError"], Severity.Error);
                    }
                }
                
            }
            MudDialog.Close(DialogResult.Ok(true));

            
        }
    }
}