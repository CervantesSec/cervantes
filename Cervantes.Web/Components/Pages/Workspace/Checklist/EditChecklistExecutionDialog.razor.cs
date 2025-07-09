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

public partial class EditChecklistExecutionDialog : ComponentBase
{
    [Parameter] public ChecklistExecution execution { get; set; } = null!;
    [Parameter] public ChecklistItem checklistItem { get; set; } = null!;
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = null!;
    
    [Inject] IDialogService DialogService { get; set; } = null!;
    [Inject] ISnackbar Snackbar { get; set; } = null!;
    [Inject] private ChecklistController _ChecklistController { get; set; } = null!;
    [Inject] private VulnController _VulnController { get; set; } = null!;
    [Inject] IStringLocalizer<Resource> localizer { get; set; } = null!;
    [Inject] AuthenticationStateProvider authenticationStateProvider { get; set; } = null!;
    
    void Cancel() => MudDialog.Cancel();
    
    private ClaimsPrincipal userAth = null!;
    MudForm form = null!;
    private ChecklistExecutionUpdateViewModel model = new ChecklistExecutionUpdateViewModel();
    private bool _processing = false;
    ExecutionModelFluentValidator validator = new ExecutionModelFluentValidator();
    
    private List<CORE.Entities.Vuln> vulnerabilities = new();
    
    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        
        // Initialize model with execution data
        model.Id = execution.Id;
        model.Status = execution.Status;
        model.Notes = execution.Notes;
        model.Evidence = execution.Evidence;
        model.EstimatedTimeMinutes = execution.EstimatedTimeMinutes;
        model.ActualTimeMinutes = execution.ActualTimeMinutes;
        model.DifficultyRating = execution.DifficultyRating;
        model.VulnId = execution.VulnId;
        
        // Load vulnerabilities for selection
        await LoadVulnerabilities();
        
        await base.OnInitializedAsync();
    }
    
    private async Task LoadVulnerabilities()
    {
        try
        {
            // Get project ID from execution's checklist
            var checklistResponse = await _ChecklistController.GetCustomChecklist(execution.ChecklistId);
            if (checklistResponse.Result is OkObjectResult okResult)
            {
                var checklist = (CORE.Entities.Checklist)okResult.Value;
                
                // Load vulnerabilities for this project
                var vulnResponse = _VulnController.GetByProject(checklist.ProjectId);
                vulnerabilities = vulnResponse.ToList();
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(localizer["errorLoadingVulnerabilities"], Severity.Warning);
        }
    }
    
    private async Task Submit()
    {
        await form.Validate();
        
        if (form.IsValid)
        {
            _processing = true;
            
            try
            {
                var response = await _ChecklistController.UpdateExecution(execution.Id, model);
                
                if (response.Result is OkObjectResult)
                {
                    Snackbar.Add(localizer["executionUpdatedSuccessfully"], Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    Snackbar.Add(localizer["errorUpdatingExecution"], Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add(localizer["errorOccurred"], Severity.Error);
            }
            finally
            {
                _processing = false;
            }
        }
    }
    
    private Color GetStatusColor(ChecklistItemStatus status)
    {
        return status switch
        {
            ChecklistItemStatus.NotTested => Color.Default,
            ChecklistItemStatus.Passed => Color.Success,
            ChecklistItemStatus.Failed => Color.Error,
            ChecklistItemStatus.InProgress => Color.Info,
            ChecklistItemStatus.NotApplicable => Color.Secondary,
            _ => Color.Default
        };
    }
    
    private string GetStatusText(ChecklistItemStatus status)
    {
        return status switch
        {
            ChecklistItemStatus.NotTested => localizer["notTested"],
            ChecklistItemStatus.Passed => localizer["passed"],
            ChecklistItemStatus.Failed => localizer["failed"],
            ChecklistItemStatus.InProgress => localizer["inProgress"],
            ChecklistItemStatus.NotApplicable => localizer["notApplicable"],
            _ => localizer["unknown"]
        };
    }
    
    public class ExecutionModelFluentValidator : AbstractValidator<ChecklistExecutionUpdateViewModel>
    {
        public ExecutionModelFluentValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Valid status is required");
                
            RuleFor(x => x.EstimatedTimeMinutes)
                .GreaterThanOrEqualTo(0)
                .When(x => x.EstimatedTimeMinutes.HasValue)
                .WithMessage("Estimated time must be positive");
                
            RuleFor(x => x.ActualTimeMinutes)
                .GreaterThanOrEqualTo(0)
                .When(x => x.ActualTimeMinutes.HasValue)
                .WithMessage("Actual time must be positive");
                
            RuleFor(x => x.DifficultyRating)
                .InclusiveBetween(1, 5)
                .When(x => x.DifficultyRating.HasValue)
                .WithMessage("Difficulty rating must be between 1 and 5");
                
            RuleFor(x => x.Notes)
                .MaximumLength(2000)
                .WithMessage("Notes must be less than 2000 characters");
                
            RuleFor(x => x.Evidence)
                .MaximumLength(2000)
                .WithMessage("Evidence must be less than 2000 characters");
        }
        
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ChecklistExecutionUpdateViewModel>.CreateWithOptions((ChecklistExecutionUpdateViewModel)model, x => x.IncludeProperties(propertyName)));
            return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
        };
    }
}