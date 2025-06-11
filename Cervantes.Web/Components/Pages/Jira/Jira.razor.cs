using System.Security.Claims;
using Cervantes.Web.Components.Pages.Vuln;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;

namespace Cervantes.Web.Components.Pages.Jira;

public partial class Jira: ComponentBase
{
    private List<BreadcrumbItem> _items;
    private List<CORE.Entities.Jira> model = new List<CORE.Entities.Jira>();
    private List<CORE.Entities.Jira> seleJiras = new List<CORE.Entities.Jira>();
    [Inject] private JiraController _JiraController { get; set; }
    private string searchString = "";
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    const string jiraSVG = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""24"" height=""24"" viewBox=""0 0 24 24""><path fill=""currentColor"" d=""M11.53 2c0 2.4 1.97 4.35 4.35 4.35h1.78v1.7c0 2.4 1.94 4.34 4.34 4.35V2.84a.84.84 0 0 0-.84-.84zM6.77 6.8a4.362 4.362 0 0 0 4.34 4.34h1.8v1.72a4.362 4.362 0 0 0 4.34 4.34V7.63a.841.841 0 0 0-.83-.83zM2 11.6c0 2.4 1.95 4.34 4.35 4.34h1.78v1.72c.01 2.39 1.95 4.34 4.34 4.34v-9.57a.84.84 0 0 0-.84-.84z""/></svg>";
    private ClaimsPrincipal userAth;
    DialogOptionsEx centerWidthEx = new DialogOptionsEx() 
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
        Position = DialogPosition.Center,
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
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Jira", href: null, disabled: true,icon: jiraSVG)
        };
        await Update();
        
    }
    
    private async Task Update()
    {
        model = _JiraController.GetJiras().ToList();
    }
    
    private Func<CORE.Entities.Jira, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Vuln.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.User.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.CreatedDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.JiraKey.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Assignee.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.JiraStatus.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
    
    async Task RowClicked(DataGridRowClickEventArgs<CORE.Entities.Jira> args)
    {
        var parameters = new DialogParameters { ["jira"]=args.Item };
        IMudExDialogReference<JiraDialog>? dlgReference = await DialogService.ShowExAsync<JiraDialog>("Simple Dialog", parameters, centerWidthEx);

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
            case 1:
                foreach (var vuln in seleJiras)
                {
                    var response = await _JiraController.Add(vuln.Id);
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
        }
    }
    void SelectedItemsChanged(HashSet<CORE.Entities.Jira> items)
    {
        
        seleJiras = items.ToList();
    }
}