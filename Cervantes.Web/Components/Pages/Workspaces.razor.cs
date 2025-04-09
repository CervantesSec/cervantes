using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.Web.Components.Pages.Clients;
using Cervantes.Web.Components.Pages.Projects;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages;

public partial class Workspaces: ComponentBase
{

    [Inject] private WorkspacesController _WorkspacesController { get; set; }
    private List<BreadcrumbItem> _items;
    protected override async Task OnInitializedAsync()
    {
        Projects = _WorkspacesController.Get().ToList();
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem(localizer["workspaces"], href: null, disabled: true,icon: Icons.Material.Filled.Workspaces)
        };
    
    }

    private List<CORE.Entities.Project > Projects = new List<Project>();

    public void Details(Guid id)
    {
        NavigationManager.NavigateTo($"/workspace/{id}");
    }
    
    /*public void Project(Guid id)
    {
        NavigationManager.NavigateTo($"/projects/{id}");
    }*/
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };

    async Task Project(Project project,DialogOptions options)
    {
        var parameters = new DialogParameters { ["project"]=project };

        var dialog =  await DialogService.ShowEx<ProjectDialog>(@localizer["delete"], parameters,maxWidthEx);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            //model.Remove(vuln);
            StateHasChanged();
        }
    }
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
    async Task Client(Client client,DialogOptions options)
    {
        var parameters = new DialogParameters { ["client"]=client };

        var dialog =  await DialogService.ShowEx<ClientDialog>(@localizer["delete"], parameters,maxWidthEx);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            //model.Remove(vuln);
            StateHasChanged();
        }
    }
    
    /*public void Client(Guid id)
    {
        NavigationManager.NavigateTo($"/clients/{id}");
    }*/

}