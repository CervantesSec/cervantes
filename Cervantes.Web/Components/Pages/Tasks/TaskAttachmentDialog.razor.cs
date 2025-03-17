using System.Security.Claims;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;

namespace Cervantes.Web.Components.Pages.Tasks;

public partial class TaskAttachmentDialog: ComponentBase
{
    [Inject] private TaskController _taskController { get; set; }
    [Parameter] public CORE.Entities.TaskAttachment attachment { get; set; } = new CORE.Entities.TaskAttachment();

    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] ProjectController _ProjectController { get; set; }
    private bool inProject = false;
    private ClaimsPrincipal userAth;
    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;

        if (attachment.Task.ProjectId != null)
        {
            inProject = await _ProjectController.VerifyUser(attachment.Task.ProjectId.Value);
        }
    }
    
    private async Task DeleteTaskAttachment(CORE.Entities.TaskAttachment attachment,DialogOptions options)
    {
        var parameters = new DialogParameters { ["attachment"]=attachment };
        var dialog = await Dialog.ShowEx<DeleteTaskAttachmentDialog>(@localizer["addMember"], parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            MudDialog.Close(DialogResult.Ok(true));
            StateHasChanged();
        }
    }
    
    private async Task Download(string path)
    {

                await JS.InvokeVoidAsync("downloadFile", path);
                Snackbar.Add(@localizer["exportSuccessfull"], Severity.Success);
        
    }

}