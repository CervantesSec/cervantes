using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Admin.Vuln;

public partial class DeleteVulnCustomFieldBulkDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public List<VulnCustomFieldViewModel> customFields { get; set; }
    
    [Inject] private VulnCustomFieldController VulnCustomFieldController { get; set; }
    
    void Cancel() => MudDialog.Cancel();

    private async System.Threading.Tasks.Task Submit()
    {
        try
        {
            int deletedCount = 0;
            int errorCount = 0;
            
            foreach (var field in customFields)
            {
                try
                {
                    var response = await VulnCustomFieldController.Delete(field.Id);
                    if (response is OkObjectResult)
                    {
                        deletedCount++;
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
            
            if (deletedCount > 0)
            {
                Snackbar.Add($"{deletedCount} {localizer["customFieldsDeleted"]}", MudBlazor.Severity.Success);
            }
            
            if (errorCount > 0)
            {
                Snackbar.Add($"{errorCount} {localizer["customFieldsDeleteError"]}", MudBlazor.Severity.Error);
            }
            
            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", MudBlazor.Severity.Error);
        }
    }
}