using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.CervantesAI;
using Cervantes.IFR.Export;
using Cervantes.IFR.Jira;
using Cervantes.Web.Controllers;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Severity = MudBlazor.Severity;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class VulnDialog: ComponentBase
{
    private const string aiSVG = @"<svg width=""24"" height=""24"" xmlns=""http://www.w3.org/2000/svg"" fill-rule=""evenodd"" clip-rule=""evenodd""><path d=""M24 11.374c0 4.55-3.783 6.96-7.146 6.796-.151 1.448.061 2.642.384 3.641l-3.72 1.189c-.338-1.129-.993-3.822-2.752-5.279-2.728.802-4.969-.646-5.784-2.627-2.833.046-4.982-1.836-4.982-4.553 0-4.199 4.604-9.541 11.99-9.541 7.532 0 12.01 5.377 12.01 10.374zm-21.992-1.069c-.145 2.352 2.179 3.07 4.44 2.826.336 2.429 2.806 3.279 4.652 2.396 1.551.74 2.747 2.37 3.729 4.967l.002.006.111-.036c-.219-1.579-.09-3.324.36-4.528 3.907.686 6.849-1.153 6.69-4.828-.166-3.829-3.657-8.011-9.843-8.109-6.302-.041-9.957 4.255-10.141 7.306zm8.165-2.484c-.692-.314-1.173-1.012-1.173-1.821 0-1.104.896-2 2-2s2 .896 2 2c0 .26-.05.509-.141.738 1.215.911 2.405 1.855 3.6 2.794.424-.333.96-.532 1.541-.532 1.38 0 2.5 1.12 2.5 2.5s-1.12 2.5-2.5 2.5c-1.171 0-2.155-.807-2.426-1.895-1.201.098-2.404.173-3.606.254-.17.933-.987 1.641-1.968 1.641-1.104 0-2-.896-2-2 0-1.033.784-1.884 1.79-1.989.12-.731.252-1.46.383-2.19zm2.059-.246c-.296.232-.66.383-1.057.417l-.363 2.18c.504.224.898.651 1.079 1.177l3.648-.289c.047-.267.137-.519.262-.749l-3.569-2.736z""/></svg>";
   [Parameter] public CORE.Entities.Vuln vuln { get; set; } = new CORE.Entities.Vuln();

    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
    
    private bool editMode = false;
    void Cancel() => MudDialog.Cancel();
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    [Inject] ISnackbar Snackbar { get; set; }

    [Inject ]private IExportToCsv ExportToCsv { get; set; }
[Inject] private IJIraService JiraService { get; set; }
    private VulnCreateViewModel model { get; set; } = new VulnCreateViewModel();

    MudForm form;
    [Inject] private VulnController _vulnController { get; set; }
    [Inject] private ProjectController _projectController { get; set; }
    [Inject] private TargetController _targetController { get; set; }
    [Inject] private TaskController _taskController { get; set; }
    [Inject] private JiraController _jiraController { get; set; }
    [Inject] private VulnCustomFieldController VulnCustomFieldController { get; set; }
    VulnModelFluentValidator vulnValidator = new VulnModelFluentValidator();
    private List<VulnCategory> Categories = new List<VulnCategory>();
    private List<Cwe> Cwes = new List<Cwe>();
    private List<VulnCwe> VulnCwes = new List<VulnCwe>();
    private List<Project> Projects = new List<Project>();
    private List<Target> Targets = new List<Target>();
    private List<VulnTargets> VulnTargets = new List<VulnTargets>();
    private List<VulnTargets> seleTargets = new List<VulnTargets>();
    private List<VulnNote> VulnNotes = new List<VulnNote>();
    private List<VulnNote> seleNotes = new List<VulnNote>();
    private List<VulnAttachment> VulnAttachments = new List<VulnAttachment>();
    private List<VulnAttachment> seleAttachments = new List<VulnAttachment>();
    private List<CORE.Entities.Vuln> VulnTemplates = new List<CORE.Entities.Vuln>();
    private List<VulnCustomFieldValueViewModel> CustomFields = new List<VulnCustomFieldValueViewModel>();
    private Guid template;
    private CORE.Entities.Jira jira = new CORE.Entities.Jira();
    private List<CORE.Entities.JiraComments> JiraComments = new List<JiraComments>();
    private string searchStringTargets = "";
    private string searchStringNotes = "";
    private string searchStringAttachment = "";
    private bool jiraEnabled = false;
    private JiraCommentCreate jiraComment = new JiraCommentCreate();
    private Guid SelectedTemplate
    {

        get => template;
        set
        {
            template = value;
            LoadTemplate();
        }
    }
    private Guid SelectedProject { get; set; } = Guid.Empty;
    private Guid SelectedCategory { get; set; } = Guid.Empty;
    private IEnumerable<int> SelectedCwes { get; set; } = new HashSet<int>();
    private IEnumerable<Guid> SelectedTargets { get; set; } = new HashSet<Guid>();
    const string jiraSVG = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""24"" height=""24"" viewBox=""0 0 24 24""><path fill=""currentColor"" d=""M11.53 2c0 2.4 1.97 4.35 4.35 4.35h1.78v1.7c0 2.4 1.94 4.34 4.34 4.35V2.84a.84.84 0 0 0-.84-.84zM6.77 6.8a4.362 4.362 0 0 0 4.34 4.34h1.8v1.72a4.362 4.362 0 0 0 4.34 4.34V7.63a.841.841 0 0 0-.83-.83zM2 11.6c0 2.4 1.95 4.34 4.35 4.34h1.78v1.72c.01 2.39 1.95 4.34 4.34 4.34v-9.57a.84.84 0 0 0-.84-.84z""/></svg>";
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
    [Inject] private IAiService _aiService { get; set; }
    private bool aiEnabled = false;
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
        VulnTargets = _vulnController.GetVulnTargets(vuln.Id).ToList();
        VulnNotes = _vulnController.GetVulnNotes(vuln.Id).ToList();
        VulnAttachments = _vulnController.GetVulnAttachments(vuln.Id).ToList();
        VulnCwes =  _vulnController.GetVulnCwes(vuln.Id).ToList();
        var selectedCwes = new List<int>();
        foreach (var cwe in VulnCwes)
        {
            selectedCwes.Add(cwe.CweId);
        }
        SelectedCwes = selectedCwes.AsEnumerable();
        var selectedTargets = new List<Guid>();
        foreach (var tar in VulnTargets)
        {
            selectedTargets.Add(tar.TargetId);	        
            TargetDisplay(tar.TargetId);
        }
        SelectedTargets = selectedTargets.AsEnumerable();
        jiraEnabled = JiraService.JiraEnabled();
        if (vuln.JiraCreated)
        {
            jira = _jiraController.GetJiraByVuln(vuln.Id);
            JiraComments = _jiraController.GetCommentsByVuln(vuln.Id).ToList();
        }
        
        if (vuln.ProjectId != null)
        {
            inProject = await _projectController.VerifyUser(vuln.ProjectId.Value);
        }
        else
        {
            inProject = false;
        }
        aiEnabled = _aiService.IsEnabled();
        
        // Load custom fields and their values for this vulnerability
        await LoadCustomFields();
        
        await base.OnInitializedAsync();
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Always try to highlight after render, not just on first render
        try
        {
            // Small delay to ensure DOM is ready
            await Task.Delay(100);
            await JS.InvokeVoidAsync("initializePrism");
        }
        catch (Exception ex)
        {
            // Log the error for debugging
            Console.WriteLine($"Prism initialization error: {ex.Message}");
        }
        await base.OnAfterRenderAsync(firstRender);
    }
    

    async Task DeleteVulnDialog(CORE.Entities.Vuln vuln,DialogOptions options)
    {
        if (inProject || vuln.Project == null || vuln.ProjectId == Guid.Empty)
        {
            var parameters = new DialogParameters { ["vuln"]=vuln };
            IMudExDialogReference<DeleteVulnDialog>? dlgReference = await Dialog.ShowExAsync<DeleteVulnDialog>("Simple Dialog", parameters, middleWidthEx);

            var result = await dlgReference.Result;

            if (!result.Canceled)
            {
                MudDialog.Close();
            }
        }
        
    }
    
    async Task EditMode()
    {
        if (editMode)
        {
            editMode = false;

        }
        else
        {
            editMode = true;
            Categories = _vulnController.GetCategories().ToList();
            Cwes = _vulnController.GetCwes().ToList();
            Projects = _projectController.Get().Where(x => x.Template == false).ToList();
            Targets = _targetController.GetTargets().ToList();
            VulnTemplates =  _vulnController.GetTemplates().ToList();
            
            // Reload custom fields for edit mode
            await LoadCustomFields();
             model.Id = vuln.Id;
        model.Name = vuln.Name;
        model.Template = vuln.Template;
        model.Description = vuln.Description;
        model.ProjectId = vuln.ProjectId;
        model.Template = vuln.Template;
        model.FindingId = vuln.FindingId;
        model.VulnCategoryId = vuln.VulnCategoryId;
        model.Risk = vuln.Risk;
        model.Status = vuln.Status;
        model.cve = vuln.cve;
        model.ProofOfConcept = vuln.ProofOfConcept;
        model.Impact = vuln.Impact;
	        model.CVSS3 = vuln.CVSS3;
	        model.CVSSVector = vuln.CVSSVector;
        
        
        model.Remediation = vuln.Remediation;
        model.RemediationComplexity = vuln.RemediationComplexity;
        model.RemediationPriority = vuln.RemediationPriority;
        if (string.IsNullOrEmpty(model.OWASPVector))
        {
	        model.OWASPVector = "(SL:1/M:1/O:0/S:2/ED:1/EE:1/A:1/ID:1/LC:2/LI:1/LAV:1/LAC:1/FD:1/RD:1/NC:2/PV:3)";
	        model.OWASPImpact = String.Empty;
	        model.OWASPLikehood = String.Empty;
	        model.OWASPRisk = String.Empty;
        }
        else
        {
	        model.OWASPRisk = vuln.OWASPRisk;
	        model.OWASPImpact = vuln.OWASPImpact;
	        model.OWASPLikehood = vuln.OWASPLikehood;
	        model.OWASPVector = vuln.OWASPVector;
        }
        
 

        if (vuln.VulnCategoryId != null)
        {
	        SelectedCategory = vuln.VulnCategoryId.Value;
        }


        if (vuln.ProjectId != null)
		{
	        SelectedProject = (Guid) vuln.ProjectId;
		}
        
        }
        model.MitreTechniques = vuln.MitreTechniques.Split(',').ToList();
        model.MitreValues = vuln.MitreValues.Split(',').ToList();
        MudDialog.StateHasChanged();
    }
    
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            model.CweId = new List<int>();
            foreach(var cwe in SelectedCwes)
            {
                model.CweId.Add(cwe);
            }
            
            model.TargetId = new List<Guid>();
            foreach (var target in SelectedTargets)
            {
                model.TargetId.Add(target);
            }
	        
            if (SelectedCategory != Guid.Empty)
            {
                model.VulnCategoryId = SelectedCategory;
            }
            else
            {
                model.VulnCategoryId = null;
            }


            if (SelectedProject != Guid.Empty)
            {
                model.ProjectId = SelectedProject;
            }
            else
            {
                model.ProjectId = null;
            }
            
            // Add custom field values to the model
            foreach (var customField in CustomFields)
            {
                if (!string.IsNullOrEmpty(customField.Value))
                {
                    model.CustomFieldValues[customField.CustomFieldId] = customField.Value;
                }
            }
            
            var response = await _vulnController.Edit(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
            {
                Snackbar.Add(@localizer["vulnCreated"], Severity.Success);
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
                Snackbar.Add(@localizer["vulnCreatedError"], Severity.Error);
            }
            
        }
    }
    
    public class VulnModelFluentValidator : AbstractValidator<VulnCreateViewModel>
    {
        public VulnModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,100);

        }
	    

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<VulnCreateViewModel>.CreateWithOptions((VulnCreateViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    
    public async Task<IEnumerable<Cwe>> SearchCweFunc(string search){
        if (string.IsNullOrEmpty(search))
        {
            return Cwes;
        }
        return await Task.FromResult(Cwes.Where(x => x.Name.Contains(search, StringComparison.OrdinalIgnoreCase)));
    } 
    
    public async Task LoadTemplate()
    {
        var vuln = VulnTemplates.FirstOrDefault(x => x.Id == template);
        model.Name = vuln.Name;
        model.VulnCategoryId = vuln.VulnCategoryId.Value;
        model.cve = vuln.cve;
        model.Description = vuln.Description;
        model.Impact = vuln.Impact;
        model.Remediation = vuln.Remediation;
        model.ProofOfConcept = vuln.ProofOfConcept;
        model.Risk = vuln.Risk;
        model.Status = vuln.Status;
        model.RemediationComplexity = vuln.RemediationComplexity;
        model.RemediationPriority = vuln.RemediationPriority;
        model.CVSS3 = vuln.CVSS3;
        model.CVSSVector = vuln.CVSSVector;
        model.OWASPImpact = vuln.OWASPImpact;
        model.OWASPLikehood = vuln.OWASPLikehood;
        model.OWASPRisk = vuln.OWASPRisk;
        model.OWASPVector = vuln.OWASPVector;
	    
        var cwesVuln = _vulnController.GetVulnCwes(model.Id);
        foreach (var cwe in cwesVuln)
        {
            SelectedCwes.Append(cwe.CweId);
        }
	    
        var vulnTargets = _vulnController.GetVulnTargets(model.Id);
        foreach (var tar in vulnTargets)
        {
            SelectedTargets.Append(tar.TargetId);
        }

        StateHasChanged();
    }
    
    private string CweDisplay(int itemId)
    {
        var item = Cwes.FirstOrDefault(i => i.Id == itemId);

        return item == null ? "!Not Found!" : $"CWE-{item.Id} - {item.Name}";
    }
   
    private string TargetDisplay(Guid itemId)
    {
        var item = Targets.FirstOrDefault(i => i.Id == itemId);

        return item == null ? "!Not Found!" : $"{item.Name}";
    }
   
    private string VulnTemplateDisplay(Guid itemId)
    {
        var item = VulnTemplates.FirstOrDefault(i => i.Id == itemId);

        return item == null ? "!Not Found!" : $"{item.Name}";
    }
    
    private Func<CORE.Entities.VulnTargets, bool> _quickFilterTargets => element =>
    {
        if (string.IsNullOrWhiteSpace(searchStringTargets))
            return true;
        if (element.Target.Name.Contains(searchStringTargets, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Target.User.FullName.ToString().Contains(searchStringTargets, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
    
    async Task RowClickedTargets(DataGridRowClickEventArgs<CORE.Entities.VulnTargets> args)
    {
        if (inProject)
        {
            var parameters = new DialogParameters { ["target"]=args.Item };
            IMudExDialogReference<DeleteVulnTarget>? dlgReference = await Dialog.ShowExAsync<DeleteVulnTarget>("Simple Dialog", parameters, middleWidthEx);

            var result = await dlgReference.Result;

            if (!result.Canceled)
            {
                VulnTargets.Remove(args.Item);
                StateHasChanged();
            }
        }
        
    }
    
    async Task OpenDialogAddTarget(DialogOptions options)
    {
        var parameters = new DialogParameters { ["vuln"]=vuln };
        IMudExDialogReference<AddVulnTarget>? dlgReference = await Dialog.ShowExAsync<AddVulnTarget>("Simple Dialog", parameters, middleWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            VulnTargets = _vulnController.GetVulnTargets(vuln.Id).ToList();
            StateHasChanged();
        }
    }
    
    private Func<CORE.Entities.VulnNote, bool> _quickFilterNotes => element =>
    {
        if (string.IsNullOrWhiteSpace(searchStringNotes))
            return true;
        if (element.Name.Contains(searchStringNotes, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.User.FullName.ToString().Contains(searchStringNotes, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
    
    async Task RowClickedNotes(DataGridRowClickEventArgs<CORE.Entities.VulnNote> args)
    {
        var parameters = new DialogParameters { ["note"]=args.Item };
        IMudExDialogReference<VulnNoteDialog>? dlgReference = await Dialog.ShowExAsync<VulnNoteDialog>("Simple Dialog", parameters, middleWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            VulnNotes = _vulnController.GetVulnNotes(vuln.Id).ToList();
            StateHasChanged();
        }
    }
    
    async Task OpenDialogAddNote(DialogOptions options)
    {
        var parameters = new DialogParameters { ["vuln"]=vuln.Id };
        IMudExDialogReference<AddVulnNote>? dlgReference = await Dialog.ShowExAsync<AddVulnNote>("Simple Dialog", parameters, middleWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            VulnNotes = _vulnController.GetVulnNotes(vuln.Id).ToList();
            StateHasChanged();
        }
    }
    
    private Func<CORE.Entities.VulnAttachment, bool> _quickFilterAttachments => element =>
    {
        if (string.IsNullOrWhiteSpace(searchStringAttachment))
            return true;
        if (element.Name.Contains(searchStringAttachment, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.User.FullName.ToString().Contains(searchStringAttachment, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
    
    async Task RowClickedAttachments(DataGridRowClickEventArgs<CORE.Entities.VulnAttachment> args)
    {
        var parameters = new DialogParameters { ["attachment"]=args.Item };
        IMudExDialogReference<VulnAttachmentDialog>? dlgReference = await Dialog.ShowExAsync<VulnAttachmentDialog>("Simple Dialog", parameters, middleWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            VulnAttachments = _vulnController.GetVulnAttachments(vuln.Id).ToList();
            StateHasChanged();
        }
    }
    
    async Task OpenDialogAddAttachment(DialogOptions options)
    {
        var parameters = new DialogParameters { ["vuln"]=vuln.Id };
        IMudExDialogReference<AddVulnAttachment>? dlgReference = await Dialog.ShowExAsync<AddVulnAttachment>("Simple Dialog", parameters, middleWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            VulnAttachments = _vulnController.GetVulnAttachments(vuln.Id).ToList();
            StateHasChanged();
        }
    }
    
    void SelectedTargetsChanged(HashSet<CORE.Entities.VulnTargets> items)
    {
        
        seleTargets = items.ToList();
    }
    
    void SelectedNotesChanged(HashSet<CORE.Entities.VulnNote> items)
    {
        
        seleNotes = items.ToList();
    }
    
    void SelectedAttachmentsChanged(HashSet<CORE.Entities.VulnAttachment> items)
    {
        
        seleAttachments = items.ToList();
    }
    private async Task BtnActionsTargets(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["targets"]=seleTargets };
                IMudExDialogReference<DeleteVulnTargetBulkDialog>? dlgReference = await Dialog.ShowExAsync<DeleteVulnTargetBulkDialog>("Simple Dialog", parameters, middleWidthEx);

                var result = await dlgReference.Result;

                if (!result.Canceled)
                {
                    VulnTargets = _vulnController.GetVulnTargets(vuln.Id).ToList();
                    StateHasChanged();
                }
                break;
        }
    }
    private async Task BtnActionsNotes(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["notes"]=seleNotes };
                IMudExDialogReference<DeleteVulnNoteBulkDialog>? dlgReference = await Dialog.ShowExAsync<DeleteVulnNoteBulkDialog>("Simple Dialog", parameters, middleWidthEx);

                var result = await dlgReference.Result;

                if (!result.Canceled)
                {
                    VulnNotes = _vulnController.GetVulnNotes(vuln.Id).ToList();
                    StateHasChanged();
                }
                break;
        }
    }
    private async Task BtnActionsAttachments(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["attachments"]=seleAttachments };
                IMudExDialogReference<DeleteVulnAttachmentBulkDialog>? dlgReference = await Dialog.ShowExAsync<DeleteVulnAttachmentBulkDialog>("Simple Dialog", parameters, middleWidthEx);

                var result = await dlgReference.Result;

                if (!result.Canceled)
                {
                    VulnAttachments = _vulnController.GetVulnAttachments(vuln.Id).ToList();
                    StateHasChanged();
                }
                break;
        }
    }

    private async Task CreateJira()
    {
        var response = await _jiraController.Add(@vuln.Id);
        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
        {
            Snackbar.Add(@localizer["jiraCreated"], Severity.Success);
            await UpdateJira();
            StateHasChanged();
        }
        else
        {
            Snackbar.Add(@localizer["jiraCreatedError"], Severity.Error);
        }
    }
    
    private async Task DeleteJira()
    {
        
        var response = await _jiraController.DeleteIssue(@vuln.Id);
        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
        {
            Snackbar.Add(@localizer["jiraDeleted"], Severity.Success);
            StateHasChanged();
        }
        else
        {
            Snackbar.Add(@localizer["jiraDeletedError"], Severity.Error);
        }
    }
    
    private async Task UpdateJira()
    {
        var response = await _jiraController.UpdateIssue(@vuln.Id);
        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
        {
            Snackbar.Add(@localizer["jiraUpdated"], Severity.Success);
            jira = _jiraController.GetJiraByVuln(vuln.Id);
            JiraComments = _jiraController.GetCommentsByVuln(vuln.Id).ToList();
        }
        else
        {
            Snackbar.Add(@localizer["jiraUpdatedError"], Severity.Error);
        }
    }

    private string test;
    private async Task AddComment()
    {
        jiraComment = new JiraCommentCreate
        {
            VulnId = vuln.Id,
            Comment = test
        };
        var response = await _jiraController.AddComment(jiraComment);
        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
        {
            Snackbar.Add(@localizer["addedComment"], Severity.Success);
            await UpdateJira();

        }
        else
        {
            Snackbar.Add(@localizer["addedCommentError"], Severity.Error);
        }
    }
    
    private async Task OpenAiDialog(DialogOptions options)
    {
        //var parameters = new DialogParameters { ["project"]=SelectedProject };
        IMudExDialogReference<AiDialog>? dlgReference = await Dialog.ShowExAsync<AiDialog>("Simple Dialog", middleWidthEx);

        // wait modal to close
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            var data = await dlgReference.GetReturnValueAsync<VulnAiModel>();
            model.Name = data.Name;
            model.Language = data.Language;
            model.Description = data.Description;
            model.Remediation = data.Remediation;
            model.Impact = data.Impact;
            model.Risk = data.Risk;
            model.ProofOfConcept = data.ProofOfConcept;
            StateHasChanged();
        }
        
    }
    
    private async Task LoadCustomFields()
    {
        try
        {
            // Load active custom fields
            var activeCustomFields = VulnCustomFieldController.GetActive();
            
            // Load existing values for this vulnerability
            var existingValues = VulnCustomFieldController.GetValues(vuln.Id)
                .ToDictionary(v => v.VulnCustomFieldId, v => v.Value ?? string.Empty);
            
            // Create ViewModels with existing values or defaults
            CustomFields = activeCustomFields.Select(cf => new VulnCustomFieldValueViewModel
            {
                CustomFieldId = cf.Id,
                Name = cf.Name,
                Label = cf.Label,
                Type = cf.Type,
                IsRequired = cf.IsRequired,
                IsUnique = cf.IsUnique,
                Options = cf.Options,
                DefaultValue = cf.DefaultValue,
                Description = cf.Description,
                Order = cf.Order,
                Value = existingValues.TryGetValue(cf.Id, out var existingValue) 
                    ? existingValue 
                    : (cf.DefaultValue ?? string.Empty)
            }).OrderBy(cf => cf.Order).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading custom fields: {ex.Message}", Severity.Error);
        }
    }
    
    private async Task OnCustomFieldChanged(VulnCustomFieldValueViewModel field)
    {
        // This method is called when a custom field value changes
        // We could add validation here if needed
        await Task.CompletedTask;
    }
    
}