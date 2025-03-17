using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class DeleteVulnAttachment: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public VulnAttachment attachment { get; set; } = new VulnAttachment();
    [Inject] private VulnController _VulnController { get; set; }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await _VulnController.DeleteAttahcment(attachment.Id);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")            
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