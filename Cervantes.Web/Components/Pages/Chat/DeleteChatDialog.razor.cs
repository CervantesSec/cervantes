using Cervantes.CORE.ViewModel;
using Cervantes.Web.Components.Pages.Clients;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Chat;

public partial class DeleteChatDialog : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;

    CreateClientDialog.ClientModelFluentValidator clientValidator = new CreateClientDialog.ClientModelFluentValidator();

    [Parameter] public ChatViewModel chat { get; set; } = new ChatViewModel();
    [Inject] private ChatController _ChatController { get; set; }


    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await _ChatController.DeleteChat(chat.Id);
            if (response)
            {
                Snackbar.Add(@localizer["chatDeleted"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["chatDeletedError"], Severity.Error);
            }
            
        }
    }
}