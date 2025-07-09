using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class CustomChecklists: ComponentBase
{
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private ChecklistController _checklistController { get; set; }
    [Inject] AuthenticationStateProvider authenticationStateProvider { get; set; }
    [Parameter] public Guid project { get; set; }
    
    private List<CORE.Entities.Checklist> model = new List<CORE.Entities.Checklist>();
    private List<CORE.Entities.Checklist> selectedChecklists = new List<CORE.Entities.Checklist>();
    private List<BreadcrumbItem> _items = new List<BreadcrumbItem>();
    private string searchString = "";
    private ClaimsPrincipal userAuth;
    
    DialogOptionsEx maxWidth = new DialogOptionsEx() { MaxWidth = MaxWidth.Medium, FullWidth = true };
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
    protected override async System.Threading.Tasks.Task OnInitializedAsync()
    {
        userAuth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        await Update();
        SetBreadcrumbs();
        await base.OnInitializedAsync();
    }

    private async System.Threading.Tasks.Task Update()
    {
        try
        {
            var response = await _checklistController.GetCustomChecklistsByProject(project);
            if (response.Result is OkObjectResult okResult)
            {
                model = (List<CORE.Entities.Checklist>)okResult.Value;
            }
            else if (response.Value != null)
            {
                model = response.Value.ToList();
            }
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Add(@localizer["errorOccurred"], MudBlazor.Severity.Error);
        }
    }

    private void SetBreadcrumbs()
    {
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/", icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Workspace", href: "/workspaces", icon: Icons.Material.Filled.Workspaces),
            new BreadcrumbItem(@localizer["project"], href: "/workspace/"+project, icon: Icons.Material.Filled.Folder),
            new BreadcrumbItem(@localizer["customChecklists"], href: null, disabled: true, icon: Icons.Material.Filled.Checklist)
        };
    }

    private Func<CORE.Entities.Checklist, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;

        if (element.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (element.ChecklistTemplate?.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (element.Target?.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (element.Notes?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        return false;
    };

    void SelectedItemsChanged(HashSet<CORE.Entities.Checklist> items)
    {
        selectedChecklists = items.ToList();
    }

    async System.Threading.Tasks.Task RowClicked(DataGridRowClickEventArgs<CORE.Entities.Checklist> args)
    {
        await OpenDialogView(args.Item, maxWidthEx);
    }

    async System.Threading.Tasks.Task OpenDialogCreate(DialogOptionsEx options)
    {
        var parameters = new DialogParameters { ["project"] = project };
        IMudExDialogReference<CreateCustomChecklistDialog>? dialog = await DialogService.ShowExAsync<CreateCustomChecklistDialog>(@localizer["createCustomChecklist"], parameters, maxWidthEx);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await Update();
        }
    }

    async System.Threading.Tasks.Task OpenDialogView(CORE.Entities.Checklist checklist, DialogOptionsEx options)
    {
        var parameters = new DialogParameters { ["checklist"] = checklist };
        IMudExDialogReference<CustomChecklistDialog>? dialog = await DialogService.ShowExAsync<CustomChecklistDialog>(@localizer["viewCustomChecklist"], parameters, maxWidthEx);

        var result = await dialog.Result;
        
        if (!result.Canceled)
        {
            await Update();
        }
    }

    async System.Threading.Tasks.Task OpenDialogDelete(CORE.Entities.Checklist checklist, DialogOptionsEx options)
    {
        var parameters = new DialogParameters { ["checklist"] = checklist };
        IMudExDialogReference<DeleteCustomChecklistDialog>? dialog = await DialogService.ShowExAsync<DeleteCustomChecklistDialog>(@localizer["deleteCustomChecklist"], parameters, middleWidthEx);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await Update();
        }
    }

    async System.Threading.Tasks.Task BtnActions(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["checklists"] = selectedChecklists };
                IMudExDialogReference<DeleteCustomChecklistBulkDialog>? dialog = await DialogService.ShowExAsync<DeleteCustomChecklistBulkDialog>(@localizer["deleteCustomChecklists"], parameters, middleWidthEx);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await Update();
                    selectedChecklists = new List<CORE.Entities.Checklist>();
                }
                break;
        }
    }

    private void OpenChecklistExecution(CORE.Entities.Checklist checklist)
    {
        navigationManager.NavigateTo($"/workspace/{project}/custom-checklist/{checklist.Id}/execute");
    }
}