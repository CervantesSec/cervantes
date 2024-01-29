using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.KnowledgeBase;

public partial class DeleteKnowledgePageDialog: ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public CORE.Entities.KnowledgeBase category { get; set; }
    [Inject] private KnowledgeBaseController _KnowledgeBaseController { get; set; }


    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await _KnowledgeBaseController.DeletePage(category.Id);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
            {
                Snackbar.Add(@localizer["pageDeleted"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["pageDeletedError"], Severity.Error);
            }
            
        }
    }
}