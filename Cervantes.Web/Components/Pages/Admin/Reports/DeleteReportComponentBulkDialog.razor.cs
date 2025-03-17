using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Admin.Reports;

public partial class DeleteReportComponentBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public List<CORE.Entities.ReportComponents> components { get; set; }
    [Inject] private ReportController _reportController { get; set; }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            foreach (var component in components)
            {
                var response = await _reportController.DeleteComponent(component.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
                {
                    Snackbar.Add(@localizer["reportComponentDeleted"], Severity.Success);
                }
                else
                {
                    Snackbar.Add(@localizer["reportComponentDeletedError"], Severity.Error);
                }
            }
            MudDialog.Close(DialogResult.Ok(true));
            
        }
    }
}