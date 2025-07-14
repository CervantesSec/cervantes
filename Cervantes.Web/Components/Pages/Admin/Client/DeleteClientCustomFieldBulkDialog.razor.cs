using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Client;

public partial class DeleteClientCustomFieldBulkDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public List<ClientCustomFieldViewModel> customFields { get; set; }
    [Inject] private ClientCustomFieldController ClientCustomFieldController { get; set; }
    [Inject] private IStringLocalizer<Resource> localizer { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }

    void Cancel() => MudDialog.Cancel();

    private async Task DeleteBulk()
    {
        try
        {
            int successCount = 0;
            int errorCount = 0;
            
            foreach (var field in customFields)
            {
                try
                {
                    var result = await ClientCustomFieldController.Delete(field.Id);
                    if (result is Microsoft.AspNetCore.Mvc.OkObjectResult)
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
                Snackbar.Add($"{successCount} {localizer["customFieldsDeleted"]}", MudBlazor.Severity.Success);
            }
            
            if (errorCount > 0)
            {
                Snackbar.Add($"{errorCount} {localizer["customFieldsDeleteError"]}", MudBlazor.Severity.Error);
            }
            
            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"{localizer["customFieldsDeleteError"]}: {ex.Message}", MudBlazor.Severity.Error);
        }
    }
}