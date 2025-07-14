using Microsoft.AspNetCore.Components;
using MudBlazor;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;

namespace Cervantes.Web.Components.Pages.Admin.Target;

public partial class DeleteTargetCustomFieldDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public TargetCustomFieldViewModel customField { get; set; }
    [Inject] private TargetCustomFieldController TargetCustomFieldController { get; set; }
    [Inject] private IStringLocalizer<Resource> localizer { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }

    void Cancel() => MudDialog.Cancel();
    
    async System.Threading.Tasks.Task Delete()
    {
        try
        {
            var response = await TargetCustomFieldController.Delete(customField.Id);
            if (response is Microsoft.AspNetCore.Mvc.NoContentResult)
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
            Snackbar.Add($"{localizer["customFieldDeleteError"]}: {ex.Message}", MudBlazor.Severity.Error);
        }
    }
}