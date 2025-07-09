using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class DeleteChecklistTemplateDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public ChecklistTemplate template { get; set; }
    
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private ChecklistController _checklistController { get; set; }
    [Inject] IStringLocalizer<Resource> localizer { get; set; }
    
    private bool _processing = false;
    
    void Cancel() => MudDialog.Cancel();

    private async System.Threading.Tasks.Task Submit()
    {
        _processing = true;
        
        try
        {
            var response = await _checklistController.DeleteCustomTemplate(template.Id);
            
            if (response is NoContentResult)
            {
                Snackbar.Add(@localizer["templateDeletedSuccessfully"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else if (response is ForbidResult)
            {
                Snackbar.Add(@localizer["cannotDeleteSystemTemplate"], Severity.Warning);
            }
            else
            {
                Snackbar.Add(@localizer["errorDeletingTemplate"], Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(@localizer["errorOccurred"], Severity.Error);
        }
        finally
        {
            _processing = false;
        }
    }
}