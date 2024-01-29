using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.KnowledgeBase;

public partial class DeleteKnowledgeCategoryDialog: ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public CORE.Entities.KnowledgeBaseCategories category { get; set; }
    [Inject] private KnowledgeBaseController _KnowledgeBaseController { get; set; }


    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await _KnowledgeBaseController.DeleteCategory(category.Id);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
            {
                Snackbar.Add(@localizer["categoryDeleted"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["categoryDeletedError"], Severity.Error);
            }
            
        }
    }
}