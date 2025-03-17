using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Severity = MudBlazor.Severity;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class AddVulnTarget: ComponentBase
{
    [Parameter] public CORE.Entities.Vuln vuln { get; set; } = new CORE.Entities.Vuln();
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private VulnController _VulnController { get; set; }
    [Inject] private TargetController _TargetController { get; set; }

    void Cancel() => MudDialog.Cancel();
    private List<Target> Targets = new List<Target>();
    private VulnTargetViewModel model { get; set; } = new VulnTargetViewModel();
    MudForm form;
    VulnTargetModelFluentValidator targetValidator = new VulnTargetModelFluentValidator();
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Targets = _TargetController.GetByProjectId(vuln.ProjectId.Value).ToList();
        StateHasChanged();
    }

    public class VulnTargetModelFluentValidator : AbstractValidator<VulnTargetViewModel>
    {
        public VulnTargetModelFluentValidator()
        {

            RuleFor(x => x.TargetId)
                .NotEmpty();
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<VulnTargetViewModel>.CreateWithOptions((VulnTargetViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            model.VulnId = vuln.Id;


            var response = await _VulnController.AddVulnTarget(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")            
            {
                Snackbar.Add(@localizer["targetAdded"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
                
            }
            else if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkObjectResult")
            {
                Snackbar.Add(@localizer["targetExists"], Severity.Info);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["targetAddedError"], Severity.Error);
            }

        }
    }
    
}