using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class DeleteProjectAttachmentBulk: ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;


    [Parameter] public List<ProjectAttachment> attachments { get; set; }
    [Inject] ProjectController _ProjectController { get; set; }

    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {

            foreach (var attachment in attachments)
            {
                var response = await _ProjectController.DeleteAttahcment(attachment.Id);
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