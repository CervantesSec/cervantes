using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Cervantes.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Severity = MudBlazor.Severity;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Workspace.Target;

public partial class TargetDialog: ComponentBase
{
    [Parameter] public CORE.Entities.Target target { get; set; }

    private List<TargetServices> Services = new List<TargetServices>();  
    private List<TargetServices> seleServices = new List<TargetServices>();  

    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    
    private bool editMode = false;
    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private TargetController _TargetController { get; set; }
    [Inject] private ProjectController _ProjectController { get; set; }
    [Inject] private TargetCustomFieldController _TargetCustomFieldController { get; set; }

    MudForm form;

    private TargetEditViewModel model = new TargetEditViewModel();
   private string searchString = "";
   
   // Custom fields
   private List<TargetCustomFieldValueViewModel> targetCustomFieldValues = new List<TargetCustomFieldValueViewModel>();
    
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
    private bool inProject = false;
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
        Services =  _TargetController.GetServices(target.Id).ToList();
        if (target.ProjectId != Guid.Empty || target.ProjectId != null)
        {
            inProject = await _ProjectController.VerifyUser(target.ProjectId.Value);
        }
        await LoadCustomFields();
    }
    
    private async Task LoadCustomFields()
    {
        try
        {
            var customFields = _TargetCustomFieldController.GetActive();
            var customFieldValues = _TargetController.GetCustomFieldValuesByTargetId(target.Id);
            
            targetCustomFieldValues = customFields.Select(cf => {
                var existingValue = customFieldValues.FirstOrDefault(cfv => cfv.TargetCustomFieldId == cf.Id);
                return new TargetCustomFieldValueViewModel
                {
                    Id = existingValue?.Id ?? Guid.Empty,
                    TargetId = target.Id,
                    TargetCustomFieldId = cf.Id,
                    Name = cf.Name,
                    Label = cf.Label,
                    Type = cf.Type,
                    IsRequired = cf.IsRequired,
                    IsUnique = cf.IsUnique,
                    IsSearchable = cf.IsSearchable,
                    IsVisible = cf.IsVisible,
                    Order = cf.Order,
                    Options = cf.Options,
                    DefaultValue = cf.DefaultValue,
                    Description = cf.Description,
                    Value = existingValue?.Value ?? cf.DefaultValue ?? string.Empty
                };
            }).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading custom fields: {ex.Message}", Severity.Error);
        }
    }
    
    private async Task OnCustomFieldChanged(TargetCustomFieldValueViewModel field)
    {
        // Initialize CustomFieldValues dictionary if null
        if (model.CustomFieldValues == null)
        {
            model.CustomFieldValues = new Dictionary<Guid, string>();
        }
        
        // Update the model's custom field values
        model.CustomFieldValues[field.TargetCustomFieldId] = field.Value;
        await InvokeAsync(StateHasChanged);
    }
    

    async Task DeleteDialog(CORE.Entities.Target target,DialogOptions options)
    {
        var parameters = new DialogParameters { ["target"]=target };
        IMudExDialogReference<DeleteTargetDialog>? dlgReference = await Dialog.ShowExAsync<DeleteTargetDialog>("Simple Dialog", parameters, centerWidthEx);

        var result = await dlgReference.Result;

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
            model.Id = target.Id;
            model.Name = target.Name;
            model.Description = target.Description;
            model.ProjectId = target.ProjectId;
            model.Type = target.Type;
            
            // Initialize CustomFieldValues dictionary if null
            if (model.CustomFieldValues == null)
            {
                model.CustomFieldValues = new Dictionary<Guid, string>();
            }
            
            // Load custom field values into the model
            foreach (var customField in targetCustomFieldValues)
            {
                model.CustomFieldValues[customField.TargetCustomFieldId] = customField.Value;
            }
        }
        MudDialog.StateHasChanged();
    }

    
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await _TargetController.Edit(model);
            if (response is Microsoft.AspNetCore.Mvc.NoContentResult)
            {
                Snackbar.Add(@localizer["targetEdited"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["targetEditedError"], Severity.Error);
            }
        }
    }
    
    TargetModelFluentValidator targetValidator = new TargetModelFluentValidator();

    public class TargetModelFluentValidator : AbstractValidator<TargetEditViewModel>
    {
        public TargetModelFluentValidator()
        {

            RuleFor(x => x.Name)
                .NotEmpty();
        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<TargetEditViewModel>.CreateWithOptions((TargetEditViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    
      async Task RowClicked(DataGridRowClickEventArgs<CORE.Entities.TargetServices> args)
    {
        var parameters = new DialogParameters { ["service"]=args.Item };
        IMudExDialogReference<TargetServiceDialog>? dlgReference = await Dialog.ShowExAsync<TargetServiceDialog>("Simple Dialog", parameters, centerWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            Services = _TargetController.GetServices(target.Id).ToList();            
            StateHasChanged();
        }
    }
    
    private async Task Export(int id)
    {
        switch (id)
        {
            case 0:
                /*var records = model.Select(e => new IFR.Export.ProjectExport()
                {
                    Name = e.Name,
                    Description = e.Description,
                    Client = e.Client.Name,
                    CreatedUser = e.User.FullName,
                    StartDate = e.StartDate.ToShortDateString(),
                    EndDate = e.EndDate.ToShortDateString(),
                    Template = e.Template,
                    Status = e.Status.ToString(),
                    ProjectType = e.ProjectType.ToString(),
                    Language = e.Language.ToString(),
                    Score = e.Score.ToString(),
                    FindingsId = e.FindingsId.ToString(),
                    ExecutiveSummary = e.ExecutiveSummary
                    
 
                }).ToList();
                var file = ExportToCsv.ExportProjects(records);
                await JS.InvokeVoidAsync("downloadFile", file);
                Snackbar.Add(@localizer["exportSuccessfull"], Severity.Success);
                ExportToCsv.DeleteFile(file);*/
                break;
        }
    }
    
    private Func<CORE.Entities.TargetServices, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Version.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Note.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.User.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Port.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };

    private async Task OpenDialogCreate(DialogOptions options)
    {
        var parameters = new DialogParameters { ["target"]=target.Id };
        IMudExDialogReference<CreateTargetServiceDialog>? dlgReference = await Dialog.ShowExAsync<CreateTargetServiceDialog>("Simple Dialog", parameters, centerWidthEx);

        // wait modal to close
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            Services = _TargetController.GetServices(target.Id).ToList();
            StateHasChanged();
        }
        
    }
    
    private async Task BtnActions(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["services"]=seleServices };
                IMudExDialogReference<DeleteTargetServiceBulkDialog>? dlgReference = await Dialog.ShowExAsync<DeleteTargetServiceBulkDialog>("Simple Dialog", parameters, middleWidthEx);

                var result = await dlgReference.Result;

                if (!result.Canceled)
                {
                    Services = _TargetController.GetServices(target.Id).ToList();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<CORE.Entities.TargetServices> items)
    {
        
        seleServices = items.ToList();
    }
    
}