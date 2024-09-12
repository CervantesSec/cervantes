using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Layout;

public partial class UserMenu: ComponentBase
{
    private string CurrentUserId { get; set; }
    private ApplicationUser CurrentUser { get; set; }
    [Inject] UserController userController { get; set; }
    private string? currentUrl;

    protected override async Task OnInitializedAsync()
    {
        NavigationManager.LocationChanged += OnLocationChanged;

        currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);

        if (_accessor.HttpContext.User == null)
        {
            NavigationManager.NavigateTo("Account/Login");
            return;
        }
        
        if (_accessor.HttpContext.User?.Identity?.IsAuthenticated == true)
        {
            CurrentUser = userController.GetUser(_accessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        await base.OnInitializedAsync();
    }
    
    private string GetAntiforgeryToken()
    {
        return Antiforgery.GetAndStoreTokens(_accessor.HttpContext).RequestToken;
    }
    
    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        currentUrl = NavigationManager.ToBaseRelativePath(e.Location);

        StateHasChanged();
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }
}