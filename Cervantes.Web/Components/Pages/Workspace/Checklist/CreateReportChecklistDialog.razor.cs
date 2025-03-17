using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Workspace.Checklist;

public partial class CreateReportChecklistDialog: ComponentBase
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
	
         [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 [Parameter] public Guid project { get; set; }
 [Parameter] public string checklistType { get; set; }
 [Parameter] public Guid checklistId { get; set; }

     void Cancel() => MudDialog.Cancel();
     
     [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;

    ReportModelFluentValidator reportValidator = new ReportModelFluentValidator();
	 
    ReportChecklistCreateModel model = new ReportChecklistCreateModel();
    private List<CORE.Entities.ReportTemplate> templates = new List<CORE.Entities.ReportTemplate>();
    [Inject] private ReportController _ReportController { get; set; }
    [Inject] private ChecklistController _ChecklistController { get; set; }

    protected override async Task OnInitializedAsync()
    {
	    templates = _ReportController.Templates().ToList();
	    model.ProjectId = project;
	    model.ChecklistId = checklistId;
	    await base.OnInitializedAsync();
		StateHasChanged();
    }

    private async Task Submit()
    {
	    await form.Validate();

	    if (form.IsValid)
	    {
		    
		   
		    if (checklistType == "OWASPWSTG")
		    {
			    var response = await _ChecklistController.GenerateWstgReport(model);
			    if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
			    {
				    Snackbar.Add(@localizer["reportCreated"], Severity.Success);
				    MudDialog.Close(DialogResult.Ok(true));
			    }
			    else
			    {
				    Snackbar.Add(@localizer["reportCreatedError"], Severity.Error);
			    }
		    }
		    else
		    {
			    var response = await _ChecklistController.GenerateMastgReport(model);
			    if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
			    {
				    Snackbar.Add(@localizer["reportCreated"], Severity.Success);
				    MudDialog.Close(DialogResult.Ok(true));
			    }
			    else
			    {
				    Snackbar.Add(@localizer["reportCreatedError"], Severity.Error);
			    }
		    }
		    
		    
            
	    }
    }
    
    /*private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
			model.ProjectId = project;

	        var response = await _ReportController.GenerateReport(model);
	        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
	        {
		        Snackbar.Add(@localizer["reportCreated"], Severity.Success);
		        MudDialog.Close(DialogResult.Ok(true));
	        }
	        else
	        {
		        Snackbar.Add(@localizer["reportCreatedError"], Severity.Error);
	        }
            
        }
    }*/

    /// <summary>
    /// A standard AbstractValidator which contains multiple rules and can be shared with the back end API
    /// </summary>
    /// <typeparam name="ProjectViewModel"></typeparam>
    public class ReportModelFluentValidator : AbstractValidator<ReportChecklistCreateModel>
    {
        public ReportModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);
            RuleFor(x => x.ReportTemplateId)
	            .NotEmpty();
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ReportChecklistCreateModel>.CreateWithOptions((ReportChecklistCreateModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}