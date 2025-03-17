using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class AddTargetDialog: ComponentBase
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
      [CascadingParameter] IMudDialogInstance MudDialog { get; set; }

    void Cancel() => MudDialog.Cancel();
    MudForm form;
    [Inject] ISnackbar Snackbar { get; set; }
    
    TargetViewModel target = new TargetViewModel();
    [Parameter] public Guid project { get; set; }

    protected override async System.Threading.Tasks.Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        StateHasChanged();
    }
    
    TargetModelFluentValidator targetValidator = new TargetModelFluentValidator();
    public class TargetModelFluentValidator : AbstractValidator<TargetViewModel>
    {
        public TargetModelFluentValidator()
        {

            RuleFor(x => x.Name)
                .NotEmpty();
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<TargetViewModel>.CreateWithOptions((TargetViewModel)model, x => x.IncludeProperties(propertyName)));
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
            target.ProjectId = project;

            var response = await Http.PostAsJsonAsync("api/Target/Add", target);
            if (response.IsSuccessStatusCode)
            {

                Snackbar.Add(@localizer["targetCreated"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["targetCreatedError"], Severity.Error);
            }

        }
    }
}