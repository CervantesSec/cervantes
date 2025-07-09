using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class DeleteCustomChecklistDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public CORE.Entities.Checklist checklist { get; set; }
    
    void Cancel() => MudDialog.Cancel();
    
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private ChecklistController _checklistController { get; set; }
    [Inject] IStringLocalizer<Resource> localizer { get; set; }
    
    bool _processing = false;

    private async System.Threading.Tasks.Task Submit()
    {
        _processing = true;
        
        try
        {
            var response = await _checklistController.DeleteCustomChecklist(checklist.Id);
            
            if (response is OkResult)
            {
                Snackbar.Add(@localizer["checklistDeletedSuccessfully"], MudBlazor.Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else if (response is BadRequestObjectResult badRequestResult)
            {
                Snackbar.Add(@localizer["errorDeletingChecklist"], MudBlazor.Severity.Error);
            }
            else
            {
                Snackbar.Add(@localizer["errorOccurred"], MudBlazor.Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(@localizer["errorOccurred"], MudBlazor.Severity.Error);
        }
        finally
        {
            _processing = false;
        }
    }
}