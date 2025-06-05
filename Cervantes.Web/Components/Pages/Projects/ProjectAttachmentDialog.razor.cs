using System.Security.Claims;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class ProjectAttachmentDialog: ComponentBase
{
    [Inject] private ProjectController _ProjectController { get; set; }
    [Parameter] public CORE.Entities.ProjectAttachment attachment { get; set; } = new CORE.Entities.ProjectAttachment();

    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    [Inject] ISnackbar Snackbar { get; set; }
    
    private bool inProject = false;
    private ClaimsPrincipal userAth;
    DialogOptionsEx centerWidthEx = new DialogOptionsEx() 
    {
        MaximizeButton = true,
        CloseButton = true,
        FullHeight = true,
        CloseOnEscapeKey = true,
        MaxWidth = MaxWidth.Medium,
        MaxHeight = MaxHeight.False,
        FullWidth = true,
        DragMode = MudDialogDragMode.Simple,
        Animations = new[] { AnimationType.SlideIn },
        Position = DialogPosition.Center,
        DisableSizeMarginY = true,
        DisablePositionMargin = true,
        BackdropClick = false,
        Resizeable = true,
    };
    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        if (attachment.ProjectId != Guid.Empty)
        {
            inProject = await _ProjectController.VerifyUser(attachment.ProjectId);
        }
    }
    
    private async Task DeleteTaskAttachment(CORE.Entities.ProjectAttachment attachment,DialogOptions options)
    {
        var parameters = new DialogParameters { ["attachment"]=attachment };
        IMudExDialogReference<DeleteProjectAttachment>? dlgReference = await Dialog.ShowExAsync<DeleteProjectAttachment>("Simple Dialog", parameters, centerWidthEx);

        var result = await dlgReference.Result;

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