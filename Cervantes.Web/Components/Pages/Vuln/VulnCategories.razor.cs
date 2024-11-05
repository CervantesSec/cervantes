using System.Security.Claims;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class VulnCategories : ComponentBase
{
    private List<BreadcrumbItem> _items;
    private List<CORE.Entities.VulnCategory> model = new List<CORE.Entities.VulnCategory>();
    private List<CORE.Entities.VulnCategory> seleCats = new List<CORE.Entities.VulnCategory>();
    private string searchString = "";
    [Inject] private VulnController VulnController { get; set; }
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    private ClaimsPrincipal userAth;
    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;

        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem(localizer["vulns"], href: "/vulns",icon: Icons.Material.Filled.BugReport),
            new BreadcrumbItem(localizer["categories"], href: null, disabled: true,icon: Icons.Material.Filled.Category)

        };
        await Update();
    }
    private async Task Update()
    {

        model = VulnController.GetCategories().ToList();
    }
    
    private async Task OpenDialogCreate(DialogOptions options)
    {

        var dialog = DialogService.Show<CreateVulnCategoryDialog>("Custom Options Dialog", options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    
    private Func<CORE.Entities.VulnCategory, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Description.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Type.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
    
    async Task RowClicked(DataGridRowClickEventArgs<CORE.Entities.VulnCategory> args)
    {
        var parameters = new DialogParameters { ["category"]=args.Item };

        var dialog =  DialogService.Show<VulnCategoryDialog>("Edit", parameters, maxWidth);
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
                var parameters = new DialogParameters { ["categories"]=seleCats };

                var dialog =  DialogService.Show<DeleteVulnCategoryBulkDialog>("Edit", parameters,maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<CORE.Entities.VulnCategory> items)
    {
        
        seleCats = items.ToList();
    }
    
}