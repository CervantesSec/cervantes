using System.Net.Http.Json;
using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.IFR.Export;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class Projects: ComponentBase
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
    protected override async Task OnInitializedAsync()
    {
    _items = new List<BreadcrumbItem>
    {
        new BreadcrumbItem(localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
        new BreadcrumbItem(localizer["projects"], href: null, disabled: true,icon: Icons.Material.Filled.Folder)
    };
        await Update();
        if (project != Guid.Empty)
        {
            if (navigationManager.Uri.Contains($"/projects/{project}"))
            {
                var pro = model.FirstOrDefault(x => x.Id == project);
                if (pro != null)
                {
                    await DetailsDialog(pro ,maxWidth);
                }
            
            }
        }
        
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

    private async Task OpenDialogCreate(DialogOptions options)
    {

        var dialog = DialogService.Show<CreateProjectDialog>("Custom Options Dialog", options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    
    async Task DetailsDialog(Project project, DialogOptions options)
    {
        /*ProjectDetailsViewModel model = new ProjectDetailsViewModel();
        //model.Project = project;
        model.Client = project.Client;
        model.Members = await Http.GetFromJsonAsync<List<CORE.Entities.ProjectUser>>("api/Project/Members/"+project.Id);
        model.Targets = await Http.GetFromJsonAsync<List<CORE.Entities.Target>>("api/Target/Project/"+project.Id);
        model.Tasks = await Http.GetFromJsonAsync<List<CORE.Entities.Task>>("api/Task/Project/"+project.Id);
        model.Vulns = await Http.GetFromJsonAsync<List<CORE.Entities.Vuln>>("api/Vuln/Project/"+project.Id);
        model.Notes = await Http.GetFromJsonAsync<List<CORE.Entities.ProjectNote>>("api/Project/Note/"+project.Id);
        model.Attachments = await Http.GetFromJsonAsync<List<CORE.Entities.ProjectAttachment>>("api/Project/Attachment/"+project.Id);
        model.Reports = await Http.GetFromJsonAsync<List<CORE.Entities.Report>>("api/Report/Project/"+project.Id);*/
        /*var parameters = new DialogParameters { ["Project"]=project };
        var dialog =  DialogService.Show<DetailsDialog>(@localizer["projectDetails"], parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }*/
    }
    
    async Task RowClicked(DataGridRowClickEventArgs<Project> args)
    {
        var parameters = new DialogParameters { ["project"]=args.Item };

        var dialog =  DialogService.Show<ProjectDialog>("Edit", parameters, maxWidth);
        var result = await dialog.Result;

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

                var dialog =  DialogService.Show<DeleteProjectBulkDialog>("Edit", parameters,maxWidth);
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