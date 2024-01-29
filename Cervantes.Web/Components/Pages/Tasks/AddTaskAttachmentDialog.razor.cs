using System.Net;
using System.Net.Http.Json;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Task = Cervantes.CORE.Entities.Task;

namespace Cervantes.Web.Components.Pages.Tasks;

public partial class AddTaskAttachmentDialog: ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }

    void Cancel() => MudDialog.Cancel();
    
    MudForm form;
    [Inject] ISnackbar Snackbar { get; set; }
    TaskAttachmentViewModel model = new TaskAttachmentViewModel();
    [Parameter] public CORE.Entities.Task task { get; set; }
    [Inject] private TaskController _taskController { get; set; }
    private long maxFileSize = 1024 * 1024 * 50;
    private IBrowserFile file;
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
            
                model.TaskId = task.Id;
                var response = await _taskController.AddAttachment(model);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
                {

                    Snackbar.Add(@localizer["attachmentAdded"], Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    Snackbar.Add(@localizer["attachmentAddedError"], Severity.Error);
                }
            }
            else
            {
                Snackbar.Add(@localizer["noFiles"], Severity.Error);

            }

        }
    }
}