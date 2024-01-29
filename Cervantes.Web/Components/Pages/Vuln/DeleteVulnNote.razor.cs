using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class DeleteVulnNote: ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    [Inject] private VulnController _VulnController { get; set; }

    [Parameter] public VulnNote note { get; set; } = new VulnNote();

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await _VulnController.DeleteVulnNote(note.Id);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult") 
            {
                Snackbar.Add(@localizer["deletedNote"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["deletedNoteError"], Severity.Error);
            }
            
        }
    }
}