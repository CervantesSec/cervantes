using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class DeleteCustomChecklistBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public IEnumerable<CORE.Entities.Checklist> checklists { get; set; }
    
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
            int successCount = 0;
            int errorCount = 0;
            
            foreach (var checklist in checklists)
            {
                try
                {
                    var response = await _checklistController.DeleteCustomChecklist(checklist.Id);
                    
                    if (response is OkResult)
                    {
                        successCount++;
                    }
                    else
                    {
                        errorCount++;
                    }
                }
                catch
                {
                    errorCount++;
                }
            }
            
            if (errorCount == 0)
            {
                Snackbar.Add($"{successCount} {localizer["checklistsDeletedSuccessfully"]}", MudBlazor.Severity.Success);
            }
            else if (successCount > 0)
            {
                Snackbar.Add($"{successCount} {localizer["checklistsDeleted"]}, {errorCount} {localizer["errors"]}", MudBlazor.Severity.Warning);
            }
            else
            {
                Snackbar.Add(@localizer["errorDeletingChecklists"], MudBlazor.Severity.Error);
            }
            
            MudDialog.Close(DialogResult.Ok(successCount > 0));
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