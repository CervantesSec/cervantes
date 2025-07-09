using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class ViewChecklistItemDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public ChecklistItem item { get; set; }
    
    void Close() => MudDialog.Close();
}