using System.Security.Claims;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.Export;
using Cervantes.Web.Components.Pages.Vuln;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;

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
    DialogOptions mediumWidth = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    [Inject] private IExportToCsv ExportToCsv { get; set; }

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
    private async Task OpenDialogCreate(DialogOptionsEx options)
    {

        var parameters = new DialogParameters { ["project"]=Project.Id };

        IMudExDialogReference<CreateVulnDialog>? dlgReference = await DialogService.ShowEx<CreateVulnDialog>("Simple Dialog", parameters, options);
        // wait modal to close
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    
    private async Task OpenDialogImport(DialogOptions options)
    {

        var dialog = DialogService.Show<ImportVulnDialog>("Custom Options Dialog", mediumWidth);
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

        var dialog =  DialogService.Show<DeleteVulnDialog>(@localizer["delete"], parameters,mediumWidth);
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
                List<VulnExport> test = new List<VulnExport>();
                foreach (var e in seleVulns)
                {
                    VulnExport vuln = new VulnExport();
                    vuln.Name = e.Name ?? "No Name";
                    vuln.Description = e.Description ?? "No Description";
                    vuln.CreatedUser = e.User.FullName ?? "No User";
                    vuln.CreatedDate = e.CreatedDate.ToShortDateString() ?? "No Date";
                    vuln.ModifiedDate = e.ModifiedDate.ToShortDateString() ?? "No Date";
                    vuln.Template = e.Template;
                    vuln.Status = e.Status.ToString();
                    vuln.Language = e.Language.ToString();
                    vuln.cve = e.cve ?? "No CVE";
                    vuln.CVSS3 = e.CVSS3;
                    vuln.CVSSVector = e.CVSSVector ?? "No Vector";
                    vuln.Impact = e.Impact ?? "No Impact";
                    vuln.JiraCreated = e.JiraCreated;
                    vuln.ProofOfConcept = e.ProofOfConcept ?? "No Proof";
                    vuln.Remediation = e.Remediation ?? "No Remediation";
                    vuln.RemediationComplexity = e.RemediationComplexity.ToString() ?? "No Complexity";
                    vuln.RemediationPriority = e.RemediationPriority.ToString() ?? "No Priority";
                    vuln.Risk = e.Risk.ToString() ?? "No Risk";
                    vuln.OWASPImpact = e.OWASPImpact?.ToString() ?? "No Impact";
                    vuln.OWASPLikehood = e.OWASPLikehood?.ToString() ?? "No Likehood";
                    vuln.OWASPRisk = e.OWASPRisk?.ToString() ?? "No Risk";
                    vuln.OWASPVector = e.OWASPVector?.ToString() ?? "No Vector";
                    vuln.VulnCategory = e.VulnCategory?.Name ?? "No Category";
                    vuln.Project = e.Project?.Name ?? "No Project";
                    test.Add(vuln);
                }
                
                var file = ExportToCsv.ExportVulns(test);
                await JS.InvokeVoidAsync("downloadFile", file);
                Snackbar.Add(@localizer["exportSuccessfull"], Severity.Success);
                ExportToCsv.DeleteFile(file);
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

        IMudExDialogReference<VulnDialog>? dlgReference = await DialogService.ShowEx<VulnDialog>("Simple Dialog", parameters, maxWidthEx);
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
                var parameters = new DialogParameters { ["vulns"]=seleVulns };

                var dialog =  DialogService.Show<DeleteVulnBulkDialog>("Edit", parameters,mediumWidth);
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