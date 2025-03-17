using System.Net.Http.Json;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Workspace.Target;

public partial class ImportDialog: ComponentBase
{
    private long maxFileSize = 1024 * 1024 * 5;

    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }

    void Cancel() => MudDialog.Cancel();
    MudForm form;
    [Inject] ISnackbar Snackbar { get; set; }
    
    TargetImportViewModel model = new TargetImportViewModel();
    [Parameter] public Guid project { get; set; }
    [Inject] private TargetController _targetController { get; set; }
    private IBrowserFile file;
    protected override async System.Threading.Tasks.Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        StateHasChanged();
    }
    

    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            if (file != null)
            {
                Stream stream = file.OpenReadStream(maxFileSize);
                MemoryStream ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                stream.Close();
	        
                model.FileName = file.Name;
                model.FileContent = ms.ToArray();
                ms.Close();
                file = null;
                model.Project = project;

                var response = await _targetController.Import(model);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
                {

                    Snackbar.Add(@localizer["targetImported"], Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    Snackbar.Add(@localizer["targetImportedError"], Severity.Error);
                }
            }
            else
            {
                Snackbar.Add(@localizer["noFiles"], Severity.Error);

            }


        }
    }
}