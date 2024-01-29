using System.IO.Compression;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Task = Cervantes.CORE.Entities.Task;

namespace Cervantes.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class BackupController : ControllerBase
{
    private readonly ILogger<BackupController> _logger = null;
    private IClientManager clientManager = null;
    private IUserManager userManager = null;
    private Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager = null;
    private IProjectManager projectManager = null;
    private IProjectUserManager projectUserManager = null;
    private IProjectNoteManager projectNoteManager = null;
    private IProjectAttachmentManager projectAttachmentManager = null;
    private IDocumentManager documentManager = null;
    private INoteManager noteManager = null;
    private IOrganizationManager organizationManager = null;
    private IReportManager reportManager = null;
    private IReportTemplateManager reportTemplateManager = null;
    private ITargetManager targetManager = null;
    private ITargetServicesManager targetServicesManager = null;
    private ITaskManager taskManager = null;
    private ITaskNoteManager taskNoteManager = null;
    private ITaskAttachmentManager taskAttachmentManager = null;
    private ITaskTargetManager taskTargetManager = null;
    private IVaultManager vaultManager = null;
    private IVulnCategoryManager vulnCategoryManager = null;
    private IVulnManager vulnManager = null;
    private IVulnNoteManager vulnNoteManager = null;
    private IVulnAttachmentManager vulnAttachmentManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private IWSTGManager wstgManager = null;
    private IMASTGManager mastgManager = null;
    private IJiraManager jiraManager = null;
    private IJiraCommentManager jiraCommentManager = null;
    private readonly IWebHostEnvironment env;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private IFileCheck fileCheck;
    private IReportComponentsManager reportComponentsManager;
    private IReportsPartsManager reportsPartsManager;
    private IKnowledgeBaseManager knowledgeBaseManager;
    private IKnowledgeBaseCategoryManager knowledgeBaseCategoryManager;

    public BackupController(IUserManager userManager,
        Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> usrManager, IClientManager clientManager,
        IProjectManager projectManager,
        IProjectUserManager projectUserManager, IProjectNoteManager projectNoteManager,
        IDocumentManager documentManager,
        INoteManager noteManager, IOrganizationManager organizationManager, IReportManager reportManager,
        ITargetManager targetManager, ITargetServicesManager targetServicesManager, ITaskManager taskManager,
        ITaskTargetManager taskTargetManager,
        ITaskNoteManager taskNoteManager, ITaskAttachmentManager taskAttachmentManager, IVaultManager vaultManager,
        IVulnManager vulnManager, IVulnNoteManager vulnNoteManager, IVulnAttachmentManager vulnAttachmentManager,
        IVulnCategoryManager vulnCategoryManager,
        IProjectAttachmentManager projectAttachmentManager, IVulnTargetManager vulnTargetManager,
        ILogger<BackupController> logger, IWebHostEnvironment env, IReportTemplateManager reportTemplateManager,
        IWSTGManager wstgManager, IMASTGManager mastgManager, IJiraManager jiraManager,
        IJiraCommentManager jiraCommentManager, IHttpContextAccessor HttpContextAccessor,
        IFileCheck fileCheck, IReportComponentsManager reportComponentsManager,
        IReportsPartsManager reportsPartsManager, IKnowledgeBaseManager knowledgeBaseManager,IKnowledgeBaseCategoryManager knowledgeBaseCategoryManager)
    {
        this.userManager = userManager;
        _userManager = usrManager;
        this.clientManager = clientManager;
        this.projectManager = projectManager;
        this.projectUserManager = projectUserManager;
        this.projectNoteManager = projectNoteManager;
        this.projectAttachmentManager = projectAttachmentManager;
        this.documentManager = documentManager;
        this.noteManager = noteManager;
        this.organizationManager = organizationManager;
        this.reportManager = reportManager;
        this.reportTemplateManager = reportTemplateManager;
        this.targetManager = targetManager;
        this.targetServicesManager = targetServicesManager;
        this.taskManager = taskManager;
        this.taskNoteManager = taskNoteManager;
        this.taskAttachmentManager = taskAttachmentManager;
        this.taskTargetManager = taskTargetManager;
        this.vaultManager = vaultManager;
        this.vulnCategoryManager = vulnCategoryManager;
        this.vulnManager = vulnManager;
        this.vulnAttachmentManager = vulnAttachmentManager;
        this.vulnNoteManager = vulnNoteManager;
        this.vulnTargetManager = vulnTargetManager;
        _logger = logger;
        this.env = env;
        this.jiraManager = jiraManager;
        this.jiraCommentManager = jiraCommentManager;
        this.wstgManager = wstgManager;
        this.mastgManager = mastgManager;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        this.fileCheck = fileCheck;
        this.reportComponentsManager = reportComponentsManager;
        this.reportsPartsManager = reportsPartsManager;
        this.knowledgeBaseManager = knowledgeBaseManager;
        this.knowledgeBaseCategoryManager = knowledgeBaseCategoryManager;
    }

