using System.Net.Http.Json;
using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Components.Pages.Workspace.Target;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class Checklists: ComponentBase
{
       [Inject] ISnackbar Snackbar { get; set; }
       [Inject] private ChecklistController _checklistController { get; set; }
       [Inject] private ProjectController _ProjectController { get; set; }

       [Parameter] public Guid project { get; set; }
    
        // Legacy checklists (WSTG/MASTG)
        private List<ChecklistViewModel> legacyModel = new List<ChecklistViewModel>();
        private List<ChecklistViewModel> seleChecklists = new List<ChecklistViewModel>();
        private string legacySearchString = "";

        private CORE.Entities.Project Project = new CORE.Entities.Project();
        private List<BreadcrumbItem> _items;
    DialogOptionsEx mediumWidth = new DialogOptionsEx() { MaxWidth = MaxWidth.Medium, FullWidth = true };

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
    DialogOptionsEx middleWidthEx = new DialogOptionsEx() 
    {
        MaximizeButton = true,
        CloseButton = true,
        FullHeight = false,
        CloseOnEscapeKey = true,
        MaxWidth = MaxWidth.Medium,
        MaxHeight = MaxHeight.False,
        FullWidth = true,
        DragMode = MudDialogDragMode.Simple,
        Animations = new[] { AnimationType.SlideIn },
        Position = DialogPosition.Center,
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
        await Update();
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Workspace", href: "/workspaces",icon: Icons.Material.Filled.Workspaces),
            new BreadcrumbItem(Project.Name, href: "/workspace/"+project,icon: Icons.Material.Filled.Folder),
            new BreadcrumbItem(@localizer["legacyChecklists"], href: null, disabled: true, icon: Icons.Material.Filled.Security)
        };
        await base.OnInitializedAsync();
    }

    private async Task Update()
    {
        await UpdateLegacyChecklists();
    }

    private async Task UpdateLegacyChecklists()
    {
        legacyModel.RemoveAll(item => true);
        var wstg = _checklistController.GetWSTG(project);
        var mstg = _checklistController.GetMSTG(project);
        Project = new CORE.Entities.Project();
        Project = _ProjectController.GetById(project);
        
        foreach (var item in wstg)
        {
            var itemWstg  = new ChecklistViewModel
            {
                Id = item.Id,
                Target = item.Target.Name,
                CreatedDate = item.CreatedDate,
                User = item.User.FullName,
                Type = ChecklistType.OWASPWSTG,
            };
            
            legacyModel.Add(itemWstg);
        }
        
        foreach (var item in mstg)
        {
            var itemMstg = new ChecklistViewModel()
            {
                Id = item.Id,
                Target = item.Target.Name,
                CreatedDate = item.CreatedDate,
                User = item.User.FullName,
                Type = ChecklistType.OWASPMASVS,
                Platform = item.MobilePlatform,
            };
            
            legacyModel.Add(itemMstg);
        }
    }


    
    
    // Legacy checklist filter
    private Func<ChecklistViewModel, bool> _legacyQuickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(legacySearchString))
            return true;
        if (element.Target.Contains(legacySearchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.CreatedDate.ToString().Contains(legacySearchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Type.ToString().Contains(legacySearchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.User.Contains(legacySearchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };

    
    DialogOptionsEx maxWidth = new DialogOptionsEx() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    private async Task OpenImportDialog(DialogOptionsEx options)
    {
        var parameters = new DialogParameters { ["project"]=project };

        IMudExDialogReference<ImportDialog>? dialog = await DialogService.ShowExAsync<ImportDialog>("Custom Options Dialog", parameters, mediumWidth);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    
    private async Task OpenDialogCreate(DialogOptionsEx options)
    {
        var parameters = new DialogParameters { ["project"]=project };
        IMudExDialogReference<CreateChecklistDialog>? dlgReference = await DialogService.ShowExAsync<CreateChecklistDialog>("Simple Dialog", parameters, maxWidthEx);

        // wait modal to close
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
    
    async Task RowClicked(DataGridRowClickEventArgs<ChecklistViewModel> args)
    {
        if (args.Item.Type == ChecklistType.OWASPWSTG)
        {
            var parameters = new DialogParameters {["project"]=project, ["checklist"]=args.Item };
            IMudExDialogReference<WstgDialog>? dlgReference = await DialogService.ShowExAsync<WstgDialog>("Simple Dialog", parameters, maxWidthEx);

            var result = await dlgReference.Result;

            if (!result.Canceled)
            {
                await Update();
                StateHasChanged();
            }
        }
        else
        {
            var parameters = new DialogParameters { ["project"]=project,["checklist"]=args.Item };
            IMudExDialogReference<MastgDialog>? dlgReference = await DialogService.ShowExAsync<MastgDialog>("Simple Dialog", parameters, maxWidthEx);

            var result = await dlgReference.Result;

            if (!result.Canceled)
            {
                await Update();
                StateHasChanged();
            }
        }
        
    }
    
    private async Task BtnActions(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["checklists"]=seleChecklists };
                IMudExDialogReference<DeleteChecklistBulkDialog>? dlgReference = await DialogService.ShowExAsync<DeleteChecklistBulkDialog>("Simple Dialog", parameters, maxWidthEx);

                var result = await dlgReference.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedLegacyItemsChanged(HashSet<ChecklistViewModel> items)
    {
        seleChecklists = items.ToList();
    }


    private void OpenChecklistExecution(ChecklistViewModel checklist)
    {
        if (checklist.Type == ChecklistType.OWASPWSTG)
        {
            navigationManager.NavigateTo($"/workspace/{project}/wstg/{checklist.Id}");
        }
        else
        {
            navigationManager.NavigateTo($"/workspace/{project}/mastg/{checklist.Id}");
        }
    }

}