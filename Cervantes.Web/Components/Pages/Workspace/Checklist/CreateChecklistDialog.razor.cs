using System.Net.Http.Json;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class CreateChecklistDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
     void Cancel() => MudDialog.Cancel();
     
     [Inject] ISnackbar Snackbar { get; set; }
     [Inject] private TargetController _targetController { get; set; }
     [Inject] private ChecklistController _ChecklistController { get; set; }

     MudForm form;

    ChecklistModelFluentValidator checklistValidator = new ChecklistModelFluentValidator();
	 
    ChecklistCreateViewModel model = new ChecklistCreateViewModel();
    [Parameter] public Guid project { get; set; }
	private List<CORE.Entities.Target> targets = new List<CORE.Entities.Target>();
    

    protected override async Task OnInitializedAsync()
    {
	    await base.OnInitializedAsync();
	    targets = _targetController.GetByProjectId(project).ToList();
		StateHasChanged();
    }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
	        model.ProjectId = project;
	        var response = await _ChecklistController.Add(model);
	        if (response.ToString() == "Microsoft.AspNetCore.Mvc.CreatedAtActionResult")
	        {
		        Snackbar.Add(@localizer["checklistCreated"], Severity.Success);
		        MudDialog.Close(DialogResult.Ok(true));
	        }
	        else
	        {
		        Snackbar.Add(@localizer["checklistCreatedError"], Severity.Error);
	        }
            
        }
    }

    /// <summary>
    /// A standard AbstractValidator which contains multiple rules and can be shared with the back end API
    /// </summary>
    /// <typeparam name="ProjectViewModel"></typeparam>
    public class ChecklistModelFluentValidator : AbstractValidator<ChecklistCreateViewModel>
    {
        public ChecklistModelFluentValidator()
        {
	        RuleFor(x => x.TargetId)
		        .NotEmpty();
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ChecklistCreateViewModel>.CreateWithOptions((ChecklistCreateViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    

}