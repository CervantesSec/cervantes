using System.Security.Claims;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.CervantesAI;
using Cervantes.Web.Components.Shared;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.KnowledgeBase;

public partial class KnowledgeBase: ComponentBase
{
    private const string aiSVG = @"<svg width=""24"" height=""24"" xmlns=""http://www.w3.org/2000/svg"" fill-rule=""evenodd"" clip-rule=""evenodd""><path d=""M24 11.374c0 4.55-3.783 6.96-7.146 6.796-.151 1.448.061 2.642.384 3.641l-3.72 1.189c-.338-1.129-.993-3.822-2.752-5.279-2.728.802-4.969-.646-5.784-2.627-2.833.046-4.982-1.836-4.982-4.553 0-4.199 4.604-9.541 11.99-9.541 7.532 0 12.01 5.377 12.01 10.374zm-21.992-1.069c-.145 2.352 2.179 3.07 4.44 2.826.336 2.429 2.806 3.279 4.652 2.396 1.551.74 2.747 2.37 3.729 4.967l.002.006.111-.036c-.219-1.579-.09-3.324.36-4.528 3.907.686 6.849-1.153 6.69-4.828-.166-3.829-3.657-8.011-9.843-8.109-6.302-.041-9.957 4.255-10.141 7.306zm8.165-2.484c-.692-.314-1.173-1.012-1.173-1.821 0-1.104.896-2 2-2s2 .896 2 2c0 .26-.05.509-.141.738 1.215.911 2.405 1.855 3.6 2.794.424-.333.96-.532 1.541-.532 1.38 0 2.5 1.12 2.5 2.5s-1.12 2.5-2.5 2.5c-1.171 0-2.155-.807-2.426-1.895-1.201.098-2.404.173-3.606.254-.17.933-.987 1.641-1.968 1.641-1.104 0-2-.896-2-2 0-1.033.784-1.884 1.79-1.989.12-.731.252-1.46.383-2.19zm2.059-.246c-.296.232-.66.383-1.057.417l-.363 2.18c.504.224.898.651 1.079 1.177l3.648-.289c.047-.267.137-.519.262-.749l-3.569-2.736z""/></svg>";

    private List<BreadcrumbItem> _items;
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    private bool editMode = false;
    private List<CORE.Entities.KnowledgeBase> Pages = new List<CORE.Entities.KnowledgeBase>();
    private CORE.Entities.KnowledgeBase Page = new CORE.Entities.KnowledgeBase();
    [Inject] ISnackbar Snackbar { get; set; }

    private List<CORE.Entities.KnowledgeBaseCategories> categories = new List<CORE.Entities.KnowledgeBaseCategories>();
    [Inject] private KnowledgeBaseController _KnowledgeBaseController { get; set; }

    private KnowledgePageEditViewModel model = new KnowledgePageEditViewModel();
    MudForm form;
    [CascadingParameter] bool _isDarkMode { get; set; }
    DialogOptionsEx centerWidthEx = new DialogOptionsEx() 
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
    private Dictionary<string, object> editorConf = new Dictionary<string, object>{
                {"plugins", "preview importcss searchreplace autolink autosave save directionality code visualblocks visualchars fullscreen image link media codesample table charmap pagebreak nonbreaking anchor insertdatetime advlist lists wordcount help charmap quickbars emoticons"},
                {"menubar", "file edit view insert format tools table help"},
                {"toolbar", "undo redo | bold italic underline strikethrough | fontselect fontsizeselect formatselect | alignleft aligncenter alignright alignjustify | outdent indent |  numlist bullist | forecolor backcolor removeformat | pagebreak | charmap emoticons | fullscreen  preview save print | insertfile image media link anchor codesample | ltr rtl"},
                {"toolbar_sticky", true},
                {"image_advtab", true},
                {"height", 600},
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
    [Inject] private IAiService _aiService { get; set; }
    private bool aiEnabled = false;
    private ClaimsPrincipal userAth;
    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        await Update();
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem(@localizer["knowledgeBase"], href: null, disabled: true,icon: Icons.Material.Filled.MenuBook)
        };
        aiEnabled = _aiService.IsEnabled();
        await base.OnInitializedAsync();
    }



    protected async Task Update()
    {
        Pages = _KnowledgeBaseController.GetPages().ToList();
        categories = _KnowledgeBaseController.GetCategories().ToList();
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
            model = new KnowledgePageEditViewModel();
            model.Id = Page.Id;
            model.Title = Page.Title;
            model.Content = Page.Content;
            model.Order = Page.Order;
            model.CategoryId = Page.CategoryId;
            
        }

    }
    
    private async Task OpenDialogCategory(DialogOptions options)
    {
        IMudExDialogReference<KnowledgeCategoryDialog>? dlgReference = await Dialog.ShowExAsync<KnowledgeCategoryDialog>("Simple Dialog", centerWidthEx);

        // wait modal to close
        var result = await dlgReference.Result;
        await Update();
        StateHasChanged();
        
    }
    
    private async Task OpenDialogPage(DialogOptions options)
    {
        IMudExDialogReference<CreateKnowledgePageDialog>? dlgReference = await Dialog.ShowExAsync<CreateKnowledgePageDialog>("Simple Dialog", centerWidthEx);

        // wait modal to close
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    
    private async Task SelectedPage(Guid id)
    {
            var selected  = Pages.FirstOrDefault(p => p.Id == id);
            if (selected != null)
            {
                Page = selected;
            }
        
    }
    
    KnowledgeFluentValidator knowledgeValidator = new KnowledgeFluentValidator();

    public class KnowledgeFluentValidator : AbstractValidator<KnowledgePageEditViewModel>
    {
        public KnowledgeFluentValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .Length(1,100);
            RuleFor(x => x.Order)
                .NotEmpty();
            RuleFor(x => x.CategoryId)
                .NotEmpty();
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<KnowledgePageEditViewModel>.CreateWithOptions((KnowledgePageEditViewModel)model, x => x.IncludeProperties(propertyName)));
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
            var response =  await _KnowledgeBaseController.EditPage(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
            {
                editMode = false;
                Snackbar.Add(@localizer["pageEdited"], Severity.Success);
            }
            else
            {
                Snackbar.Add(@localizer["pageEditedError"], Severity.Error);
            }

        }
    }
    private async Task OpenDeleteDialog()
    {
        var parameters = new DialogParameters { ["category"]=Page };
        IMudExDialogReference<DeleteKnowledgePageDialog>? dlgReference = await Dialog.ShowExAsync<DeleteKnowledgePageDialog>("Simple Dialog",parameters, centerWidthEx);

        // wait modal to close
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    private async Task OpenAiDialog(DialogOptions options)
    {
        //var parameters = new DialogParameters { ["project"]=SelectedProject };
        IMudExDialogReference<AiDialog>? dlgReference = await Dialog.ShowExAsync<AiDialog>("Simple Dialog",centerWidthEx);

        // wait modal to close
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            var data = await dlgReference.GetReturnValueAsync<FunctionResult>();
            model.Content = model.Content + data;
            StateHasChanged();
        }
        
    }
}