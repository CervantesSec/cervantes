using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.Export;
using Cervantes.Web.Components.Pages.Projects;
using Cervantes.Web.Components.Pages.Workspace.Target;
using Cervantes.Web.Controllers;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using Severity = MudBlazor.Severity;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Tasks;

public partial class TaskDialog: ComponentBase
{
      [Parameter] public CORE.Entities.Task task { get; set; } = new CORE.Entities.Task();

    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    
    private bool editMode = false;
    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    [Inject] ISnackbar Snackbar { get; set; }

    [Inject ]private IExportToCsv ExportToCsv { get; set; }

    private TaskEditViewModel model { get; set; } = new TaskEditViewModel();
    TaskModelFluentValidator taskValidator = new TaskModelFluentValidator();

    MudForm form;
    private static IBrowserFile file;
    private DateTime? dateStart;
    private DateTime? dateEnd;
    private List<Project> Projects = new List<Project>();
    private List<ApplicationUser> Users = new List<ApplicationUser>();
    private List<TaskTargets> Targets = new List<TaskTargets>();
    private List<TaskTargets> seleTargets = new List<TaskTargets>();
    private List<TaskNote> Notes = new List<TaskNote>();
    private List<TaskNote> seleNotes = new List<TaskNote>();
    private List<TaskAttachment> attachments = new List<TaskAttachment>();
    private List<TaskAttachment> seleAttachments = new List<TaskAttachment>();
    [Inject] private UserController _userController { get; set; }
    [Inject] private ProjectController _projectController { get; set; }
    [Inject] private TargetController _targetController { get; set; }
    [Inject] private TaskController _taskController { get; set; }
    private Guid SelectedProject { get; set; } = Guid.Empty;

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
        {"contextmenu", "link image imagetools table"}
    };
    private bool inProject = false;
    private ClaimsPrincipal userAth;
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        //model.SelectedTargets = new HashSet<Guid>();
        Targets = _taskController.GetTargets(task.Id).ToList();
        Notes = _taskController.GetNotes(task.Id).ToList();
        attachments = _taskController.GetAttachments(task.Id).ToList();
        if (task.ProjectId != null)
        {
            inProject = await _projectController.VerifyUser(task.ProjectId.Value);

        }
        StateHasChanged();
    }
    

    async Task DeleteTaskDialog(CORE.Entities.Task task,DialogOptions options)
    {
        var parameters = new DialogParameters { ["task"]=task };

        var dialog =  await Dialog.ShowEx<DeleteTaskDialog>("Edit", parameters,options);
        var result = await dialog.Result;

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
            model = new TaskEditViewModel();
            model.Id = task.Id;
            model.Name = task.Name;
            model.CreatedUserId = task.CreatedUserId;
            if (task.AsignedUserId != null)
            {
                model.AsignedUserId = task.AsignedUserId;
            }
            else
            {
                model.AsignedUserId = String.Empty;

            }
            model.Description = task.Description;
            model.Status = task.Status;
            dateStart = task.StartDate;
            dateEnd = task.EndDate;
            if (task.ProjectId != null)
            {
                SelectedProject = (Guid) task.ProjectId;
                model.ProjectId = task.ProjectId;
            }
            else
            {
                model.ProjectId = Guid.Empty;
            }

        }
        Users = _userController.Get().ToList();
        Projects = _projectController.Get().Where(x => x.Template == false).ToList();
        MudDialog.StateHasChanged();
    }
    
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            model.StartDate = dateStart.Value;
            model.EndDate = dateEnd.Value;
            if (SelectedProject != Guid.Empty)
            {
                model.ProjectId = SelectedProject;
            }
            else
            {
                model.ProjectId = null;
            }
            if (model.AsignedUserId == string.Empty)
            {
                model.AsignedUserId = null;
            }	
            var response = await _taskController.Edit(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
            {
                Snackbar.Add(@localizer["taskEdited"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["taskEditedError"], Severity.Error);
            }
            
        }
    }
    
    public class TaskModelFluentValidator : AbstractValidator<TaskEditViewModel>
    {
        public TaskModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);
            RuleFor(x => x.EndDate)
                .NotEmpty();
            RuleFor(x => x.StartDate)
                .NotEmpty();
            RuleFor(x => x.CreatedUserId)
                .NotEmpty();

        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<TaskEditViewModel>.CreateWithOptions((TaskEditViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    
    private string searchStringTarget = "";
    private Func<CORE.Entities.TaskTargets, bool> _quickFilterTarget => x =>
    {
        if (string.IsNullOrWhiteSpace(searchStringTarget))
            return true;
        if (x.Target.Name.Contains(searchStringTarget, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.Target.Description.ToString().Contains(searchStringTarget, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.Target.Type.ToString().Contains(searchStringTarget, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
    
    private string searchStringNote = "";
    private Func<CORE.Entities.TaskNote, bool> _quickFilterNote=> x =>
    {
        if (string.IsNullOrWhiteSpace(searchStringNote))
            return true;
        if (x.Name.Contains(searchStringNote, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.User.FullName.ToString().Contains(searchStringNote, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };

    private string searchStringAttachments = "";
    private Func<CORE.Entities.TaskAttachment, bool> _quickFilterAttachment=> x =>
    {
        if (string.IsNullOrWhiteSpace(searchStringAttachments))
            return true;
        if (x.Name.Contains(searchStringNote, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.User.FullName.ToString().Contains(searchStringNote, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
    
    private async Task OpenCreateAttachment(CORE.Entities.Task task,DialogOptions options)
    {
        var parameters = new DialogParameters { ["task"]=task };
        var dialog = await Dialog.ShowEx<AddTaskAttachmentDialog>(@localizer["addMember"], parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            attachments = _taskController.GetAttachments(task.Id).ToList();
            StateHasChanged();
        }
    }
    
    private async Task OpenCreateNote(CORE.Entities.Task task,DialogOptions options)
    {
        var parameters = new DialogParameters { ["task"]=task };
        var dialog = await Dialog.ShowEx<AddTaskNoteDialog>(@localizer["addMember"], parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Notes = _taskController.GetNotes(task.Id).ToList();
            StateHasChanged();
        }
    }
    
    private async Task OpenAddTarget(CORE.Entities.Task task,DialogOptions options)
    {
        var parameters = new DialogParameters { ["task"]=task };
        var dialog = await Dialog.ShowEx<AddTaskTargetDialog>(@localizer["addMember"], parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Targets = _taskController.GetTargets(task.Id).ToList();
            StateHasChanged();
        }
    }
    
    private async Task RowClickedTarget(DataGridRowClickEventArgs<TaskTargets> args)
    {
        if (inProject)
        {
            
            var item = _targetController.GetTargets().FirstOrDefault(x => x.Id == args.Item.TargetId);
            var parameters = new DialogParameters { ["target"]=item };

            var dialog =  await Dialog.ShowEx<TargetDialog>("Edit", parameters, maxWidth);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                Targets.Remove(args.Item);
                StateHasChanged();
            }
        }
        
    }
    
    private async Task RowClickedNote(DataGridRowClickEventArgs<TaskNote> args)
    {
        var parameters = new DialogParameters { ["note"]=args.Item };

        var dialog =  await Dialog.ShowEx<TaskNoteDialog>("Edit", parameters, maxWidth);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Notes = _taskController.GetNotes(task.Id).ToList();           
            StateHasChanged();
        }
    }
    
    private async Task RowClickedAttachment(DataGridRowClickEventArgs<TaskAttachment> args)
    {
        var parameters = new DialogParameters { ["attachment"]=args.Item };

        var dialog =  await Dialog.ShowEx<TaskAttachmentDialog>("Edit", parameters, maxWidth);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            attachments = _taskController.GetAttachments(task.Id).ToList();
            StateHasChanged();
        }
    }
    
    private async Task BtnActionsTarget(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["targets"]=seleTargets };

                var dialog =  await Dialog.ShowEx<DeleteTaskTargetBulkDialog>("Edit", parameters,maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    Targets = _taskController.GetTargets(task.Id).ToList();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedTargetsChanged(HashSet<CORE.Entities.TaskTargets> items)
    {
        
        seleTargets = items.ToList();
    }
    
    private async Task BtnActionsNotes(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["notes"]=seleNotes };

                var dialog =  await Dialog.ShowEx<DeleteTaskNoteBulkDialog>("Edit", parameters,maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    Notes = _taskController.GetNotes(task.Id).ToList();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedNotesChanged(HashSet<CORE.Entities.TaskNote> items)
    {
        
        seleNotes = items.ToList();
    }
    
    
    void SelectedAttachmentsChanged(HashSet<CORE.Entities.TaskAttachment> items)
    {
        
        seleAttachments = items.ToList();
    }
    
    private async Task BtnActionsAttachments(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["attachments"]=seleAttachments };

                var dialog =  await Dialog.ShowEx<DeleteTaskAttachmentBulkDialog>("Edit", parameters,maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    attachments = _taskController.GetAttachments(task.Id).ToList();
                    StateHasChanged();
                }
                break;
        }
    }
    
    

}