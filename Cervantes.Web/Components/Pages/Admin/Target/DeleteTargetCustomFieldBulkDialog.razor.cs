using Microsoft.AspNetCore.Components;
using MudBlazor;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;

namespace Cervantes.Web.Components.Pages.Admin.Target;

public partial class DeleteTargetCustomFieldBulkDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public List<TargetCustomFieldViewModel> customFields { get; set; }
    [Inject] private TargetCustomFieldController TargetCustomFieldController { get; set; }
    [Inject] private IStringLocalizer<Resource> localizer { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }

    void Cancel() => MudDialog.Cancel();
    
    async System.Threading.Tasks.Task DeleteBulk()
    {
        try
        {
            int successCount = 0;
            int failCount = 0;
            
            foreach (var field in customFields)
            {
                var response = await TargetCustomFieldController.Delete(field.Id);
                if (response is Microsoft.AspNetCore.Mvc.NoContentResult)
                {
                    successCount++;
                }
                else
                {
                    failCount++;
                }
            }
            
            if (successCount > 0)
            {
                Snackbar.Add($"{localizer["customFieldsDeleted"]} ({successCount})", MudBlazor.Severity.Success);
            }
            
            if (failCount > 0)
            {
                Snackbar.Add($"{localizer["customFieldsDeleteError"]} ({failCount})", MudBlazor.Severity.Error);
            }
            
            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"{localizer["customFieldsDeleteError"]}: {ex.Message}", MudBlazor.Severity.Error);
        }
    }
}