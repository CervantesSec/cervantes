using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class DownloadReportDialog: ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    [Parameter] public CORE.Entities.Report report { get; set; }
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;

	 
    ReportDownloadModel model = new ReportDownloadModel();
    private List<CORE.Entities.ReportTemplate> templates = new List<CORE.Entities.ReportTemplate>();
  
    [Inject] private ReportController _ReportController { get; set; }

    protected override async Task OnInitializedAsync()
    {
        templates = _ReportController.Templates().ToList();
        await base.OnInitializedAsync();
        StateHasChanged();
    }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            model.Id = report.Id;
            var response =  await _ReportController.DownloadReport(model);
            if (response == null)
            {
                Snackbar.Add(@localizer["reportDownloadError"], Severity.Error);
            }
            switch (response.ContentType)
            {
                case "text/html":
                    Snackbar.Add(@localizer["reportDownloaded"], Severity.Success);

                    var fileName = report.Name+"_"+report.Version + ".html";
                    MemoryStream fileStream = new MemoryStream(response.FileContents);
                    using (var streamRef = new DotNetStreamReference(stream: fileStream))
                    {
                        await jsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
                    }
                    MudDialog.Close(DialogResult.Ok(true));
                    break;
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    Snackbar.Add(@localizer["reportDownloaded"], Severity.Success);

                    var fileName2 = report.Name+"_"+report.Version + ".docx";
                    MemoryStream fileStream2 = new MemoryStream(response.FileContents);
                    using (var streamRef = new DotNetStreamReference(stream: fileStream2))
                    {
                        await jsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName2, streamRef);
                    }
                    MudDialog.Close(DialogResult.Ok(true));
                    break;
                case "application/pdf":
                    Snackbar.Add(@localizer["reportDownloaded"], Severity.Success);

                    var fileName3 = report.Name+"_"+report.Version + ".pdf";
                    MemoryStream fileStream3 = new MemoryStream(response.FileContents);
                    using (var streamRef = new DotNetStreamReference(stream: fileStream3))
                    {
                        await jsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName3, streamRef);
                    }
                    MudDialog.Close(DialogResult.Ok(true));
                    break;
                case null:
                    Snackbar.Add(@localizer["reportDownloadError"], Severity.Error);
                    break;
            }
            
        }
    }
}