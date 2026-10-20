using System.IO.Compression;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AuthPermissions.AdminCode;
using AuthPermissions.AspNetCore;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.File;
using Cervantes.Web.Helpers;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Task = Cervantes.CORE.Entities.Task;

namespace Cervantes.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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
    private readonly StoredHtmlResanitizer resanitizer;
    private readonly IAuthUsersAdminService authUsersAdminService;
    private readonly Sanitizer sanitizer;

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
        IReportsPartsManager reportsPartsManager, IKnowledgeBaseManager knowledgeBaseManager,
        IKnowledgeBaseCategoryManager knowledgeBaseCategoryManager, StoredHtmlResanitizer resanitizer,
        IAuthUsersAdminService authUsersAdminService, Sanitizer sanitizer)
    {
        this.resanitizer = resanitizer;
        this.authUsersAdminService = authUsersAdminService;
        this.sanitizer = sanitizer;
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
    [HasPermission(Permissions.BackupRead)]
    public FileContentResult BackupData()
    {
        try
        {
            var db = userManager.Context;
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
                    FindingsId = x.FindingsId,
                    BusinessImpact = x.BusinessImpact
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
                    ContentCss = x.ContentCss,
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
                    Status = X.Status,
                    ClosedDate = X.ClosedDate
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
                    JiraCreated = x.JiraCreated,
                    Language = x.Language,
                    ModifiedDate = x.ModifiedDate,
                    MitreValues = x.MitreValues,
                    MitreTechniques = x.MitreTechniques
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
                    VulnId = x.VulnId,
                    Visibility = x.Visibility
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
                    MobilePlatform = mastg.MobilePlatform,
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
                    Idnt01Note = x.Idnt01Note,
                    Idnt02Note = x.Idnt02Note,
                    Idnt03Note = x.Idnt03Note,
                    Idnt04Note = x.Idnt04Note,
                    Idnt05Note = x.Idnt05Note,
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
                    Idnt01Status = x.Idnt01Status,
                    Idnt02Status = x.Idnt02Status,
                    Idnt03Status = x.Idnt03Status,
                    Idnt04Status = x.Idnt04Status,
                    Idnt05Status = x.Idnt05Status,
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
                KnowledgeBaseCategories = knowledgeBaseCategoryManager.GetAll().Select(x =>
                    new KnowledgeBaseCategories()
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
                // The context has no lazy loading, so untracked rows serialize without their navigations.
                VulnCustomFields = db.Set<VulnCustomField>().AsNoTracking().ToList(),
                VulnCustomFieldValues = db.Set<VulnCustomFieldValue>().AsNoTracking().ToList(),
                ProjectCustomFields = db.Set<ProjectCustomField>().AsNoTracking().ToList(),
                ProjectCustomFieldValues = db.Set<ProjectCustomFieldValue>().AsNoTracking().ToList(),
                TargetCustomFields = db.Set<TargetCustomField>().AsNoTracking().ToList(),
                TargetCustomFieldValues = db.Set<TargetCustomFieldValue>().AsNoTracking().ToList(),
                ClientCustomFields = db.Set<ClientCustomField>().AsNoTracking().ToList(),
                ClientCustomFieldValues = db.Set<ClientCustomFieldValue>().AsNoTracking().ToList(),
                VulnCwes = db.Set<VulnCwe>().AsNoTracking().ToList(),
                RssCategories = db.Set<RssCategory>().AsNoTracking().ToList(),
                // News is fetched again from the sources, so it is not kept.
                RssSources = db.Set<RssSource>().AsNoTracking().ToList(),
                KnowledgeBaseTags = db.Set<KnowledgeBaseTags>().AsNoTracking().Select(x => new KnowledgeBaseTags { Id = x.Id, Name = x.Name }).ToList(),
                KnowledgeBaseTagLinks = db.Set<KnowledgeBase>().AsNoTracking()
                    .SelectMany(k => k.Tags.Select(t => new KnowledgeBaseTagLink { NoteId = k.Id, TagId = t.Id })).ToList(),
                // System templates go too: their item codes are what moves the executions to this instance's items.
                ChecklistTemplates = db.Set<ChecklistTemplate>().AsNoTracking().ToList(),
                ChecklistCategories = db.Set<ChecklistCategory>().AsNoTracking().ToList(),
                ChecklistItems = db.Set<ChecklistItem>().AsNoTracking().ToList(),
                Checklists = db.Set<Checklist>().AsNoTracking().ToList(),
                ChecklistExecutions = db.Set<ChecklistExecution>().AsNoTracking().ToList(),
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
    [HasPermission(Permissions.BackupRead)]
    public FileContentResult BackupAttachments()
    {
        try
        {
            string startPath = env.WebRootPath + "/" + "Attachments";
            var zipTmpDir = Directory.CreateTempSubdirectory();
            string zipPath = zipTmpDir.FullName + "/" + @"result" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".zip";

            ZipFile.CreateFromDirectory(startPath, zipPath);

            /*const string contentType ="application/zip";
            HttpContext.Response.ContentType = "application/zip";*/
            var result = new FileContentResult(System.IO.File.ReadAllBytes(zipPath), "application/zip")
            {
                FileDownloadName = "Attachments.zip"
            };

            _logger.LogInformation("Attachments backup made successfully. User: {0}",
                aspNetUserId);
            Directory.Delete(zipTmpDir.FullName, true);
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
    [HasPermission(Permissions.BackupRestore)]
    public async Task<IActionResult> RestoreAttachments(BackupFormViewModel model)
    {
        try
        {
            string extractPath = env.WebRootPath + "/Attachments/";
            if (model.FileContent == null)
            {
                return BadRequest("Invalid request");
            }
            if (!fileCheck.CheckFile(model.FileContent))
            {
                _logger.LogError("An error ocurred adding a client. User: {0}",
                    aspNetUserId);
                return BadRequest("Invalid file type");
            }

            // Extract straight from memory, so a failed restore leaves no copy of the upload on disk.
            using (var archive = new ZipArchive(new MemoryStream(model.FileContent), ZipArchiveMode.Read))
            {
                archive.ExtractToDirectory(extractPath, overwriteFiles: true);
            }

            _logger.LogInformation("Attachments restored successfully. User: {0}",
                aspNetUserId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred restoring attachments. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
    }

    [HttpPost]
    [Route("Data")]
    [HasPermission(Permissions.BackupRestore)]
    public async Task<IActionResult> RestoreData(BackupFormViewModel model)
    {
        IDbContextTransaction? transaction = null;
        try
        {

            if (model.FileContent == null)
            {
                return BadRequest();
            }
            if (!fileCheck.CheckFile(model.FileContent))
            {
                _logger.LogError("An error ocurred adding a client. User: {0}",
                    aspNetUserId);
                return await System.Threading.Tasks.Task.FromResult<IActionResult>(BadRequest("Invalid file type"));
            }

            BackupViewModel backupViewModel;
            // Read the upload from memory; it is never written to disk. The stream overload, unlike the span one,
            // accepts a file saved with a UTF-8 BOM.
            using (var stream = new MemoryStream(model.FileContent))
            {
                backupViewModel = JsonSerializer.Deserialize<BackupViewModel>(stream);
            }

            // All blocks go in one transaction: a row that fails late must not leave the earlier blocks restored.
            // Every manager shares this request's DbContext, so their SaveChangesAsync calls all join it.
            transaction = await userManager.Context.Database.BeginTransactionAsync();
            // AuthPermissions writes through its own DbContext and connection, outside the transaction, so users get
            // their roles only once the restore has committed.
            var pendingRoles = new List<(ApplicationUser User, string Role)>();

            // Every user id in the backup goes through this map. On this instance the same person may have another id
            // (the default admin always does), so a backup user whose email or user name exists here is mapped to that
            // user, and their memberships and authorship follow them.
            var users = new BackupUserMap(aspNetUserId, async id => await _userManager.FindByIdAsync(id) != null);

            // Users are restored before clients, so a client user's link is set once its client exists.
            var pendingClientLinks = new Dictionary<string, Guid>();
            if (backupViewModel.Users != null)
            {
                foreach (var backupUser in backupViewModel.Users)
                {
                    // Only users this instance does not have are restored.
                    if (string.IsNullOrEmpty(backupUser.Email) || string.IsNullOrEmpty(backupUser.UserName)
                        || await _userManager.FindByIdAsync(backupUser.Id) != null)
                    {
                        continue;
                    }

                    var sameUser = await _userManager.FindByEmailAsync(backupUser.Email)
                                   ?? await _userManager.FindByNameAsync(backupUser.UserName);
                    if (sameUser != null)
                    {
                        users.Map(backupUser.Id, sameUser.Id);
                        continue;
                    }

                    // The default admin is never created from a backup.
                    if (backupUser.Email == "admin@cervantes.local")
                    {
                        continue;
                    }

                    // Built from an explicit list of fields: related rows (API keys, chat messages) are never taken
                    // from the file, and the normalized names are computed here, not trusted. The password hash is
                    // restored so the account keeps its password; two-factor is turned off because the backup does
                    // not carry the authenticator keys.
                    var user = new ApplicationUser
                    {
                        Id = backupUser.Id,
                        UserName = backupUser.UserName,
                        NormalizedUserName = _userManager.NormalizeName(backupUser.UserName),
                        Email = backupUser.Email,
                        NormalizedEmail = _userManager.NormalizeEmail(backupUser.Email),
                        EmailConfirmed = backupUser.EmailConfirmed,
                        PasswordHash = backupUser.PasswordHash,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        PhoneNumber = backupUser.PhoneNumber,
                        LockoutEnabled = true,
                        TwoFactorEnabled = false,
                        FullName = sanitizer.Sanitize(backupUser.FullName),
                        Description = sanitizer.Sanitize(backupUser.Description),
                        Position = sanitizer.Sanitize(backupUser.Position),
                        Avatar = AttachmentPath.TryResolve(env.WebRootPath, backupUser.Avatar, "Users", out _) ? backupUser.Avatar : null,
                        ClientId = backupUser.ClientId,
                        ExternalLogin = backupUser.ExternalLogin,
                    };
                    if (backupUser.Avatar != null && user.Avatar == null)
                    {
                        LogDiscardedPath("User", backupUser.Id);
                    }

                    // The backup has no AuthPermissions data, so the user gets the default role for its kind.
                    var role = user.ClientId != null ? "Client" : "User";
                    if (user.ClientId != null && clientManager.GetById(user.ClientId.Value) == null)
                    {
                        pendingClientLinks[user.Id] = user.ClientId.Value;
                        user.ClientId = null;
                    }

                    // Permissions come from AuthPermissions roles, as in UserController.Add; ASP.NET Identity roles
                    // are not used (there is no "User" Identity role to add the user to).
                    await userManager.AddAsync(user);
                    await userManager.Context.SaveChangesAsync();
                    pendingRoles.Add((user, role));
                }

                await userManager.Context.SaveChangesAsync();

            }

            if (backupViewModel.Clients != null)
            {
                foreach (var client in backupViewModel.Clients)
                {
                    var clientResult = clientManager.GetById(client.Id);
                    if (clientResult != null)
                    {
                        continue;
                    }
                    client.UserId = await users.ResolveAsync(client.UserId);
                    if (!AttachmentPath.TryResolve(env.WebRootPath, client.ImagePath, "Clients", out _))
                    {
                        if (client.ImagePath != null && client.ImagePath != "None")
                        {
                            LogDiscardedPath("Client", client.Id);
                        }
                        client.ImagePath = "None";
                    }
                    await clientManager.AddAsync(client);
                }

                await clientManager.Context.SaveChangesAsync();
            }

            foreach (var (userId, clientId) in pendingClientLinks)
            {
                var restoredUser = await _userManager.FindByIdAsync(userId);
                if (restoredUser != null && clientManager.GetById(clientId) != null)
                {
                    restoredUser.ClientId = clientId;
                    await _userManager.UpdateAsync(restoredUser);
                }
            }

            if (backupViewModel.Projects != null)
            {
                foreach (var project in backupViewModel.Projects)
                {
                    var projectResult = projectManager.GetById(project.Id);
                    if (projectResult != null)
                    {
                        continue;
                    }
                    project.UserId = await users.ResolveAsync(project.UserId);
                    await projectManager.AddAsync(project);
                }

                await projectManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.ProjectAttachments != null)
            {
                foreach (var attachment in backupViewModel.ProjectAttachments)
                {
                    var attachmentResult = projectAttachmentManager.GetById(attachment.Id);
                    if (attachmentResult != null)
                    {
                        continue;
                    }
                    if (!AttachmentPath.TryResolve(env.WebRootPath, attachment.FilePath, AttachmentPath.OwnerFolder("Projects", attachment.ProjectId), out _))
                    {
                        LogDiscardedPath("ProjectAttachment", attachment.Id);
                        continue;
                    }
                    attachment.UserId = await users.ResolveAsync(attachment.UserId);
                    await projectAttachmentManager.AddAsync(attachment);
                }

                await projectAttachmentManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.ProjectNotes != null)
            {
                foreach (var note in backupViewModel.ProjectNotes)
                {
                    var noteResult = projectNoteManager.GetById(note.Id);
                    if (noteResult != null)
                    {
                        continue;
                    }
                    note.UserId = await users.ResolveAsync(note.UserId);
                    await projectNoteManager.AddAsync(note);
                }

                await projectNoteManager.Context.SaveChangesAsync();
            }

            // Members who are not admins only reach a project through these rows.
            if (backupViewModel.ProjectUsers != null)
            {
                foreach (var member in backupViewModel.ProjectUsers)
                {
                    member.UserId = await users.ResolveOptionalAsync(member.UserId);
                    if (projectUserManager.GetById(member.Id) != null
                        || projectUserManager.GetAll().Any(x => x.ProjectId == member.ProjectId && x.UserId == member.UserId))
                    {
                        continue;
                    }

                    if (projectManager.GetById(member.ProjectId) == null || member.UserId == null)
                    {
                        _logger.LogWarning("Restore skipped project member {MemberId}: its project or user is missing. User: {UserId}",
                            member.Id, aspNetUserId);
                        continue;
                    }

                    await projectUserManager.AddAsync(member);
                }

                await projectUserManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.Documents != null)
            {
                foreach (var doc in backupViewModel.Documents)
                {
                    var docResult = documentManager.GetById(doc.Id);
                    if (docResult != null)
                    {
                        continue;
                    }
                    doc.UserId = await users.ResolveAsync(doc.UserId);
                    if (!AttachmentPath.TryResolve(env.WebRootPath, doc.FilePath, "Documents", out _))
                    {
                        // "Attachments/Documents/" is what a document without a file stores.
                        if (doc.FilePath != "Attachments/Documents/")
                        {
                            LogDiscardedPath("Document", doc.Id);
                        }
                        doc.FilePath = "Attachments/Documents/";
                    }
                    await documentManager.AddAsync(doc);
                }

                await documentManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.Notes != null)
            {
                foreach (var note in backupViewModel.Notes)
                {
                    var noteResult = noteManager.GetById(note.Id);
                    if (noteResult != null)
                    {
                        continue;
                    }
                    note.UserId = await users.ResolveAsync(note.UserId);
                    await noteManager.AddAsync(note);
                }

                await noteManager.Context.SaveChangesAsync();
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
                if (AttachmentPath.TryResolve(env.WebRootPath, backupViewModel.Organization.ImagePath, "Organization", out _))
                {
                    org.ImagePath = backupViewModel.Organization.ImagePath;
                }
                else
                {
                    if (backupViewModel.Organization.ImagePath != null)
                    {
                        LogDiscardedPath("Organization", org.Id);
                    }
                    org.ImagePath = null;
                }

                await organizationManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.Reports != null)
            {
                foreach (var rep in backupViewModel.Reports)
                {
                    var repResult = reportManager.GetById(rep.Id);
                    if (repResult != null)
                    {
                        continue;
                    }
                    rep.UserId = await users.ResolveAsync(rep.UserId);
                    await reportManager.AddAsync(rep);
                }

                await reportManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.Targets != null)
            {
                foreach (var target in backupViewModel.Targets)
                {
                    var targetResult = targetManager.GetById(target.Id);
                    if (targetResult != null)
                    {
                        continue;
                    }
                    target.UserId = await users.ResolveAsync(target.UserId);
                    await targetManager.AddAsync(target);
                }

                await targetManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.TargetServices != null)
            {
                foreach (var services in backupViewModel.TargetServices)
                {
                    var servicesResult = targetServicesManager.GetById(services.Id);
                    if (servicesResult != null)
                    {
                        continue;
                    }
                    services.UserId = await users.ResolveAsync(services.UserId);
                    await targetServicesManager.AddAsync(services);
                }

                await targetServicesManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.Tasks != null)
            {
                foreach (var task in backupViewModel.Tasks)
                {
                   
                    
                    var taskResult = taskManager.GetById(task.Id);
                    if (taskResult != null)
                    {
                        continue;
                    }
                    
                    // The creator and the assignee are kept when they exist here; a task with no assignee stays so.
                    task.CreatedUserId = await users.ResolveAsync(task.CreatedUserId);
                    if (!string.IsNullOrEmpty(task.AsignedUserId))
                    {
                        task.AsignedUserId = await users.ResolveAsync(task.AsignedUserId);
                    }


                    await taskManager.AddAsync(task);
                }

                await taskManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.TaskAttachments != null)
            {
                foreach (var att in backupViewModel.TaskAttachments)
                {
                    var attResult = taskAttachmentManager.GetById(att.Id);
                    if (attResult != null)
                    {
                        continue;
                    }
                    if (!AttachmentPath.TryResolve(env.WebRootPath, att.FilePath, AttachmentPath.OwnerFolder("Task", att.TaskId), out _))
                    {
                        LogDiscardedPath("TaskAttachment", att.Id);
                        continue;
                    }
                    att.UserId = await users.ResolveAsync(att.UserId);
                    await taskAttachmentManager.AddAsync(att);
                }

                await taskAttachmentManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.TaskNotes != null)
            {
                foreach (var note in backupViewModel.TaskNotes)
                {
                    var noteResult = taskNoteManager.GetById(note.Id);
                    if (noteResult != null)
                    {
                        continue;
                    }
                    note.UserId = await users.ResolveAsync(note.UserId);
                    await taskNoteManager.AddAsync(note);
                }

                await taskNoteManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.TaskTargets != null)
            {
                foreach (var tar in backupViewModel.TaskTargets)
                {
                    var tarResult = taskTargetManager.GetById(tar.Id);
                    if (tarResult != null)
                    {
                        continue;
                    }
                    await taskTargetManager.AddAsync(tar);
                }

                await taskTargetManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.Vaults != null)
            {
                foreach (var vault in backupViewModel.Vaults)
                {
                    var vaultResult = vaultManager.GetById(vault.Id);
                    if (vaultResult != null)
                    {
                        continue;
                    }
                    vault.UserId = await users.ResolveAsync(vault.UserId);
                    await vaultManager.AddAsync(vault);
                }

                await vaultManager.Context.SaveChangesAsync();
            }

            // Seeded categories, report components and templates get random ids on each instance. A backup from another
            // instance would insert a second copy of each, so rows that match an existing one by name are not inserted
            // and the rows pointing at them are rewritten to the existing id.
            var categoryMap = new Dictionary<Guid, Guid>();
            if (backupViewModel.VulnCategories != null)
            {
                categoryMap = BackupKeyMatch.Match(backupViewModel.VulnCategories, vulnCategoryManager.GetAll().ToList(),
                    x => x.Id, x => $"{x.Name}|{x.Type}");
                foreach (var cat in backupViewModel.VulnCategories)
                {
                    if (categoryMap.ContainsKey(cat.Id))
                    {
                        continue;
                    }
                    var result = vulnCategoryManager.GetById(cat.Id);
                    if (result == null)
                    {
                        vulnCategoryManager.Add(cat);
                    }
                    await vulnCategoryManager.Context.SaveChangesAsync();
                }
            }

            if (backupViewModel.Vulns != null)
            {
                foreach (var vulns in backupViewModel.Vulns)
                {
                    var vulnResult = vulnManager.GetById(vulns.Id);
                    if (vulnResult != null)
                    {
                        continue;
                    }
                    vulns.UserId = await users.ResolveAsync(vulns.UserId);
                    if (vulns.VulnCategoryId != null && categoryMap.TryGetValue(vulns.VulnCategoryId.Value, out var categoryId))
                    {
                        vulns.VulnCategoryId = categoryId;
                    }
                    await vulnManager.AddAsync(vulns);
                }

                await vulnManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.VulnAttachments != null)
            {
                foreach (var att in backupViewModel.VulnAttachments)
                {
                    var attResult = vulnAttachmentManager.GetById(att.Id);
                    if (attResult != null)
                    {
                        continue;
                    }
                    if (!AttachmentPath.TryResolve(env.WebRootPath, att.FilePath, AttachmentPath.OwnerFolder("Vuln", att.VulnId), out _))
                    {
                        LogDiscardedPath("VulnAttachment", att.Id);
                        continue;
                    }
                    att.UserId = await users.ResolveAsync(att.UserId);
                    await vulnAttachmentManager.AddAsync(att);
                }

                await vulnAttachmentManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.VulnNotes != null)
            {
                foreach (var note in backupViewModel.VulnNotes)
                {
                    var noteResult = vulnNoteManager.GetById(note.Id);
                    if (noteResult != null)
                    {
                        continue;
                    }
                    note.UserId = await users.ResolveAsync(note.UserId);
                    await vulnNoteManager.AddAsync(note);
                }

                await vulnNoteManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.VulnTargets != null)
            {
                foreach (var vuln in backupViewModel.VulnTargets)
                {
                    var vulnResult = vulnTargetManager.GetById(vuln.Id);
                    if (vulnResult != null)
                    {
                        continue;
                    }
                    await vulnTargetManager.AddAsync(vuln);
                }

                await vulnTargetManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.Jira != null)
            {
                foreach (var jira in backupViewModel.Jira)
                {
                    var jiraResult = jiraManager.GetById(jira.Id);
                    if (jiraResult != null)
                    {
                        continue;
                    }
                    jira.UserId = await users.ResolveAsync(jira.UserId);
                    await jiraManager.AddAsync(jira);
                }

                await jiraManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.JiraComments != null)
            {
                foreach (var com in backupViewModel.JiraComments)
                {
                    var comResult = jiraCommentManager.GetById(com.Id);
                    if (comResult != null)
                    {
                        continue;
                    }
                    await jiraCommentManager.AddAsync(com);
                }

                await jiraCommentManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.Wstgs != null)
            {
                foreach (var wstg in backupViewModel.Wstgs)
                {
                    var wstgResult = wstgManager.GetById(wstg.Id);
                    if (wstgResult != null)
                    {
                        continue;
                    }
                    wstg.UserId = await users.ResolveAsync(wstg.UserId);
                    await wstgManager.AddAsync(wstg);
                }

                await wstgManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.Mastgs != null)
            {
                foreach (var mastg in backupViewModel.Mastgs)
                {
                    var mastgResult = mastgManager.GetById(mastg.Id);
                    if (mastgResult != null)
                    {
                        continue;
                    }
                    mastg.UserId = await users.ResolveAsync(mastg.UserId);
                    
                    await mastgManager.AddAsync(mastg);
                }

                await mastgManager.Context.SaveChangesAsync();
            }
            
            var componentMap = new Dictionary<Guid, Guid>();
            if (backupViewModel.ReportComponents != null)
            {
                componentMap = BackupKeyMatch.Match(backupViewModel.ReportComponents, reportComponentsManager.GetAll().ToList(),
                    x => x.Id, x => $"{x.Name}|{x.Language}|{x.ComponentType}");
                foreach (var comp in backupViewModel.ReportComponents)
                {
                    var compResult = reportComponentsManager.GetById(comp.Id);
                    if (compResult != null || componentMap.ContainsKey(comp.Id))
                    {
                        continue;
                    }
                    // Backups made before ContentCss was exported carry null; the column is NOT NULL.
                    comp.ContentCss ??= string.Empty;
                    await reportComponentsManager.AddAsync(comp);
                }

                await reportComponentsManager.Context.SaveChangesAsync();
            }
            
  
            
            var templateMap = new Dictionary<Guid, Guid>();
            if (backupViewModel.ReportTemplates != null)
            {
                templateMap = BackupKeyMatch.Match(backupViewModel.ReportTemplates, reportTemplateManager.GetAll().ToList(),
                    x => x.Id, x => $"{x.Name}|{x.Language}|{x.ReportType}");
                foreach (var comp in backupViewModel.ReportTemplates)
                {
                    var compResult = reportTemplateManager.GetById(comp.Id);
                    if (compResult != null || templateMap.ContainsKey(comp.Id))
                    {
                        continue;
                    }
                    comp.UserId = await users.ResolveAsync(comp.UserId);
                    await reportTemplateManager.AddAsync(comp);
                }
                
                await reportTemplateManager.Context.SaveChangesAsync();
            }

            if (backupViewModel.ReportParts != null)
            {
                foreach (var comp in backupViewModel.ReportParts)
                {
                    var compResult = reportsPartsManager.GetById(comp.Id);
                    // A matched template already has its parts here; copying the backup's would double them.
                    if (compResult != null || templateMap.ContainsKey(comp.TemplateId))
                    {
                        continue;
                    }
                    if (componentMap.TryGetValue(comp.ComponentId, out var componentId))
                    {
                        comp.ComponentId = componentId;
                    }
                    await reportsPartsManager.AddAsync(comp);
                }
                await reportsPartsManager.Context.SaveChangesAsync();
            }
            
            if (backupViewModel.KnowledgeBaseCategories != null)
            {
                foreach (var cat in backupViewModel.KnowledgeBaseCategories)
                {
                    var catResult = knowledgeBaseCategoryManager.GetById(cat.Id);
                    if (catResult != null)
                    {
                        continue;
                    }
                    cat.UserId = await users.ResolveAsync(cat.UserId);
                    
                    await knowledgeBaseCategoryManager.AddAsync(cat);
                }

                await knowledgeBaseCategoryManager.Context.SaveChangesAsync();
            }
            
            if (backupViewModel.KnowledgeBase != null)
            {
                foreach (var cat in backupViewModel.KnowledgeBase)
                {
                    var catResult = knowledgeBaseManager.GetById(cat.Id);
                    if (catResult != null)
                    {
                        continue;
                    }
                    
                    cat.CreatedUserId = await users.ResolveAsync(cat.CreatedUserId);
                    
                    cat.UpdatedUserId = await users.ResolveAsync(cat.UpdatedUserId);
                    
                    await knowledgeBaseManager.AddAsync(cat);
                }

                await knowledgeBaseManager.Context.SaveChangesAsync();
            }

            var vulnFieldMap = await RestoreCustomFieldsAsync(backupViewModel.VulnCustomFields, users);
            await RestoreCustomFieldValuesAsync<VulnCustomFieldValue, Vuln, VulnCustomField>(backupViewModel.VulnCustomFieldValues,
                vulnFieldMap, x => x.VulnId, x => x.VulnCustomFieldId, (x, id) => x.VulnCustomFieldId = id, users);
            var projectFieldMap = await RestoreCustomFieldsAsync(backupViewModel.ProjectCustomFields, users);
            await RestoreCustomFieldValuesAsync<ProjectCustomFieldValue, Project, ProjectCustomField>(backupViewModel.ProjectCustomFieldValues,
                projectFieldMap, x => x.ProjectId, x => x.ProjectCustomFieldId, (x, id) => x.ProjectCustomFieldId = id, users);
            var targetFieldMap = await RestoreCustomFieldsAsync(backupViewModel.TargetCustomFields, users);
            await RestoreCustomFieldValuesAsync<TargetCustomFieldValue, Target, TargetCustomField>(backupViewModel.TargetCustomFieldValues,
                targetFieldMap, x => x.TargetId, x => x.TargetCustomFieldId, (x, id) => x.TargetCustomFieldId = id, users);
            var clientFieldMap = await RestoreCustomFieldsAsync(backupViewModel.ClientCustomFields, users);
            await RestoreCustomFieldValuesAsync<ClientCustomFieldValue, CORE.Entities.Client, ClientCustomField>(backupViewModel.ClientCustomFieldValues,
                clientFieldMap, x => x.ClientId, x => x.ClientCustomFieldId, (x, id) => x.ClientCustomFieldId = id, users);
            await RestoreVulnCwesAsync(backupViewModel.VulnCwes);
            await RestoreRssAsync(backupViewModel.RssCategories, backupViewModel.RssSources, users);
            await RestoreKnowledgeBaseTagsAsync(backupViewModel.KnowledgeBaseTags, backupViewModel.KnowledgeBaseTagLinks);
            await RestoreChecklistsAsync(backupViewModel.ChecklistTemplates, backupViewModel.ChecklistCategories,
                backupViewModel.ChecklistItems, backupViewModel.Checklists, backupViewModel.ChecklistExecutions, users);

            await transaction.CommitAsync();
            // From here the data is in: nothing below may roll back or turn the restore into an error.
            await transaction.DisposeAsync();
            transaction = null;

            foreach (var (user, role) in pendingRoles)
            {
                try
                {
                    var status = await authUsersAdminService.AddNewUserAsync(user.Id, user.Email, user.UserName, new List<string> { role });
                    if (status.HasErrors)
                    {
                        // The user is restored without permissions; an admin has to give it a role by hand.
                        _logger.LogWarning("Restore could not give user {UserId} a role: {Errors}", user.Id, status.GetAllErrors());
                    }
                }
                catch (Exception roleEx)
                {
                    _logger.LogError(roleEx, "Restore could not give user {UserId} a role", user.Id);
                }
            }

            // A backup can carry HTML stored before a sanitizer fix (or crafted by hand); clean it like any write.
            // The rows are already committed here, so a failure must not turn the restore into a 400: queue the
            // same pass in a fresh scope instead.
            try
            {
                var changed = await resanitizer.RunAsync();
                _logger.LogInformation("Restored data re-sanitized, {Changed} fields changed. User: {UserId}",
                    changed, aspNetUserId);
            }
            catch (Exception resanitizeEx)
            {
                _logger.LogError(resanitizeEx, "Re-sanitizing restored data failed, queueing a background run. User: {UserId}",
                    aspNetUserId);
                QueueResanitize();
            }

            _logger.LogInformation("Data restored successfully. User: {0}",
                aspNetUserId);
            return await System.Threading.Tasks.Task.FromResult<IActionResult>(Ok());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred restoring data. User: {0}",
                aspNetUserId);
            if (transaction != null)
            {
                // Nothing of this restore is kept. The failed entries stay tracked, so later work in the same scope
                // (a Blazor circuit) would try to save them again.
                try
                {
                    await transaction.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    // A failed commit can leave the transaction unusable; the database rolls it back on its own.
                    _logger.LogError(rollbackEx, "Rolling back the restore failed. User: {UserId}", aspNetUserId);
                }
                userManager.Context.ChangeTracker.Clear();
            }
            QueueResanitize();
            return await System.Threading.Tasks.Task.FromResult<IActionResult>(BadRequest());
        }
        finally
        {
            if (transaction != null)
            {
                await transaction.DisposeAsync();
            }
        }
    }

    /// <summary>
    /// Restores one kind of custom field definition. The name is unique, so a definition whose name already exists here
    /// is not inserted: the returned map sends its values to the existing definition.
    /// </summary>
    private async Task<Dictionary<Guid, Guid>> RestoreCustomFieldsAsync<TField>(IEnumerable<TField>? rows, BackupUserMap users)
        where TField : class, ICustomFieldDefinition
    {
        if (rows == null)
        {
            return new Dictionary<Guid, Guid>();
        }

        var db = userManager.Context;
        var existing = db.Set<TField>().AsNoTracking().ToList();
        var map = BackupKeyMatch.Match(rows, existing, x => x.Id, x => x.Name);
        var existingIds = existing.Select(x => x.Id).ToHashSet();
        foreach (var field in rows)
        {
            if (existingIds.Contains(field.Id) || map.ContainsKey(field.Id))
            {
                continue;
            }

            field.Name = sanitizer.Sanitize(field.Name);
            field.Label = sanitizer.Sanitize(field.Label);
            field.Options = sanitizer.Sanitize(field.Options);
            field.DefaultValue = sanitizer.Sanitize(field.DefaultValue);
            field.Description = sanitizer.Sanitize(field.Description);
            field.UserId = await users.ResolveAsync(field.UserId);
            db.Set<TField>().Add(field);
        }

        await db.SaveChangesAsync();
        return map;
    }

    /// <summary>
    /// Restores the values of one kind of custom field. A value is skipped when its row, its field or its owner is
    /// missing, or when the owner already has a value for that field (the pair is unique).
    /// </summary>
    private async System.Threading.Tasks.Task RestoreCustomFieldValuesAsync<TValue, TOwner, TField>(IEnumerable<TValue>? rows,
        Dictionary<Guid, Guid> fieldMap, Func<TValue, Guid> ownerId, Func<TValue, Guid> fieldId, Action<TValue, Guid> setFieldId,
        BackupUserMap users)
        where TValue : class, ICustomFieldValue where TOwner : class where TField : class
    {
        if (rows == null)
        {
            return;
        }

        var db = userManager.Context;
        var existing = db.Set<TValue>().AsNoTracking().ToList();
        var existingIds = existing.Select(x => x.Id).ToHashSet();
        var pairs = existing.Select(x => (ownerId(x), fieldId(x))).ToHashSet();
        foreach (var value in rows)
        {
            if (existingIds.Contains(value.Id))
            {
                continue;
            }

            if (fieldMap.TryGetValue(fieldId(value), out var mapped))
            {
                setFieldId(value, mapped);
            }

            if (await db.Set<TOwner>().FindAsync(ownerId(value)) == null || await db.Set<TField>().FindAsync(fieldId(value)) == null
                || !pairs.Add((ownerId(value), fieldId(value))))
            {
                continue;
            }

            value.Value = sanitizer.Sanitize(value.Value);
            value.UserId = await users.ResolveAsync(value.UserId);
            db.Set<TValue>().Add(value);
        }

        await db.SaveChangesAsync();
    }

    /// <summary>Restores the vuln-CWE links. The CWE ids are the CWE numbers, the same on every instance.</summary>
    private async System.Threading.Tasks.Task RestoreVulnCwesAsync(IEnumerable<VulnCwe>? rows)
    {
        if (rows == null)
        {
            return;
        }

        var db = userManager.Context;
        var existing = db.Set<VulnCwe>().AsNoTracking().ToList();
        var existingIds = existing.Select(x => x.Id).ToHashSet();
        var pairs = existing.Select(x => (x.VulnId, x.CweId)).ToHashSet();
        foreach (var link in rows)
        {
            if (existingIds.Contains(link.Id) || await db.Set<Vuln>().FindAsync(link.VulnId) == null
                || await db.Set<Cwe>().FindAsync(link.CweId) == null || !pairs.Add((link.VulnId, link.CweId)))
            {
                continue;
            }

            db.Set<VulnCwe>().Add(link);
        }

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Restores the RSS categories (matched by name) and sources. A source's image is stored as base64 data, not as a
    /// file path, so it is kept as it is.
    /// </summary>
    private async System.Threading.Tasks.Task RestoreRssAsync(IEnumerable<RssCategory>? categories, IEnumerable<RssSource>? sources,
        BackupUserMap users)
    {
        var db = userManager.Context;
        var categoryMap = new Dictionary<Guid, Guid>();
        if (categories != null)
        {
            var existing = db.Set<RssCategory>().AsNoTracking().ToList();
            categoryMap = BackupKeyMatch.Match(categories, existing, x => x.Id, x => x.Name);
            var existingIds = existing.Select(x => x.Id).ToHashSet();
            foreach (var category in categories)
            {
                if (existingIds.Contains(category.Id) || categoryMap.ContainsKey(category.Id))
                {
                    continue;
                }

                category.Name = sanitizer.Sanitize(category.Name);
                category.Description = sanitizer.Sanitize(category.Description);
                db.Set<RssCategory>().Add(category);
            }

            await db.SaveChangesAsync();
        }

        if (sources == null)
        {
            return;
        }

        foreach (var source in sources)
        {
            if (await db.Set<RssSource>().FindAsync(source.Id) != null)
            {
                continue;
            }

            if (categoryMap.TryGetValue(source.CategoryId, out var mapped))
            {
                source.CategoryId = mapped;
            }

            if (await db.Set<RssCategory>().FindAsync(source.CategoryId) == null)
            {
                continue;
            }

            source.Name = sanitizer.Sanitize(source.Name);
            source.UserId = await users.ResolveAsync(source.UserId);
            db.Set<RssSource>().Add(source);
        }

        await db.SaveChangesAsync();
    }

    /// <summary>Restores the knowledge base tags (matched by name) and their links to pages.</summary>
    private async System.Threading.Tasks.Task RestoreKnowledgeBaseTagsAsync(IEnumerable<KnowledgeBaseTags>? tags,
        IEnumerable<KnowledgeBaseTagLink>? links)
    {
        var db = userManager.Context;
        var tagMap = new Dictionary<Guid, Guid>();
        if (tags != null)
        {
            var existing = db.Set<KnowledgeBaseTags>().AsNoTracking().ToList();
            tagMap = BackupKeyMatch.Match(tags, existing, x => x.Id, x => x.Name);
            var existingIds = existing.Select(x => x.Id).ToHashSet();
            foreach (var tag in tags)
            {
                if (existingIds.Contains(tag.Id) || tagMap.ContainsKey(tag.Id))
                {
                    continue;
                }

                db.Set<KnowledgeBaseTags>().Add(new KnowledgeBaseTags { Id = tag.Id, Name = sanitizer.Sanitize(tag.Name) });
            }

            await db.SaveChangesAsync();
        }

        if (links == null)
        {
            return;
        }

        foreach (var link in links)
        {
            var tagId = tagMap.TryGetValue(link.TagId, out var mapped) ? mapped : link.TagId;
            var page = await db.Set<KnowledgeBase>().Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == link.NoteId);
            var tag = await db.Set<KnowledgeBaseTags>().FindAsync(tagId);
            if (page == null || tag == null)
            {
                continue;
            }

            page.Tags ??= new List<KnowledgeBaseTags>();
            if (page.Tags.All(x => x.Id != tagId))
            {
                page.Tags.Add(tag);
            }
        }

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Restores checklist templates, their categories and items, checklists and executions. The WSTG and MASTG system
    /// templates are created at startup with random ids on every instance, so a system template is matched by name and
    /// its items by category name, code and name (WSTG repeats one code); executions are moved to the matched items.
    /// Custom templates are matched by id only.
    /// </summary>
    private async System.Threading.Tasks.Task RestoreChecklistsAsync(IEnumerable<ChecklistTemplate>? templates,
        IEnumerable<ChecklistCategory>? categories, IEnumerable<ChecklistItem>? items, IEnumerable<Checklist>? checklists,
        IEnumerable<ChecklistExecution>? executions, BackupUserMap users)
    {
        if (templates == null)
        {
            return;
        }

        var db = userManager.Context;
        var existingTemplates = db.Set<ChecklistTemplate>().AsNoTracking().ToList();
        var templateMap = BackupKeyMatch.Match(templates.Where(x => x.IsSystemTemplate),
            existingTemplates.Where(x => x.IsSystemTemplate), x => x.Id, x => x.Name);
        var existingTemplateIds = existingTemplates.Select(x => x.Id).ToHashSet();
        foreach (var template in templates)
        {
            if (existingTemplateIds.Contains(template.Id) || templateMap.ContainsKey(template.Id))
            {
                continue;
            }

            template.Name = sanitizer.Sanitize(template.Name);
            template.Description = sanitizer.Sanitize(template.Description);
            template.Version = sanitizer.Sanitize(template.Version);
            template.UserId = await users.ResolveAsync(template.UserId);
            if (template.OrganizationId != null && await db.Set<Organization>().FindAsync(template.OrganizationId) == null)
            {
                template.OrganizationId = null;
            }

            db.Set<ChecklistTemplate>().Add(template);
        }

        await db.SaveChangesAsync();

        var backupCategories = (categories ?? Enumerable.Empty<ChecklistCategory>()).ToDictionary(x => x.Id);
        foreach (var category in backupCategories.Values)
        {
            if (templateMap.ContainsKey(category.ChecklistTemplateId)
                || await db.Set<ChecklistCategory>().FindAsync(category.Id) != null
                || await db.Set<ChecklistTemplate>().FindAsync(category.ChecklistTemplateId) == null)
            {
                continue;
            }

            category.Name = sanitizer.Sanitize(category.Name);
            category.Description = sanitizer.Sanitize(category.Description);
            db.Set<ChecklistCategory>().Add(category);
        }

        await db.SaveChangesAsync();

        // Items of a matched system template are not inserted: each one is paired with this instance's item.
        var mappedTemplateIds = templateMap.Values.ToHashSet();
        var targetItems = db.Set<ChecklistItem>().AsNoTracking()
            .Where(x => mappedTemplateIds.Contains(x.ChecklistCategory.ChecklistTemplateId))
            .Select(x => new { x.Id, x.Code, x.Name, Category = x.ChecklistCategory.Name, Template = x.ChecklistCategory.ChecklistTemplateId })
            .ToList()
            .GroupBy(x => $"{x.Template}|{x.Category}|{x.Code}|{x.Name}")
            .ToDictionary(x => x.Key, x => x.Min(y => y.Id));
        var itemMap = new Dictionary<Guid, Guid>();
        foreach (var item in items ?? Enumerable.Empty<ChecklistItem>())
        {
            if (backupCategories.TryGetValue(item.ChecklistCategoryId, out var category)
                && templateMap.TryGetValue(category.ChecklistTemplateId, out var mappedTemplate))
            {
                if (targetItems.TryGetValue($"{mappedTemplate}|{category.Name}|{item.Code}|{item.Name}", out var targetItem))
                {
                    itemMap[item.Id] = targetItem;
                }

                continue;
            }

            if (await db.Set<ChecklistItem>().FindAsync(item.Id) != null
                || await db.Set<ChecklistCategory>().FindAsync(item.ChecklistCategoryId) == null)
            {
                continue;
            }

            item.Code = sanitizer.Sanitize(item.Code);
            item.Name = sanitizer.Sanitize(item.Name);
            item.Description = sanitizer.Sanitize(item.Description);
            item.Objectives = sanitizer.Sanitize(item.Objectives);
            item.TestProcedure = sanitizer.Sanitize(item.TestProcedure);
            item.PassCriteria = sanitizer.Sanitize(item.PassCriteria);
            item.References = sanitizer.Sanitize(item.References);
            db.Set<ChecklistItem>().Add(item);
        }

        await db.SaveChangesAsync();

        foreach (var checklist in checklists ?? Enumerable.Empty<Checklist>())
        {
            if (templateMap.TryGetValue(checklist.ChecklistTemplateId, out var mappedTemplate))
            {
                checklist.ChecklistTemplateId = mappedTemplate;
            }

            if (await db.Set<Checklist>().FindAsync(checklist.Id) != null
                || await db.Set<Project>().FindAsync(checklist.ProjectId) == null
                || await db.Set<ChecklistTemplate>().FindAsync(checklist.ChecklistTemplateId) == null)
            {
                continue;
            }

            if (checklist.TargetId != null && await db.Set<Target>().FindAsync(checklist.TargetId) == null)
            {
                checklist.TargetId = null;
            }

            checklist.Name = sanitizer.Sanitize(checklist.Name);
            checklist.Notes = sanitizer.Sanitize(checklist.Notes);
            checklist.UserId = await users.ResolveAsync(checklist.UserId);
            db.Set<Checklist>().Add(checklist);
        }

        await db.SaveChangesAsync();

        foreach (var execution in executions ?? Enumerable.Empty<ChecklistExecution>())
        {
            if (await db.Set<ChecklistExecution>().FindAsync(execution.Id) != null
                || await db.Set<Checklist>().FindAsync(execution.ChecklistId) == null)
            {
                continue;
            }

            if (itemMap.TryGetValue(execution.ChecklistItemId, out var mappedItem))
            {
                execution.ChecklistItemId = mappedItem;
            }

            if (await db.Set<ChecklistItem>().FindAsync(execution.ChecklistItemId) == null)
            {
                _logger.LogWarning("Restore skipped checklist execution {ExecutionId}: its item does not exist here. User: {UserId}",
                    execution.Id, aspNetUserId);
                continue;
            }

            if (execution.VulnId != null && await db.Set<Vuln>().FindAsync(execution.VulnId) == null)
            {
                execution.VulnId = null;
            }

            execution.Notes = sanitizer.Sanitize(execution.Notes);
            execution.Evidence = sanitizer.Sanitize(execution.Evidence);
            execution.TestedByUserId = await users.ResolveOptionalAsync(execution.TestedByUserId);
            db.Set<ChecklistExecution>().Add(execution);
        }

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Logs a restored file path that was dropped because it does not resolve inside its attachments folder.
    /// </summary>
    private void LogDiscardedPath(string entity, object id)
    {
        _logger.LogWarning("Restore discarded the stored file of {Entity} {Id}: it is outside its attachments folder. User: {UserId}",
            entity, id, aspNetUserId);
    }

    /// <summary>
    /// Queues <see cref="StoredHtmlResanitizer"/> as a background job (it runs in its own DI scope). Never throws:
    /// callers are on a failure path that must keep its own response.
    /// </summary>
    private void QueueResanitize()
    {
        try
        {
            BackgroundJob.Enqueue<StoredHtmlResanitizer>(r => r.RunAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not queue the stored HTML re-sanitize job. User: {UserId}", aspNetUserId);
        }
    }

    /// <summary>
    /// Re-sanitizes stored HTML (see <see cref="StoredHtmlResanitizer"/>). Run once after upgrading to a release
    /// that fixes the sanitizer. Not called by any component, so the attribute is enforced (HTTP only).
    /// </summary>
    [HttpPost]
    [Route("Resanitize")]
    [HasPermission(Permissions.BackupRestore)]
    public async Task<IActionResult> Resanitize()
    {
        try
        {
            var changed = await resanitizer.RunAsync();
            _logger.LogInformation("Stored HTML re-sanitized, {Changed} fields changed. User: {UserId}", changed, aspNetUserId);
            return Ok(new { changed });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred re-sanitizing stored data. User: {UserId}", aspNetUserId);
            return StatusCode(500, "An error occurred while re-sanitizing stored data. Please try again later.");
        }
    }
}