using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;
using Severity = MudBlazor.Severity;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Tasks;

public partial class CreateTaskDialog: ComponentBase
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
	
         [CascadingParameter] MudDialogInstance MudDialog { get; set; }
 
     void Cancel() => MudDialog.Cancel();
     
     [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;

    TaskModelFluentValidator taskValidator = new TaskModelFluentValidator();
	 
    TaskCreateViewModel model = new TaskCreateViewModel();
    [Parameter] public Guid project { get; set; }
    private List<Project> Projects = new List<Project>();
    private Guid SelectedProject { get; set; } = Guid.Empty;
    private List<ApplicationUser> Users = new List<ApplicationUser>();
    //private Guid SelectedProject { get; set; } = Guid.Empty;
    private List<Target> Targets = new List<Target>();
    private DateTime? dateStart = DateTime.Today;
    private DateTime? dateEnd = DateTime.Today;
    [Inject] private UserController _userController { get; set; }
	[Inject] private ProjectController _projectController { get; set; }
	[Inject] private TargetController _targetController { get; set; }
	[Inject] private TaskController _taskController { get; set; }
    protected override async Task OnInitializedAsync()
    {
	    await base.OnInitializedAsync();
		//model.SelectedTargets = new HashSet<Guid>();
		Targets = _targetController.GetTargets().ToList();
		Users = _userController.Get().ToList();
		Projects = _projectController.Get().Where(x => x.Template == false).ToList();
		if (project != null || project != Guid.Empty)
		{
			SelectedProject = project;
		}
		model.CreatedUserId = _accessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
		model.AsignedUserId = _accessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);

		StateHasChanged();
    }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
	       model.StartDate = dateStart.Value.ToUniversalTime();
	       model.EndDate = dateEnd.Value.ToUniversalTime();

	       if (SelectedProject != Guid.Empty)
	       {
		       model.ProjectId = SelectedProject;
	       }
	       else
	       {
		       model.ProjectId = null;
	       }
	        
	        var response = await _taskController.Add(model);
	        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
	        {
		        Snackbar.Add(@localizer["taskCreated"], Severity.Success);
		        MudDialog.Close(DialogResult.Ok(true));
	        }
	        else if (response is BadRequestObjectResult badRequestResult)
	        {
		        var message = badRequestResult.Value;
		        if (message.ToString() == "NotAllowed")
		        {
			        Snackbar.Add(@localizer["noInProject"], Severity.Warning);
		        }
			        
	        }
	        else
	        {
		        Snackbar.Add(@localizer["taskCreatedError"], Severity.Error);
	        }
            
        }
    }

    /// <summary>
    /// A standard AbstractValidator which contains multiple rules and can be shared with the back end API
    /// </summary>
    /// <typeparam name="ProjectViewModel"></typeparam>
    public class TaskModelFluentValidator : AbstractValidator<TaskCreateViewModel>
    {
        public TaskModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);
            RuleFor(x => x.StartDate)
	            .NotEmpty();
            RuleFor(x => x.EndDate)
	            .NotEmpty();
      
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<TaskCreateViewModel>.CreateWithOptions((TaskCreateViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }

    private string GetMultiSelectionText(List<string> selectedValues)
    {
	    return $"{string.Join(", ", Targets.Where(x => selectedValues.Contains(x.Id.ToString())).Select(x => x.Name) ) }";
    }


}