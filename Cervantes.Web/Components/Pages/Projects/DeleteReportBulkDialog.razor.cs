using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class DeleteReportBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    [Parameter] public List<Report> reports { get; set; } 
    [Inject] private ReportController _ReportController { get; set; }


    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            foreach (var report in reports)
            {
                var response = await _ReportController.DeleteReport(report.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
                {
                    Snackbar.Add(@localizer["reportDeleted"], Severity.Success);
                }
                else
                {
                    Snackbar.Add(@localizer["reportDeletedError"], Severity.Error);
                }
            }
            MudDialog.Close(DialogResult.Ok(true));

            
        }
    }
}