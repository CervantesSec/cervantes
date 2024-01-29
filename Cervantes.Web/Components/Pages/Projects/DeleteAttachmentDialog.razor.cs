using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class DeleteAttachmentDialog: ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public ProjectAttachment attachment { get; set; } = new ProjectAttachment();

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await Http.PostAsJsonAsync("api/Project/Attachment/Delete", attachment);
            if (response.IsSuccessStatusCode)
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