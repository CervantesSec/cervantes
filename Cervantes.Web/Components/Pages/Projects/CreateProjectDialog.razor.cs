using System.Net;
using Microsoft.AspNetCore.Components;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using Severity = MudBlazor.Severity;
using Variant = MudBlazor.Variant;
using System.Net.Http.Json;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components.Forms;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class CreateProjectDialog: ComponentBase
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
 
     void Cancel() => MudDialog.Cancel();
     
     [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;

    ProjectModelFluentValidator projectValidator = new ProjectModelFluentValidator();
	 
    ProjectCreateViewModel model = new ProjectCreateViewModel();
    private List<CORE.Entities.Client> Clients = new List<CORE.Entities.Client>();
    private DateTime? dateStart = DateTime.Today;
    private DateTime? dateEnd = DateTime.Today;
    [Inject] private ClientsController _clientsController { get; set; }
    [Inject] private ProjectController _projectController { get; set; }
    private List<Project> ProjectTemplates = new List<Project>();
    private Guid template;
    private Guid SelectedTemplate
    {

	    get => template;
	    set
	    {
		    template = value;
		    LoadTemplate();
	    }
    }
    protected override async Task OnInitializedAsync()
    {
	    await base.OnInitializedAsync();
	    Clients = _clientsController.Get().ToList();
		model.ClientId = Guid.Empty;
		var pro = ProjectTemplates = _projectController.Get().ToList();
		ProjectTemplates = pro.Where(x => x.Template == true).ToList();
		StateHasChanged();
    }

    private async Task Submit()
    {
	    model.StartDate = dateStart.Value;
	    model.EndDate = dateEnd.Value;
        await form.Validate();

        if (form.IsValid)
        {


	        var response = await _projectController.Add(model);
	        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
	        {
		        Snackbar.Add(@localizer["projectCreated"], Severity.Success);
		        MudDialog.Close(DialogResult.Ok(true));
	        }
	        else
	        {
		        Snackbar.Add(@localizer["projectCreatedError"], Severity.Error);
	        }
            
        }
    }

    /// <summary>
    /// A standard AbstractValidator which contains multiple rules and can be shared with the back end API
    /// </summary>
    /// <typeparam name="ProjectViewModel"></typeparam>
    public class ProjectModelFluentValidator : AbstractValidator<ProjectCreateViewModel>
    {
        public ProjectModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);
            RuleFor(x => x.EndDate)
	            .NotEmpty();
            RuleFor(x => x.StartDate)
	            .NotEmpty();
            RuleFor(x => x.ClientId)
	            .NotEmpty();
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ProjectCreateViewModel>.CreateWithOptions((ProjectCreateViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    private string ProjectTemplateDisplay(Guid itemId)
    {
	    var item = ProjectTemplates.FirstOrDefault(i => i.Id == itemId);

	    return item == null ? "!Not Found!" : $"{item.Name}";
    }
	
    public async Task LoadTemplate()
    {
	    var pro = ProjectTemplates.FirstOrDefault(x => x.Id == template);
	    model.Name = pro.Name;
	    model.Description = pro.Description;
	    model.Status = pro.Status;
	    model.ProjectType = pro.ProjectType;
	    model.Language = pro.Language;
	    model.Score = pro.Score;
	    model.FindingsId = pro.FindingsId;
	    StateHasChanged();
    }
    
    private int selectedVal = 0;
    private int? activeVal;

    private void HandleHoveredValueChanged(int? val) => activeVal = val;

    private string BusinessImpactLabelText => (activeVal ?? model.BusinessImpact) switch
    {
	    1 => @localizer["minimalImpact"],
	    2 => @localizer["lowImpact"],
	    3 => @localizer["moderateImpact"],
	    4 => @localizer["highImpact"],
	    5 => @localizer["veryHighImpact"],
	    _ => @localizer["noImpact"],
    };
    
    
    private Color GetRatingColor()
    {
	    return (activeVal ?? model.BusinessImpact) switch
	    {
		    1 => Color.Success,
		    2 => Color.Info,
		    3 => Color.Warning,
		    4 => Color.Secondary,
		    5 => Color.Error,
		    _ => Color.Default
	    };
    }
}
