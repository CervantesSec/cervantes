using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.IFR.CervantesAI;
using Cervantes.IFR.Export;
using Cervantes.Web.Components.Pages.Projects;
using Cervantes.Web.Components.Pages.Tasks;
using Cervantes.Web.Components.Pages.Vuln;
using Cervantes.Web.Components.Pages.Workspace.Target;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Microsoft.SemanticKernel;
using MudBlazor;
using MudBlazor.Extensions;
using Severity = MudBlazor.Severity;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Workspace.Project;

public partial class WorkspaceProject: ComponentBase
{
    [Parameter] public Guid project { get; set; }
    [CascadingParameter] bool _isDarkMode { get; set; }

    private bool editMode = false;
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    DialogOptions medium = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private ClientsController _clientsController { get; set; }
    [Inject] private ProjectController _projectController { get; set; }
    [Inject] private VulnController _VulnController { get; set; }
    [Inject] private TaskController _TaskController { get; set; }
    [Inject] private TargetController _TargetController { get; set; }
    [Inject] private ReportController _ReportController { get; set; }
    ExecutiveSummaryViewModel executive = new ExecutiveSummaryViewModel();


private List<CORE.Entities.Project> projects = new List<CORE.Entities.Project>();
    private List<CORE.Entities.Vuln> vulns = new List<CORE.Entities.Vuln>();
    public CORE.Entities.Project pro { get; set; } = new CORE.Entities.Project();
    [Inject] private IExportToCsv ExportToCsv { get; set; }


    MudForm form;
    private static IBrowserFile file;
    ProjectEditModelFluentValidator projectEditValidator = new ProjectEditModelFluentValidator();
    private ProjectEditViewModel model { get; set; } = new ProjectEditViewModel();
    private List<CORE.Entities.Client> Clients = new List<CORE.Entities.Client>();
    private DateTime? dateStart;
    private DateTime? dateEnd;
    private string searchStringMembers = "";
    private string searchStringTarget = "";
    private string searchStringTask = "";
    private string searchStringVuln = "";
    private string searchStringNote = "";
    private string searchStringAttachment = "";
    private string searchStringReport = "";
    private List<ProjectUser> Members = new List<ProjectUser>();
    private List<ProjectUser> seleMembers = new List<ProjectUser>();

    private List<CORE.Entities.Target> Targets = new List<CORE.Entities.Target>();
    private List<CORE.Entities.Target> seleTargets = new List<CORE.Entities.Target>();
    private List<ProjectNote> Notes = new List<ProjectNote>();
    private List<ProjectNote> seleNotes = new List<ProjectNote>();
    private List<ProjectAttachment> Attachments = new List<ProjectAttachment>();
    private List<ProjectAttachment> seleAttachments = new List<ProjectAttachment>();
    private List<CORE.Entities.Task> Tasks = new List<CORE.Entities.Task>();
    private List<CORE.Entities.Task> seleTasks = new List<CORE.Entities.Task>();

