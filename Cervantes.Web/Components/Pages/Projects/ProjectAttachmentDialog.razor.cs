using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class ProjectAttachmentDialog: ComponentBase
{
    [Inject] private ProjectController _ProjectController { get; set; }
    [Parameter] public CORE.Entities.ProjectAttachment attachment { get; set; } = new CORE.Entities.ProjectAttachment();

    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    [Inject] ISnackbar Snackbar { get; set; }
    
    private bool inProject = false;
    
    protected override async Task OnInitializedAsync()
    {
        if (attachment.ProjectId != Guid.Empty)
        {
            inProject = await _ProjectController.VerifyUser(attachment.ProjectId);
        }
    }
    
    private async Task DeleteTaskAttachment(CORE.Entities.ProjectAttachment attachment,DialogOptions options)
    {
        var parameters = new DialogParameters { ["attachment"]=attachment };
        var dialog = Dialog.Show<DeleteProjectAttachment>(@localizer["addMember"], parameters, options);
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