using System.Net.Http.Json;
using System.Xml.Linq;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;
using Severity = MudBlazor.Severity;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class ImportVulnDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }

    void Cancel() => MudDialog.Cancel();
    MudForm form;
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private VulnController _vulnController { get; set; }
    [Inject] private ProjectController _projectController { get; set; }
    VulnImportViewModel model = new VulnImportViewModel();
    private static IBrowserFile file;
    private List<Project> Projects = new List<Project>();
    private Guid Project { get; set; }
    private long maxFileSize = 1024 * 1024 * 5;

    protected override async System.Threading.Tasks.Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Projects = _projectController.Get().Where(x => x.Template == false).ToList();
        Project  = Guid.Empty;
        StateHasChanged();
    }
    
    /*
    TargetImportModelFluentValidator targetValidator = new TargetImportModelFluentValidator();
    public class TargetImportModelFluentValidator : AbstractValidator<VulnImportViewModel>
    {
        public TargetImportModelFluentValidator()
        {

            RuleFor(x => x.Type)
                .NotEmpty();
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<VulnImportViewModel>.CreateWithOptions((VulnImportViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    */

    private bool importLoading = false;
    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            if (file != null)
            {
                importLoading = true;
                Stream stream = file.OpenReadStream(maxFileSize);
                MemoryStream ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                stream.Close();

                model.FileName = file.Name;
                model.FileContent = ms.ToArray();
                ms.Close();
                file = null;
                if (Project != Guid.Empty)
                {
                    model.Project = Project;
                }
                else
                {
                    model.Project = null;
                }

                var response = await _vulnController.Import(model);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
                {

                    Snackbar.Add(@localizer["vulnImported"], Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else if (response is BadRequestObjectResult badRequestResult)
                {
                    var message = badRequestResult.Value;
                    if (message.ToString() == "NotAllowed")
                    {
                        Snackbar.Add(@localizer["noInProject"], Severity.Warning);
                    }
                    importLoading = false;
			        
                }
                else
                {
                    importLoading = false;
                    Snackbar.Add(@localizer["vulnImportedError"], Severity.Error);
                }
            }
            else
            {
                Snackbar.Add(@localizer["noFileSelected"], Severity.Error);
            }
        }
    }
}