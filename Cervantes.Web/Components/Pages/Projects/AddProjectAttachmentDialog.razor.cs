using System.Net.Http.Json;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class AddProjectAttachmentDialog: ComponentBase
{
    private long maxFileSize = 1024 * 1024 * 50;

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
    MudForm form;
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] ProjectController _projectController { get; set; }
    ProjectAttachmentViewModel attachment = new ProjectAttachmentViewModel();
    [Parameter] public Guid project { get; set; }
    private IBrowserFile file;

    protected override async System.Threading.Tasks.Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        StateHasChanged();
    }
    
    AttachmentModelFluentValidator attachmentValidator = new AttachmentModelFluentValidator();
    public class AttachmentModelFluentValidator : AbstractValidator<ProjectAttachmentViewModel>
    {
        public AttachmentModelFluentValidator()
        {

            RuleFor(x => x.Name)
                .NotEmpty();
    
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ProjectAttachmentViewModel>.CreateWithOptions((ProjectAttachmentViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            if (file != null)
            {
                attachment.ProjectId = project;
                Stream stream = file.OpenReadStream(maxFileSize);
                MemoryStream ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                stream.Close();
	        
                attachment.FileName = file.Name;
                attachment.FileContent = ms.ToArray();
                ms.Close();
                file = null;

                var response = await _projectController.AddAttachment(attachment);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.CreatedResult")
                {

                    Snackbar.Add(@localizer["addAttachment"], Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    Snackbar.Add(@localizer["addAttachmentError"], Severity.Error);
                }
            }
            else
            {
                Snackbar.Add(@localizer["noFile"], Severity.Error);

            }

        }
        else
        {
            Snackbar.Add(@localizer["addAttachmentError"], Severity.Error);

        }
    }
}