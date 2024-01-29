using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Workspace.Target;

public partial class Targets: ComponentBase
{
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private TargetController _targetController { get; set; }
    [Inject] private ProjectController _ProjectController { get; set; }
    [Parameter] public Guid project { get; set; }
    private CORE.Entities.Project Project = new CORE.Entities.Project();
    
        private List<CORE.Entities.Target> model = new List<CORE.Entities.Target>();
        private List<CORE.Entities.Target> seleTargets = new List<CORE.Entities.Target>();

        private List<BreadcrumbItem> _items;
    private string searchString = "";

    protected override async Task OnInitializedAsync()
    {
        var user = await _ProjectController.VerifyUser(project);
        if (user == false)
        {
            Snackbar.Add(@localizer["noRolePermission"], Severity.Warning);
            navigationManager.NavigateTo("workspaces");
        }
        
        Project = _ProjectController.GetById(project);
    _items = new List<BreadcrumbItem>
    {
        new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
        new BreadcrumbItem("Workspace", href: "/workspaces",icon: Icons.Material.Filled.Workspaces),
        new BreadcrumbItem(Project.Name, href: "/workspace/"+project,icon: Icons.Material.Filled.Folder),
        new BreadcrumbItem(@localizer["targets"], href: null, disabled: true, icon: Icons.Material.Filled.Adjust)
    };
        await Update();
        await base.OnInitializedAsync();
    }
    
    protected async Task Update()
    {

        model = _targetController.GetByProjectId(project).ToList();
        
    }


    async Task RowClicked(DataGridRowClickEventArgs<CORE.Entities.Target> args)
    {
        var parameters = new DialogParameters { ["target"]=args.Item };

        var dialog =  DialogService.Show<TargetDialog>("Edit", parameters, maxWidth);
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
    
    private Func<CORE.Entities.Target, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Type.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.User.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
    
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    private async Task OpenImportDialog(DialogOptions options)
    {
        var parameters = new DialogParameters { ["project"]=project };

        var dialog = DialogService.Show<ImportDialog>("Custom Options Dialog", parameters, options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    private async Task OpenDialogCreate(DialogOptions options)
    {
        var parameters = new DialogParameters { ["project"]=project };

        var dialog = DialogService.Show<CreateTargetDialog>("Custom Options Dialog", parameters, options);
        // wait modal to close
        var result = await dialog.Result;
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
                var parameters = new DialogParameters { ["targets"]=seleTargets };

                var dialog =  DialogService.Show<DeleteTargetBulkDialog>("Edit", parameters,maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<CORE.Entities.Target> items)
    {
        
        seleTargets = items.ToList();
    }
    
 
}