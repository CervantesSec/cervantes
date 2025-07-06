using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Admin.Vuln;

public partial class DeleteVulnCustomFieldDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public VulnCustomFieldViewModel customField { get; set; }
    
    [Inject] private VulnCustomFieldController VulnCustomFieldController { get; set; }
    
    void Cancel() => MudDialog.Cancel();

    private async System.Threading.Tasks.Task Submit()
    {
        try
        {
            var response = await VulnCustomFieldController.Delete(customField.Id);
            if (response is OkObjectResult)
            {
                Snackbar.Add(localizer["customFieldDeleted"], MudBlazor.Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(localizer["customFieldDeleteError"], MudBlazor.Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", MudBlazor.Severity.Error);
        }
    }
}