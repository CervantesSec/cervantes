using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Cervantes.Web.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using FluentValidation;
using Severity = MudBlazor.Severity;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class CustomChecklistDialog: ComponentBase
{
    [Parameter] public CORE.Entities.Checklist checklist { get; set; } = null!;
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] IDialogService DialogService { get; set; } = null!;
    [Inject] ISnackbar Snackbar { get; set; } = null!;
    [Inject] private ChecklistController _ChecklistController { get; set; } = null!;
    
    private bool editMode = false;
    void Cancel() => MudDialog.Cancel();
    DialogOptionsEx medium = new DialogOptionsEx() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    
    private ClaimsPrincipal userAth = null!;
    MudForm form = null!;
    private ChecklistUpdateViewModel model = new ChecklistUpdateViewModel();
    private bool _processing = false;
    CustomChecklistModelFluentValidator validator = new CustomChecklistModelFluentValidator();
    
    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        await LoadChecklistDetails();
    }
    
    private async Task LoadChecklistDetails()
    {
        try
        {
            var response = await _ChecklistController.GetCustomChecklistWithDetails(checklist.Id);
            if (response.Result is OkObjectResult okResult)
            {
                var fullChecklist = (CORE.Entities.Checklist)okResult.Value;
                // Update the checklist with full details including categories, items, and executions
                checklist.ChecklistTemplate = fullChecklist.ChecklistTemplate;
                checklist.Executions = fullChecklist.Executions;
                checklist.Target = fullChecklist.Target;
                checklist.User = fullChecklist.User;
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(localizer["errorLoadingChecklistDetails"], Severity.Error);
        }
    }
    
    async Task DeleteDialog(CORE.Entities.Checklist checklistToDelete, DialogOptionsEx options)
    {
        var parameters = new DialogParameters { ["checklist"] = checklistToDelete };
        IMudExDialogReference<DeleteCustomChecklistDialog>? dlgReference = await DialogService.ShowExAsync<DeleteCustomChecklistDialog>("Delete Checklist", parameters, options);
        
        var result = await dlgReference.Result;
        
        if (!result.Canceled)
        {
            MudDialog.Close();
        }
    }
    
    void EditMode()
    {
        if (editMode)
        {
            editMode = false;
        }
        else
        {
            editMode = true;
            // Initialize edit model with current checklist data
            model.Id = checklist.Id;
            model.Name = checklist.Name;
            model.Notes = checklist.Notes;
            model.Status = checklist.Status;
        }
        MudDialog.StateHasChanged();
    }
    
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
    
    private async Task Submit()
    {
        await form.Validate();
        
        if (form.IsValid)
        {
            _processing = true;
            
            try
            {
                var response = await _ChecklistController.UpdateCustomChecklist(model.Id, model);
                
                if (response.Result is OkObjectResult)
                {
                    Snackbar.Add(localizer["checklistEdited"], Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    Snackbar.Add(localizer["checklistEditedError"], Severity.Error);
                }
            }
            catch (Exception)
            {
                Snackbar.Add(localizer["errorOccurred"], Severity.Error);
            }
            finally
            {
                _processing = false;
            }
        }
    }
    
    private async Task ViewItem(ChecklistItem item)
    {
        var parameters = new DialogParameters { ["item"] = item };
        
        IMudExDialogReference<ViewChecklistItemDialog>? dlgReference = await DialogService.ShowExAsync<ViewChecklistItemDialog>(localizer["viewChecklistItem"], parameters, middleWidthEx);
    }
    
    private async Task UpdateExecutionStatus(ChecklistItem item, CORE.Entities.ChecklistItemStatus newStatus)
    {
        try
        {
            var execution = checklist.Executions?.FirstOrDefault(e => e.ChecklistItemId == item.Id);
            
            if (execution == null)
            {
                // Create new execution if it doesn't exist
                var createModel = new ChecklistExecutionUpdateViewModel
                {
                    Id = Guid.NewGuid(),
                    Status = newStatus
                };
                
                var response = await _ChecklistController.CreateExecution(checklist.Id, item.Id, createModel);
                
                if (response.Result is CreatedAtActionResult)
                {
                    Snackbar.Add(localizer["executionStatusUpdated"], Severity.Success);
                    await LoadChecklistDetails(); // Reload to get updated data
                }
                else
                {
                    Snackbar.Add(localizer["errorUpdatingExecutionStatus"], Severity.Error);
                }
            }
            else
            {
                // Update existing execution
                var updateModel = new ChecklistExecutionUpdateViewModel
                {
                    Id = execution.Id,
                    Status = newStatus,
                    Notes = execution.Notes,
                    Evidence = execution.Evidence,
                    EstimatedTimeMinutes = execution.EstimatedTimeMinutes,
                    ActualTimeMinutes = execution.ActualTimeMinutes,
                    DifficultyRating = execution.DifficultyRating,
                    VulnId = execution.VulnId
                };
                
                var response = await _ChecklistController.UpdateExecution(execution.Id, updateModel);
                
                if (response.Result is OkObjectResult)
                {
                    Snackbar.Add(localizer["executionStatusUpdated"], Severity.Success);
                    // Update local status immediately for better UX
                    execution.Status = newStatus;
                    execution.TestedDate = DateTime.UtcNow;
                    StateHasChanged();
                }
                else
                {
                    Snackbar.Add(localizer["errorUpdatingExecutionStatus"], Severity.Error);
                }
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(localizer["errorOccurred"], Severity.Error);
        }
    }
    
    private async Task EditExecution(ChecklistExecution execution)
    {
        var parameters = new DialogParameters 
        { 
            ["execution"] = execution,
            ["checklistItem"] = execution.ChecklistItem
        };
        
        IMudExDialogReference<EditChecklistExecutionDialog>? dlgReference = await DialogService.ShowExAsync<EditChecklistExecutionDialog>(
            localizer["editExecution"], parameters, middleWidthEx);
        
        var result = await dlgReference.Result;
        
        if (!result.Canceled)
        {
            await LoadChecklistDetails(); // Reload to get updated data
        }
    }
    
    public class CustomChecklistModelFluentValidator : AbstractValidator<ChecklistUpdateViewModel>
    {
        public CustomChecklistModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Checklist name is required")
                .MaximumLength(255)
                .WithMessage("Checklist name must be less than 255 characters");
                
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Valid status is required");
        }
        
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ChecklistUpdateViewModel>.CreateWithOptions((ChecklistUpdateViewModel)model, x => x.IncludeProperties(propertyName)));
            return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
        };
    }
}