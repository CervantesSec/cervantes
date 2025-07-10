using Microsoft.AspNetCore.Components;
using MudBlazor;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Project;

public partial class DeleteProjectCustomFieldDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public ProjectCustomFieldViewModel customField { get; set; }
    [Inject] private ProjectCustomFieldController ProjectCustomFieldController { get; set; }
    [Inject] private IStringLocalizer<Resource> localizer { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }

    void Cancel() => MudDialog.Cancel();

    private async Task Delete()
    {
        try
        {
            var result = await ProjectCustomFieldController.Delete(customField.Id);
            if (result is Microsoft.AspNetCore.Mvc.OkResult)
            {
                Snackbar.Add($"{localizer["customFieldDeleted"]}", MudBlazor.Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add($"{localizer["customFieldDeleteError"]}", MudBlazor.Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"{localizer["customFieldDeleteError"]}: {ex.Message}", MudBlazor.Severity.Error);
        }
    }
}