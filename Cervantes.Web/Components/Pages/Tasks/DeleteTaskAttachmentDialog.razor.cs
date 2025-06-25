using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Tasks;

public partial class DeleteTaskAttachmentDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;


    [Parameter] public TaskAttachment attachment { get; set; }
	 [Inject] TaskController _taskController { get; set; }

    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {

            var response = await _taskController.DeleteAttachment(attachment.Id);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
            {
                Snackbar.Add(@localizer["attachmentDeleted"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["attachmentDeletedError"], Severity.Error);
            }
            
        }
    }
}