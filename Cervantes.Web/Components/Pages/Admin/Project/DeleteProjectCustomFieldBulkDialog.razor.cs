using Microsoft.AspNetCore.Components;
using MudBlazor;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Project;

public partial class DeleteProjectCustomFieldBulkDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public List<ProjectCustomFieldViewModel> customFields { get; set; }
    [Inject] private ProjectCustomFieldController ProjectCustomFieldController { get; set; }
    [Inject] private IStringLocalizer<Resource> localizer { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }

    void Cancel() => MudDialog.Cancel();

    private async Task Delete()
    {
        try
        {
            var deleteCount = 0;
            var errorCount = 0;
            
            foreach (var customField in customFields)
            {
                try
                {
                    var result = await ProjectCustomFieldController.Delete(customField.Id);
                    if (result is Microsoft.AspNetCore.Mvc.OkResult)
                    {
                        deleteCount++;
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
            
            if (deleteCount > 0)
            {
                Snackbar.Add($"{deleteCount} {localizer["customFieldsDeleted"]}", MudBlazor.Severity.Success);
            }
            
            if (errorCount > 0)
            {
                Snackbar.Add($"{errorCount} {localizer["customFieldsDeleteError"]}", MudBlazor.Severity.Error);
            }
            
            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"{localizer["customFieldDeleteError"]}: {ex.Message}", MudBlazor.Severity.Error);
        }
    }
}