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

public partial class ChecklistTemplates: ComponentBase
{
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private ChecklistController _checklistController { get; set; }
    [Inject] AuthenticationStateProvider authenticationStateProvider { get; set; }
    [Parameter] public Guid project { get; set; }
    
    private List<ChecklistTemplate> model = new List<ChecklistTemplate>();
    private List<ChecklistTemplate> selectedTemplates = new List<ChecklistTemplate>();
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
            var response = await _checklistController.GetCustomTemplates();
            if (response.Result is OkObjectResult okResult)
            {
                model = (List<ChecklistTemplate>)okResult.Value;
            }
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Add(@localizer["errorOccurred"], Severity.Error);
        }
    }

    private void SetBreadcrumbs()
    {
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/", icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Workspace", href: "/workspaces", icon: Icons.Material.Filled.Workspaces),
            new BreadcrumbItem(@localizer["project"], href: "/workspace/"+project, icon: Icons.Material.Filled.Folder),
            new BreadcrumbItem(@localizer["checklistTemplates"], href: null, disabled: true, icon: Icons.Material.Filled.LibraryBooks)
        };
    }

    private Func<ChecklistTemplate, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;

        if (element.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (element.Description?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (element.Version?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        return false;
    };

    void SelectedItemsChanged(HashSet<ChecklistTemplate> items)
    {
        selectedTemplates = items.ToList();
    }

    async System.Threading.Tasks.Task RowClicked(DataGridRowClickEventArgs<ChecklistTemplate> args)
    {
        await OpenDialogView(args.Item, maxWidthEx);
    }

    async System.Threading.Tasks.Task OpenDialogCreate(DialogOptionsEx options)
    {
        var dialog = await DialogService.ShowExAsync<CreateChecklistTemplateDialog>(@localizer["createChecklistTemplate"], options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await Update();
        }
    }

    async System.Threading.Tasks.Task OpenDialogView(ChecklistTemplate template, DialogOptionsEx options)
    {
        // Cargar el template completo con categorías e items para el dialog
        var response = await _checklistController.GetCustomTemplate(template.Id);
        ChecklistTemplate fullTemplate = template; // fallback en caso de error
        
        if (response.Result is Microsoft.AspNetCore.Mvc.OkObjectResult okResult)
        {
            fullTemplate = (ChecklistTemplate)okResult.Value;
        }
        
        var parameters = new DialogParameters { ["template"] = fullTemplate };
        var dialog = await DialogService.ShowExAsync<ChecklistTemplateDialog>(@localizer["viewChecklistTemplate"], parameters, options);
        var result = await dialog.Result;
        
        if (!result.Canceled)
        {
            await Update();
        }
    }

    async System.Threading.Tasks.Task OpenDialogDelete(ChecklistTemplate template, DialogOptionsEx options)
    {
        var parameters = new DialogParameters { ["template"] = template };
        IMudExDialogReference<DeleteChecklistTemplateDialog>? dialog = await DialogService.ShowExAsync<DeleteChecklistTemplateDialog>(@localizer["deleteChecklistTemplate"], parameters, options);
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
                var parameters = new DialogParameters { ["templates"] = selectedTemplates };
                IMudExDialogReference<DeleteChecklistTemplateBulkDialog>? dialog = await DialogService.ShowExAsync<DeleteChecklistTemplateBulkDialog>(@localizer["deleteChecklistTemplates"], parameters, middleWidthEx);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await Update();
                    selectedTemplates = new List<ChecklistTemplate>();
                }
                break;
        }
    }
}