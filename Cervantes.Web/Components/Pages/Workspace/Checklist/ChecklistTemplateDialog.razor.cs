using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Severity = MudBlazor.Severity;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class ChecklistTemplateDialog: ComponentBase
{
    [Parameter] public ChecklistTemplate template { get; set; }
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    [Inject] IDialogService DialogService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private ChecklistController _ChecklistController { get; set; }
    
    private bool editMode = false;
    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    
    MudForm form;
    private ChecklistTemplateCreateViewModel model = new ChecklistTemplateCreateViewModel();
    private ClaimsPrincipal userAth;
    
    private Dictionary<string, object> editorConf = new Dictionary<string, object>{
        {"plugins", "preview importcss searchreplace autolink autosave save directionality code visualblocks visualchars fullscreen image link media codesample table charmap pagebreak nonbreaking anchor insertdatetime advlist lists wordcount help charmap quickbars emoticons"},
        {"menubar", "file edit view insert format tools table help"},
        {"toolbar", "undo redo | bold italic underline strikethrough | fontselect fontsizeselect formatselect | alignleft aligncenter alignright alignjustify | outdent indent |  numlist bullist | forecolor backcolor removeformat | pagebreak | charmap emoticons | fullscreen  preview save print | insertfile image media link anchor codesample | ltr rtl"},
        {"toolbar_sticky", true},
        {"image_advtab", true},
        {"height", 300},
        {"image_caption", true},
        {"promotion", false},
        {"quickbars_selection_toolbar", "bold italic | quicklink h2 h3 blockquote quickimage quicktable"},
        {"noneditable_noneditable_class", "mceNonEditable"},
        {"toolbar_mode", "sliding"},
        {"contextmenu", "link image imagetools table"},
        {"textpattern_patterns", new object[] {
            new {start = "#", format = "h1"},
            new {start = "##", format = "h2"},
            new {start = "###", format = "h3"},
            new {start = "####", format = "h4"},
            new {start = "#####", format = "h5"},
            new {start = "######", format = "h6"},
            new {start = ">", format = "blockquote"},
            new {start = "*", end = "*", format = "italic"},
            new {start = "_", end = "_", format = "italic"},
            new {start = "**", end = "**", format = "bold"},
            new {start = "__", end = "__", format = "bold"},
            new {start = "***", end = "***", format = "bold italic"},
            new {start = "___", end = "___", format = "bold italic"},
            new {start = "__*", end = "*__", format = "bold italic"},
            new {start = "**_", end = "_**", format = "bold italic"},
            new {start = "`", end = "`", format = "code"},
            new {start = "---", replacement = "<hr/>"},
            new {start = "--", replacement = "—"},
            new {start = "-", replacement = "—"},
            new {start = "(c)", replacement = "©"},
            new {start = "~", end = "~", cmd = "createLink"},
            new {start = "<", end = ">", cmd = "createLink"},
            new {start = "* ", cmd = "InsertUnorderedList"},
            new {start = "-", cmd = "InsertUnorderedList"},
            new {start = "1. ", cmd = "InsertOrderedList", value = "decimal"},
            new {start = "1) ", cmd = "InsertOrderedList", value = "decimal"},
            new {start = "a. ", cmd = "InsertOrderedList", value = "lower-alpha"},
            new {start = "a) ", cmd = "InsertOrderedList", value = "lower-alpha"},
            new {start = "i. ", cmd = "InsertOrderedList", value = "lower-roman"},
            new {start = "i) ", cmd = "InsertOrderedList", value = "lower-roman"}
        }}
    };
    DialogOptionsEx maxWidthEx = new DialogOptionsEx() 
    {
        MaximizeButton = true,
        CloseButton = true,
        FullHeight = true,
        CloseOnEscapeKey = true,
        MaxWidth = MaxWidth.Medium,
        MaxHeight = MaxHeight.False,
        FullWidth = true,
        DragMode = MudDialogDragMode.Simple,
        Animations = new[] { AnimationType.SlideIn },
        Position = DialogPosition.CenterRight,
        DisableSizeMarginY = true,
        DisablePositionMargin = true,
        BackdropClick = false,
        Resizeable = true,
    };
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
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
    }
    
    async Task DeleteDialog(ChecklistTemplate template, DialogOptions options)
    {
        var parameters = new DialogParameters { ["template"] = template };
        
        IMudExDialogReference<DeleteChecklistTemplateDialog>? dlgReference = await DialogService.ShowExAsync<DeleteChecklistTemplateDialog>("Delete Template", parameters, middleWidthEx);
        
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
            model.Id = template.Id;
            model.Name = template.Name;
            model.Version = template.Version;
            model.Description = template.Description;
            model.OrganizationId = template.OrganizationId;
            
            // Map categories and items
            model.Categories = template.Categories?.Select(c => new ChecklistCategoryCreateViewModel
            {
                Name = c.Name,
                Description = c.Description,
                Order = c.Order,
                Items = c.Items?.Select(i => new ChecklistItemCreateViewModel
                {
                    Code = i.Code,
                    Name = i.Name,
                    Description = i.Description,
                    Objectives = i.Objectives,
                    TestProcedure = i.TestProcedure,
                    PassCriteria = i.PassCriteria,
                    Order = i.Order,
                    IsRequired = i.IsRequired,
                    Severity = i.Severity,
                    References = i.References
                }).ToList() ?? new List<ChecklistItemCreateViewModel>()
            }).ToList() ?? new List<ChecklistCategoryCreateViewModel>();
        }
        MudDialog.StateHasChanged();
    }
    
    private async Task Submit()
    {
        await form.Validate();
        
        if (form.IsValid)
        {
            try
            {
                var response = await _ChecklistController.UpdateCustomTemplate(template.Id, model);
                if (response.Result is Microsoft.AspNetCore.Mvc.OkObjectResult)
                {
                    Snackbar.Add(@localizer["templateUpdated"], Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    Snackbar.Add(@localizer["errorOccurred"], Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add(@localizer["errorOccurred"], Severity.Error);
            }
        }
    }
    
    private async Task ViewItem(DataGridRowClickEventArgs<ChecklistItem> item)
    {
        var parameters = new DialogParameters { ["item"] = item.Item };
        var optionsEx = new DialogOptionsEx { MaxWidth = MaxWidth.Medium, FullWidth = true };
        
        IMudExDialogReference<ViewChecklistItemDialog>? dlgReference = await DialogService.ShowExAsync<ViewChecklistItemDialog>(@localizer["viewChecklistItem"], parameters, middleWidthEx);
    }
    
    private void AddCategory()
    {
        var newCategory = new ChecklistCategoryCreateViewModel
        {
            Name = "New Category",
            Description = "",
            Order = model.Categories.Count + 1,
            Items = new List<ChecklistItemCreateViewModel>()
        };
        
        model.Categories.Add(newCategory);
        StateHasChanged();
    }
    
    private void RemoveCategory(int categoryIndex)
    {
        if (categoryIndex >= 0 && categoryIndex < model.Categories.Count)
        {
            model.Categories.RemoveAt(categoryIndex);
            StateHasChanged();
        }
    }
    
    private void AddItem(int categoryIndex)
    {
        if (categoryIndex >= 0 && categoryIndex < model.Categories.Count)
        {
            var category = model.Categories[categoryIndex];
            var newItem = new ChecklistItemCreateViewModel
            {
                Code = $"ITEM-{category.Items.Count + 1:D2}",
                Name = "New Item",
                Description = "",
                Objectives = "",
                TestProcedure = "",
                PassCriteria = "",
                References = "",
                Order = category.Items.Count + 1,
                IsRequired = false,
                Severity = 2
            };
            
            category.Items.Add(newItem);
            StateHasChanged();
        }
    }
    
    private void RemoveItem(int categoryIndex, int itemIndex)
    {
        if (categoryIndex >= 0 && categoryIndex < model.Categories.Count)
        {
            var category = model.Categories[categoryIndex];
            if (itemIndex >= 0 && itemIndex < category.Items.Count)
            {
                category.Items.RemoveAt(itemIndex);
                StateHasChanged();
            }
        }
    }
    
    ChecklistTemplateModelFluentValidator templateValidator = new ChecklistTemplateModelFluentValidator();
    
    public class ChecklistTemplateModelFluentValidator : AbstractValidator<ChecklistTemplateCreateViewModel>
    {
        public ChecklistTemplateModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1, 250);
            
            RuleFor(x => x.Version)
                .NotEmpty()
                .Length(1, 50);
        }
        
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ChecklistTemplateCreateViewModel>.CreateWithOptions((ChecklistTemplateCreateViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return new string[0];
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}