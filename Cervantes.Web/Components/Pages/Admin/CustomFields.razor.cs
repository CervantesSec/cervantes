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
using Cervantes.Web.Components.Pages.Admin.Vuln;
using Cervantes.Web.Components.Pages.Admin.Project;
using Cervantes.Web.Components.Pages.Admin.Client;
using Cervantes.Web.Components.Pages.Admin.Target;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin;

public partial class CustomFields : ComponentBase
{
    // Vuln Custom Fields
    private List<VulnCustomFieldViewModel> vulnModel = new List<VulnCustomFieldViewModel>();
    private List<VulnCustomFieldViewModel> selectedVulnCustomFields = new List<VulnCustomFieldViewModel>();
    
    // Project Custom Fields
    private List<ProjectCustomFieldViewModel> projectModel = new List<ProjectCustomFieldViewModel>();
    private List<ProjectCustomFieldViewModel> selectedProjectCustomFields = new List<ProjectCustomFieldViewModel>();
    
    // Client Custom Fields
    private List<ClientCustomFieldViewModel> clientModel = new List<ClientCustomFieldViewModel>();
    private List<ClientCustomFieldViewModel> selectedClientCustomFields = new List<ClientCustomFieldViewModel>();
    
    // Target Custom Fields
    private List<TargetCustomFieldViewModel> targetModel = new List<TargetCustomFieldViewModel>();
    private List<TargetCustomFieldViewModel> selectedTargetCustomFields = new List<TargetCustomFieldViewModel>();
    
    private List<BreadcrumbItem> _items;
    private string vulnSearchString = "";
    private string projectSearchString = "";
    private string clientSearchString = "";
    private string targetSearchString = "";
    
