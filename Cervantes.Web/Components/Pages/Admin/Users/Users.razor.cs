using System.Net.Http.Json;
using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModels;
using Cervantes.IFR.Export;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Users;

public partial class Users: ComponentBase
{
    private List<ApplicationUser> users = new List<ApplicationUser>();
    private List<UserViewModel> model = new List<UserViewModel>();
    private List<UserViewModel> seleUsers = new List<UserViewModel>();

    private List<BreadcrumbItem> _items;
    private string searchString = "";
    [Inject] private UserController _UserController { get; set; }
    [Inject ]private IExportToCsv ExportToCsv { get; set; }
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions mediumWidth = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };

    DialogOptionsEx maxWidthEx = new DialogOptionsEx() 
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
        Position = DialogPosition.CenterRight,
        DisableSizeMarginY = true,
        DisablePositionMargin = true,
        BackdropClick = false,
        Resizeable = true,
    };

    private ClaimsPrincipal userAth;
    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Admin", href: null,icon: Icons.Material.Filled.AdminPanelSettings),
            new BreadcrumbItem(@localizer["users"], href: null, disabled: true,icon: Icons.Material.Filled.Group)
        };
        await Update();
        if (navigationManager.Uri.Contains("/users/create"))
        {
             await OpenDialogCreate(maxWidthEx);
        }
    }
    
    protected async Task Update()
    {
        
        users.RemoveAll(item => true);
        users = _UserController.Get().ToList();
        model.RemoveAll(item => true);
        foreach (var item in users)
        {
            var rl = await _UserController.GetRole(item.Id);
            var user = new UserViewModel()
            {
                Id = item.Id,
                FullName = item.FullName,
                Email = item.Email,
                TwoFactorEnabled = item.TwoFactorEnabled,
                Position = item.Position,
                LockoutEnd = item.LockoutEnd,
                Role = rl,
                ExternalLogin = item.ExternalLogin,
                Avatar = item.Avatar
            };
            model.Add(user);
        }

    }


    
    
    private Func<UserViewModel, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    };
    
    private async Task OpenDialogCreate(DialogOptionsEx options)
    {
        IMudExDialogReference<CreateUserDialog>? dlgReference = await Dialog.ShowEx<CreateUserDialog>("Simple Dialog", options);
        // wait modal to close
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    
    
    async Task RowClicked(DataGridRowClickEventArgs<UserViewModel> args)
    {
        var parameters = new DialogParameters { ["userSelected"]=args.Item };
        IMudExDialogReference<UserDialog>? dlgReference = await Dialog.ShowEx<UserDialog>("Simple Dialog", parameters, maxWidthEx);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
    }
    private async Task Export(int id)
    {
        switch (id)
        {
            case 0:
                /*var records = model.Select(e => new IFR.Export.ProjectExport()
                {
                    Name = e.Name,
                    Description = e.Description,
                    Client = e.Client.Name,
                    CreatedUser = e.User.FullName,
                    StartDate = e.StartDate.ToShortDateString(),
                    EndDate = e.EndDate.ToShortDateString(),
                    Template = e.Template,
                    Status = e.Status.ToString(),
                    ProjectType = e.ProjectType.ToString(),
                    Language = e.Language.ToString(),
                    Score = e.Score.ToString(),
                    FindingsId = e.FindingsId.ToString(),
                    ExecutiveSummary = e.ExecutiveSummary
                    
 
                }).ToList();
                var file = ExportToCsv.ExportProjects(records);
                await JS.InvokeVoidAsync("downloadFile", file);
                Snackbar.Add(@localizer["exportSuccessfull"], Severity.Success);
                ExportToCsv.DeleteFile(file);*/
                break;
        }
    }
    
    private async Task BtnActions(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["users"]=seleUsers };

                var dialog =  await Dialog.ShowEx<DeleteUsersBulkDialog>("Edit", parameters,mediumWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<UserViewModel> items)
    {
        
        seleUsers = items.ToList();
    }
    
}