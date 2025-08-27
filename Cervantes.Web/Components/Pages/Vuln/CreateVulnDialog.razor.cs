using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.CervantesAI;
using Cervantes.Web.Components.Pages.Workspace.Target;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;
using MudExtensions;
using Severity = MudBlazor.Severity;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class CreateVulnDialog: ComponentBase
{
	private const string aiSVG = @"<svg width=""24"" height=""24"" xmlns=""http://www.w3.org/2000/svg"" fill-rule=""evenodd"" clip-rule=""evenodd""><path d=""M24 11.374c0 4.55-3.783 6.96-7.146 6.796-.151 1.448.061 2.642.384 3.641l-3.72 1.189c-.338-1.129-.993-3.822-2.752-5.279-2.728.802-4.969-.646-5.784-2.627-2.833.046-4.982-1.836-4.982-4.553 0-4.199 4.604-9.541 11.99-9.541 7.532 0 12.01 5.377 12.01 10.374zm-21.992-1.069c-.145 2.352 2.179 3.07 4.44 2.826.336 2.429 2.806 3.279 4.652 2.396 1.551.74 2.747 2.37 3.729 4.967l.002.006.111-.036c-.219-1.579-.09-3.324.36-4.528 3.907.686 6.849-1.153 6.69-4.828-.166-3.829-3.657-8.011-9.843-8.109-6.302-.041-9.957 4.255-10.141 7.306zm8.165-2.484c-.692-.314-1.173-1.012-1.173-1.821 0-1.104.896-2 2-2s2 .896 2 2c0 .26-.05.509-.141.738 1.215.911 2.405 1.855 3.6 2.794.424-.333.96-.532 1.541-.532 1.38 0 2.5 1.12 2.5 2.5s-1.12 2.5-2.5 2.5c-1.171 0-2.155-.807-2.426-1.895-1.201.098-2.404.173-3.606.254-.17.933-.987 1.641-1.968 1.641-1.104 0-2-.896-2-2 0-1.033.784-1.884 1.79-1.989.12-.731.252-1.46.383-2.19zm2.059-.246c-.296.232-.66.383-1.057.417l-.363 2.18c.504.224.898.651 1.079 1.177l3.648-.289c.047-.267.137-.519.262-.749l-3.569-2.736z""/></svg>";
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
                {"content_style","img { width: 100%; height: auto; }"},
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
		[Parameter] public Guid project { get; set; }
     void Cancel() => MudDialog.Cancel();
     
     [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;

    VulnModelFluentValidator vulnValidator = new VulnModelFluentValidator();
	 
    VulnCreateViewModel model = new VulnCreateViewModel();
    
    private List<VulnCategory> Categories = new List<VulnCategory>();
    private List<Cwe> Cwes = new List<Cwe>();
    private List<Project> Projects = new List<Project>();
    private List<Target> Targets = new List<Target>();
    private List<CORE.Entities.Vuln> VulnTemplates = new List<CORE.Entities.Vuln>();
    private List<VulnCustomFieldValueViewModel> CustomFields = new List<VulnCustomFieldValueViewModel>();

    private Guid template;
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
    [Inject] private VulnController VulnController { get; set; }
    [Inject] private VulnCustomFieldController VulnCustomFieldController { get; set; }
    [Inject] private ProjectController ProjectController { get; set; }
    [Inject] private TargetController TargetController { get; set; }
	[Inject] private IAiService _aiService { get; set; }
	private bool aiEnabled = false;

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

    protected override async Task OnInitializedAsync()
    {
        Categories = VulnController.GetCategories().ToList();
        Cwes = VulnController.GetCwes().ToList();
        Projects = ProjectController.Get().Where(x => x.Template == false).ToList();
        Targets = TargetController.GetTargets().ToList();
        model.ProjectId = Guid.Empty;
        model.VulnCategoryId = Guid.Empty;
        //model.CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:C/C:H/I:H/A:H";
        //model.CVSS3 = 10;
        model.OWASPVector = "(SL:1/M:1/O:0/S:2/ED:1/EE:1/A:1/ID:1/LC:2/LI:1/LAV:1/LAC:1/FD:1/RD:1/NC:2/PV:3)";
        model.OWASPImpact = String.Empty;
        model.OWASPLikehood = String.Empty;
        model.OWASPRisk = String.Empty;
        VulnTemplates = VulnController.GetTemplates().ToList();
        if (project != Guid.Empty)
        {
	        SelectedProject = project;
	        StateHasChanged();

        }
        aiEnabled = _aiService.IsEnabled();
        
        // Load custom fields
        await LoadCustomFields();
        
        await base.OnInitializedAsync();
	
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
            
	        var response = await VulnController.Add(model);
	        if (response.ToString() == "Microsoft.AspNetCore.Mvc.CreatedResult")
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
    

    private async Task<IEnumerable<string>> SearchCatFunc(string value)
    {
	    // In real life use an asynchronous function for fetching data from an api.
	    await Task.Delay(1);

	    // if text is null or empty, show complete list
	    if (string.IsNullOrEmpty(value))
		    return Categories.Select(x => x.Name);
	    return Categories.Select(x => x.Name).Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
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
	    
	    var cwesVuln = VulnController.GetVulnCwes(model.Id);
	    foreach (var cwe in cwesVuln)
	    {
		    SelectedCwes.Append(cwe.CweId);
	    }
	    
	    var vulnTargets = VulnController.GetVulnTargets(model.Id);
	    foreach (var tar in vulnTargets)
	    {
		    SelectedTargets.Append(tar.TargetId);
	    }

        StateHasChanged();
    }
    
    public async Task SearchCVE()
    {
	    
	    /*var response = await Http.GetAsync("https://services.nvd.nist.gov/rest/json/cves/2.0?cveId=" + model.cve);
	    if (response.IsSuccessStatusCode)
	    {
		    
	    }
	    else
	    {
		    Snackbar.Add(@localizer["searchCVELoadError"], Severity.Error);
	    }

	    StateHasChanged();*/
    }
    
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };

    private async Task OpenTargetCreate(DialogOptions options)
    {
	    var parameters = new DialogParameters { ["project"]=SelectedProject };

	    var dialog = DialogService.Show<CreateTargetDialog>("Custom Options Dialog",parameters, options);
	    // wait modal to close
	    var result = await dialog.Result;
	    if (!result.Canceled)
	    {
		    Targets = TargetController.GetTargets().ToList();
		    StateHasChanged();
	    }
        
    }
    
    private async Task OpenAiDialog(DialogOptions options)
    {
	    //var parameters = new DialogParameters { ["project"]=SelectedProject };

	    var dialog = DialogService.Show<AiDialog>("Custom Options Dialog", options);
	    // wait modal to close
	    var result = await dialog.Result;
	    if (!result.Canceled)
	    {
		    var data = await dialog.GetReturnValueAsync<VulnAiModel>();
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
    
    private Task LoadCustomFields()
    {
        try
        {
            var customFields = VulnCustomFieldController.GetActive();
            CustomFields = customFields.Select(cf => new VulnCustomFieldValueViewModel
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
                Value = cf.DefaultValue ?? string.Empty
            }).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading custom fields: {ex.Message}", Severity.Error);
        }
        
        return Task.CompletedTask;
    }
    
    private async Task OnCustomFieldChanged(VulnCustomFieldValueViewModel field)
    {
        // This method is called when a custom field value changes
        // We could add validation here if needed
        await Task.CompletedTask;
    }
}