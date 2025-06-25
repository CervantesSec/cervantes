using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Tasks;

public partial class TaskNoteDialog: ComponentBase
{
    [Inject] private TaskController _taskController { get; set; }
    [Inject] private ProjectController _ProjectController { get; set; }

    [Parameter] public CORE.Entities.TaskNote note { get; set; } = new CORE.Entities.TaskNote();

    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    
    private TaskNoteEditViewModel model = new TaskNoteEditViewModel();
    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool editMode = false;
    private bool inProject = false;

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

    private ClaimsPrincipal userAth;
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
        Position = DialogPosition.Center,
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
    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        if (note.Task.ProjectId != null)
        {
            inProject = await _ProjectController.VerifyUser(note.Task.ProjectId.Value);
        }
    }

    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {

            var response = await _taskController.EditNote(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
            {

                Snackbar.Add(@localizer["noteEdited"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["noteEditedError"], Severity.Error);
            }
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
            model = new TaskNoteEditViewModel();
            model.Id = note.Id;
            model.Name = note.Name;
            model.Description = note.Description;
            model.TaskId = note.TaskId;
            
        }
        MudDialog.StateHasChanged();
    }
    
    private async Task DeleteTaskNoteDialog(CORE.Entities.TaskNote note,DialogOptions options)
    {
        var parameters = new DialogParameters { ["note"]=note };
        IMudExDialogReference<DeleteTaskNoteDialog>? dlgReference = await Dialog.ShowExAsync<DeleteTaskNoteDialog>("Simple Dialog", parameters, middleWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            MudDialog.Close(DialogResult.Ok(true));
            StateHasChanged();
        }
    }

}