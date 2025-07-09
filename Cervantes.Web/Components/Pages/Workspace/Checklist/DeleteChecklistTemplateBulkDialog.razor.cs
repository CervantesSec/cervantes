using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class DeleteChecklistTemplateBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public List<ChecklistTemplate> templates { get; set; } = new();
    
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
            int successCount = 0;
            int errorCount = 0;
            
            // Only delete non-system templates
            var templatesToDelete = templates.Where(t => !t.IsSystemTemplate).ToList();
            
            foreach (var template in templatesToDelete)
            {
                try
                {
                    var response = await _checklistController.DeleteCustomTemplate(template.Id);
                    
                    if (response is NoContentResult)
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
            
            if (successCount > 0)
            {
                Snackbar.Add(@localizer["templatesDeletedSuccessfully"].ToString().Replace("{count}", successCount.ToString()), Severity.Success);
            }
            
            if (errorCount > 0)
            {
                Snackbar.Add(@localizer["someTemplatesCouldNotBeDeleted"].ToString().Replace("{count}", errorCount.ToString()), Severity.Warning);
            }
            
            MudDialog.Close(DialogResult.Ok(true));
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