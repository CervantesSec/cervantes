using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.KnowledgeBase;

public partial class CreateKnowledgeCategoryDialog: ComponentBase
{
     [CascadingParameter] MudDialogInstance MudDialog { get; set; }
 
     void Cancel() => MudDialog.Cancel();

     [Inject] ISnackbar Snackbar { get; set; }
     [Inject] private KnowledgeBaseController _KnowledgeBaseController { get; set; }


    MudForm form;

    KnowledgeFluentValidator knowledgeValidator = new KnowledgeFluentValidator();
	 
    KnowledgeCategoryCreateVIewModel model = new KnowledgeCategoryCreateVIewModel();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        model.Order = _KnowledgeBaseController.GetCategories().Count() + 1;
    }

	 private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response =  await _KnowledgeBaseController.AddCategory(model);
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

    /// <summary>
    /// A standard AbstractValidator which contains multiple rules and can be shared with the back end API
    /// </summary>
    /// <typeparam name="ClientModel"></typeparam>
    public class KnowledgeFluentValidator : AbstractValidator<KnowledgeCategoryCreateVIewModel>
    {
        public KnowledgeFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);
            RuleFor(x => x.Order)
	            .NotEmpty();
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<KnowledgeCategoryCreateVIewModel>.CreateWithOptions((KnowledgeCategoryCreateVIewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}