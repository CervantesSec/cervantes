using System.Net.Http.Json;
using System.Security.Claims;
using Blazor.Flags;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.IFR.Export;
using Cervantes.IFR.Jira;
using Cervantes.Web.Components.Pages.Clients;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class Vulns : ComponentBase
{
    private List<CORE.Entities.Vuln> model = new List<CORE.Entities.Vuln>();
    private List<CORE.Entities.Vuln> seleVulns = new List<CORE.Entities.Vuln>();
    private List<VulnViewModel> model2 = new List<VulnViewModel>();
    private List<BreadcrumbItem> _items;
    private List<Project> Projects = new List<Project>();
    private string searchString = "";
    private string selectedTemplate = "All";
    private Guid selectedProject = Guid.Empty;
    private string selectedStatus = "All";
    private string selectedRisk = "All";
    private string selectedLanguage = "All";
    [Inject] private JiraController _jiraController { get; set; }
    private HashSet<Guid> openDialogs = new HashSet<Guid>();

    [Parameter] public Guid vuln { get; set; }
    [Inject] private VulnController VulnController { get; set; }
[Inject] private IExportToCsv ExportToCsv { get; set; }
[Inject] private UserController userController { get; set; }
private ApplicationUser user;
private bool jiraEnabled = false;
[Inject] private IJIraService JiraService { get; set; }
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
            new BreadcrumbItem(localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem(localizer["vulns"], href: null,disabled: true,icon: Icons.Material.Filled.BugReport),

        };
        await Update();
        //Projects = await Http.GetFromJsonAsync<List<CORE.Entities.Project>>("api/Project");
        if (vuln != Guid.Empty)
        {
            if (navigationManager.Uri.Contains($"/vulns/{vuln}"))
            {
                var pro = model.FirstOrDefault(x => x.Id == vuln);
                if (pro != null)
                {
                    //await DetailsDialog(pro ,maxWidth);
                }

            }
        }
        
        user = userController.GetUser(_accessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier)); 
        jiraEnabled = JiraService.JiraEnabled();

    }


private async Task Update()
    {

        model = VulnController.GetVulns().ToList();
}


    private async Task OpenDialogCreate(DialogOptionsEx options)
    {
        IMudExDialogReference<CreateVulnDialog>? dlgReference = await DialogService.ShowEx<CreateVulnDialog>("Simple Dialog", options);
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
        if (element.CreatedDate.ToString().Contains(searchString))
            return true;
        if (element.ModifiedDate.ToString().Contains(searchString))
            return true;
        return false;
    };
    
    async Task RowClicked(DataGridRowClickEventArgs<CORE.Entities.Vuln> args)
    {
        // Check if dialog is already open for this vulnerability
        if (openDialogs.Contains(args.Item.Id))
        {
            return;
        }
        
        // Add to tracking set
        openDialogs.Add(args.Item.Id);
        
        try
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
        finally
        {
            // Remove from tracking set when dialog is closed
            openDialogs.Remove(args.Item.Id);
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
            case 1:
                foreach (var vuln in seleVulns)
                {
                    var response = await _jiraController.Add(vuln.Id);
                    if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
                    {
                        Snackbar.Add(@localizer["jiraCreated"], Severity.Success);
                    }
                    else
                    {
                        Snackbar.Add(@localizer["jiraCreatedError"], Severity.Error);
                    }
                }
                break;
            case 2:
                foreach (var vuln in seleVulns)
                {
                    var response = await _jiraController.DeleteIssue(vuln.Id);
                    if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
                    {
                        Snackbar.Add(@localizer["jiraDeleted"], Severity.Success);
                    }
                    else
                    {
                        Snackbar.Add(@localizer["jiraDeletedError"], Severity.Error);
                    }
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<CORE.Entities.Vuln> items)
    {
        
        seleVulns = items.ToList();
    }
    
}
