using Cervantes.CORE.ViewModel;
using Cervantes.IFR.CervantesAI;
using Cervantes.Web.Components.Shared;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Admin.Reports;

public partial class CreateReportComponentDialog: ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    [Inject] private ReportController _reportController { get; set; }
    
    private CreateReportComponentModel model = new CreateReportComponentModel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };

    static string contentStyle = @"
body {
  background-color: #f0eeee;
  padding: 1rem;
}

section {
  padding: 4rem 4rem 1rem;
  box-sizing: border-box;
  max-width: 1050px;
  min-width: 820px;
  min-height: 600px;
  margin: 2rem auto;
  background-color: #fff;
  box-shadow: rgba(0, 0, 0, 0.1) 0px 10px 15px -3px, rgba(0, 0, 0, 0.05) 0px 4px 6px -2px;
  background-size: 100%;
  background-position: right bottom;
  background-repeat: no-repeat;
 
  row-gap: 1rem;
}

section header,
section aside {
  grid-column: 2 / 3;
  grid-row: 1 / 2;
}

section main {
  position: relative;
  grid-column: 1 / 2;
  grid-row: 1 / 2;
}

section footer {
  grid-column: 1 / 3;
  grid-row: 2 / 3;
}

section main:empty::before,
section main:has(> br[data-mce-bogus]:first-child)::before {
  content: ""Write something..."";
  position: absolute;
  top: 0;
  left: 0;
  color: #999;
}

section main > * {
  margin: 0;
}

section main > * + * {
  margin-top: .5rem;
}

section.cover {
  background-color: #fed330;
  background-image: url(""images/template-document-cover-landscape_2x.png"");
  background-size: cover;
  background-position: right bottom;
  background-repeat: no-repeat;
}

section.cover main {
  align-self: center;
}

section.cover h1 {
  font-weight: 900;
  font-size: 5rem;
  letter-spacing: -1px;
  line-height: 1em;
  margin-bottom: 2.5rem;
}

section.end {
  background-color: #2C3A47;
  background-image: url(""images/template-document-end-landscape_2x.png"");
  background-size: 100%;
  background-position: right bottom;
  background-repeat: no-repeat;
  color: #C0C4C8;
}

section.end  a {
  color: #fff;
}

.editable:hover:not(:focus) {
  outline: 3px solid #b4d7ff;
  outline-offset: 8px;
}

.editable:focus {
  outline: none;
}
";

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
                {"editable_root",true},
                {"editable_class",true},
                {"elementpath",false},
                {"content_style", contentStyle},
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
                }},
            };
    [Inject] private IAiService _aiService { get; set; }
    private bool aiEnabled = false;
    private const string aiSVG = @"<svg width=""24"" height=""24"" xmlns=""http://www.w3.org/2000/svg"" fill-rule=""evenodd"" clip-rule=""evenodd""><path d=""M24 11.374c0 4.55-3.783 6.96-7.146 6.796-.151 1.448.061 2.642.384 3.641l-3.72 1.189c-.338-1.129-.993-3.822-2.752-5.279-2.728.802-4.969-.646-5.784-2.627-2.833.046-4.982-1.836-4.982-4.553 0-4.199 4.604-9.541 11.99-9.541 7.532 0 12.01 5.377 12.01 10.374zm-21.992-1.069c-.145 2.352 2.179 3.07 4.44 2.826.336 2.429 2.806 3.279 4.652 2.396 1.551.74 2.747 2.37 3.729 4.967l.002.006.111-.036c-.219-1.579-.09-3.324.36-4.528 3.907.686 6.849-1.153 6.69-4.828-.166-3.829-3.657-8.011-9.843-8.109-6.302-.041-9.957 4.255-10.141 7.306zm8.165-2.484c-.692-.314-1.173-1.012-1.173-1.821 0-1.104.896-2 2-2s2 .896 2 2c0 .26-.05.509-.141.738 1.215.911 2.405 1.855 3.6 2.794.424-.333.96-.532 1.541-.532 1.38 0 2.5 1.12 2.5 2.5s-1.12 2.5-2.5 2.5c-1.171 0-2.155-.807-2.426-1.895-1.201.098-2.404.173-3.606.254-.17.933-.987 1.641-1.968 1.641-1.104 0-2-.896-2-2 0-1.033.784-1.884 1.79-1.989.12-.731.252-1.46.383-2.19zm2.059-.246c-.296.232-.66.383-1.057.417l-.363 2.18c.504.224.898.651 1.079 1.177l3.648-.289c.047-.267.137-.519.262-.749l-3.569-2.736z""/></svg>";
    protected override async Task OnInitializedAsync()
    {
        aiEnabled = _aiService.IsEnabled();
        model.Content = "<section>  <main class=\"editable\"></main></section>";
        await base.OnInitializedAsync();
    }
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await _reportController.CreateComponent(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
            {
                Snackbar.Add(@localizer["reportComponentCreated"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["reportComponentCreatedError"], Severity.Error);
            }
            
        }
    }
    
    ReportTemplateModelFluentValidator templateValidator = new ReportTemplateModelFluentValidator();
    public class ReportTemplateModelFluentValidator : AbstractValidator<CreateReportComponentModel>
    {
        public ReportTemplateModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<CreateReportComponentModel>.CreateWithOptions((CreateReportComponentModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    
    private async Task OpenAiDialog(DialogOptions options)
    {
        //var parameters = new DialogParameters { ["project"]=SelectedProject };

        var dialog = Dialog.Show<AiDialog>("Custom Options Dialog", options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var data = await dialog.GetReturnValueAsync<FunctionResult>();
            model.Content = model.Content + data;
            StateHasChanged();
        }
        
    }
}