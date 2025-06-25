using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class DeleteVulnNoteBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    [Inject] private VulnController _VulnController { get; set; }

    [Parameter] public List<VulnNote> notes { get; set; }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            foreach (var note in notes)
            {
                var response = await _VulnController.DeleteVulnNote(note.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult") 
                {
                    Snackbar.Add(@localizer["deletedNote"], Severity.Success);
                }
                else
                {
                    Snackbar.Add(@localizer["deletedNoteError"], Severity.Error);
                }
            }
            MudDialog.Close(DialogResult.Ok(true));

            
        }
    }
}