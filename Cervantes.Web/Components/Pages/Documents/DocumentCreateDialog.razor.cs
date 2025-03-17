using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Documents;

public partial class DocumentCreateDialog: ComponentBase
{
	
	[CascadingParameter] bool _isDarkMode { get; set; }

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
         [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
     void Cancel() => MudDialog.Cancel();
     private static IBrowserFile file;
     private long maxFileSize = 50 * 1024 * 1024;

     [Inject] ISnackbar Snackbar { get; set; }
     [Inject] private DocumentController _DocumentController { get; set; }


    MudForm form;

    DocumentModelFluentValidator docValidator = new DocumentModelFluentValidator();
	 
    DocumentCreateViewModel model = new DocumentCreateViewModel();

 protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
    
	 private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
	        if (file != null)
	        {
		        Stream stream = file.OpenReadStream(maxFileSize);
		        MemoryStream ms = new MemoryStream();
		        await stream.CopyToAsync(ms);
		        stream.Close();

		        model.FileName = file.Name;
		        model.FileContent = ms.ToArray();
		        ms.Close();
		        file = null;

		         var response =  await _DocumentController.Add(model);
		        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
		        {
			        Snackbar.Add(@localizer["documentCreated"], Severity.Success);
			        MudDialog.Close(DialogResult.Ok(true));
		        }
		        else
		        {
			        Snackbar.Add(@localizer["documentCreatedError"], Severity.Error);
		        }
	        }
	        else
	        {
		        Snackbar.Add(@localizer["noFileSelected"], Severity.Error);
	        }

        }
    }
	 
    public class DocumentModelFluentValidator : AbstractValidator<DocumentCreateViewModel>
    {
        public DocumentModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);
		}
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<DocumentCreateViewModel>.CreateWithOptions((DocumentCreateViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}