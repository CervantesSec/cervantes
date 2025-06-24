using System.Net.Http.Json;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Note;

public partial class DeleteNoteDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }
[Inject] NoteController _NoteController { get; set; }
    MudForm form;


    [Parameter] public CORE.Entities.Note note { get; set; } 
	 

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await _NoteController.Delete(note.Id);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
            {
                Snackbar.Add(@localizer["noteDeleted"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["noteDeletedError"], Severity.Error);
            }
            
        }
    }
}