using System.Security.Claims;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.KnowledgeBase;

public partial class EditKnowledgeCategoryDialog: ComponentBase
{
    [Parameter] public CORE.Entities.KnowledgeBaseCategories category { get; set; } 

    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    
    private bool editMode = false;
    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    private KnowledgeCategoryEditViewModel model { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private KnowledgeBaseController _KnowledgeBaseController { get; set; }
    MudForm form;
    KnowledgeFluentValidator knowledgeValidator = new KnowledgeFluentValidator();
    private ClaimsPrincipal userAth;
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
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
    }
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response =  await _KnowledgeBaseController.EditCategory(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
            {
                Snackbar.Add(@localizer["categoryEdited"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["categoryEditedError"], Severity.Error);
            }

        }
    }

    public class KnowledgeFluentValidator : AbstractValidator<KnowledgeCategoryEditViewModel>
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
            var result = await ValidateAsync(ValidationContext<KnowledgeCategoryEditViewModel>.CreateWithOptions((KnowledgeCategoryEditViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
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
            model = new KnowledgeCategoryEditViewModel();
            model.Id = category.Id;
            model.Name = category.Name;
            model.Description = category.Description;
            model.Order = category.Order;
        }
        MudDialog.StateHasChanged();
    }
    
    async Task DeleteDialog(CORE.Entities.KnowledgeBaseCategories item,DialogOptions options)
    {
        var parameters = new DialogParameters { ["category"]=item };
        IMudExDialogReference<DeleteKnowledgeCategoryDialog>? dlgReference = await Dialog.ShowExAsync<DeleteKnowledgeCategoryDialog>("Simple Dialog", parameters, middleWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            MudDialog.Close();
        }
    }
    
}