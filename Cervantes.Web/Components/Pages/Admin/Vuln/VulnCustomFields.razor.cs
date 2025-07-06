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

namespace Cervantes.Web.Components.Pages.Admin. Vuln;

public partial class VulnCustomFields : ComponentBase
{
    private List<VulnCustomFieldViewModel> model = new List<VulnCustomFieldViewModel>();
    private List<VulnCustomFieldViewModel> selectedCustomFields = new List<VulnCustomFieldViewModel>();
    private List<BreadcrumbItem> _items;
    private string searchString = "";
    
    [Inject] private VulnCustomFieldController VulnCustomFieldController { get; set; }
    
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
            var customFields = VulnCustomFieldController.Get();
            model = customFields.Select(cf => new VulnCustomFieldViewModel
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
        IMudExDialogReference<CreateVulnCustomFieldDialog>? dlgReference = await Dialog.ShowExAsync<CreateVulnCustomFieldDialog>(localizer["createCustomField"], maxWidthEx);
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
    }
    
    private async Task OpenEditDialog(VulnCustomFieldViewModel customField, DialogOptionsEx options)
    {
        var parameters = new DialogParameters 
        { 
            ["customFieldSelected"] = customField
        };

        IMudExDialogReference<VulnCustomFieldDetailsDialog>? dlgReference = await Dialog.ShowExAsync<VulnCustomFieldDetailsDialog>(localizer["editCustomField"], parameters, options);
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
    }

    private async Task DeleteDialog(VulnCustomFieldViewModel customField, DialogOptionsEx options)
    {
        var parameters = new DialogParameters { ["customField"] = customField };
        IMudExDialogReference<DeleteVulnCustomFieldDialog>? dlgReference = await Dialog.ShowExAsync<DeleteVulnCustomFieldDialog>(localizer["delete"], parameters, options);
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
                var parameters = new DialogParameters { ["customFields"] = selectedCustomFields };
                IMudExDialogReference<DeleteVulnCustomFieldBulkDialog>? dlgReference = await Dialog.ShowExAsync<DeleteVulnCustomFieldBulkDialog>(localizer["delete"], parameters, middleWidthEx);
                var result = await dlgReference.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    private Func<VulnCustomFieldViewModel, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Label.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Type.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (!string.IsNullOrEmpty(element.Description) && element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
    
    async Task RowClicked(DataGridRowClickEventArgs<VulnCustomFieldViewModel> args)
    {
        await OpenEditDialog(args.Item, maxWidthEx);
    }
    
    void SelectedItemsChanged(HashSet<VulnCustomFieldViewModel> items)
    {
        selectedCustomFields = items.ToList();
    }
    
    private string GetFieldTypeDisplay(VulnCustomFieldType type)
    {
        return type switch
        {
            VulnCustomFieldType.Input => localizer["fieldTypeInput"],
            VulnCustomFieldType.Textarea => localizer["fieldTypeTextarea"],
            VulnCustomFieldType.Select => localizer["fieldTypeSelect"],
            VulnCustomFieldType.Number => localizer["fieldTypeNumber"],
            VulnCustomFieldType.Date => localizer["fieldTypeDate"],
            VulnCustomFieldType.Boolean => localizer["fieldTypeBoolean"],
            _ => type.ToString()
        };
    }
}