using System.Net.Http.Json;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class AddVulnAttachment: ComponentBase
{
    private long maxFileSize = 1024 * 1024 * 5;

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
    private IBrowserFile file { get; set; }
      [CascadingParameter] IMudDialogInstance MudDialog { get; set; }

    void Cancel() => MudDialog.Cancel();
    MudForm form;
    [Inject] ISnackbar Snackbar { get; set; }
    
    VulnAttachmentViewModel attachment = new VulnAttachmentViewModel();
    [Parameter] public Guid vuln { get; set; }
    [Inject] private VulnController _VulnController { get; set; }

    protected override async System.Threading.Tasks.Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        StateHasChanged();
    }
    
    AttachmentModelFluentValidator attachmentValidator = new AttachmentModelFluentValidator();
    public class AttachmentModelFluentValidator : AbstractValidator<VulnAttachmentViewModel>
    {
        public AttachmentModelFluentValidator()
        {

            RuleFor(x => x.Name)
                .NotEmpty();
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<VulnAttachmentViewModel>.CreateWithOptions((VulnAttachmentViewModel)model, x => x.IncludeProperties(propertyName)));
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
                attachment.VulnId = vuln;
                Stream stream = file.OpenReadStream(maxFileSize);
                MemoryStream ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                stream.Close();
	        
                attachment.FileName = file.Name;
                attachment.FileContent = ms.ToArray();
                ms.Close();
                file = null;


                var response = await _VulnController.AddAttachment(attachment);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.CreatedAtActionResult")            
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
                Snackbar.Add(@localizer["noFiles"], Severity.Error);

            }
            
        }
    }
}