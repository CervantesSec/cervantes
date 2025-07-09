using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Localization;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class CreateChecklistTemplateDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    void Cancel() => MudDialog.Cancel();
    
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private ChecklistController _checklistController { get; set; }
    [Inject] IStringLocalizer<Resource> localizer { get; set; }
    
    MudForm form;
    ChecklistTemplateModelFluentValidator validator = new ChecklistTemplateModelFluentValidator();
    ChecklistTemplateCreateViewModel model = new ChecklistTemplateCreateViewModel();
    bool _processing = false;

    protected override async Task OnInitializedAsync()
    {
        model = new ChecklistTemplateCreateViewModel
        {
            Categories = new List<ChecklistCategoryCreateViewModel>()
        };
        
        // Add a default category to start with
        AddCategory();
        
        await base.OnInitializedAsync();
    }

    private async Task Submit()
    {
        await form.Validate();
        
        if (form.IsValid)
        {
            _processing = true;
            
            try
            {
                var response = await _checklistController.CreateCustomTemplate(model);
                
                if (response.Result is CreatedAtActionResult)
                {
                    Snackbar.Add(@localizer["templateCreatedSuccessfully"], MudBlazor.Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else if (response.Result is BadRequestObjectResult badRequestResult)
                {
                    Snackbar.Add(@localizer["errorCreatingTemplate"], MudBlazor.Severity.Error);
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

    private void AddCategory()
    {
        var category = new ChecklistCategoryCreateViewModel
        {
            Name = "",
            Description = "",
            Order = model.Categories.Count + 1,
            Items = new List<ChecklistItemCreateViewModel>()
        };
        
        model.Categories.Add(category);
        StateHasChanged();
    }

    private void RemoveCategory(ChecklistCategoryCreateViewModel category)
    {
        model.Categories.Remove(category);
        
        // Reorder remaining categories
        for (int i = 0; i < model.Categories.Count; i++)
        {
            model.Categories[i].Order = i + 1;
        }
        
        StateHasChanged();
    }

    private void AddItem(ChecklistCategoryCreateViewModel category)
    {
        var item = new ChecklistItemCreateViewModel
        {
            Code = "",
            Name = "",
            Description = "",
            Objectives = "",
            TestProcedure = "",
            PassCriteria = "",
            Order = category.Items.Count + 1,
            IsRequired = true,
            Severity = 3,
            References = ""
        };
        
        category.Items.Add(item);
        StateHasChanged();
    }

    private void RemoveItem(ChecklistCategoryCreateViewModel category, ChecklistItemCreateViewModel item)
    {
        category.Items.Remove(item);
        
        // Reorder remaining items
        for (int i = 0; i < category.Items.Count; i++)
        {
            category.Items[i].Order = i + 1;
        }
        
        StateHasChanged();
    }

    public class ChecklistTemplateModelFluentValidator : AbstractValidator<ChecklistTemplateCreateViewModel>
    {
        public ChecklistTemplateModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Template name is required");
                
            RuleFor(x => x.Categories)
                .Must(x => x.Count > 0)
                .WithMessage("At least one category is required");
                
            RuleForEach(x => x.Categories).ChildRules(category =>
            {
                category.RuleFor(x => x.Name)
                    .NotEmpty()
                    .WithMessage("Category name is required");
                    
                category.RuleFor(x => x.Items)
                    .Must(x => x.Count > 0)
                    .WithMessage("At least one item is required per category");
                    
                category.RuleForEach(x => x.Items).ChildRules(item =>
                {
                    item.RuleFor(x => x.Code)
                        .NotEmpty()
                        .WithMessage("Item code is required");
                        
                    item.RuleFor(x => x.Name)
                        .NotEmpty()
                        .WithMessage("Item name is required");
                });
            });
        }
        
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ChecklistTemplateCreateViewModel>.CreateWithOptions((ChecklistTemplateCreateViewModel)model, x => x.IncludeProperties(propertyName)));
            return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
        };
    }
}