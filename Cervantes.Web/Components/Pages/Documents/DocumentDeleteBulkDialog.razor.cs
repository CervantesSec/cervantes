using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Documents;

public partial class DocumentDeleteBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public List<Document> documents { get; set; }
    [Inject] DocumentController _documentController { get; set; }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            foreach (var docs in documents)
            {
                var response = await _documentController.Delete(docs.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
                {
                    Snackbar.Add(@localizer["documentDeleted"], Severity.Success);
                }
                else
                {
                    Snackbar.Add(@localizer["documentDeletedError"], Severity.Error);
                }
            }
            MudDialog.Close(DialogResult.Ok(true));
            
        }
    }
}