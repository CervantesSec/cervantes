using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Cervantes.Web.Localization;
using MudBlazor;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Cervantes.Web.Components.Pages.Cve;

public partial class CveNotesDialog
{
    [CascadingParameter] IDialogReference? MudDialog { get; set; } = default!;
    [Parameter] public Guid CveId { get; set; }
    [Parameter] public string CurrentNotes { get; set; } = "";


    private string notes = "";
    private bool loading = false;

    protected override async Task OnInitializedAsync()
    {
        notes = CurrentNotes ?? "";
    }

    private async Task SaveNotes()
    {
        loading = true;
        try
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await CveManager.UpdateNotesAsync(CveId, notes, userId);
            
            MudDialog.Close(DialogResult.Ok(notes));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error saving notes: {ex.Message}", Severity.Error);
        }
        finally
        {
            loading = false;
        }
    }

    private void Cancel()
    {
        MudDialog.Close();
    }
}