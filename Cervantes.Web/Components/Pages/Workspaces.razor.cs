using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.Web.Components.Pages.Clients;
using Cervantes.Web.Components.Pages.Projects;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
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

        var dialog =  DialogService.Show<ProjectDialog>(@localizer["delete"], parameters,options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            //model.Remove(vuln);
            StateHasChanged();
        }
    }
    
    async Task Client(Client client,DialogOptions options)
    {
        var parameters = new DialogParameters { ["client"]=client };

        var dialog =  DialogService.Show<ClientDialog>(@localizer["delete"], parameters,options);
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