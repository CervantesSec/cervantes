using System.Net.Http.Json;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Admin.Reports;

public partial class EditDialog: ComponentBase
{
   private Dictionary<string, object> editorConf = new Dictionary<string, object>{
		{"plugins", "preview importcss searchreplace autolink autosave save directionality code visualblocks visualchars fullscreen image link media template codesample table charmap pagebreak nonbreaking anchor insertdatetime advlist lists wordcount help charmap quickbars emoticons"},
		{"menubar", "file edit view insert format tools table help"},
		{"toolbar", "undo redo | bold italic underline strikethrough | fontselect fontsizeselect formatselect | alignleft aligncenter alignright alignjustify | outdent indent |  numlist bullist | forecolor backcolor removeformat | pagebreak | charmap emoticons | fullscreen  preview save print | insertfile image media template link anchor codesample | ltr rtl"},
		{"toolbar_sticky", true},
		{"image_advtab", true},
		{"height", 300},
		{"image_caption", true},
		{"quickbars_selection_toolbar", "bold italic | quicklink h2 h3 blockquote quickimage quicktable"},
		{"noneditable_noneditable_class", "mceNonEditable"},
		{"toolbar_mode", "sliding"},
		{"contextmenu", "link image imagetools table"}
	};
	
         [CascadingParameter] MudDialogInstance MudDialog { get; set; }
 
     void Cancel() => MudDialog.Cancel();
     
     [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;

    ReportTemplateModelFluentValidator templateValidator = new ReportTemplateModelFluentValidator();

    [Parameter] public ReportTemplateViewModel report { get; set; } = new ReportTemplateViewModel();
	 

	 private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {

	        var response = await Http.PostAsJsonAsync("api/Report/Template/Edit", report);
	        if (response.IsSuccessStatusCode)
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
    public class ReportTemplateModelFluentValidator : AbstractValidator<ReportTemplateViewModel>
    {
        public ReportTemplateModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);
            RuleFor(x => x.File)
	            .Cascade(CascadeMode.Stop)		
		        .NotEmpty();
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ReportTemplateViewModel>.CreateWithOptions((ReportTemplateViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    
	
}