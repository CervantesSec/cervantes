using Cervantes.CORE.ViewModel;
using Cervantes.Web.Components.Pages.Clients;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Admin.Reports;

public partial class ImportReport : ComponentBase
{
    private List<BreadcrumbItem> _items;
    protected override async Task OnInitializedAsync()
    {
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Admin", href: null,icon: Icons.Material.Filled.AdminPanelSettings),
            new BreadcrumbItem(@localizer["reports"], href: "/",icon: Icons.Custom.FileFormats.FilePdf),
            new BreadcrumbItem(@localizer["import"], href: null, disabled: true,icon: Icons.Material.Filled.UploadFile)
        };
    }
    
     private static IBrowserFile file;
     private bool showHtml = false;
     [Inject] ISnackbar Snackbar { get; set; }
     [Inject] private ReportController _ReportController { get; set; }


    MudForm form;

    ImportModelFluentValidator importValidator = new ImportModelFluentValidator();
	 
    ReportImportViewModel model = new ReportImportViewModel();

    
	private long maxFileSize = 1024 * 1024 * 50;
	private ReportImportResultViewModel _reportImportResultViewModel = new ReportImportResultViewModel();
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
	        }
	        
	        var response =  await _ReportController.Import(model);
	        if (response != null)
	        {
		        Snackbar.Add(@localizer["clientCreated"], Severity.Success);
		        _reportImportResultViewModel = response;
		        showHtml = true;
	        }
	        else
	        {
		        Snackbar.Add(@localizer["clientCreatedError"], Severity.Error);
	        }

        }
    }

    /// <summary>
    /// A standard AbstractValidator which contains multiple rules and can be shared with the back end API
    /// </summary>
    /// <typeparam name="ClientModel"></typeparam>
    public class ImportModelFluentValidator : AbstractValidator<ReportImportViewModel>
    {
        public ImportModelFluentValidator()
        {
            /*RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);*/
          
		}
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ReportImportViewModel>.CreateWithOptions((ReportImportViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}