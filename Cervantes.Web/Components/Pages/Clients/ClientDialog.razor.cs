using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.Export;
using Cervantes.Web.Components.Pages.Projects;
using Cervantes.Web.Components.Pages.Vuln;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Severity = MudBlazor.Severity;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Components.Pages.Clients;

public partial class ClientDialog: ComponentBase
{
    [Parameter] public CORE.Entities.Client client { get; set; } = new CORE.Entities.Client();
    private long maxFileSize = 1024 * 1024 * 5;

    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    
    private bool editMode = false;
    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    private ClientEditViewModel model { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private ClientsController _clientsController { get; set; }
    [Inject] private ProjectController _projectController { get; set; }
    [Inject] private VulnController _VulnController { get; set; }
    [Inject] private ClientCustomFieldController _ClientCustomFieldController { get; set; }

    private List<CORE.Entities.Project> projects = new List<CORE.Entities.Project>();
    private List<CORE.Entities.Vuln> vulns = new List<CORE.Entities.Vuln>();
    private List<CORE.Entities.Project> selectedProjects = new List<CORE.Entities.Project>();
    private List<ClientCustomFieldValueViewModel> clientCustomFieldValues = new List<ClientCustomFieldValueViewModel>();

    [Inject ]private IExportToCsv ExportToCsv { get; set; }


    MudForm form;
    private static IBrowserFile file;
    

    ClientModelFluentValidator clientValidator = new ClientModelFluentValidator();
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
     
     ClaimsPrincipal user;
    ClaimsPrincipal userAth;
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
        user = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        projects =  _projectController.GetByClientId(client.Id).ToList();
        vulns =  _VulnController.GetByClientId(client.Id).ToList();
        await LoadClientCustomFields();
    }