    [Inject] private VulnCustomFieldController VulnCustomFieldController { get; set; }
    [Inject] private ProjectCustomFieldController ProjectCustomFieldController { get; set; }
    [Inject] private ClientCustomFieldController ClientCustomFieldController { get; set; }
    [Inject] private TargetCustomFieldController TargetCustomFieldController { get; set; }
    
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
        await UpdateVulnFields();
        await UpdateProjectFields();
        await UpdateClientFields();
        await UpdateTargetFields();
    }

    private async Task UpdateVulnFields()
    {
        try
        {
            var customFields = VulnCustomFieldController.Get();
            vulnModel = customFields.Select(cf => new VulnCustomFieldViewModel
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
            Snackbar.Add($"Error loading vuln custom fields: {ex.Message}", MudBlazor.Severity.Error);
        }
    }
    
    private async Task UpdateProjectFields()
    {
        try
        {
            var customFields = ProjectCustomFieldController.Get();
            projectModel = customFields.Select(cf => new ProjectCustomFieldViewModel
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
            Snackbar.Add($"Error loading project custom fields: {ex.Message}", MudBlazor.Severity.Error);
        }
    }
    
    private async Task UpdateClientFields()
    {
        try
        {
            var customFields = ClientCustomFieldController.Get();
            clientModel = customFields.Select(cf => new ClientCustomFieldViewModel
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
            Snackbar.Add($"Error loading client custom fields: {ex.Message}", MudBlazor.Severity.Error);
        }
    }
    
    private async Task UpdateTargetFields()
    {
        try
        {
            var customFields = TargetCustomFieldController.Get();
            targetModel = customFields.Select(cf => new TargetCustomFieldViewModel
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
            Snackbar.Add($"Error loading target custom fields: {ex.Message}", MudBlazor.Severity.Error);
        }
    }

    #region Vuln Custom Fields Methods

    private async Task OpenCreateVulnDialog(DialogOptionsEx options)
    {
        IMudExDialogReference<CreateVulnCustomFieldDialog>? dlgReference = await Dialog.ShowExAsync<CreateVulnCustomFieldDialog>(localizer["createCustomField"], maxWidthEx);
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await UpdateVulnFields();
            StateHasChanged();
        }
    }
    

    private async Task OpenEditVulnDialog(VulnCustomFieldViewModel customField, DialogOptionsEx options)
    {
        var parameters = new DialogParameters 
        { 
            ["customFieldSelected"] = customField
        };

        IMudExDialogReference<VulnCustomFieldDetailsDialog>? dlgReference = await Dialog.ShowExAsync<VulnCustomFieldDetailsDialog>(localizer["editCustomField"], parameters, options);
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await UpdateVulnFields();
            StateHasChanged();
        }
    }
    
    private async Task OpenDeleteVulnDialog(Guid customFieldId, DialogOptionsEx options)
    {
        var customField = vulnModel.FirstOrDefault(cf => cf.Id == customFieldId);
        if (customField == null) return;
        
        var parameters = new DialogParameters { ["customField"] = customField };
        IMudExDialogReference<DeleteVulnCustomFieldDialog>? dlgReference = await Dialog.ShowExAsync<DeleteVulnCustomFieldDialog>(localizer["delete"], parameters, options);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await UpdateVulnFields();
            StateHasChanged();
        }
    }
    
    private async Task OpenDeleteVulnBulkDialog(DialogOptionsEx options)
    {
        var parameters = new DialogParameters { ["customFields"] = selectedVulnCustomFields };
        IMudExDialogReference<DeleteVulnCustomFieldBulkDialog>? dlgReference = await Dialog.ShowExAsync<DeleteVulnCustomFieldBulkDialog>(localizer["delete"], parameters, options);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await UpdateVulnFields();
            StateHasChanged();
        }
    }

    private void VulnSelectedItemsChanged(HashSet<VulnCustomFieldViewModel> items)
    {
        selectedVulnCustomFields = items.ToList();
    }

    async Task VulnRowClicked(DataGridRowClickEventArgs<VulnCustomFieldViewModel> args)
    {
        await OpenEditVulnDialog(args.Item, maxWidthEx);
    }

    private Func<VulnCustomFieldViewModel, bool> _vulnQuickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(vulnSearchString))
            return true;

        if (x.Name?.Contains(vulnSearchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (x.Label?.Contains(vulnSearchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (x.Description?.Contains(vulnSearchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        return false;
    };

    #endregion

    #region Project Custom Fields Methods

    private async Task OpenCreateProjectDialog(DialogOptionsEx options)
    {
        IMudExDialogReference<CreateProjectCustomFieldDialog>? dlgReference = await Dialog.ShowExAsync<CreateProjectCustomFieldDialog>(localizer["createCustomField"], maxWidthEx);
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await UpdateProjectFields();
            StateHasChanged();
        }
    }
    
    
    private async Task OpenEditProjectDialog(Guid customFieldId, DialogOptionsEx options)
    {
        var customField = projectModel.FirstOrDefault(cf => cf.Id == customFieldId);
        if (customField == null) return;
        
        var parameters = new DialogParameters 
        { 
            ["customFieldSelected"] = customField
        };

        IMudExDialogReference<ProjectCustomFieldDetailsDialog>? dlgReference = await Dialog.ShowExAsync<ProjectCustomFieldDetailsDialog>(localizer["editCustomField"], parameters, options);
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await UpdateProjectFields();
            StateHasChanged();
        }
    }

    private async Task OpenDeleteProjectDialog(Guid customFieldId, DialogOptionsEx options)
    {
        var customField = projectModel.FirstOrDefault(cf => cf.Id == customFieldId);
        if (customField == null) return;
        
        var parameters = new DialogParameters { ["customField"] = customField };
        IMudExDialogReference<DeleteProjectCustomFieldDialog>? dlgReference = await Dialog.ShowExAsync<DeleteProjectCustomFieldDialog>(localizer["delete"], parameters, options);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await UpdateProjectFields();
            StateHasChanged();
        }
    }
    
    private async Task OpenDeleteProjectBulkDialog(DialogOptionsEx options)
    {
        var parameters = new DialogParameters { ["customFields"] = selectedProjectCustomFields };
        IMudExDialogReference<DeleteProjectCustomFieldBulkDialog>? dlgReference = await Dialog.ShowExAsync<DeleteProjectCustomFieldBulkDialog>(localizer["delete"], parameters, options);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await UpdateProjectFields();
            StateHasChanged();
        }
    }

    private void ProjectSelectedItemsChanged(HashSet<ProjectCustomFieldViewModel> items)
    {
        selectedProjectCustomFields = items.ToList();
    }

    private async Task ProjectRowClicked(DataGridRowClickEventArgs<ProjectCustomFieldViewModel> args)
    {
        await OpenEditProjectDialog(args.Item.Id, maxWidthEx);
    }

    private Func<ProjectCustomFieldViewModel, bool> _projectQuickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(projectSearchString))
            return true;

        if (x.Name?.Contains(projectSearchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (x.Label?.Contains(projectSearchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (x.Description?.Contains(projectSearchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        return false;
    };

    #endregion

    #region Client Custom Fields Methods

    private async Task OpenCreateClientDialog(DialogOptionsEx options)
    {
        IMudExDialogReference<CreateClientCustomFieldDialog>? dlgReference = await Dialog.ShowExAsync<CreateClientCustomFieldDialog>(localizer["createCustomField"], maxWidthEx);
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await UpdateClientFields();
            StateHasChanged();
        }
    }
    
    
    private async Task OpenEditClientDialog(Guid customFieldId, DialogOptionsEx options)
    {
        var customField = clientModel.FirstOrDefault(cf => cf.Id == customFieldId);
        if (customField == null) return;
        
        var parameters = new DialogParameters 
        { 
            ["customFieldSelected"] = customField
        };

        IMudExDialogReference<ClientCustomFieldDetailsDialog>? dlgReference = await Dialog.ShowExAsync<ClientCustomFieldDetailsDialog>(localizer["editCustomField"], parameters, options);
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await UpdateClientFields();
            StateHasChanged();
        }
    }

    private async Task OpenDeleteClientDialog(Guid customFieldId, DialogOptionsEx options)
    {
        var customField = clientModel.FirstOrDefault(cf => cf.Id == customFieldId);
        if (customField == null) return;
        
        var parameters = new DialogParameters { ["customField"] = customField };
        IMudExDialogReference<DeleteClientCustomFieldDialog>? dlgReference = await Dialog.ShowExAsync<DeleteClientCustomFieldDialog>(localizer["delete"], parameters, options);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await UpdateClientFields();
            StateHasChanged();
        }
    }
    
    private async Task OpenDeleteClientBulkDialog(DialogOptionsEx options)
    {
        var parameters = new DialogParameters { ["customFields"] = selectedClientCustomFields };
        IMudExDialogReference<DeleteClientCustomFieldBulkDialog>? dlgReference = await Dialog.ShowExAsync<DeleteClientCustomFieldBulkDialog>(localizer["delete"], parameters, options);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await UpdateClientFields();
            StateHasChanged();
        }
    }

    private void ClientSelectedItemsChanged(HashSet<ClientCustomFieldViewModel> items)
    {
        selectedClientCustomFields = items.ToList();
    }

    private async Task ClientRowClicked(DataGridRowClickEventArgs<ClientCustomFieldViewModel> args)
    {
        await OpenEditClientDialog(args.Item.Id, maxWidthEx);
    }

    private Func<ClientCustomFieldViewModel, bool> _clientQuickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(clientSearchString))
            return true;

        if (x.Name?.Contains(clientSearchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (x.Label?.Contains(clientSearchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (x.Description?.Contains(clientSearchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        return false;
    };

    #endregion

    #region Target Custom Fields Methods

    private async Task OpenCreateTargetDialog(DialogOptionsEx options)
    {
        IMudExDialogReference<CreateTargetCustomFieldDialog>? dlgReference = await Dialog.ShowExAsync<CreateTargetCustomFieldDialog>(localizer["createCustomField"], maxWidthEx);
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await UpdateTargetFields();
            StateHasChanged();
        }
    }
    
    
    private async Task OpenEditTargetDialog(Guid customFieldId, DialogOptionsEx options)
    {
        var customField = targetModel.FirstOrDefault(cf => cf.Id == customFieldId);
        if (customField == null) return;
        
        var parameters = new DialogParameters 
        { 
            ["customFieldSelected"] = customField
        };

        IMudExDialogReference<TargetCustomFieldDetailsDialog>? dlgReference = await Dialog.ShowExAsync<TargetCustomFieldDetailsDialog>(localizer["editCustomField"], parameters, options);
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await UpdateTargetFields();
            StateHasChanged();
        }
    }

    private async Task OpenDeleteTargetDialog(Guid customFieldId, DialogOptionsEx options)
    {
        var customField = targetModel.FirstOrDefault(cf => cf.Id == customFieldId);
        if (customField == null) return;
        
        var parameters = new DialogParameters { ["customField"] = customField };
        IMudExDialogReference<DeleteTargetCustomFieldDialog>? dlgReference = await Dialog.ShowExAsync<DeleteTargetCustomFieldDialog>(localizer["delete"], parameters, options);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await UpdateTargetFields();
            StateHasChanged();
        }
    }
    
    private async Task OpenDeleteTargetBulkDialog(DialogOptionsEx options)
    {
        var parameters = new DialogParameters { ["customFields"] = selectedTargetCustomFields };
        IMudExDialogReference<DeleteTargetCustomFieldBulkDialog>? dlgReference = await Dialog.ShowExAsync<DeleteTargetCustomFieldBulkDialog>(localizer["delete"], parameters, options);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await UpdateTargetFields();
            StateHasChanged();
        }
    }

    private void TargetSelectedItemsChanged(HashSet<TargetCustomFieldViewModel> items)
    {
        selectedTargetCustomFields = items.ToList();
    }

    private async Task TargetRowClicked(DataGridRowClickEventArgs<TargetCustomFieldViewModel> args)
    {
        await OpenEditTargetDialog(args.Item.Id, maxWidthEx);
    }

    private Func<TargetCustomFieldViewModel, bool> _targetQuickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(targetSearchString))
            return true;

        if (x.Name?.Contains(targetSearchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (x.Label?.Contains(targetSearchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (x.Description?.Contains(targetSearchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        return false;
    };

    #endregion
}