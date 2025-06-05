using System.Security.Claims;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;

namespace Cervantes.Web.Components.Pages.Admin.Backup;

public partial class Index: ComponentBase
{
    private List<BreadcrumbItem> _items;
[Inject] private BackupController _BackupController { get; set; }

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

        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Admin", href: null,icon: Icons.Material.Filled.AdminPanelSettings),
            new BreadcrumbItem(@localizer["backup"], href: null, disabled: true,icon: Icons.Material.Filled.Backup)
        };
    }

    private async Task BackupData()
    {

            var response =  _BackupController.BackupData();
            if (response.ContentType == "application/json")
            {
                Snackbar.Add(@localizer["backupSuccessfull"], Severity.Success);
                /*//return File(response.Content.ReadAsStringAsync());
                var fileName = $"CervantesBackup-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.json";
                var jsonstr = System.Text.Json.JsonSerializer.Serialize(response.Content.ReadAsStringAsync());  
                byte[] byteArray = System.Text.ASCIIEncoding.ASCII.GetBytes(jsonstr);  
            
                return System.IO.File(byteArray, "application/json", fileName);*/
                var fileName = $"CervantesBackup-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.json";
                //var fileStream = response.FileContents;
                MemoryStream fileStream = new MemoryStream(response.FileContents);
                using var streamRef = new DotNetStreamReference(stream: fileStream);

                await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
            }
            else
            {
                Snackbar.Add(@localizer["backupError"], Severity.Error);
            }
            
        
    }
    
    private async Task BackupAttachments()
    {
        
        var response =  _BackupController.BackupAttachments();
        if (response.ContentType == "application/zip")
        {
            Snackbar.Add(@localizer["backupSuccessfull"], Severity.Success);
            /*//return File(response.Content.ReadAsStringAsync());
            var fileName = $"CervantesBackup-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.json";
            var jsonstr = System.Text.Json.JsonSerializer.Serialize(response.Content.ReadAsStringAsync());  
            byte[] byteArray = System.Text.ASCIIEncoding.ASCII.GetBytes(jsonstr);  
        
            return System.IO.File(byteArray, "application/json", fileName);*/
            var fileName = $"CervantesAttachments-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.zip";
            //var fileStream = await response.Content.ReadAsStreamAsync();
            MemoryStream fileStream = new MemoryStream(response.FileContents);
            using var streamRef = new DotNetStreamReference(stream: fileStream);

            await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }
        else
        {
            Snackbar.Add(@localizer["backupError"], Severity.Error);
        }
    }
    
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    private async Task RestoreData(DialogOptions options)
    {
        IMudExDialogReference<RestoreData>? dlgReference = await Dialog.ShowExAsync<RestoreData>("Simple Dialog", centerWidthEx);

        // wait modal to close
        var result = await dlgReference.Result;
 
        
    }
    
    private async Task RestoreAttachments(DialogOptions options)
    {
        IMudExDialogReference<RestoreAttachments>? dlgReference = await Dialog.ShowExAsync<RestoreAttachments>("Simple Dialog", centerWidthEx);

        // wait modal to close
        var result = await dlgReference.Result;
 
        
    }
    
}