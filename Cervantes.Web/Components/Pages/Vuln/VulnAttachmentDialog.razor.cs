using System.Security.Claims;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class VulnAttachmentDialog: ComponentBase
{
    [Inject] private VulnController _vulnController { get; set; }
    [Inject] private ProjectController _ProjectController { get; set; }

    [Parameter] public CORE.Entities.VulnAttachment attachment { get; set; }

    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    [Inject] ISnackbar Snackbar { get; set; }
    private bool inProject = false;
    private ClaimsPrincipal userAth;
    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        if (attachment.Vuln.Project != null)
        {
            inProject = await _ProjectController.VerifyUser(attachment.Vuln.ProjectId.Value);
        }
    }
    
    private async Task DeleteVulnAttachment(CORE.Entities.VulnAttachment attachment,DialogOptions options)
    {
        var parameters = new DialogParameters { ["attachment"]=attachment };
        var dialog = Dialog.Show<DeleteVulnAttachment>(@localizer["addMember"], parameters, options);
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