using Cervantes.CORE.ViewModel;
using Cervantes.Web.Components.Pages.Vuln;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Workspace.Vulns;

public partial class WorkspaceVulns: ComponentBase
{
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private ProjectController _ProjectController { get; set; }
    [Parameter] public Guid project { get; set; }
    private CORE.Entities.Project Project = new CORE.Entities.Project();
    
    private List<CORE.Entities.Vuln> model = new List<CORE.Entities.Vuln>();
    private List<CORE.Entities.Vuln> seleVulns = new List<CORE.Entities.Vuln>();
    private List<BreadcrumbItem> _items;
    private string searchString = "";
    [Inject] private VulnController VulnController { get; set; }

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
            new BreadcrumbItem(@localizer["vulns"], href: null, disabled: true, icon: Icons.Material.Filled.BugReport)
        };
        await Update();
        await base.OnInitializedAsync();
    }
    
    private async Task Update()
    {

        model = VulnController.GetByProject(project).ToList();
      
    }


    DialogOptions fullScreen = new DialogOptions() { FullScreen = true, CloseButton = true };
    private async Task OpenDialogCreate(DialogOptions options)
    {

        var dialog = DialogService.Show<CreateVulnDialog>("Custom Options Dialog", options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    
    private async Task OpenDialogImport(DialogOptions options)
    {

        var dialog = DialogService.Show<ImportVulnDialog>("Custom Options Dialog", options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
   
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };

    async Task DeleteDialog(VulnViewModel vuln,DialogOptions options)
    {
        var parameters = new DialogParameters { ["vuln"]=vuln };

        var dialog =  DialogService.Show<DeleteVulnDialog>(@localizer["delete"], parameters,options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            //model.Remove(vuln);
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
    
    private Func<CORE.Entities.Vuln, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Risk.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Language.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Status.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Project.Name.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.CreatedDate.ToString().Contains(searchString))
            return true;
        if (element.ModifiedDate.ToString().Contains(searchString))
            return true;
        return false;
    };
    
    async Task RowClicked(DataGridRowClickEventArgs<CORE.Entities.Vuln> args)
    {
        var parameters = new DialogParameters { ["vuln"]=args.Item };

        var dialog =  DialogService.Show<VulnDialog>("Edit", parameters, maxWidth);
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
                var parameters = new DialogParameters { ["vulns"]=seleVulns };

                var dialog =  DialogService.Show<DeleteVulnBulkDialog>("Edit", parameters,maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<CORE.Entities.Vuln> items)
    {
        
        seleVulns = items.ToList();
    }
}