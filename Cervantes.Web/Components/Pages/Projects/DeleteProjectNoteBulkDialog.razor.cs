using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class DeleteProjectNoteBulkDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public List<ProjectNote> notes { get; set; }
    [Inject] private ProjectController ProjectController { get; set; }
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            foreach (var note in notes)
            {
                var response = await ProjectController.DeleteNote(note.Id);
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