    public class ClientModelFluentValidator : AbstractValidator<ClientEditViewModel>
    {
        public ClientModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);
            RuleFor(x => x.Url)
                .Length(0,100);
            RuleFor(x => x.ContactName)
                .Length(0,100);
            RuleFor(x => x.ContactPhone)
                .Length(0,100);
            RuleFor(x => x.ContactEmail != null ? x.ContactEmail.ToLower() : null)
                    .EmailAddress();
            
            
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ClientEditViewModel>.CreateWithOptions((ClientEditViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            
            if (file != null)
            {
                Stream stream = file.OpenReadStream(maxFileSize);
                MemoryStream ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                stream.Close();

                model.FileName = file.Name;
                model.FileContent = ms.ToArray();
                ms.Close();
                file = null;
            }

            var response = await _clientsController.Edit(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
            {
                // Save custom fields
                await SaveClientCustomFields();
                
                Snackbar.Add(@localizer["clientEdited"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["clientEditedError"], Severity.Error);
            }
        }
    }

    async Task DeleteDialog(CORE.Entities.Client client,DialogOptions options)
    {
        var parameters = new DialogParameters { ["client"]=client };
        IMudExDialogReference<DeleteClientDialog>? dlgReference = await Dialog.ShowExAsync<DeleteClientDialog>("Simple Dialog", parameters, middleWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            MudDialog.Close();
        }
    }
    private async Task DeleteLogo(Guid id)
    {
        var response = await _clientsController.DeleteAvatar(id);
        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
        {
            Snackbar.Add(@localizer["logoDeleted"], Severity.Success);
            client.ImagePath = "None";
            MudDialog.StateHasChanged();
        }
        else
        {
            Snackbar.Add(@localizer["logoDeletedError"], Severity.Error);
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
            model = new ClientEditViewModel();
            model.Id = client.Id;
            model.Name = client.Name;
            model.Description = client.Description;
            model.Url = client.Url;
            model.ContactName = client.ContactName;
            model.ContactEmail = client.ContactEmail;
            model.ContactPhone = client.ContactPhone;

        }
        MudDialog.StateHasChanged();
    }
    
    private async Task ExportProjects(int id)
    {
        switch (id)
        {
            case 0:
                var records = projects.Select(e => new IFR.Export.ClientExport
                {
                    Name = e.Name,
                    Description = e.Description,
 
                }).ToList();
                var file = ExportToCsv.ExportClients(records);
                await JS.InvokeVoidAsync("downloadFile", file);
                Snackbar.Add(@localizer["exportSuccessfull"], Severity.Success);
                ExportToCsv.DeleteFile(file);
                break;
        }
    }
    
    private async Task ExportVulns(int id)
    {
        switch (id)
        {
            case 0:
                var records = projects.Select(e => new IFR.Export.ClientExport
                {
                    Name = e.Name,
                    Description = e.Description,
 
                }).ToList();
                var file = ExportToCsv.ExportClients(records);
                await JS.InvokeVoidAsync("downloadFile", file);
                Snackbar.Add(@localizer["exportSuccessfull"], Severity.Success);
                ExportToCsv.DeleteFile(file);
                break;
        }
    }
    private string searchStringPro = "";
    private Func<Project, bool> _quickFilterPro => x =>
    {
        if (string.IsNullOrWhiteSpace(searchStringPro))
            return true;
        if (x.Name.Contains(searchStringPro, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.ProjectType.ToString().Contains(searchStringPro, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.Status.ToString().Contains(searchStringPro, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.StartDate.ToString().Contains(searchStringPro, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.EndDate.ToString().Contains(searchStringPro))
            return true;
        return false;
    };
    
    private string searchStringVuln = "";
    private Func<CORE.Entities.Vuln, bool> _quickFilterVuln => x =>
    {
        if (string.IsNullOrWhiteSpace(searchStringVuln))
            return true;
        if (x.Name.Contains(searchStringVuln, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.Language.ToString().Contains(searchStringVuln, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.Status.ToString().Contains(searchStringVuln, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.CreatedDate.ToString().Contains(searchStringVuln, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.Project.Name.Contains(searchStringVuln))
            return true;
        if (x.Risk.ToString().Contains(searchStringVuln, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
    
    private async Task BtnActionsProject(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["projects"]=selectedProjects };
                IMudExDialogReference<DeleteProjectBulkDialog>? dlgReference = await Dialog.ShowExAsync<DeleteProjectBulkDialog>("Simple Dialog", parameters, middleWidthEx);

                var result = await dlgReference.Result;

                if (!result.Canceled)
                {
                    projects  = _projectController.GetByClientId(client.Id).ToList();                  
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedProjectsChanged(HashSet<Project> items)
    {
        
        selectedProjects = items.ToList();
    }
    async Task RowClickedProject(DataGridRowClickEventArgs<Project> args)
    {
        var parameters = new DialogParameters { ["project"]=args.Item };
        IMudExDialogReference<ProjectDialog>? dlgReference = await Dialog.ShowExAsync<ProjectDialog>("Simple Dialog", parameters, centerWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            projects  = _projectController.GetByClientId(client.Id).ToList();
            StateHasChanged();
        }
    }
    
    void SelectedVulnssChanged(HashSet<Project> items)
    {
        
        selectedProjects = items.ToList();
    }
    async Task RowClickedVuln(DataGridRowClickEventArgs<CORE.Entities.Vuln> args)
    {
        var parameters = new DialogParameters { ["vuln"]=args.Item };
        IMudExDialogReference<VulnDialog>? dlgReference = await Dialog.ShowExAsync<VulnDialog>("Simple Dialog", parameters, centerWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            vulns  = _VulnController.GetByClientId(client.Id).ToList();
            StateHasChanged();
        }
    }

    #region Custom Fields

    private async Task LoadClientCustomFields()
    {
        try
        {
            var customFields = _ClientCustomFieldController.Get().Where(cf => cf.IsActive).ToList();
            var existingValues = client.CustomFieldValues.ToList();
            clientCustomFieldValues = new List<ClientCustomFieldValueViewModel>();
            
            foreach (var customField in customFields.OrderBy(cf => cf.Order))
            {
                var existingValue = existingValues?.FirstOrDefault(v => v.ClientCustomFieldId == customField.Id);
                
                clientCustomFieldValues.Add(new ClientCustomFieldValueViewModel
                {
                    CustomFieldId = customField.Id,
                    Name = customField.Name,
                    Label = customField.Label,
                    Type = customField.Type,
                    IsRequired = customField.IsRequired,
                    IsUnique = customField.IsUnique,
                    Options = customField.Options,
                    DefaultValue = customField.DefaultValue,
                    Description = customField.Description,
                    Order = customField.Order,
                    Value = existingValue?.Value ?? customField.DefaultValue ?? string.Empty
                });
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading custom fields: {ex.Message}", Severity.Error);
        }
    }

    private async Task SaveClientCustomFields()
    {
        try
        {
            var customFieldData = clientCustomFieldValues.ToDictionary(
                cf => cf.CustomFieldId, 
                cf => cf.Value ?? string.Empty
            );
            
            var result = await _clientsController.UpdateCustomFieldValues(client.Id, customFieldData);
            if (result is not OkResult)
            {
                throw new Exception("Failed to update custom field values");
            }
            Snackbar.Add("Custom fields saved successfully", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error saving custom fields: {ex.Message}", Severity.Error);
        }
    }

    private async Task OnCustomFieldChanged(ClientCustomFieldValueViewModel field)
    {
        StateHasChanged();
    }

    #endregion

    
}