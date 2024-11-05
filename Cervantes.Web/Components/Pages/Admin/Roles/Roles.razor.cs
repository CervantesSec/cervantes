using System.Security.Claims;
using AuthPermissions.AdminCode;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Components.Pages.Admin.Users;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Roles;

public partial class Roles : ComponentBase
{
    private List<RolesViewModel> model = new List<RolesViewModel>();
    private List<RolesViewModel> seleUsers = new List<RolesViewModel>();
    private List<BreadcrumbItem> _items;
    private string searchString = "";
    [Inject] private UserController _UserController { get; set; }
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
            new BreadcrumbItem(@localizer["roles"], href: null, disabled: true,icon: Icons.Material.Filled.Group)
        };
        await Update();
        if (navigationManager.Uri.Contains("/roles/create"))
        {
             await OpenDialogCreate(maxWidthEx);
        }
    }
    
    protected async Task Update()
    {
        
        model.RemoveAll(item => true);
        var roles = _UserController.GetRoles().ToList();
        foreach (var item in roles)
        {
            model.Add(new RolesViewModel
            {
                Name = item.RoleName,
                Description = item.Description,
                PermmissioNumber = item.PermissionNames.Count,
                PermissionNames = item.PermissionNames
            });
        }

    }


    
    
    private Func<RolesViewModel, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    };
    
    private async Task OpenDialogCreate(DialogOptionsEx options)
    {
        IMudExDialogReference<CreateRole>? dlgReference = await DialogEx.ShowEx<CreateRole>("Simple Dialog", options);
        // wait modal to close
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    
    
    async Task RowClicked(DataGridRowClickEventArgs<RolesViewModel> args)
    {
        var parameters = new DialogParameters { ["role"]=args.Item };
        IMudExDialogReference<RoleDialog>? dlgReference = await DialogEx.ShowEx<RoleDialog>("Simple Dialog", parameters, maxWidthEx);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
    }
    
    private async Task BtnActions(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["roles"]=seleUsers };

                var dialog =  Dialog.Show<DeleteRoleBulkDialog>("Edit", parameters,mediumWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<RolesViewModel> items)
    {
        
        seleUsers = items.ToList();
    }
}