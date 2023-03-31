using System;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cervantes.Web.Controllers;
[Authorize(Roles = "Admin")]
public class BackupController : Controller
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
    private readonly IHostingEnvironment _appEnvironment;

    public BackupController(IUserManager userManager, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> usrManager,IClientManager clientManager, IProjectManager projectManager,
        IProjectUserManager projectUserManager, IProjectNoteManager projectNoteManager, IDocumentManager documentManager,
        INoteManager noteManager, IOrganizationManager organizationManager, IReportManager reportManager,
        ITargetManager targetManager, ITargetServicesManager targetServicesManager, ITaskManager taskManager, ITaskTargetManager taskTargetManager,
        ITaskNoteManager taskNoteManager, ITaskAttachmentManager taskAttachmentManager, IVaultManager vaultManager,
        IVulnManager vulnManager, IVulnNoteManager vulnNoteManager, IVulnAttachmentManager vulnAttachmentManager, IVulnCategoryManager vulnCategoryManager,
        IProjectAttachmentManager projectAttachmentManager,IVulnTargetManager vulnTargetManager ,ILogger<BackupController> logger,IHostingEnvironment _appEnvironment,IReportTemplateManager reportTemplateManager,
        IWSTGManager wstgManager, IMASTGManager mastgManager, IJiraManager jiraManager, IJiraCommentManager jiraCommentManager)
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
        this._appEnvironment = _appEnvironment;
        this.jiraManager = jiraManager;
        this.jiraCommentManager = jiraCommentManager;
        this.wstgManager = wstgManager;
        this.mastgManager = mastgManager;

    }
    // GET
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult BackupData()
    {
        try
        {

            var model = new BackupViewModel
            {
                Users = userManager.GetAll().ToList(),
                Clients = clientManager.GetAll().Select(X => new Client
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
                ProjectUsers = projectUserManager.GetAll().Select( x => new ProjectUser
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
                ProjectNotes = projectNoteManager.GetAll().Select(x=> new ProjectNote
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
                Reports = reportManager.GetAll().Select(x => new Report
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    ProjectId = x.ProjectId,
                    CreatedDate = x.CreatedDate,
                    Name = x.Name,
                    Description = x.Description,
                    Version = x.Version,
                    FilePath = x.FilePath,
                    Language = x.Language
                    
                }).ToList(),
                ReportTemplates = reportTemplateManager.GetAll().Select(x => new ReportTemplate
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    CreatedDate = x.CreatedDate,
                    Name = x.Name,
                    Description = x.Description,
                    FilePath = x.FilePath,
                    Language = x.Language
                    
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
                Tasks = taskManager.GetAll().Select(X => new Task
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
                Mastgs = mastgManager.GetAll().Select(x => new MASTG
                {
                    Id = x.Id,
                    TargetId = x.TargetId,
                    UserId = x.UserId,
                    ProjectId = x.ProjectId,
                    CreatedDate = x.CreatedDate,
                    ArchNote01 = x.ArchNote01,
                    ArchNote02 = x.ArchNote02,
                    ArchNote03 = x.ArchNote03,
                    ArchNote04 = x.ArchNote04,
                    ArchNote05 = x.ArchNote05,
                    ArchNote06 = x.ArchNote06,
                    ArchNote07 = x.ArchNote07,
                    ArchNote08 = x.ArchNote08,
                    ArchNote09 = x.ArchNote09,
                    ArchNote10 = x.ArchNote10,
                    ArchNote11 = x.ArchNote11,
                    ArchNote12 = x.ArchNote12,
                    StorageNote01 = x.StorageNote01,
                    StorageNote02 = x.StorageNote02,
                    StorageNote03 = x.StorageNote03,
                    StorageNote04 = x.StorageNote04,
                    StorageNote05 = x.StorageNote05,
                    StorageNote06 = x.StorageNote06,
                    StorageNote07 = x.StorageNote07,
                    StorageNote08 = x.StorageNote08,
                    StorageNote09 = x.StorageNote09,
                    StorageNote10 = x.StorageNote10,
                    StorageNote11 = x.StorageNote11,
                    StorageNote12 = x.StorageNote12,
                    StorageNote13 = x.StorageNote13,
                    StorageNote14 = x.StorageNote14,
                    StorageNote15 = x.StorageNote15,
                    CryptoNote01 = x.CryptoNote01,
                    CryptoNote02 = x.CryptoNote02,
                    CryptoNote03 = x.CryptoNote03,
                    CryptoNote04 = x.CryptoNote04,
                    CryptoNote05 = x.CryptoNote05,
                    CryptoNote06 = x.CryptoNote06,
                    AuthNote01 = x.AuthNote01,
                    AuthNote02 = x.AuthNote02,
                    AuthNote03 = x.AuthNote03,
                    AuthNote04 = x.AuthNote04,
                    AuthNote05 = x.AuthNote05,
                    AuthNote06 = x.AuthNote06,
                    AuthNote07 = x.AuthNote07,
                    AuthNote08 = x.AuthNote08,
                    AuthNote09 = x.AuthNote09,
                    AuthNote10 = x.AuthNote10,
                    AuthNote11 = x.AuthNote11,
                    AuthNote12 = x.AuthNote12,
                    NetworkNote01 = x.NetworkNote01,
                    NetworkNote02 = x.NetworkNote02,
                    NetworkNote03 = x.NetworkNote03,
                    NetworkNote04 = x.NetworkNote04,
                    NetworkNote05 = x.NetworkNote05,
                    NetworkNote06 = x.NetworkNote06,
                    NetworkNote07 = x.NetworkNote07,
                    NetworkNote08 = x.NetworkNote08,
                    NetworkNote09 = x.NetworkNote09,
                    NetworkNote10 = x.NetworkNote10,
                    NetworkNote11 = x.NetworkNote11,
                    NetworkNote12 = x.NetworkNote12,
                    NetworkNote13 = x.NetworkNote13,
                    NetworkNote14 = x.NetworkNote14,
                    NetworkNote15 = x.NetworkNote15,
                    NetworkNote16 = x.NetworkNote16,
                    NetworkNote17 = x.NetworkNote17,
                    CodeNote01 = x.CodeNote01,
                    CodeNote02 = x.CodeNote02,
                    CodeNote03 = x.CodeNote03,
                    CodeNote04 = x.CodeNote04,
                    CodeNote05 = x.CodeNote05,
                    CodeNote06 = x.CodeNote06,
                    CodeNote07 = x.CodeNote07,
                    CodeNote08 = x.CodeNote08,
                    CodeNote09 = x.CodeNote09,
                    ResilienceNote01 = x.ResilienceNote01,
                    ResilienceNote02 = x.ResilienceNote02,
                    ResilienceNote03 = x.ResilienceNote03,
                    ResilienceNote04 = x.ResilienceNote04,
                    ResilienceNote05 = x.ResilienceNote05,
                    ResilienceNote06 = x.ResilienceNote06,
                    ResilienceNote07 = x.ResilienceNote07,
                    ResilienceNote08 = x.ResilienceNote08,
                    ResilienceNote09 = x.ResilienceNote09,
                    ResilienceNote10 = x.ResilienceNote10,
                    ResilienceNote11 = x.ResilienceNote11,
                    ResilienceNote12 = x.ResilienceNote12,
                    ResilienceNote13 = x.ResilienceNote13,
                    PlatformNote01 = x.PlatformNote01,
                    PlatformNote02 = x.PlatformNote02,
                    PlatformNote03 = x.PlatformNote03,
                    PlatformNote04 = x.PlatformNote04,
                    PlatformNote05 = x.PlatformNote05,
                    PlatformNote06 = x.PlatformNote06,
                    PlatformNote07 = x.PlatformNote07,
                    PlatformNote08 = x.PlatformNote08,
                    PlatformNote09 = x.PlatformNote09,
                    PlatformNote10 = x.PlatformNote10,
                    PlatformNote11 = x.PlatformNote11,
                    ArchStatus01 = x.ArchStatus01,
                    ArchStatus02 = x.ArchStatus02,
                    ArchStatus03 = x.ArchStatus03,
                    ArchStatus04 = x.ArchStatus04,
                    ArchStatus05 = x.ArchStatus05,
                    ArchStatus06 = x.ArchStatus06,
                    ArchStatus07 = x.ArchStatus07,
                    ArchStatus08 = x.ArchStatus08,
                    ArchStatus09 = x.ArchStatus09,
                    ArchStatus10 = x.ArchStatus10,
                    ArchStatus11 = x.ArchStatus11,
                    ArchStatus12 = x.ArchStatus12,
                    StorageStatus01 = x.StorageStatus01,
                    StorageStatus02 = x.StorageStatus02,
                    StorageStatus03 = x.StorageStatus03,
                    StorageStatus04 = x.StorageStatus04,
                    StorageStatus05 = x.StorageStatus05,
                    StorageStatus06 = x.StorageStatus06,
                    StorageStatus07 = x.StorageStatus07,
                    StorageStatus08 = x.StorageStatus08,
                    StorageStatus09 = x.StorageStatus09,
                    StorageStatus10 = x.StorageStatus10,
                    StorageStatus11 = x.StorageStatus11,
                    StorageStatus12 = x.StorageStatus12,
                    StorageStatus13 = x.StorageStatus13,
                    StorageStatus14 = x.StorageStatus14,
                    StorageStatus15 = x.StorageStatus15,
                    CryptoStatus01 = x.CryptoStatus01,
                    CryptoStatus02 = x.CryptoStatus02,
                    CryptoStatus03 = x.CryptoStatus03,
                    CryptoStatus04 = x.CryptoStatus04,
                    CryptoStatus05 = x.CryptoStatus05,
                    CryptoStatus06 = x.CryptoStatus06,
                    AuthStatus01 = x.ArchStatus01,
                    AuthStatus02 = x.ArchStatus02,
                    AuthStatus03 = x.ArchStatus03,
                    AuthStatus04 = x.ArchStatus04,
                    AuthStatus05 = x.ArchStatus05,
                    AuthStatus06 = x.ArchStatus06,
                    AuthStatus07 = x.ArchStatus07,
                    AuthStatus08 = x.ArchStatus08,
                    AuthStatus09 = x.ArchStatus09,
                    AuthStatus10 = x.ArchStatus10,
                    AuthStatus11 = x.ArchStatus11,
                    AuthStatus12 = x.ArchStatus12,
                    NetworkStatus01 = x.NetworkStatus01,
                    NetworkStatus02 = x.NetworkStatus02,
                    NetworkStatus03 = x.NetworkStatus03,
                    NetworkStatus04 = x.NetworkStatus04,
                    NetworkStatus05 = x.NetworkStatus05,
                    NetworkStatus06 = x.NetworkStatus06,
                    NetworkStatus07 = x.NetworkStatus07,
                    NetworkStatus08 = x.NetworkStatus08,
                    NetworkStatus09 = x.NetworkStatus09,
                    NetworkStatus10 = x.NetworkStatus10,
                    NetworkStatus11 = x.NetworkStatus11,
                    NetworkStatus12 = x.NetworkStatus12,
                    NetworkStatus13 = x.NetworkStatus13,
                    NetworkStatus14 = x.NetworkStatus14,
                    NetworkStatus15 = x.NetworkStatus15,
                    NetworkStatus16 = x.NetworkStatus16,
                    NetworkStatus17 = x.NetworkStatus17,
                    CodeStatus01 = x.CodeStatus01,
                    CodeStatus02 = x.CodeStatus02,
                    CodeStatus03 = x.CodeStatus03,
                    CodeStatus04 = x.CodeStatus04,
                    CodeStatus05 = x.CodeStatus05,
                    CodeStatus06 = x.CodeStatus06,
                    CodeStatus07 = x.CodeStatus07,
                    CodeStatus08 = x.CodeStatus08,
                    CodeStatus09 = x.CodeStatus09,
                    ResilienceStatus01 = x.ResilienceStatus01,
                    ResilienceStatus02 = x.ResilienceStatus02,
                    ResilienceStatus03 = x.ResilienceStatus03,
                    ResilienceStatus04 = x.ResilienceStatus04,
                    ResilienceStatus05 = x.ResilienceStatus05,
                    ResilienceStatus06 = x.ResilienceStatus06,
                    ResilienceStatus07 = x.ResilienceStatus07,
                    ResilienceStatus08 = x.ResilienceStatus08,
                    ResilienceStatus09 = x.ResilienceStatus09,
                    ResilienceStatus10 = x.ResilienceStatus10,
                    ResilienceStatus11 = x.ResilienceStatus11,
                    ResilienceStatus12 = x.ResilienceStatus12,
                    ResilienceStatus13 = x.ResilienceStatus13,
                    PlatformStatus01 = x.PlatformStatus01,
                    PlatformStatus02 = x.PlatformStatus02,
                    PlatformStatus03 = x.PlatformStatus03,
                    PlatformStatus04 = x.PlatformStatus04,
                    PlatformStatus05 = x.PlatformStatus05,
                    PlatformStatus06 = x.PlatformStatus06,
                    PlatformStatus07 = x.PlatformStatus07,
                    PlatformStatus08 = x.PlatformStatus08,
                    PlatformStatus09 = x.PlatformStatus09,
                    PlatformStatus10 = x.PlatformStatus10,
                    PlatformStatus11 = x.PlatformStatus11
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
                })



            };
            
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(model, options);
            
            string fileName = "BackupData_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            
            var fileBytes = Encoding.ASCII.GetBytes(jsonString);
            var mimeType = "application/json";
            return new FileContentResult(fileBytes, mimeType)
            {
                FileDownloadName = fileName
            };
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error backup data!";

            _logger.LogError(ex, "An error ocurred making backup data. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult BackupAttachments()
    {
        try
        {
            string startPath = _appEnvironment.WebRootPath+ "/"+ "Attachments";
            string zipPath = _appEnvironment.WebRootPath +"/"+@"result"+DateTime.Now.ToString()+".zip";

           ZipFile.CreateFromDirectory(startPath, zipPath);

           const string contentType ="application/zip";
           HttpContext.Response.ContentType = contentType;
           var result = new FileContentResult(System.IO.File.ReadAllBytes(zipPath), contentType)
           {
               FileDownloadName = "Attachments.zip"
           };

           return result;

        }
        catch (Exception ex)
        {
            TempData["error"] = "Error backup attachments!";

            _logger.LogError(ex, "An error ocurred making attachments backup Index. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
    }

    public IActionResult Restore()
    {
        try
        {
            return View("Restore");
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error restore !";

            _logger.LogError(ex, "An error ocurred loading restore backup page. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult RestoreAttachments(IFormFile backup)
    {
        try
        {
            var file = Request.Form.Files["uploadAttachments"];
            var uploads = _appEnvironment.WebRootPath + "/Temp";
            string extractPath = Path.Combine(_appEnvironment.WebRootPath, "");
            var uniqueName = "Attachments.zip";

            if (Directory.Exists(uploads))
            {
                using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            else
            {
                Directory.CreateDirectory(uploads);
                using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }

            ZipFile.ExtractToDirectory(uploads+"/"+uniqueName, extractPath,true);

            System.IO.File.Delete(Path.Combine(uploads, uniqueName));
            Directory.Delete(uploads);
            TempData["restoredAttachments"] = "restored";
            return View("Restore");


        }
        catch (Exception ex)
        {
            TempData["errorAttachments"] = "Error backup attachments!";

            _logger.LogError(ex, "An error ocurred making attachments backup Index. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return View("Restore");
        }
    }
    
    [HttpPost]
    public IActionResult RestoreData()
    {
        try
        {
            var file = Request.Form.Files["uploadData"];
            
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Temp");
            var uniqueName = Guid.NewGuid().ToString() + "_" + file.FileName;

            if (Directory.Exists(uploads))
            {
                using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            else
            {
                Directory.CreateDirectory(uploads);
                using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }

            using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            
            
            using FileStream openStream = System.IO.File.OpenRead(Path.Combine(uploads, uniqueName));
            BackupViewModel backupViewModel = JsonSerializer.Deserialize<BackupViewModel>(openStream);

            if (backupViewModel.Users != null)
            {
                foreach (var user in backupViewModel.Users)
                {
                    if (user.Email == "admin@cervantes.local")
                    {
                        
                        var result = userManager.GetByEmail("admin@cervantes.local");
                        userManager.Remove(result);
                        userManager.Context.SaveChanges();
                        userManager.Add(user);
                        userManager.Context.SaveChanges();
                        _userManager.AddToRoleAsync(user, "Admin").Wait();
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
                    clientManager.Add(client);
                    
                }
                clientManager.Context.SaveChanges();
            }
            
            if (backupViewModel.Projects != null)
            {
                foreach (var project in backupViewModel.Projects)
                {
                    projectManager.Add(project);
                }
                projectManager.Context.SaveChanges();

            }
            
            if (backupViewModel.ProjectAttachments != null)
            {
                foreach (var attachment in backupViewModel.ProjectAttachments)
                {
                    projectAttachmentManager.Add(attachment);
                }
                projectAttachmentManager.Context.SaveChanges();

            }
            
            if (backupViewModel.ProjectNotes != null)
            {
                foreach (var note in backupViewModel.ProjectNotes)
                {
                    projectNoteManager.Add(note);
                }
                projectNoteManager.Context.SaveChanges();

            }
            
            if (backupViewModel.ProjectUsers != null)
            {
                foreach (var user in backupViewModel.ProjectUsers)
                {
                    projectUserManager.Add(user);
                }
                projectUserManager.Context.SaveChanges();

            }
            
            if (backupViewModel.Documents != null)
            {
                foreach (var doc in backupViewModel.Documents)
                {
                    documentManager.Add(doc);
                    
                }
                documentManager.Context.SaveChanges();
            }
            
            if (backupViewModel.Notes != null)
            {
                foreach (var note in backupViewModel.Notes)
                {
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
                    reportManager.Add(rep);
                }
                reportManager.Context.SaveChanges();

            }
            
            if (backupViewModel.Targets != null)
            {
                foreach (var target in backupViewModel.Targets)
                {
                    targetManager.Add(target);
                }
                targetManager.Context.SaveChanges();

            }
            
            if (backupViewModel.TargetServices != null)
            {
                foreach (var services in backupViewModel.TargetServices)
                {
                    targetServicesManager.Add(services);
                }
                targetServicesManager.Context.SaveChanges();

            }
            
            if (backupViewModel.Tasks != null)
            {
                foreach (var task in backupViewModel.Tasks)
                {
                    taskManager.Add(task);
                }
                taskManager.Context.SaveChanges();

            }
            
            if (backupViewModel.TaskAttachments != null)
            {
                foreach (var att in backupViewModel.TaskAttachments)
                {
                    taskAttachmentManager.Add(att);
                }
                taskAttachmentManager.Context.SaveChanges();

            }
            
            if (backupViewModel.TaskNotes != null)
            {
                foreach (var note in backupViewModel.TaskNotes)
                {
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
                    vulnManager.Add(vulns);
                }
                vulnManager.Context.SaveChanges();

            }
            
            if (backupViewModel.VulnAttachments != null)
            {
                foreach (var att in backupViewModel.VulnAttachments)
                {
                    vulnAttachmentManager.Add(att);
                }
                vulnAttachmentManager.Context.SaveChanges();

            }
            
            if (backupViewModel.VulnNotes != null)
            {
                foreach (var note in backupViewModel.VulnNotes)
                {
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
                    wstgManager.Add(wstg);
                }
                wstgManager.Context.SaveChanges();
            }
            
            if (backupViewModel.Mastgs != null)
            {
                foreach (var mastg in backupViewModel.Mastgs)
                {
                    mastgManager.Add(mastg);
                }
                mastgManager.Context.SaveChanges();
            }

            
            TempData["restoredData"] = "restored";
            System.IO.File.Delete(Path.Combine(uploads, uniqueName));
            Directory.Delete(uploads);
            return View("Restore");
        }
        catch (Exception ex)
        {
            TempData["errorDataRestore"] = "Error backup attachments!";

            _logger.LogError(ex, "An error ocurred making attachments backup Index. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Restore");
        }
    }
    
}