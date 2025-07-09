using Cervantes.CORE.ViewModel;
using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class CreateCustomChecklistDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Parameter] public Guid project { get; set; }
    
    void Cancel() => MudDialog.Cancel();
    
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private ChecklistController _checklistController { get; set; }
    [Inject] private ProjectController _projectController { get; set; }
    [Inject] private TargetController _targetController { get; set; }
    [Inject] IStringLocalizer<Resource> localizer { get; set; }
    
    MudForm form;
    CustomChecklistModelFluentValidator validator = new CustomChecklistModelFluentValidator();
    ChecklistCreateViewModelNew model = new ChecklistCreateViewModelNew();
    bool _processing = false;
    
    private List<ChecklistTemplate> templates = new();
    private List<CORE.Entities.Target> targets = new();
    private ChecklistTemplate selectedTemplate;

    protected override async System.Threading.Tasks.Task OnInitializedAsync()
    {
        model.ProjectId = project;
        await LoadTemplates();
        await LoadTargets();
        await base.OnInitializedAsync();
    }

    private async System.Threading.Tasks.Task LoadTemplates()
    {
        try
        {
            var response = await _checklistController.GetCustomTemplates();
            if (response.Result is OkObjectResult okResult)
            {
                templates = (List<ChecklistTemplate>)okResult.Value;
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(@localizer["errorLoadingTemplates"], MudBlazor.Severity.Error);
        }
    }

    private async System.Threading.Tasks.Task LoadTargets()
    {
        try
        {
            targets = _targetController.GetByProjectId(project).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(@localizer["errorLoadingTargets"], MudBlazor.Severity.Error);
        }
    }

    private async System.Threading.Tasks.Task OnTemplateChanged(Guid templateId)
    {
        model.ChecklistTemplateId = templateId;
        
        if (templateId != Guid.Empty)
        {
            selectedTemplate = templates.FirstOrDefault(t => t.Id == templateId);
            
            // Load full template details including categories and items for preview
            try
            {
                var response = await _checklistController.GetCustomTemplate(templateId);
                if (response.Result is OkObjectResult okResult)
                {
                    selectedTemplate = (ChecklistTemplate)okResult.Value;
                }
            }
            catch (Exception ex)
            {
                // If loading fails, keep the basic template info
                Snackbar.Add(@localizer["errorLoadingTemplates"], MudBlazor.Severity.Warning);
            }
        }
        else
        {
            selectedTemplate = null;
        }
        StateHasChanged();
    }

    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();
        
        if (form.IsValid)
        {
            _processing = true;
            
            try
            {
                var response = await _checklistController.CreateCustomChecklist(model);
                
                if (response.Result is CreatedAtActionResult)
                {
                    Snackbar.Add(@localizer["checklistCreatedSuccessfully"], MudBlazor.Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else if (response.Result is BadRequestObjectResult badRequestResult)
                {
                    Snackbar.Add(@localizer["errorCreatingChecklist"], MudBlazor.Severity.Error);
                }
                else
                {
                    Snackbar.Add(@localizer["errorOccurred"], MudBlazor.Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add(@localizer["errorOccurred"], MudBlazor.Severity.Error);
            }
            finally
            {
                _processing = false;
            }
        }
    }

    public class CustomChecklistModelFluentValidator : AbstractValidator<ChecklistCreateViewModelNew>
    {
        public CustomChecklistModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Checklist name is required");
                
            RuleFor(x => x.ChecklistTemplateId)
                .NotEmpty()
                .WithMessage("Template selection is required");
                
            RuleFor(x => x.ProjectId)
                .NotEmpty()
                .WithMessage("Project ID is required");
        }
        
        public Func<object, string, System.Threading.Tasks.Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ChecklistCreateViewModelNew>.CreateWithOptions((ChecklistCreateViewModelNew)model, x => x.IncludeProperties(propertyName)));
            return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
        };
    }
}