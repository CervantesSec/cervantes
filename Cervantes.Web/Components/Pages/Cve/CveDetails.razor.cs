using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Contracts;
using Cervantes.CORE;
using global::AuthPermissions.BaseCode.PermissionsCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Cervantes.Web.Localization;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Cervantes.Web.Components.Pages.Cve;

[Authorize]
public partial class CveDetails
{
    [Parameter] public Guid Id { get; set; }
    

    private CORE.Entities.Cve cve;
    private bool loading = true;
    private List<BreadcrumbItem> breadcrumbs = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadCve();
        UpdateBreadcrumbs();
    }

    private async Task LoadCve()
    {
        loading = true;
        try
        {
            cve = CveManager.GetById(Id);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading CVE: {ex.Message}", Severity.Error);
        }
        finally
        {
            loading = false;
        }
    }

    private void UpdateBreadcrumbs()
    {
        breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(localizer["home"], href: "/"),
            new BreadcrumbItem(localizer["cveManagement"], href: "/cve"),
            new BreadcrumbItem(cve?.CveId ?? localizer["cveDetails"], href: null, disabled: true)
        };
    }

    private async Task ToggleFavorite()
    {
        try
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            await CveManager.MarkAsFavoriteAsync(cve.Id, userId);
            cve.IsFavorite = !cve.IsFavorite;
            
            var message = cve.IsFavorite ? "Added to favorites" : "Removed from favorites";
            Snackbar.Add(message, Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error updating favorite: {ex.Message}", Severity.Error);
        }
    }

    private async Task ArchiveCve()
    {
        try
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            await CveManager.ArchiveAsync(cve.Id, userId);
            cve.IsArchived = !cve.IsArchived;
            
            var message = cve.IsArchived ? "CVE archived" : "CVE unarchived";
            Snackbar.Add(message, Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error archiving CVE: {ex.Message}", Severity.Error);
        }
    }

    private DialogOptionsEx mediumWidthEx = new DialogOptionsEx() 
    {
        MaximizeButton = true,
        CloseButton = true,
        FullHeight = false,
        CloseOnEscapeKey = true,
        MaxWidth = MaxWidth.Medium,
        MaxHeight = MaxHeight.False,
        FullWidth = true,
        DragMode = MudDialogDragMode.Simple,
        Animations = new[] { MudBlazor.Extensions.Options.AnimationType.SlideIn },
        Position = DialogPosition.Center,
        DisableSizeMarginY = true,
        DisablePositionMargin = true,
        BackdropClick = false,
        Resizeable = true,
    };

    private async Task EditNotes()
    {
        var parameters = new DialogParameters<CveNotesDialog>
        {
            { x => x.CveId, cve.Id },
            { x => x.CurrentNotes, cve.Notes }
        };

        IMudExDialogReference<CveNotesDialog>? dlgReference = await DialogService.ShowEx<CveNotesDialog>("Edit CVE Notes", parameters, mediumWidthEx);
        var result = await dlgReference.Result;

        if (!result.Canceled && result.Data is string newNotes)
        {
            cve.Notes = newNotes;
            Snackbar.Add("Notes updated successfully", Severity.Success);
        }
    }

    private async Task ShareCve()
    {
        var url = Navigation.Uri;
        Navigation.NavigateTo("javascript:navigator.clipboard.writeText('" + url + "')");
        Snackbar.Add("CVE URL copied to clipboard", Severity.Success);
    }

    private Color GetSeverityColor(string severity)
    {
        return severity?.ToUpper() switch
        {
            "CRITICAL" => Color.Error,
            "HIGH" => Color.Warning,
            "MEDIUM" => Color.Info,
            "LOW" => Color.Success,
            _ => Color.Default
        };
    }

    private Color GetStateColor(string state)
    {
        return state?.ToUpper() switch
        {
            "PUBLISHED" => Color.Success,
            "MODIFIED" => Color.Info,
            "WITHDRAWN" => Color.Warning,
            "REJECTED" => Color.Error,
            _ => Color.Default
        };
    }
}