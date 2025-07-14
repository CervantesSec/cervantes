using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Client;

public partial class DeleteClientCustomFieldDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public ClientCustomFieldViewModel customField { get; set; }
    [Inject] private ClientCustomFieldController ClientCustomFieldController { get; set; }
    [Inject] private IStringLocalizer<Resource> localizer { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }

    void Cancel() => MudDialog.Cancel();

    private async Task Delete()
    {
        try
        {
            var result = await ClientCustomFieldController.Delete(customField.Id);
            if (result is Microsoft.AspNetCore.Mvc.OkObjectResult)
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