using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class CreateVulnCategoryDialog: ComponentBase
{
    [Inject] private VulnController _VulnController { get; set; }
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    void Cancel() => MudDialog.Cancel();
    MudForm form;
    VulnCategoryModelFluentValidator vulnValidator = new VulnCategoryModelFluentValidator();
    VulnCategoryCreate model = new VulnCategoryCreate();
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await _VulnController.AddCategory(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
            {
                Snackbar.Add(@localizer["categoryCreated"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["categoryCreatedError"], Severity.Error);
            }
            
        }
    }
    
    public class VulnCategoryModelFluentValidator : AbstractValidator<VulnCategoryCreate>
    {
        public VulnCategoryModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<VulnCategoryCreate>.CreateWithOptions((VulnCategoryCreate)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}