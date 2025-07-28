using Cervantes.CORE.ViewModel;
using Cervantes.IFR.CveServices;
using Cervantes.Web.Controllers;
using Cervantes.Web.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Linq;

namespace Cervantes.Web.Components.Pages.Cve;

public partial class CveSyncDialog
{
    [Inject] private CveSyncController _cveSyncController { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private IStringLocalizer<Resource> localizer { get; set; }

    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }

    public CveSyncOptionsViewModel Model { get; set; } = new();
    public ValidationResult? ValidationResult { get; set; }
    public bool IsProcessing { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Set default values
        Model.MaxTotalCves = 1000;
        Model.ResultsPerPage = 100;
        Model.SkipExisting = true;
        Model.UpdateExisting = true;
    }

    private async Task ApplyPreset(string presetKey)
    {
        Model.QuickPreset = presetKey;
        Model.ApplyQuickPreset(presetKey);
        await ValidateOptionsAsync();
        StateHasChanged();
    }

    private async Task ValidateOptionsAsync()
    {
        try
        {
            ValidationResult = _cveSyncController.ValidateOptionsInternal(Model);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error validating options: {ex.Message}", Severity.Error);
        }
    }

    private async Task StartSyncAsync()
    {
        try
        {
            IsProcessing = true;
            StateHasChanged();

            // Final validation
            ValidationResult = _cveSyncController.ValidateOptionsInternal(Model);
            if (!ValidationResult.IsValid)
            {
                Snackbar.Add(localizer["validationFailed"], Severity.Error);
                return;
            }

            var result = await _cveSyncController.SyncCvesInternalAsync(Model);

            if (result.IsSuccess)
            {
                var message = $"{localizer["syncCompleted"]} - " +
                            $"{localizer["new"]}: {result.NewCount}, " +
                            $"{localizer["updated"]}: {result.UpdatedCount}, " +
                            $"{localizer["skipped"]}: {result.SkippedCount}";
                
                if (result.ErrorCount > 0)
                {
                    message += $", {localizer["errors"]}: {result.ErrorCount}";
                }

                Snackbar.Add(message, Severity.Success);
                MudDialog.Close(DialogResult.Ok(result));
            }
            else
            {
                Snackbar.Add($"{localizer["syncFailed"]}: {result.ErrorMessage}", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"{localizer["syncError"]}: {ex.Message}", Severity.Error);
        }
        finally
        {
            IsProcessing = false;
            StateHasChanged();
        }
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }

    private Color GetSeverityColor(string severity)
    {
        return severity switch
        {
            "CRITICAL" => Color.Error,
            "HIGH" => Color.Warning,
            "MEDIUM" => Color.Info,
            "LOW" => Color.Success,
            _ => Color.Default
        };
    }

    private void OnSeveritiesChanged(IEnumerable<string> values)
    {
        Model.Severities = values.ToList();
    }

    private string GetSourceIcon(string source)
    {
        return source switch
        {
            "NVD" => Icons.Material.Filled.Security,
            "RedHat" => Icons.Material.Filled.Business,
            "MITRE" => Icons.Material.Filled.Shield,
            _ => Icons.Material.Filled.DataUsage
        };
    }
}