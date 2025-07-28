using Cervantes.CORE.ViewModel;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Cve;

public partial class TestResultsDialog
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public List<CveNotificationTestResult> Results { get; set; } = new();

    private void Cancel()
    {
        MudDialog.Cancel();
    }
}