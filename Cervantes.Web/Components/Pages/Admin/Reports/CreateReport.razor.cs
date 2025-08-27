using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Severity = MudBlazor.Severity;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Reports;

public partial class CreateReport: ComponentBase
{
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
	
	private long maxFileSize = 1024 * 1024 * 50;

         [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
     void Cancel() => MudDialog.Cancel();
     
     [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;

    ReportTemplateModelFluentValidator templateValidator = new ReportTemplateModelFluentValidator();
	 
    CreateReportModel model = new CreateReportModel();
    [Inject] private ReportController _reportController { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var comp = _reportController.Components().ToList();
        _dropItems = new List<DropItem>();
        foreach (var com in comp)
        {
            var item = new DropItem();
            item.Id = com.Id;
            item.Name = com.Name;
            item.Language = com.Language;
            item.ComponentType = com.ComponentType;
            item.Identifier = "None";
            _dropItems.Add(item);
        }
    }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {

            var itemsTemplate = _dropItems.Where(x => x.Identifier != "None" && x.Language == model.Language).ToList();
            model.Components = new List<ReportPartsModel>();
            
            foreach (var item in itemsTemplate)
            {
                var part = new ReportPartsModel();
                part.Id = item.Id;
                part.Order = item.Order;
                model.Components.Add(part);
            }
            
	        var response = await _reportController.CreateTemplate(model);
	        if (response.ToString() == "Microsoft.AspNetCore.Mvc.CreatedResult")
	        {
		        Snackbar.Add(@localizer["reportTemplateCreated"], Severity.Success);
		        MudDialog.Close(DialogResult.Ok(true));
	        }
	        else
	        {
		        Snackbar.Add(@localizer["reportTemplateCreatedError"], Severity.Error);
	        }
            
        }
    }
    
    /// <summary>
    /// A standard AbstractValidator which contains multiple rules and can be shared with the back end API
    /// </summary>
    /// <typeparam name="ReportTemplateViewModel"></typeparam>
    public class ReportTemplateModelFluentValidator : AbstractValidator<CreateReportModel>
    {
        public ReportTemplateModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<CreateReportModel>.CreateWithOptions((CreateReportModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    
    private void ItemUpdated(MudItemDropInfo<DropItem> dropItem)
    {
        dropItem.Item.Identifier = dropItem.DropzoneIdentifier;
        dropItem.Item.Order = _dropItems.Count(item => item.Identifier == dropItem.DropzoneIdentifier);

    }

    private List<DropItem> _dropItems = new();

    
    public class DropItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Language Language { get; set; }
        public ReportPartType ComponentType { get; set; }
        
        public string Identifier { get; set; }
        public int Order { get; set; } 

    }
    
}