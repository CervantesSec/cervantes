using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Note;

public partial class DeleteNoteBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] NoteController _NoteController { get; set; }
    MudForm form;


    [Parameter] public List<CORE.Entities.Note> notes { get; set; } 
	 

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            foreach (var note in notes)
            {
                var response = await _NoteController.Delete(note.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
                {
                    Snackbar.Add(@localizer["noteDeleted"], Severity.Success);
                }
                else
                {
                    Snackbar.Add(@localizer["noteDeletedError"], Severity.Error);
                }
            }
            MudDialog.Close(DialogResult.Ok(true));

            
        }
    }
}