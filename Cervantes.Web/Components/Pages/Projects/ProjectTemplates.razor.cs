using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.IFR.Export;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class ProjectTemplates : ComponentBase
{
     private List<Project> model = new List<Project>();
        private List<BreadcrumbItem> _items;
    private string searchString = "";
    private string searchStringTemp = "";

    [Parameter] public Guid project { get; set; }

    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    [Parameter] public Guid client { get; set; }

    [Inject] private ProjectController _projectController { get; set; }
    [Inject ]private IExportToCsv ExportToCsv { get; set; }
    [Inject] private UserController userController { get; set; }
    private List<CORE.Entities.Project> selectedProjects = new List<CORE.Entities.Project>();
    private ApplicationUser user;
    ClaimsPrincipal userAth;

    protected override async Task OnInitializedAsync()
    {
    _items = new List<BreadcrumbItem>
    {
        new BreadcrumbItem(localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
        new BreadcrumbItem(localizer["projects"], href: "/projects",icon: Icons.Material.Filled.Folder),

        new BreadcrumbItem(localizer["templates"], href: null, disabled: true,icon: Icons.Material.Filled.Folder)
    };
    userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;

        await Update();
        /*if (project != Guid.Empty)
        {
            if (navigationManager.Uri.Contains($"/projects/{project}"))
            {
                var pro = model.FirstOrDefault(x => x.Id == project);
                if (pro != null)
                {
                    await DetailsDialog(pro ,maxWidth);
                }
            
            }
        }*/
        
        user = userController.GetUser(_accessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier)); 
        
    }
    
    protected async Task Update()
    {
        model =  _projectController.Get().ToList();
    }


    private Func<Project, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Client.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Language.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.ProjectType.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.StartDate.ToString().Contains(searchString))
            return true;
        if (element.EndDate.ToString().Contains(searchString))
            return true;
        return false;
    };

    private Func<Project, bool> _quickFilterTemp => element =>
    {
        if (string.IsNullOrWhiteSpace(searchStringTemp))
            return true;
        if (element.Name.Contains(searchStringTemp, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Client.Name.Contains(searchStringTemp, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Language.ToString().Contains(searchStringTemp, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.ProjectType.ToString().Contains(searchStringTemp, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.StartDate.ToString().Contains(searchStringTemp))
            return true;
        if (element.EndDate.ToString().Contains(searchStringTemp))
            return true;
        return false;
    };
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
    private async Task OpenDialogCreate(DialogOptionsEx options)
    {

        IMudExDialogReference<CreateProjectDialog>? dlgReference = await DialogService.ShowEx<CreateProjectDialog>("Simple Dialog", options);

        // wait modal to close
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }

    async Task RowClicked(DataGridRowClickEventArgs<Project> args)
    {
        var parameters = new DialogParameters { ["project"]=args.Item };
        IMudExDialogReference<ProjectDialog>? dlgReference = await DialogService.ShowEx<ProjectDialog>("Simple Dialog", parameters, maxWidthEx);

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
                var records = model.Select(e => new IFR.Export.ProjectExport()
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
                ExportToCsv.DeleteFile(file);
                break;
        }
    }
    
    private async Task BtnActions(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["projects"]=selectedProjects };

                var dialog =  DialogService.Show<DeleteProjectBulkDialog>("Edit", parameters,mediumWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<Project> items)
    {
        
        selectedProjects = items.ToList();
    }
    
}