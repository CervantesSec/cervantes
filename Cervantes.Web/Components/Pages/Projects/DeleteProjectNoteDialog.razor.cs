using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class DeleteProjectNoteDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public ProjectNote note { get; set; } = new ProjectNote();
    [Inject] private ProjectController ProjectController { get; set; }
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await ProjectController.DeleteNote(note.Id);
            if (response is NoContentResult)
            {
                Snackbar.Add(@localizer["deletedNote"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else if (response is Microsoft.AspNetCore.Mvc.ObjectResult { StatusCode: 403 })
            {
                Snackbar.Add(@localizer["noInProject"], Severity.Warning);
            }
            else
            {
                Snackbar.Add(@localizer["deletedNoteError"], Severity.Error);
            }
            
        }
    }
}