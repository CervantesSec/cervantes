using System.Net.Http.Json;
using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Project;

public partial class ProjectCustomFields : ComponentBase
{
    private List<ProjectCustomFieldViewModel> model = new List<ProjectCustomFieldViewModel>();
    private List<ProjectCustomFieldViewModel> selectedCustomFields = new List<ProjectCustomFieldViewModel>();
    private List<BreadcrumbItem> _items;
    private string searchString = "";
    
    [Inject] private ProjectCustomFieldController ProjectCustomFieldController { get; set; }
    
    private ClaimsPrincipal userAuth;
    
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
    
    protected override async Task OnInitializedAsync()
    {
        userAuth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(localizer["home"], href: "/", icon: Icons.Material.Filled.Home),
            new BreadcrumbItem(localizer["Admin"], href: null, icon: Icons.Material.Filled.AdminPanelSettings),
            new BreadcrumbItem(localizer["customFields"], href: null, disabled: true, icon: Icons.Material.Filled.DynamicForm),
        };
        await Update();
    }

    private async Task Update()
    {
        try
        {
            var customFields = ProjectCustomFieldController.Get();
            model = customFields.Select(cf => new ProjectCustomFieldViewModel
            {
                Id = cf.Id,
                Name = cf.Name,
                Label = cf.Label,
                Type = cf.Type,
                IsRequired = cf.IsRequired,
                IsUnique = cf.IsUnique,
                IsSearchable = cf.IsSearchable,
                IsVisible = cf.IsVisible,
                Order = cf.Order,
                Options = cf.Options,
                DefaultValue = cf.DefaultValue,
                Description = cf.Description,
                IsActive = cf.IsActive
            }).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading custom fields: {ex.Message}", MudBlazor.Severity.Error);
        }
    }

    private async Task OpenCreateDialog(DialogOptionsEx options)
    {
        IMudExDialogReference<CreateProjectCustomFieldDialog>? dlgReference = await Dialog.ShowExAsync<CreateProjectCustomFieldDialog>(localizer["createCustomField"], maxWidthEx);
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
    }
    
    private async Task OpenEditDialog(Guid customFieldId, DialogOptionsEx options)
    {
        var customField = model.FirstOrDefault(cf => cf.Id == customFieldId);
        if (customField == null) return;
        
        var parameters = new DialogParameters 
        { 
            ["customFieldSelected"] = customField
        };

        IMudExDialogReference<ProjectCustomFieldDetailsDialog>? dlgReference = await Dialog.ShowExAsync<ProjectCustomFieldDetailsDialog>(localizer["editCustomField"], parameters, options);
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
    }

    private async Task OpenDeleteDialog(Guid customFieldId, DialogOptionsEx options)
    {
        var customField = model.FirstOrDefault(cf => cf.Id == customFieldId);
        if (customField == null) return;
        
        var parameters = new DialogParameters { ["customField"] = customField };
        IMudExDialogReference<DeleteProjectCustomFieldDialog>? dlgReference = await Dialog.ShowExAsync<DeleteProjectCustomFieldDialog>(localizer["delete"], parameters, options);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
    }
    
    private async Task OpenDeleteBulkDialog(DialogOptionsEx options)
    {
        var parameters = new DialogParameters { ["customFields"] = selectedCustomFields };
        IMudExDialogReference<DeleteProjectCustomFieldBulkDialog>? dlgReference = await Dialog.ShowExAsync<DeleteProjectCustomFieldBulkDialog>(localizer["delete"], parameters, options);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
    }

    private void SelectedItemsChanged(HashSet<ProjectCustomFieldViewModel> items)
    {
        selectedCustomFields = items.ToList();
    }

    private void RowClicked(DataGridRowClickEventArgs<ProjectCustomFieldViewModel> args)
    {
        // Handle row click if needed
    }

    private Func<ProjectCustomFieldViewModel, bool> _quickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;

        if (x.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (x.Label?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (x.Description?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        return false;
    };
}