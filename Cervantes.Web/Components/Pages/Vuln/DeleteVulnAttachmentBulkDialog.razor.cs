using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class DeleteVulnAttachmentBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public List<VulnAttachment> attachments { get; set; }
    [Inject] private VulnController _VulnController { get; set; }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            foreach (var attachment in attachments)
            {
                var response = await _VulnController.DeleteAttahcment(attachment.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")            
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