    [HttpGet]
    [Route("Data")]
    public FileContentResult BackupData()
    {
        try
        {
            BackupViewModel model = new BackupViewModel
            {
                Users = userManager.GetAll().ToList(),
                Clients = clientManager.GetAll().Select(X => new CORE.Entities.Client
                {
                    Id = X.Id,
                    Name = X.Name,
                    Description = X.Description,
                    ContactEmail = X.ContactEmail,
                    ContactPhone = X.ContactPhone,
                    ContactName = X.ContactName,
                    CreatedDate = X.CreatedDate,
                    Url = X.Url,
                    UserId = X.UserId,
                    ImagePath = X.ImagePath,
                }).ToList(),
                Projects = projectManager.GetAll().Select(x => new Project
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UserId = x.UserId,
                    Status = x.Status,
                    Language = x.Language,
                    ClientId = x.ClientId,
                    ProjectType = x.ProjectType,
                    Template = x.Template,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Score = x.Score,
                    ExecutiveSummary = x.ExecutiveSummary,
                    FindingsId = x.FindingsId
                }).ToList(),
                ProjectUsers = projectUserManager.GetAll().Select(x => new ProjectUser
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    ProjectId = x.ProjectId
                }).ToList(),
                ProjectAttachments = projectAttachmentManager.GetAll().Select(x => new ProjectAttachment
                {
                    Id = x.Id,
                    Name = x.Name,
                    FilePath = x.FilePath,
                    UserId = x.UserId,
                    ProjectId = x.ProjectId,
                }).ToList(),
                ProjectNotes = projectNoteManager.GetAll().Select(x => new ProjectNote
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UserId = x.UserId,
                    ProjectId = x.ProjectId,
                    Visibility = x.Visibility,
                }).ToList(),
                Documents = documentManager.GetAll().Select(x => new Document
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    FilePath = x.FilePath,
                    CreatedDate = x.CreatedDate,
                    Visibility = x.Visibility,
                    UserId = x.UserId
                }).ToList(),
                Notes = noteManager.GetAll().Select(x => new Note
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UserId = x.UserId,
                    CreatedDate = x.CreatedDate,
                }).ToList(),
                Organization = organizationManager.GetAll().First(),

                ReportComponents = reportComponentsManager.GetAll().Select(x => new ReportComponents()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Content = x.Content,
                    Language = x.Language,
                    ComponentType = x.ComponentType,
                    Created = x.Created,
                    Updated = x.Updated
                }).ToList(),

                ReportTemplates = reportTemplateManager.GetAll().Select(x => new ReportTemplate
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    CreatedDate = x.CreatedDate,
                    Name = x.Name,
                    Description = x.Description,
                    Language = x.Language,
                    ReportType = x.ReportType
                }).ToList(),
                ReportParts = reportsPartsManager.GetAll().Select(x => new ReportParts()
                {
                    Id = x.Id,
                    ComponentId = x.ComponentId,
                    TemplateId = x.TemplateId,
                    Order = x.Order,
                    PartType = x.PartType,
                }).ToList(),

                Reports = reportManager.GetAll().Select(x => new Report
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    ProjectId = x.ProjectId,
                    CreatedDate = x.CreatedDate,
                    Name = x.Name,
                    Description = x.Description,
                    Version = x.Version,
                    Language = x.Language,
                    HtmlCode = x.HtmlCode,
                    ReportType = x.ReportType
                }).ToList(),


                Targets = targetManager.GetAll().Select(x => new Target
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    ProjectId = x.ProjectId,
                    Name = x.Name,
                    Description = x.Description,
                    Type = x.Type
                }).ToList(),
                TargetServices = targetServicesManager.GetAll().Select(x => new TargetServices
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    TargetId = x.TargetId,
                    Name = x.Name,
                    Description = x.Description,
                    Port = x.Port,
                    Version = x.Version,
                    Note = x.Note
                }).ToList(),
                Tasks = taskManager.GetAll().Select(X => new CORE.Entities.Task
                {
                    Id = X.Id,
                    Template = X.Template,
                    CreatedUserId = X.CreatedUserId,
                    AsignedUserId = X.AsignedUserId,
                    ProjectId = X.ProjectId,
                    StartDate = X.StartDate,
                    EndDate = X.EndDate,
                    Name = X.Name,
                    Description = X.Description,
                    Status = X.Status
                }).ToList(),
                TaskAttachments = taskAttachmentManager.GetAll().Select(x => new TaskAttachment
                {
                    Id = x.Id,
                    Name = x.Name,
                    FilePath = x.FilePath,
                    UserId = x.UserId,
                    TaskId = x.TaskId
                }).ToList(),
                TaskNotes = taskNoteManager.GetAll().Select(x => new TaskNote
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UserId = x.UserId,
                    TaskId = x.TaskId,
                    Visibility = x.Visibility
                }).ToList(),
                TaskTargets = taskTargetManager.GetAll().Select(x => new TaskTargets
                {
                    Id = x.Id,
                    TargetId = x.TargetId,
                    TaskId = x.TaskId
                }),
                Vaults = vaultManager.GetAll().Select(x => new Vault
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    Name = x.Name,
                    Type = x.Type,
                    Description = x.Description,
                    Value = x.Value,
                    CreatedDate = x.CreatedDate,
                    UserId = x.UserId
                }).ToList(),
                VulnCategories = vulnCategoryManager.GetAll().ToList(),
                Vulns = vulnManager.GetAll().Select(x => new Vuln
                {
                    Id = x.Id,
                    Template = x.Template,
                    FindingId = x.FindingId,
                    Name = x.Name,
                    CreatedDate = x.CreatedDate,
                    UserId = x.UserId,
                    ProjectId = x.ProjectId,
                    VulnCategoryId = x.VulnCategoryId,
                    Risk = x.Risk,
                    Status = x.Status,
                    cve = x.cve,
                    Description = x.Description,
                    ProofOfConcept = x.ProofOfConcept,
                    Impact = x.Impact,
                    CVSS3 = x.CVSS3,
                    CVSSVector = x.CVSSVector,
                    Remediation = x.Remediation,
                    RemediationComplexity = x.RemediationComplexity,
                    RemediationPriority = x.RemediationPriority,
                    OWASPLikehood = x.OWASPLikehood,
                    OWASPImpact = x.OWASPImpact,
                    OWASPRisk = x.OWASPRisk,
                    OWASPVector = x.OWASPVector,
                    JiraCreated = x.JiraCreated
                }).ToList(),
                VulnAttachments = vulnAttachmentManager.GetAll().Select(x => new VulnAttachment
                {
                    Id = x.Id,
                    Name = x.Name,
                    FilePath = x.FilePath,
                    UserId = x.UserId,
                    VulnId = x.VulnId
                }).ToList(),
                VulnNotes = vulnNoteManager.GetAll().Select(x => new VulnNote
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UserId = x.UserId,
                    VulnId = x.VulnId
                }).ToList(),
                VulnTargets = vulnTargetManager.GetAll().Select(x => new VulnTargets
                {
                    Id = x.Id,
                    VulnId = x.VulnId,
                    TargetId = x.TargetId
                }),
                Jira = jiraManager.GetAll().Select(x => new Jira
                {
                    Id = x.Id,
                    VulnId = x.VulnId,
                    UserId = x.UserId,
                    CreatedDate = x.CreatedDate,
                    JiraIdentifier = x.JiraIdentifier,
                    JiraKey = x.JiraKey,
                    Name = x.Name,
                    Reporter = x.Reporter,
                    Assignee = x.Assignee,
                    JiraType = x.JiraType,
                    Label = x.Label,
                    Votes = x.Votes,
                    Interested = x.Interested,
                    JiraCreatedDate = x.JiraCreatedDate,
                    JiraUpdatedDate = x.JiraUpdatedDate,
                    JiraStatus = x.JiraStatus,
                    JiraComponent = x.JiraComponent,
                    Priority = x.Priority,
                    JiraProject = x.JiraProject,
                    Resolution = x.Resolution,
                    ResolutionDate = x.ResolutionDate,
                    DueDate = x.DueDate,
                    SecurityLevel = x.SecurityLevel
                }),
                JiraComments = jiraCommentManager.GetAll().Select(x => new JiraComments
                {
                    Id = x.Id,
                    JiraId = x.JiraId,
                    JiraIdComment = x.JiraIdComment,
                    Author = x.Author,
                    Body = x.Body,
                    GroupLevel = x.GroupLevel,
                    RoleLevel = x.RoleLevel,
                    CreatedDate = x.CreatedDate,
                    UpdateAuthor = x.UpdateAuthor,
                    UpdatedDate = x.UpdatedDate,
                }),
                Mastgs = mastgManager.GetAll().Select(mastg => new MASTG
                {
                    Id = mastg.Id,
                    TargetId = mastg.TargetId,
                    UserId = mastg.UserId,
                    ProjectId = mastg.ProjectId,
                    CreatedDate = mastg.CreatedDate,
                    Storage1Note = mastg.Storage1Note,
                    Storage1Status = mastg.Storage1Status,
                    Storage1Note1 = mastg.Storage1Note1,
                    Storage1Status1 = mastg.Storage1Status1,
                    Storage1Note2 = mastg.Storage1Note2,
                    Storage1Status2 = mastg.Storage1Status2,
                    Storage1Note3 = mastg.Storage1Note3,
                    Storage1Status3 = mastg.Storage1Status3,
                    Storage2Note = mastg.Storage2Note,
                    Storage2Status = mastg.Storage2Status,
                    Storage2Note1 = mastg.Storage2Note1,
                    Storage2Status1 = mastg.Storage2Status1,
                    Storage2Note2 = mastg.Storage2Note2,
                    Storage2Status2 = mastg.Storage2Status2,
                    Storage2Note3 = mastg.Storage2Note3,
                    Storage2Status3 = mastg.Storage2Status3,
                    Storage2Note4 = mastg.Storage2Note4,
                    Storage2Status4 = mastg.Storage2Status4,
                    Storage2Note5 = mastg.Storage2Note5,
                    Storage2Status5 = mastg.Storage2Status5,
                    Storage2Note6 = mastg.Storage2Note6,
                    Storage2Status6 = mastg.Storage2Status6,
                    Storage2Note7 = mastg.Storage2Note7,
                    Storage2Status7 = mastg.Storage2Status7,
                    Storage2Note8 = mastg.Storage2Note8,
                    Storage2Status8 = mastg.Storage2Status8,
                    Storage2Note9 = mastg.Storage2Note9,
                    Storage2Status9 = mastg.Storage2Status9,
                    Storage2Note10 = mastg.Storage2Note10,
                    Storage2Status10 = mastg.Storage2Status10,
                    Storage2Note11 = mastg.Storage2Note11,
                    Storage2Status11 = mastg.Storage2Status11,
                    Crypto1Note = mastg.Crypto1Note,
                    Crypto1Status = mastg.Crypto1Status,
                    Crypto1Note1 = mastg.Crypto1Note1,
                    Crypto1Status1 = mastg.Crypto1Status1,
                    Crypto1Note2 = mastg.Crypto1Note2,
                    Crypto1Status2 = mastg.Crypto1Status2,
                    Crypto1Note3 = mastg.Crypto1Note3,
                    Crypto1Status3 = mastg.Crypto1Status3,
                    Crypto1Note4 = mastg.Crypto1Note4,
                    Crypto1Status4 = mastg.Crypto1Status4,
                    Crypto1Note5 = mastg.Crypto1Note5,
                    Crypto1Status5 = mastg.Crypto1Status5,
                    Crypto2Note = mastg.Crypto2Note,
                    Crypto2Status = mastg.Crypto2Status,
                    Crypto2Note1 = mastg.Crypto2Note1,
                    Crypto2Status1 = mastg.Crypto2Status1,
                    Crypto2Note2 = mastg.Crypto2Note2,
                    Crypto2Status2 = mastg.Crypto2Status2,
                    Auth1Note = mastg.Auth1Note,
                    Auth1Status = mastg.Auth1Status,
                    Auth2Note = mastg.Auth2Note,
                    Auth2Status = mastg.Auth2Status,
                    Auth2Note1 = mastg.Auth2Note1,
                    Auth2Status1 = mastg.Auth2Status1,
                    Auth2Note2 = mastg.Auth2Note2,
                    Auth2Status2 = mastg.Auth2Status2,
                    Auth2Note3 = mastg.Auth2Note3,
                    Auth2Status3 = mastg.Auth2Status3,
                    Auth3Note = mastg.Auth3Note,
                    Auth3Status = mastg.Auth3Status,
                    Network1Note = mastg.Network1Note,
                    Network1Status = mastg.Network1Status,
                    Network1Note1 = mastg.Network1Note1,
                    Network1Status1 = mastg.Network1Status1,
                    Network1Note2 = mastg.Network1Note2,
                    Network1Status2 = mastg.Network1Status2,
                    Network1Note3 = mastg.Network1Note3,
                    Network1Status3 = mastg.Network1Status3,
                    Network1Note4 = mastg.Network1Note4,
                    Network1Status4 = mastg.Network1Status4,
                    Network1Note5 = mastg.Network1Note5,
                    Network1Status5 = mastg.Network1Status5,
                    Network1Note6 = mastg.Network1Note6,
                    Network1Status6 = mastg.Network1Status6,
                    Network1Note7 = mastg.Network1Note7,
                    Network1Status7 = mastg.Network1Status7,
                    Network2Note = mastg.Network2Note,
                    Network2Status = mastg.Network2Status,
                    Network2Note1 = mastg.Network2Note1,
                    Network2Status1 = mastg.Network2Status1,
                    Network2Note2 = mastg.Network2Note2,
                    Network2Status2 = mastg.Network2Status2,
                    Platform1Note = mastg.Platform1Note,
                    Platform1Status = mastg.Platform1Status,
                    Platform1Note1 = mastg.Platform1Note1,
                    Platform1Status1 = mastg.Platform1Status1,
                    Platform1Note2 = mastg.Platform1Note2,
                    Platform1Status2 = mastg.Platform1Status2,
                    Platform1Note3 = mastg.Platform1Note3,
                    Platform1Status3 = mastg.Platform1Status3,
                    Platform1Note4 = mastg.Platform1Note4,
                    Platform1Status4 = mastg.Platform1Status4,
                    Platform1Note5 = mastg.Platform1Note5,
                    Platform1Status5 = mastg.Platform1Status5,
                    Platform1Note6 = mastg.Platform1Note6,
                    Platform1Status6 = mastg.Platform1Status6,
                    Platform1Note7 = mastg.Platform1Note7,
                    Platform1Status7 = mastg.Platform1Status7,
                    Platform1Note8 = mastg.Platform1Note8,
                    Platform1Status8 = mastg.Platform1Status8,
                    Platform1Note9 = mastg.Platform1Note9,
                    Platform1Status9 = mastg.Platform1Status9,
                    Platform1Note10 = mastg.Platform1Note10,
                    Platform1Status10 = mastg.Platform1Status10,
                    Platform1Note11 = mastg.Platform1Note11,
                    Platform1Status11 = mastg.Platform1Status11,
                    Platform1Note12 = mastg.Platform1Note12,
                    Platform1Status12 = mastg.Platform1Status12,
                    Platform1Note13 = mastg.Platform1Note13,
                    Platform1Status13 = mastg.Platform1Status13,
                    Platform2Note = mastg.Platform2Note,
                    Platform2Status = mastg.Platform2Status,
                    Platform2Note1 = mastg.Platform2Note1,
                    Platform2Status1 = mastg.Platform2Status1,
                    Platform2Note2 = mastg.Platform2Note2,
                    Platform2Status2 = mastg.Platform2Status2,
                    Platform2Note3 = mastg.Platform2Note3,
                    Platform2Status3 = mastg.Platform2Status3,
                    Platform2Note4 = mastg.Platform2Note4,
                    Platform2Status4 = mastg.Platform2Status4,
                    Platform2Note5 = mastg.Platform2Note5,
                    Platform2Status5 = mastg.Platform2Status5,
                    Platform2Note6 = mastg.Platform2Note6,
                    Platform2Status6 = mastg.Platform2Status6,
                    Platform2Note7 = mastg.Platform2Note7,
                    Platform2Status7 = mastg.Platform2Status7,
                    Platform3Note = mastg.Platform3Note,
                    Platform3Status = mastg.Platform3Status,
                    Platform3Note1 = mastg.Platform3Note1,
                    Platform3Status1 = mastg.Platform3Status1,
                    Platform3Note2 = mastg.Platform3Note2,
                    Platform3Status2 = mastg.Platform3Status2,
                    Platform3Note3 = mastg.Platform3Note3,
                    Platform3Status3 = mastg.Platform3Status3,
                    Platform3Note4 = mastg.Platform3Note4,
                    Platform3Status4 = mastg.Platform3Status4,
                    Platform3Note5 = mastg.Platform3Note5,
                    Platform3Status5 = mastg.Platform3Status5,
                    Code1Note = mastg.Code1Note,
                    Code1Status = mastg.Code1Status,
                    Code2Note = mastg.Code2Note,
                    Code2Status = mastg.Code2Status,
                    Code2Note1 = mastg.Code2Note1,
                    Code2Status1 = mastg.Code2Status1,
                    Code2Note2 = mastg.Code2Note2,
                    Code2Status2 = mastg.Code2Status2,
                    Code3Note = mastg.Code3Note,
                    Code3Status = mastg.Code3Status,
                    Code3Note1 = mastg.Code3Note1,
                    Code3Status1 = mastg.Code3Status1,
                    Code3Note2 = mastg.Code3Note2,
                    Code3Status2 = mastg.Code3Status2,
                    Code4Note = mastg.Code4Note,
                    Code4Status = mastg.Code4Status,
                    Code4Note1 = mastg.Code4Note1,
                    Code4Status1 = mastg.Code4Status1,
                    Code4Note2 = mastg.Code4Note2,
                    Code4Status2 = mastg.Code4Status2,
                    Code4Note3 = mastg.Code4Note3,
                    Code4Status3 = mastg.Code4Status3,
                    Code4Note4 = mastg.Code4Note4,
                    Code4Status4 = mastg.Code4Status4,
                    Code4Note5 = mastg.Code4Note5,
                    Code4Status5 = mastg.Code4Status5,
                    Code4Note6 = mastg.Code4Note6,
                    Code4Status6 = mastg.Code4Status6,
                    Code4Note7 = mastg.Code4Note7,
                    Code4Status7 = mastg.Code4Status7,
                    Code4Note8 = mastg.Code4Note8,
                    Code4Status8 = mastg.Code4Status8,
                    Code4Note9 = mastg.Code4Note9,
                    Code4Status9 = mastg.Code4Status9,
                    Code4Note10 = mastg.Code4Note10,
                    Code4Status10 = mastg.Code4Status10,
                    Resilience1Note = mastg.Resilience1Note,
                    Resilience1Status = mastg.Resilience1Status,
                    Resilience1Note1 = mastg.Resilience1Note1,
                    Resilience1Status1 = mastg.Resilience1Status1,
                    Resilience1Note2 = mastg.Resilience1Note2,
                    Resilience1Status2 = mastg.Resilience1Status2,
                    Resilience1Note3 = mastg.Resilience1Note3,
                    Resilience1Status3 = mastg.Resilience1Status3,
                    Resilience1Note4 = mastg.Resilience1Note4,
                    Resilience1Status4 = mastg.Resilience1Status4,
                    Resilience2Status = mastg.Resilience2Status,
                    Resilience2Note = mastg.Resilience2Note,
                    Resilience2Status1 = mastg.Resilience2Status1,
                    Resilience2Note1 = mastg.Resilience2Note1,
                    Resilience2Status2 = mastg.Resilience2Status2,
                    Resilience2Note2 = mastg.Resilience2Note2,
                    Resilience2Status3 = mastg.Resilience2Status3,
                    Resilience2Note3 = mastg.Resilience2Note3,
                    Resilience2Status4 = mastg.Resilience2Status4,
                    Resilience2Note4 = mastg.Resilience2Note4,
                    Resilience2Status5 = mastg.Resilience2Status5,
                    Resilience2Note5 = mastg.Resilience2Note5,
                    Resilience3Note = mastg.Resilience3Note,
                    Resilience3Status = mastg.Resilience3Status,
                    Resilience3Note1 = mastg.Resilience3Note1,
                    Resilience3Status1 = mastg.Resilience3Status1,
                    Resilience3Note2 = mastg.Resilience3Note2,
                    Resilience3Status2 = mastg.Resilience3Status2,
                    Resilience3Note3 = mastg.Resilience3Note3,
                    Resilience3Status3 = mastg.Resilience3Status3,
                    Resilience3Note4 = mastg.Resilience3Note4,
                    Resilience3Status4 = mastg.Resilience3Status4,
                    Resilience3Note5 = mastg.Resilience3Note5,
                    Resilience3Status5 = mastg.Resilience3Status5,
                    Resilience3Note6 = mastg.Resilience3Note6,
                    Resilience3Status6 = mastg.Resilience3Status6,
                    Resilience4Note = mastg.Resilience4Note,
                    Resilience4Status = mastg.Resilience4Status,
                    Resilience4Status1 = mastg.Resilience4Status1,
                    Resilience4Note1 = mastg.Resilience4Note1,
                    Resilience4Status2 = mastg.Resilience4Status2,
                    Resilience4Note2 = mastg.Resilience4Note2,
                    Resilience4Status3 = mastg.Resilience4Status3,
                    Resilience4Note3 = mastg.Resilience4Note3,
                    Resilience4Status4 = mastg.Resilience4Status4,
                    Resilience4Note4 = mastg.Resilience4Note4,
                    Resilience4Status5 = mastg.Resilience4Status5,
                    Resilience4Note5 = mastg.Resilience4Note5,
                    Resilience4Status6 = mastg.Resilience4Status6,
                    Resilience4Note6 = mastg.Resilience4Note6,
                }),
                Wstgs = wstgManager.GetAll().Select(x => new WSTG
                {
                    Id = x.Id,
                    TargetId = x.TargetId,
                    UserId = x.UserId,
                    ProjectId = x.ProjectId,
                    CreatedDate = x.CreatedDate,
                    Info01Note = x.Info01Note,
                    Info02Note = x.Info02Note,
                    Info03Note = x.Info03Note,
                    Info04Note = x.Info04Note,
                    Info05Note = x.Info05Note,
                    Info06Note = x.Info06Note,
                    Info07Note = x.Info07Note,
                    Info08Note = x.Info08Note,
                    Info09Note = x.Info09Note,
                    Info10Note = x.Info10Note,
                    Conf01Note = x.Conf01Note,
                    Conf02Note = x.Conf02Note,
                    Conf03Note = x.Conf03Note,
                    Conf04Note = x.Conf04Note,
                    Conf05Note = x.Conf05Note,
                    Conf06Note = x.Conf06Note,
                    Conf07Note = x.Conf07Note,
                    Conf08Note = x.Conf08Note,
                    Conf09Note = x.Conf09Note,
                    Conf10Note = x.Conf10Note,
                    Conf11Note = x.Conf11Note,
                    Idnt1Note = x.Idnt1Note,
                    Idnt2Note = x.Idnt2Note,
                    Idnt3Note = x.Idnt3Note,
                    Idnt4Note = x.Idnt4Note,
                    Idnt5Note = x.Idnt5Note,
                    Athn01Note = x.Athn01Note,
                    Athn02Note = x.Athn02Note,
                    Athn03Note = x.Athn03Note,
                    Athn04Note = x.Athn04Note,
                    Athn05Note = x.Athn05Note,
                    Athn06Note = x.Athn06Note,
                    Athn07Note = x.Athn07Note,
                    Athn08Note = x.Athn08Note,
                    Athn09Note = x.Athn09Note,
                    Athn10Note = x.Athn10Note,
                    Athz01Note = x.Athz01Note,
                    Athz02Note = x.Athz02Note,
                    Athz03Note = x.Athz03Note,
                    Athz04Note = x.Athz04Note,
                    Sess01Note = x.Sess01Note,
                    Sess02Note = x.Sess02Note,
                    Sess03Note = x.Sess03Note,
                    Sess04Note = x.Sess04Note,
                    Sess05Note = x.Sess05Note,
                    Sess06Note = x.Sess06Note,
                    Sess07Note = x.Sess07Note,
                    Sess08Note = x.Sess08Note,
                    Sess09Note = x.Sess09Note,
                    Inpv01Note = x.Inpv01Note,
                    Inpv02Note = x.Inpv02Note,
                    Inpv03Note = x.Inpv03Note,
                    Inpv04Note = x.Inpv04Note,
                    Inpv05Note = x.Inpv05Note,
                    Inpv06Note = x.Inpv06Note,
                    Inpv07Note = x.Inpv07Note,
                    Inpv08Note = x.Inpv08Note,
                    Inpv09Note = x.Inpv09Note,
                    Inpv10Note = x.Inpv10Note,
                    Inpv11Note = x.Inpv11Note,
                    Inpv12Note = x.Inpv12Note,
                    Inpv13Note = x.Inpv13Note,
                    Inpv14Note = x.Inpv14Note,
                    Inpv15Note = x.Inpv15Note,
                    Inpv16Note = x.Inpv16Note,
                    Inpv17Note = x.Inpv17Note,
                    Inpv18Note = x.Inpv18Note,
                    Inpv19Note = x.Inpv19Note,
                    Errh01Note = x.Errh01Note,
                    Errh02Note = x.Errh02Note,
                    Cryp01Note = x.Cryp01Note,
                    Cryp02Note = x.Cryp02Note,
                    Cryp03Note = x.Cryp03Note,
                    Cryp04Note = x.Cryp04Note,
                    Busl01Note = x.Busl01Note,
                    Busl02Note = x.Busl02Note,
                    Busl03Note = x.Busl03Note,
                    Busl04Note = x.Busl04Note,
                    Busl05Note = x.Busl05Note,
                    Busl06Note = x.Busl06Note,
                    Busl07Note = x.Busl07Note,
                    Busl08Note = x.Busl08Note,
                    Busl09Note = x.Busl09Note,
                    Clnt01Note = x.Clnt01Note,
                    Clnt02Note = x.Clnt02Note,
                    Clnt03Note = x.Clnt03Note,
                    Clnt04Note = x.Clnt04Note,
                    Clnt05Note = x.Clnt05Note,
                    Clnt06Note = x.Clnt06Note,
                    Clnt07Note = x.Clnt07Note,
                    Clnt08Note = x.Clnt08Note,
                    Clnt09Note = x.Clnt09Note,
                    Clnt10Note = x.Clnt10Note,
                    Clnt11Note = x.Clnt11Note,
                    Clnt12Note = x.Clnt12Note,
                    Clnt13Note = x.Clnt13Note,
                    Apit01Note = x.Apit01Note,
                    Info01Status = x.Info01Status,
                    Info02Status = x.Info02Status,
                    Info03Status = x.Info03Status,
                    Info04Status = x.Info04Status,
                    Info05Status = x.Info05Status,
                    Info06Status = x.Info06Status,
                    Info07Status = x.Info07Status,
                    Info08Status = x.Info08Status,
                    Info09Status = x.Info09Status,
                    Info10Status = x.Info10Status,
                    Conf01Status = x.Conf01Status,
                    Conf02Status = x.Conf02Status,
                    Conf03Status = x.Conf03Status,
                    Conf04Status = x.Conf04Status,
                    Conf05Status = x.Conf05Status,
                    Conf06Status = x.Conf06Status,
                    Conf07Status = x.Conf07Status,
                    Conf08Status = x.Conf08Status,
                    Conf09Status = x.Conf09Status,
                    Conf10Status = x.Conf10Status,
                    Conf11Status = x.Conf11Status,
                    Idnt1Status = x.Idnt1Status,
                    Idnt2Status = x.Idnt2Status,
                    Idnt3Status = x.Idnt3Status,
                    Idnt4Status = x.Idnt4Status,
                    Idnt5Status = x.Idnt5Status,
                    Athn01Status = x.Athn01Status,
                    Athn02Status = x.Athn02Status,
                    Athn03Status = x.Athn03Status,
                    Athn04Status = x.Athn04Status,
                    Athn05Status = x.Athn05Status,
                    Athn06Status = x.Athn06Status,
                    Athn07Status = x.Athn07Status,
                    Athn08Status = x.Athn08Status,
                    Athn09Status = x.Athn09Status,
                    Athn10Status = x.Athn10Status,
                    Athz01Status = x.Athz01Status,
                    Athz02Status = x.Athz02Status,
                    Athz03Status = x.Athz03Status,
                    Athz04Status = x.Athz04Status,
                    Sess01Status = x.Sess01Status,
                    Sess02Status = x.Sess02Status,
                    Sess03Status = x.Sess03Status,
                    Sess04Status = x.Sess04Status,
                    Sess05Status = x.Sess05Status,
                    Sess06Status = x.Sess06Status,
                    Sess07Status = x.Sess07Status,
                    Sess08Status = x.Sess08Status,
                    Sess09Status = x.Sess09Status,
                    Inpv01Status = x.Inpv01Status,
                    Inpv02Status = x.Inpv02Status,
                    Inpv03Status = x.Inpv03Status,
                    Inpv04Status = x.Inpv04Status,
                    Inpv05Status = x.Inpv05Status,
                    Inpv06Status = x.Inpv06Status,
                    Inpv07Status = x.Inpv07Status,
                    Inpv08Status = x.Inpv08Status,
                    Inpv09Status = x.Inpv09Status,
                    Inpv10Status = x.Inpv10Status,
                    Inpv11Status = x.Inpv11Status,
                    Inpv12Status = x.Inpv12Status,
                    Inpv13Status = x.Inpv13Status,
                    Inpv14Status = x.Inpv14Status,
                    Inpv15Status = x.Inpv15Status,
                    Inpv16Status = x.Inpv16Status,
                    Inpv17Status = x.Inpv17Status,
                    Inpv18Status = x.Inpv18Status,
                    Inpv19Status = x.Inpv19Status,
                    Errh01Status = x.Errh01Status,
                    Errh02Status = x.Errh02Status,
                    Cryp01Status = x.Cryp01Status,
                    Cryp02Status = x.Cryp02Status,
                    Cryp03Status = x.Cryp03Status,
                    Cryp04Status = x.Cryp04Status,
                    Busl01Status = x.Busl01Status,
                    Busl02Status = x.Busl02Status,
                    Busl03Status = x.Busl03Status,
                    Busl04Status = x.Busl04Status,
                    Busl05Status = x.Busl05Status,
                    Busl06Status = x.Busl06Status,
                    Busl07Status = x.Busl07Status,
                    Busl08Status = x.Busl08Status,
                    Busl09Status = x.Busl09Status,
                    Clnt01Status = x.Clnt01Status,
                    Clnt02Status = x.Clnt02Status,
                    Clnt03Status = x.Clnt03Status,
                    Clnt04Status = x.Clnt04Status,
                    Clnt05Status = x.Clnt05Status,
                    Clnt06Status = x.Clnt06Status,
                    Clnt07Status = x.Clnt07Status,
                    Clnt08Status = x.Clnt08Status,
                    Clnt09Status = x.Clnt09Status,
                    Clnt10Status = x.Clnt10Status,
                    Clnt11Status = x.Clnt11Status,
                    Clnt12Status = x.Clnt12Status,
                    Clnt13Status = x.Clnt13Status,
                    Apit01Status = x.Apit01Status
                }),
                KnowledgeBaseCategories = knowledgeBaseCategoryManager.GetAll().Select(x => new KnowledgeBaseCategories()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UserId = x.UserId,
                    Icon = x.Icon,
                    Order = x.Order
                    
                }),
                KnowledgeBase = knowledgeBaseManager.GetAll().Select(x => new KnowledgeBase()
                {
                    Id = x.Id,
                    Order = x.Order,
                    Title = x.Title,
                    Content = x.Content,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    CreatedUserId = x.CreatedUserId,
                    UpdatedUserId = x.UpdatedUserId,
                    CategoryId = x.CategoryId,
                }),
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(model, options);

            string fileName = "BackupData_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";

            var fileBytes = Encoding.ASCII.GetBytes(jsonString);
            var mimeType = "application/json";
            _logger.LogInformation("Data backup made successfully. User: {0}",
                aspNetUserId);
            return new FileContentResult(fileBytes, mimeType)
            {
                FileDownloadName = fileName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred making backup data. User: {0}",
                aspNetUserId);
            return null;
        }
    }

    [HttpGet]
    [Route("Attachments")]
    public FileContentResult BackupAttachments()
    {
        try
        {
            string startPath = env.WebRootPath + "/" + "Attachments";
            string zipPath = env.WebRootPath + "/" + @"result" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".zip";

            ZipFile.CreateFromDirectory(startPath, zipPath);

            /*const string contentType ="application/zip";
            HttpContext.Response.ContentType = "application/zip";*/
            var result = new FileContentResult(System.IO.File.ReadAllBytes(zipPath), "application/zip")
            {
                FileDownloadName = "Attachments.zip"
            };

            _logger.LogInformation("Attachments backup made successfully. User: {0}",
                aspNetUserId);
            System.IO.File.Delete(zipPath);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred making attachments backup Index. User: {0}",
                aspNetUserId);
            return null;
        }
    }

    [HttpPost]
    [Route("Attachments")]
    public async Task<IActionResult> RestoreAttachments(BackupFormViewModel model)
    {
        try
        {
            string extractPath = env.WebRootPath + "/Attachments/";
            var path = "";
            var unique = "";
            if (model.FileContent != null)
            {
                if (fileCheck.CheckFile(model.FileContent))
                {
                    unique = Guid.NewGuid().ToString() + model.FileName;
                    path = $"{env.WebRootPath}/Attachments/Temp/{unique}";
                    var fs = System.IO.File.Create(path);
                    fs.Write(model.FileContent, 0,
                        model.FileContent.Length);
                    fs.Close();
                }
                else
                {
                    _logger.LogError("An error ocurred adding a client. User: {0}",
                        aspNetUserId);
                    return BadRequest("Invalid file type");
                }
            }


            ZipFile.ExtractToDirectory(path, extractPath, true);

            System.IO.File.Delete(Path.Combine(path));
            _logger.LogInformation("Attachments restored successfully. User: {0}",
                aspNetUserId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred restoring attachments. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("Data")]
    public Task<IActionResult> RestoreData(BackupFormViewModel model)
    {
        try
        {
            _logger.LogInformation("RestoreData method started.");

            var path = "";
            var unique = "";
            if (model.FileContent != null)
            {
                _logger.LogInformation("RestoreData contest if");

                if (fileCheck.CheckFile(model.FileContent))
                {
                    unique = Guid.NewGuid().ToString() + model.FileName;
                    path = $"{env.WebRootPath}/Attachments/Temp/{unique}";
                    var fs = System.IO.File.Create(path);
                    fs.Write(model.FileContent, 0,
                        model.FileContent.Length);
                    fs.Close();
                    _logger.LogInformation("RestoreData content check");
                }
                else
                {
                    _logger.LogError("An error ocurred adding a client. User: {0}",
                        aspNetUserId);
                    return System.Threading.Tasks.Task.FromResult<IActionResult>(BadRequest("Invalid file type"));
                }
            }

            using FileStream openStream = System.IO.File.OpenRead(path);
            BackupViewModel backupViewModel = JsonSerializer.Deserialize<BackupViewModel>(openStream);

            if (backupViewModel.Users != null)
            {
                foreach (var user in backupViewModel.Users)
                {
                    if (user.Email == "admin@cervantes.local")
                    {
                        /*user.Email = "adminbackup@cervantes.local";
                        user.UserName = "adminbackup@cervantes.local";
                        user.NormalizedEmail = "ADMINBACKUP@CERVANTES.LOCAL";
                        user.NormalizedUserName = "ADMINBACKUP@CERVANTES.LOCAL";
                        userManager.Add(user);
                        userManager.Context.SaveChanges();
                        _userManager.AddToRoleAsync(user, "Admin").Wait();*/
                        continue;
                    }

                    userManager.Add(user);
                    userManager.Context.SaveChanges();
                    if (user.ClientId != Guid.Empty)
                    {
                        _userManager.AddToRoleAsync(user, "Client").Wait();
                        continue;
                    }

                    _userManager.AddToRoleAsync(user, "User").Wait();
                }
            }

            if (backupViewModel.Clients != null)
            {
                foreach (var client in backupViewModel.Clients)
                {
                    client.UserId = aspNetUserId;
                    clientManager.Add(client);
                }

                clientManager.Context.SaveChanges();
            }

            if (backupViewModel.Projects != null)
            {
                foreach (var project in backupViewModel.Projects)
                {
                    project.UserId = aspNetUserId;
                    projectManager.Add(project);
                }

                projectManager.Context.SaveChanges();
            }

            if (backupViewModel.ProjectAttachments != null)
            {
                foreach (var attachment in backupViewModel.ProjectAttachments)
                {
                    attachment.UserId = aspNetUserId;
                    projectAttachmentManager.Add(attachment);
                }

                projectAttachmentManager.Context.SaveChanges();
            }

            if (backupViewModel.ProjectNotes != null)
            {
                foreach (var note in backupViewModel.ProjectNotes)
                {
                    note.UserId = aspNetUserId;
                    projectNoteManager.Add(note);
                }

                projectNoteManager.Context.SaveChanges();
            }

            /*if (backupViewModel.ProjectUsers != null)
            {
                foreach (var user in backupViewModel.ProjectUsers)
                {
                    projectUserManager.Add(user);
                }
                projectUserManager.Context.SaveChanges();

            }*/

            if (backupViewModel.Documents != null)
            {
                foreach (var doc in backupViewModel.Documents)
                {
                    doc.UserId = aspNetUserId;
                    documentManager.Add(doc);
                }

                documentManager.Context.SaveChanges();
            }

            if (backupViewModel.Notes != null)
            {
                foreach (var note in backupViewModel.Notes)
                {
                    note.UserId = aspNetUserId;
                    noteManager.Add(note);
                }

                noteManager.Context.SaveChanges();
            }

            if (backupViewModel.Organization != null)
            {
                var org = organizationManager.GetAll().First();
                org.Name = backupViewModel.Organization.Name;
                org.Description = backupViewModel.Organization.Description;
                org.Url = backupViewModel.Organization.Url;
                org.ContactEmail = backupViewModel.Organization.ContactEmail;
                org.ContactName = backupViewModel.Organization.ContactName;
                org.ContactPhone = backupViewModel.Organization.ContactPhone;
                org.ImagePath = backupViewModel.Organization.ImagePath;

                organizationManager.Context.SaveChanges();
            }

            if (backupViewModel.Reports != null)
            {
                foreach (var rep in backupViewModel.Reports)
                {
                    rep.UserId = aspNetUserId;
                    reportManager.Add(rep);
                }

                reportManager.Context.SaveChanges();
            }

            if (backupViewModel.Targets != null)
            {
                foreach (var target in backupViewModel.Targets)
                {
                    target.UserId = aspNetUserId;
                    targetManager.Add(target);
                }

                targetManager.Context.SaveChanges();
            }

            if (backupViewModel.TargetServices != null)
            {
                foreach (var services in backupViewModel.TargetServices)
                {
                    services.UserId = aspNetUserId;
                    targetServicesManager.Add(services);
                }

                targetServicesManager.Context.SaveChanges();
            }

            if (backupViewModel.Tasks != null)
            {
                foreach (var task in backupViewModel.Tasks)
                {
                    task.CreatedUserId = aspNetUserId;
                    if (task.AsignedUser.UserName == "admin@cervantes.local")
                    {
                        task.AsignedUserId = aspNetUserId;
                    }

                    taskManager.Add(task);
                }

                taskManager.Context.SaveChanges();
            }

            if (backupViewModel.TaskAttachments != null)
            {
                foreach (var att in backupViewModel.TaskAttachments)
                {
                    att.UserId = aspNetUserId;
                    taskAttachmentManager.Add(att);
                }

                taskAttachmentManager.Context.SaveChanges();
            }

            if (backupViewModel.TaskNotes != null)
            {
                foreach (var note in backupViewModel.TaskNotes)
                {
                    note.UserId = aspNetUserId;
                    taskNoteManager.Add(note);
                }

                taskNoteManager.Context.SaveChanges();
            }

            if (backupViewModel.TaskTargets != null)
            {
                foreach (var tar in backupViewModel.TaskTargets)
                {
                    taskTargetManager.Add(tar);
                }

                taskTargetManager.Context.SaveChanges();
            }

            if (backupViewModel.Vaults != null)
            {
                foreach (var vault in backupViewModel.Vaults)
                {
                    vault.UserId = aspNetUserId;
                    vaultManager.Add(vault);
                }

                vaultManager.Context.SaveChanges();
            }

            if (backupViewModel.VulnCategories != null)
            {
                foreach (var cat in backupViewModel.VulnCategories)
                {
                    var result = vulnCategoryManager.GetById(cat.Id);
                    if (result == null)
                    {
                        vulnCategoryManager.Add(cat);
                    }

                    vulnCategoryManager.Context.SaveChanges();
                }
            }

            if (backupViewModel.Vulns != null)
            {
                foreach (var vulns in backupViewModel.Vulns)
                {
                    vulns.UserId = aspNetUserId;
                    vulnManager.Add(vulns);
                }

                vulnManager.Context.SaveChanges();
            }

            if (backupViewModel.VulnAttachments != null)
            {
                foreach (var att in backupViewModel.VulnAttachments)
                {
                    att.UserId = aspNetUserId;
                    vulnAttachmentManager.Add(att);
                }

                vulnAttachmentManager.Context.SaveChanges();
            }

            if (backupViewModel.VulnNotes != null)
            {
                foreach (var note in backupViewModel.VulnNotes)
                {
                    note.UserId = aspNetUserId;
                    vulnNoteManager.Add(note);
                }

                vulnNoteManager.Context.SaveChanges();
            }

            if (backupViewModel.VulnTargets != null)
            {
                foreach (var vuln in backupViewModel.VulnTargets)
                {
                    vulnTargetManager.Add(vuln);
                }

                vulnTargetManager.Context.SaveChanges();
            }

            if (backupViewModel.Jira != null)
            {
                foreach (var jira in backupViewModel.Jira)
                {
                    jira.UserId = aspNetUserId;
                    jiraManager.Add(jira);
                }

                jiraManager.Context.SaveChanges();
            }

            if (backupViewModel.JiraComments != null)
            {
                foreach (var com in backupViewModel.JiraComments)
                {
                    jiraCommentManager.Add(com);
                }

                jiraCommentManager.Context.SaveChanges();
            }

            if (backupViewModel.Wstgs != null)
            {
                foreach (var wstg in backupViewModel.Wstgs)
                {
                    wstg.UserId = aspNetUserId;
                    wstgManager.Add(wstg);
                }

                wstgManager.Context.SaveChanges();
            }

            if (backupViewModel.Mastgs != null)
            {
                foreach (var mastg in backupViewModel.Mastgs)
                {
                    mastg.UserId = aspNetUserId;
                    mastgManager.Add(mastg);
                }

                mastgManager.Context.SaveChanges();
            }

            System.IO.File.Delete(path);
            _logger.LogInformation("Data restored successfully. User: {0}",
                aspNetUserId);
            return System.Threading.Tasks.Task.FromResult<IActionResult>(Ok());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred restoring data. User: {0}",
                aspNetUserId);
            return System.Threading.Tasks.Task.FromResult<IActionResult>(BadRequest());
        }
    }
}