    private List<CORE.Entities.Vuln> Vulns = new List<CORE.Entities.Vuln>();
    private List<CORE.Entities.Vuln> seleVulns = new List<CORE.Entities.Vuln>();
    private List<CORE.Entities.Report> Reports = new List<CORE.Entities.Report>();
    private List<CORE.Entities.Report> seleReports = new List<CORE.Entities.Report>();

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
private List<BreadcrumbItem> _items;
[Inject] private IAiService _aiService { get; set; }
private bool aiEnabled = false;
private const string aiSVG = @"<svg width=""24"" height=""24"" xmlns=""http://www.w3.org/2000/svg"" fill-rule=""evenodd"" clip-rule=""evenodd""><path d=""M24 11.374c0 4.55-3.783 6.96-7.146 6.796-.151 1.448.061 2.642.384 3.641l-3.72 1.189c-.338-1.129-.993-3.822-2.752-5.279-2.728.802-4.969-.646-5.784-2.627-2.833.046-4.982-1.836-4.982-4.553 0-4.199 4.604-9.541 11.99-9.541 7.532 0 12.01 5.377 12.01 10.374zm-21.992-1.069c-.145 2.352 2.179 3.07 4.44 2.826.336 2.429 2.806 3.279 4.652 2.396 1.551.74 2.747 2.37 3.729 4.967l.002.006.111-.036c-.219-1.579-.09-3.324.36-4.528 3.907.686 6.849-1.153 6.69-4.828-.166-3.829-3.657-8.011-9.843-8.109-6.302-.041-9.957 4.255-10.141 7.306zm8.165-2.484c-.692-.314-1.173-1.012-1.173-1.821 0-1.104.896-2 2-2s2 .896 2 2c0 .26-.05.509-.141.738 1.215.911 2.405 1.855 3.6 2.794.424-.333.96-.532 1.541-.532 1.38 0 2.5 1.12 2.5 2.5s-1.12 2.5-2.5 2.5c-1.171 0-2.155-.807-2.426-1.895-1.201.098-2.404.173-3.606.254-.17.933-.987 1.641-1.968 1.641-1.104 0-2-.896-2-2 0-1.033.784-1.884 1.79-1.989.12-.731.252-1.46.383-2.19zm2.059-.246c-.296.232-.66.383-1.057.417l-.363 2.18c.504.224.898.651 1.079 1.177l3.648-.289c.047-.267.137-.519.262-.749l-3.569-2.736z""/></svg>";
private ClaimsPrincipal userAth;
    #region Dialog
protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        pro = _projectController.GetById(project);
        vulns = _VulnController.GetByProject(project).ToList();
        Clients = new List<CORE.Entities.Client>();
        Clients = _clientsController.Get().ToList();
        Members = _projectController.GetMembers(project).ToList();
        Tasks = _TaskController.GetByProject(project).ToList();
        Targets = _TargetController.GetByProjectId(project).ToList();
        Notes = _projectController.GetNotes(project).ToList();
        Attachments = _projectController.GetAttachments(project).ToList();
        Reports = _ReportController.GetByProject(project).ToList();
        executive = new ExecutiveSummaryViewModel
        {
            Project = project,
            ExecutiveSummary = pro.ExecutiveSummary
        };
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Workspace", href: "/workspaces",icon: Icons.Material.Filled.Workspaces),
            new BreadcrumbItem(pro.Name, href: "/workspace/"+project,icon: Icons.Material.Filled.Folder),
            new BreadcrumbItem(@localizer["project"], href: null, disabled: true, icon: Icons.Material.Filled.Folder)
        };
        aiEnabled = _aiService.IsEnabled();
        await base.OnInitializedAsync();
    }
    

    #endregion
    
    #region Project
    
    private async Task SubmitExecutive()
    {
        await form.Validate();

        if (form.IsValid)
        {

            executive.Project = project;
            var response = await _projectController.ExecutiveSumamry(executive);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
            {
                Snackbar.Add(@localizer["executiveSummarySaved"], Severity.Success);
            }
            else
            {
                Snackbar.Add(@localizer["executiveSummarySavedError"], Severity.Error);
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
            model.Id = project;
            model.Name = pro.Name;
            model.Description = pro.Description;
            model.ClientId = pro.ClientId;
            model.Template = pro.Template;
            model.Status = pro.Status;
            model.ProjectType = pro.ProjectType;
            model.Language = pro.Language;
            model.Score = pro.Score;
            model.FindingsId = pro.FindingsId;
            model.StartDate = pro.StartDate;
            model.EndDate = pro.EndDate;
            dateStart = model.StartDate;
            dateEnd = model.EndDate;
        }

        StateHasChanged();
    }


    private async Task Submit()
    {
        model.StartDate = dateStart.Value;
        model.EndDate = dateEnd.Value;
        await form.Validate();

        if (form.IsValid)
        {
            model.StartDate = dateStart.Value;
            model.EndDate = dateEnd.Value;
            var response = await _projectController.Edit(model);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
            {
                Snackbar.Add(@localizer["projectEdited"], Severity.Success);
            }
            else
            {
                Snackbar.Add(@localizer["projectEditedError"], Severity.Error);
            }
        }
    }

    public class ProjectEditModelFluentValidator : AbstractValidator<ProjectEditViewModel>
    {
        public ProjectEditModelFluentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1, 100);
            RuleFor(x => x.ClientId)
                .NotEmpty();
        }


        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result =
                await ValidateAsync(
                    ValidationContext<ProjectEditViewModel>.CreateWithOptions((ProjectEditViewModel)model,
                        x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }

    #endregion

    #region Members

    private Func<CORE.Entities.ProjectUser, bool> _quickFilterMember => x =>
    {
        if (string.IsNullOrWhiteSpace(searchStringMembers))
            return true;
        if (x.User.FullName.Contains(searchStringMembers, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.User.Email.ToString().Contains(searchStringMembers, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };

    private async Task OpenProjectMembersDialog(Guid project, DialogOptions options)
    {
        var parameters = new DialogParameters { ["project"] = project };
        var dialog = await Dialog.ShowEx<AddMembersProjectDialog>(@localizer["addMember"], parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Members = _projectController.GetMembers(project).ToList();
            StateHasChanged();
        }
    }

    async Task OpenProjectDeleteMembersDialog(DataGridRowClickEventArgs<ProjectUser> args)
    {
        var parameters = new DialogParameters { ["user"] = args.Item };

        var dialog = await Dialog.ShowEx<DeleteProjectMemberDialog>("Edit", parameters, maxWidth);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Members.Remove(args.Item);
            StateHasChanged();
        }
    }

    void SelectedMembersChanged(HashSet<ProjectUser> items)
    {
        seleMembers = items.ToList();
    }

    private async Task BtnActionsMembers(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["users"] = seleMembers };

                var dialog = await Dialog.ShowEx<DeleteProjectMemberBulkDialog>("Edit", parameters, maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    Members = _projectController.GetMembers(project).ToList();
                    StateHasChanged();
                }

                break;
        }
    }

    #endregion
    
    #region Tasks

    private async Task OpenDialogTaskCreate(Guid project, DialogOptions options)
    {
        var parameters = new DialogParameters { ["project"] = project };
        var dialog = await Dialog.ShowEx<CreateTaskDialog>("Custom Options Dialog", parameters, options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            Tasks = _TaskController.GetByProject(project).ToList();
            StateHasChanged();
        }
    }

    async Task RowClickedTask(DataGridRowClickEventArgs<CORE.Entities.Task> args)
    {
        var parameters = new DialogParameters { ["task"] = args.Item };

        var dialog = await Dialog.ShowEx<TaskDialog>(args.Item.Name, parameters, maxWidth);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Tasks = _TaskController.GetByProject(project).ToList();
            StateHasChanged();
        }
    }

    private Func<CORE.Entities.Task, bool> _quickFilterTasks => element =>
    {
        if (string.IsNullOrWhiteSpace(searchStringTask))
            return true;
        if (element.Name.Contains(searchStringTask, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.AsignedUser.FullName.Contains(searchStringTask, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Status.ToString().Contains(searchStringTask, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.StartDate.ToString().Contains(searchStringTask))
            return true;
        if (element.EndDate.ToString().Contains(searchStringTask))
            return true;
        return false;
    };

    void SelectedTasksChanged(HashSet<CORE.Entities.Task> items)
    {
        seleTasks = items.ToList();
    }

    private async Task BtnActionsTasks(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["tasks"] = seleTasks };

                var dialog = await Dialog.ShowEx<DeleteTaskBulkDialog>("Edit", parameters, maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    Tasks = _TaskController.GetByProject(project).ToList();
                    StateHasChanged();
                }

                break;
        }
    }

    #endregion

    #region Vulns

    private async Task ExportVulns(int id)
    {
        switch (id)
        {
            case 0:
                List<VulnExport> test = new List<VulnExport>();
                foreach (var e in seleVulns)
                {
                    VulnExport vuln = new VulnExport();
                    vuln.Name = e.Name ?? "No Name";
                    vuln.Description = e.Description ?? "No Description";
                    vuln.CreatedUser = e.User.FullName ?? "No User";
                    vuln.CreatedDate = e.CreatedDate.ToShortDateString() ?? "No Date";
                    vuln.ModifiedDate = e.ModifiedDate.ToShortDateString() ?? "No Date";
                    vuln.Template = e.Template;
                    vuln.Status = e.Status.ToString();
                    vuln.Language = e.Language.ToString();
                    vuln.cve = e.cve ?? "No CVE";
                    vuln.CVSS3 = e.CVSS3;
                    vuln.CVSSVector = e.CVSSVector ?? "No Vector";
                    vuln.Impact = e.Impact ?? "No Impact";
                    vuln.JiraCreated = e.JiraCreated;
                    vuln.ProofOfConcept = e.ProofOfConcept ?? "No Proof";
                    vuln.Remediation = e.Remediation ?? "No Remediation";
                    vuln.RemediationComplexity = e.RemediationComplexity.ToString() ?? "No Complexity";
                    vuln.RemediationPriority = e.RemediationPriority.ToString() ?? "No Priority";
                    vuln.Risk = e.Risk.ToString() ?? "No Risk";
                    vuln.OWASPImpact = e.OWASPImpact?.ToString() ?? "No Impact";
                    vuln.OWASPLikehood = e.OWASPLikehood?.ToString() ?? "No Likehood";
                    vuln.OWASPRisk = e.OWASPRisk?.ToString() ?? "No Risk";
                    vuln.OWASPVector = e.OWASPVector?.ToString() ?? "No Vector";
                    vuln.VulnCategory = e.VulnCategory?.Name ?? "No Category";
                    vuln.Project = e.Project?.Name ?? "No Project";
                    test.Add(vuln);
                }
                
                var file = ExportToCsv.ExportVulns(test);
                await JS.InvokeVoidAsync("downloadFile", file);
                Snackbar.Add(@localizer["exportSuccessfull"], Severity.Success);
                ExportToCsv.DeleteFile(file);
                break;
        }
    }

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

    private async Task OpenDialogVulnCreate(Guid project, DialogOptions options)
    {
        var parameters = new DialogParameters { ["project"] = project };
        var dialog = await Dialog.ShowEx<CreateVulnDialog>("Custom Options Dialog", parameters, options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            
            vulns = _VulnController.GetByProject(project).ToList();
            StateHasChanged();
        }
    }

    async Task RowClickedVuln(DataGridRowClickEventArgs<CORE.Entities.Vuln> args)
    {
        var parameters = new DialogParameters { ["vuln"] = args.Item };

        var dialog = await Dialog.ShowEx<VulnDialog>(args.Item.Name, parameters, maxWidth);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            vulns = _VulnController.GetByProject(project).ToList();
            StateHasChanged();
        }
    }

    void SelectedVulnsChanged(HashSet<CORE.Entities.Vuln> items)
    {
        seleVulns = items.ToList();
    }

    private async Task BtnActionsVulns(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["vulns"] = seleVulns };

                var dialog = await Dialog.ShowEx<DeleteVulnBulkDialog>("Edit", parameters, maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    Vulns = _VulnController.GetByProject(project).ToList();
                    StateHasChanged();
                }

                break;
        }
    }

    private async Task OpenDialogImport(DialogOptions options)
    {

        var dialog = await Dialog.ShowEx<ImportVulnDialog>("Custom Options Dialog", options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            vulns = _VulnController.GetByProject(project).ToList();            
            StateHasChanged();
        }
        
    }
    
    #endregion
    
    #region Targets
         async Task RowClickedTarget(DataGridRowClickEventArgs<CORE.Entities.Target> args)
    {
        var parameters = new DialogParameters { ["target"]=args.Item };

        var dialog =  await Dialog.ShowEx<TargetDialog>("Edit", parameters, maxWidth);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Targets = _TargetController.GetByProjectId(project).ToList();
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
    
    private Func<CORE.Entities.Target, bool> _quickFilterTarget => element =>
    {
        if (string.IsNullOrWhiteSpace(searchStringTarget))
            return true;
        if (element.Name.Contains(searchStringTarget, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Description.Contains(searchStringTarget, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Type.ToString().Contains(searchStringTarget, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.User.FullName.Contains(searchStringTarget, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
    
    private async Task OpenImportTargetDialog(DialogOptions options)
    {
        var parameters = new DialogParameters { ["project"]=project };

        var dialog = await Dialog.ShowEx<ImportDialog>("Custom Options Dialog", parameters, options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            Targets = _TargetController.GetByProjectId(project).ToList();
            StateHasChanged();
        }
        
    }
    private async Task OpenDialogTargetCreate(DialogOptions options)
    {
        var parameters = new DialogParameters { ["project"]=project };

        var dialog = await Dialog.ShowEx<CreateTargetDialog>("Custom Options Dialog", parameters, options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            Targets = _TargetController.GetByProjectId(project).ToList();
            StateHasChanged();
        }
        
    }
    
    private async Task BtnActionsTarget(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["targets"]=seleTargets };

                var dialog =  await Dialog.ShowEx<DeleteTargetBulkDialog>("Edit", parameters,maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    Targets = _TargetController.GetByProjectId(project).ToList();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedTargetChanged(HashSet<CORE.Entities.Target> items)
    {
        
        seleTargets = items.ToList();
    }
    #endregion

    #region Notes

     async Task RowClickedNote(DataGridRowClickEventArgs<CORE.Entities.ProjectNote> args)
    {
        var parameters = new DialogParameters { ["note"]=args.Item };

        var dialog =  await Dialog.ShowEx<ProjectNoteDialog>("Edit", parameters, maxWidth);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Notes = _projectController.GetNotes(project).ToList();
            StateHasChanged();
        }
    }
     
    
    private Func<CORE.Entities.ProjectNote, bool> _quickFilterNotes => element =>
    {
        if (string.IsNullOrWhiteSpace(searchStringNote))
            return true;
        if (element.Name.Contains(searchStringNote, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Description.Contains(searchStringNote, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.User.FullName.Contains(searchStringNote, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
    

    private async Task OpenDialogNotesCreate(DialogOptions options)
    {
        var parameters = new DialogParameters { ["project"]=project };

        var dialog = await Dialog.ShowEx<CreateProjectNoteDialog>("Custom Options Dialog", parameters, options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            Notes = _projectController.GetNotes(project).ToList();
            StateHasChanged();
        }
        
    }
    
    private async Task BtnActionsNotes(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["notes"]=seleNotes };

                var dialog =  await Dialog.ShowEx<DeleteProjectNoteBulkDialog>("Edit", parameters,maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    Notes = _projectController.GetNotes(project).ToList();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedNotesChanged(HashSet<CORE.Entities.ProjectNote> items)
    {
        
        seleNotes = items.ToList();
    }

    #endregion

    #region Attachments

      async Task RowClickedAttachment(DataGridRowClickEventArgs<CORE.Entities.ProjectAttachment> args)
    {
        var parameters = new DialogParameters { ["attachment"]=args.Item };

        var dialog =  await Dialog.ShowEx<ProjectAttachmentDialog>("Edit", parameters, maxWidth);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Attachments = _projectController.GetAttachments(project).ToList();
            StateHasChanged();
        }
    }
     
    
    private Func<CORE.Entities.ProjectAttachment, bool> _quickFilterAttachments => element =>
    {
        if (string.IsNullOrWhiteSpace(searchStringNote))
            return true;
        if (element.Name.Contains(searchStringNote, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.User.FullName.Contains(searchStringNote, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
    

    private async Task OpenDialogAttachmentCreate(DialogOptions options)
    {
        var parameters = new DialogParameters { ["project"]=project };

        var dialog = await Dialog.ShowEx<AddProjectAttachmentDialog>("Custom Options Dialog", parameters, options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            Attachments = _projectController.GetAttachments(project).ToList();
            StateHasChanged();
        }
        
    }
    
    private async Task BtnActionsAttachments(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["attachments"]=seleAttachments };

                var dialog =  await Dialog.ShowEx<DeleteProjectAttachmentBulk>("Edit", parameters,maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    Attachments = _projectController.GetAttachments(project).ToList();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedAttachmentsChanged(HashSet<CORE.Entities.ProjectAttachment> items)
    {
        
        seleAttachments = items.ToList();
    }

    #endregion

    #region Reports
 async Task RowClickedReports(DataGridRowClickEventArgs<CORE.Entities.Report> args)
    {
        var parameters = new DialogParameters { ["report"]=args.Item };

        var dialog =  await Dialog.ShowEx<ReportDialog>("Edit", parameters, maxWidth);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Reports = _ReportController.GetByProject(project).ToList();
            StateHasChanged();
        }
    }
     
    
    private Func<CORE.Entities.Report, bool> _quickFilterReports => element =>
    {
        if (string.IsNullOrWhiteSpace(searchStringReport))
            return true;
        if (element.Name.Contains(searchStringReport, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.User.FullName.Contains(searchStringReport, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Language.ToString().Contains(searchStringReport, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Version.Contains(searchStringReport, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Description.Contains(searchStringReport, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
    

    private async Task OpenDialogReportCreate(DialogOptions options)
    {
        var parameters = new DialogParameters { ["project"]=project };

        var dialog = await Dialog.ShowEx<CreateReportDialog>("Custom Options Dialog", parameters, options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            Reports = _ReportController.GetByProject(project).ToList();
            StateHasChanged();
        }
        
    }
    
    private async Task BtnActionsReports(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["reports"]=seleReports };

                var dialog =  await Dialog.ShowEx<DeleteReportBulkDialog>("Edit", parameters,maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    Reports = _ReportController.GetByProject(project).ToList();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedReportsChanged(HashSet<CORE.Entities.Report> items)
    {
        
        seleReports = items.ToList();
    }
    

    #endregion
    
    private async Task OpenAiDialog(DialogOptions options)
    {
        //var parameters = new DialogParameters { ["project"]=SelectedProject };

        var dialog = await Dialog.ShowEx<Cervantes.Web.Components.Shared.AiDialog>("Custom Options Dialog", options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var data = await dialog.GetReturnValueAsync<FunctionResult>();
            executive.ExecutiveSummary = executive.ExecutiveSummary + data;
            StateHasChanged();
        }
        
    }  
    
    private async Task OpenExecutiveAiDialog(DialogOptions options)
    {
        var parameters = new DialogParameters { ["project"]=pro };

        var dialog = await Dialog.ShowEx<ExecutiveAiDialog>("Custom Options Dialog",parameters, options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var data = await dialog.GetReturnValueAsync<string>();
            executive.ExecutiveSummary = executive.ExecutiveSummary + data;
            StateHasChanged();
        }
        
    }
}