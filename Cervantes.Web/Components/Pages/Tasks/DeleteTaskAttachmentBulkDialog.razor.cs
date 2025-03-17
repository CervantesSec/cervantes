using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Tasks;

public partial class DeleteTaskAttachmentBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;


    [Parameter] public List<TaskAttachment> attachments { get; set; }
    [Inject] TaskController _taskController { get; set; }

    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {

            foreach (var attachment in attachments)
            {
                var response = await _taskController.DeleteAttachment(attachment.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
                {
                    Snackbar.Add(@localizer["attachmentDeleted"], Severity.Success);
                }
                else
                {
                    Snackbar.Add(@localizer["attachmentDeletedError"], Severity.Error);
                }
            }
            MudDialog.Close(DialogResult.Ok(true));

            
        }
    }
}