using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Reports;

public partial class DeleteReportBulkDialog: ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public List<ReportTemplate> reports { get; set; }
    [Inject] private ReportController _reportController { get; set; }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            foreach (var report in reports)
            {
                var response = await _reportController.Delete(report.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
                {
                    Snackbar.Add(@localizer["reportTemplateDeleted"], Severity.Success);
                }
                else
                {
                    Snackbar.Add(@localizer["reportTemplateDeletedError"], Severity.Error);
                }
            }
            MudDialog.Close(DialogResult.Ok(true));

            
        }
    } 
}