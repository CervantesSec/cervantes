using System.Security.Claims;
using System.Text;
using System.Web;
using AuthPermissions.AspNetCore;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.File;
using Cervantes.IFR.Jira;
using Cervantes.IFR.Parsers.Acunetix;
using Cervantes.IFR.Parsers.Bandit;
using Cervantes.IFR.Parsers.Burp;
using Cervantes.IFR.Parsers.CSV;
using Cervantes.IFR.Parsers.DependencyCheck;
using Cervantes.IFR.Parsers.Masscan;
using Cervantes.IFR.Parsers.Nessus;
using Cervantes.IFR.Parsers.Nikto;
using Cervantes.IFR.Parsers.Nuclei;
using Cervantes.IFR.Parsers.OpenVAS;
using Cervantes.IFR.Parsers.Prowler;
using Cervantes.IFR.Parsers.Pwndoc;
using Cervantes.IFR.Parsers.Qualys;
using Cervantes.IFR.Parsers.Trivy;
using Cervantes.IFR.Parsers.Zap;
using Cervantes.Server.Helpers;
using Cervantes.Web.Helpers;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cervantes.Web.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VulnController: ControllerBase
{
     private IVulnManager vulnManager = null;
    private IProjectManager projectManager = null;
    private IProjectUserManager projectUserManager = null;
    private ITargetManager targetManager = null;
    private IVulnCategoryManager vulnCategoryManager = null;
    private IVulnNoteManager vulnNoteManager = null;
    private IVulnAttachmentManager vulnAttachmentManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private IJIraService jiraService = null;
    private IJiraManager jiraManager = null;
    private IJiraCommentManager jiraCommentManager = null;
    private IProjectAttachmentManager projectAttachmentManager = null;
    private ICsvParser csvParser = null;
    private IPwndocParser pwndocParser = null;
    private IBurpParser burpParser = null;
    private INessusParser nessusParser = null;
    private IZapParser zapParser = null;
    private IOpenVASParser openvasParser = null;
    private IQualysParser qualysParser = null;
    private INucleiParser nucleiParser = null;
    private IAcunetixParser acunetixParser = null;
    private INiktoParser niktoParser = null;
    private IProwlerParser prowlerParser = null;
    private ITrivyParser trivyParser = null;
    private IBanditParser banditParser = null;
    private IMasscanParser masscanParser = null;
    private IDependencyCheckParser dependencyCheckParser = null;
    private ICweManager cweManager = null;
    private IVulnCweManager vulnCweManager = null;
    private IVulnCustomFieldValueManager vulnCustomFieldValueManager = null;
    private readonly ILogger<VulnController> _logger = null;
    private readonly IWebHostEnvironment env;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private IFileCheck fileCheck;
    private Sanitizer Sanitizer;

    public VulnController(IVulnManager vulnManager, IProjectManager projectManager,
        ITargetManager targetManager, IVulnTargetManager vulnTargetManager,
        IVulnCategoryManager vulnCategoryManager, IVulnNoteManager vulnNoteManager,
        IVulnAttachmentManager vulnAttachmentManager, IJIraService jiraService,
        IJiraManager jiraManager, IJiraCommentManager jiraCommentManager, IProjectAttachmentManager projectAttachmentManager,
        ICsvParser csvParser, IPwndocParser pwndocParser, IBurpParser burpParser, INessusParser nessusParser,
        IZapParser zapParser, IOpenVASParser openvasParser, IQualysParser qualysParser,
        INucleiParser nucleiParser, IAcunetixParser acunetixParser,
        INiktoParser niktoParser, IProwlerParser prowlerParser, ITrivyParser trivyParser,
        IBanditParser banditParser, IMasscanParser masscanParser, IDependencyCheckParser dependencyCheckParser,
        ICweManager cweManager, IVulnCweManager vulnCweManager, IVulnCustomFieldValueManager vulnCustomFieldValueManager, ILogger<VulnController> logger, IWebHostEnvironment env,IHttpContextAccessor HttpContextAccessor,
        IFileCheck fileCheck, IProjectUserManager projectUserManager,Sanitizer Sanitizer)
    {
        this.vulnManager = vulnManager;
        this.projectManager = projectManager;
        this.targetManager = targetManager;
        this.vulnCategoryManager = vulnCategoryManager;
        this.vulnAttachmentManager = vulnAttachmentManager;
        this.vulnNoteManager = vulnNoteManager;
        this.vulnTargetManager = vulnTargetManager;
        this.jiraService = jiraService;
        this.jiraManager = jiraManager;
        this.jiraCommentManager = jiraCommentManager;
        this.projectAttachmentManager = projectAttachmentManager;
        this.csvParser = csvParser;
        this.pwndocParser = pwndocParser;
        this.burpParser = burpParser;
        this.nessusParser = nessusParser;
        this.zapParser = zapParser;
        this.openvasParser = openvasParser;
        this.qualysParser = qualysParser;
        this.nucleiParser = nucleiParser;
        this.acunetixParser = acunetixParser;
        this.niktoParser = niktoParser;
        this.prowlerParser = prowlerParser;
        this.trivyParser = trivyParser;
        this.banditParser = banditParser;
        this.masscanParser = masscanParser;
        this.dependencyCheckParser = dependencyCheckParser;
        this.cweManager = cweManager;
        this.vulnCweManager = vulnCweManager;
        this.vulnCustomFieldValueManager = vulnCustomFieldValueManager;
        _logger = logger;
        this.env = env;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        this.fileCheck = fileCheck;
        this.projectUserManager = projectUserManager;
        this.Sanitizer = Sanitizer;
    }
    
    [HasPermission(Permissions.VulnsRead)]
    [HttpGet]
    [Route("Client/{id}")]
    
    public IEnumerable<CORE.Entities.Vuln> GetByClientId(Guid id)
    {
        try
        {
            var ids = projectManager.GetAll().Where(x => x.ClientId == id).Select(x => x.Id).ToList();
            List<CORE.Entities.Vuln> model = new List<CORE.Entities.Vuln>();
            foreach (var pro in ids)
            {
                var vulns = vulnManager.GetAll().Where(x => x.ProjectId == pro).Include(x => x.Project).ToList();
                foreach (var vuln in vulns)
                {
                    model.Add(vuln);
                }
            
            }
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting client vulns. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HasPermission(Permissions.VulnsRead)]
    [HttpGet]
    [Route("{id}")]
    public CORE.Entities.Vuln GetById(Guid id)
    {
        try
        {
            var model = vulnManager.GetById(id);
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting the vuln. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HasPermission(Permissions.VulnsRead)]
    [HttpGet]
    [Route("Project/{id}")]
    public IEnumerable<CORE.Entities.Vuln> GetByProject(Guid id)
    {
        try
        {
            IEnumerable<CORE.Entities.Vuln> model = vulnManager.GetAll().Where(x => x.ProjectId == id).Include(x=> x.VulnCategory).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting project vulns. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpGet]
    [Route("Categories")]
    [HasPermission(Permissions.VulnCategoriesRead)]
    public IEnumerable<CORE.Entities.VulnCategory> GetCategories()
    {
        try
        {
            IEnumerable<CORE.Entities.VulnCategory> model = vulnCategoryManager.GetAll().ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting vuln categories. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpGet]
    [Route("Cwe")]
    [HasPermission(Permissions.VulnsRead)]
    public IEnumerable<CORE.Entities.Cwe> GetCwes()
    {
        try
        {
            IEnumerable<CORE.Entities.Cwe> model = cweManager.GetAll().ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting CWEs. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpGet]
    [Route("Templates")]
    [HasPermission(Permissions.VulnsRead)]
    public IEnumerable<CORE.Entities.Vuln> GetTemplates()
    {
        try
        {
            IEnumerable<CORE.Entities.Vuln> model = vulnManager.GetAll().Where(x => x.Template == true).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting vuln templates. User: {0}",
                aspNetUserId);
            throw;
        }
       
    }
    
    [HttpGet]
    [HasPermission(Permissions.VulnsRead)]
    public IEnumerable<CORE.Entities.Vuln> GetVulns()
    {
        try
        {
            IEnumerable<CORE.Entities.Vuln> model = vulnManager.GetAll().Include(x => x.VulnCategory).Include(X => X.Project).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting vulns. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [NonAction]
    public CORE.Entities.Vuln GetVulnById(Guid vulnId)
    {
        try
        {
            return vulnManager.GetById(vulnId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting vulns. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }

    [HttpPost]
    [HasPermission(Permissions.VulnsAdd)]
    public async Task<IActionResult> Add([FromBody] VulnCreateViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (model.ProjectId != null && model.ProjectId != Guid.Empty)
                {
                    var user = projectUserManager.VerifyUser(model.ProjectId.Value, aspNetUserId);
                    if (user == null)
                    {
                        return Forbid("You do not have permission to access this project");
                    }
                }
                
                var vuln = new Vuln();
                vuln.Template = model.Template;
                vuln.UserId = aspNetUserId;
                vuln.ProjectId = model.ProjectId;
                if (model.ProjectId != null)
                {
                    var number = vulnManager.GetAll().Where(x => x.ProjectId == model.ProjectId).Count() + 1;
                    var fin = projectManager.GetById(model.ProjectId.Value).FindingsId;
                    vuln.FindingId = fin+"-"+number.ToString();
                }
                else
                {
                    vuln.FindingId = "";
                
                }
                vuln.VulnCategoryId = model.VulnCategoryId;
                vuln.Name = Sanitizer.Sanitize(model.Name);
                vuln.Description = Sanitizer.Sanitize(model.Description);
                vuln.Risk = model.Risk;
                vuln.Status = model.Status;
                vuln.Template = model.Template;
                vuln.CreatedDate = DateTime.Now.ToUniversalTime();
                vuln.ModifiedDate = DateTime.Now.ToUniversalTime();
                vuln.cve = Sanitizer.Sanitize(model.cve);
                vuln.Language = model.Language;
                vuln.Remediation = Sanitizer.Sanitize(model.Remediation);
                vuln.JiraCreated = false;
                vuln.RemediationComplexity = model.RemediationComplexity;
                vuln.RemediationPriority = model.RemediationPriority;
                vuln.ProofOfConcept = Sanitizer.Sanitize(model.ProofOfConcept);
                vuln.Impact = Sanitizer.Sanitize(model.Impact);
                vuln.CVSS3 = model.CVSS3;
                if (string.IsNullOrEmpty(model.CVSSVector))
                {
                    vuln.CVSSVector = string.Empty;

                }
                else
                {
                    vuln.CVSSVector = model.CVSSVector;
                }
                vuln.OWASPImpact = model.OWASPImpact;
                vuln.OWASPLikehood = model.OWASPLikehood;
                vuln.OWASPRisk = model.OWASPRisk;
                vuln.OWASPVector = model.OWASPVector;
                if (model.MitreTechniques != null)
                {
                    StringBuilder mt = new StringBuilder();

                    foreach (var item in model.MitreTechniques)
                    {
                        mt.Append(item + ",");
                    }
                    vuln.MitreTechniques = mt.ToString();
                }
                else
                {
                    vuln.MitreTechniques = string.Empty;
                }
                
                if (model.MitreValues != null)
                {
                    StringBuilder mv = new StringBuilder();

                    foreach (var item in model.MitreValues)
                    {
                        mv.Append(item + ",");
                    } 
                    vuln.MitreValues = mv.ToString();

                }
                else
                {
                    vuln.MitreValues = string.Empty;
                }

                await vulnManager.AddAsync(vuln);
                await vulnManager.Context.SaveChangesAsync();
                
                if (model.CweId != null)
                {
                    foreach (var cwe in model.CweId)
                    {
                        var vulnCwe = new VulnCwe();
                        vulnCwe.Id = Guid.NewGuid();
                        vulnCwe.VulnId = vuln.Id;
                        vulnCwe.CweId = cwe;
                        await vulnCweManager.AddAsync(vulnCwe);
                        await vulnCweManager.Context.SaveChangesAsync();
                    }
                }
                
                if (model.TargetId != null)
                {
                    foreach (var target in model.TargetId)
                    {
                        var vulnTarget = new VulnTargets();
                        vulnTarget.Id = Guid.NewGuid();
                        vulnTarget.VulnId = vuln.Id;
                        vulnTarget.TargetId = target;
                        await vulnTargetManager.AddAsync(vulnTarget);
                        await vulnTargetManager.Context.SaveChangesAsync();
                    }
                }
                
                // Process custom field values
                if (model.CustomFieldValues != null && model.CustomFieldValues.Any())
                {
                    // Create custom field values using basic GenericManager methods
                    foreach (var kvp in model.CustomFieldValues)
                    {
                        var customFieldValue = new VulnCustomFieldValue
                        {
                            Id = Guid.NewGuid(),
                            VulnId = vuln.Id,
                            VulnCustomFieldId = kvp.Key,
                            Value = kvp.Value,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow,
                            UserId = aspNetUserId
                        };
                        await vulnCustomFieldValueManager.AddAsync(customFieldValue);
                    }
                    await vulnCustomFieldValueManager.Context.SaveChangesAsync();
                }
  
                _logger.LogInformation("Vuln added successfully. User: {0}",
                    aspNetUserId);
                return Created($"/api/Vuln/{vuln.Id}", vuln);
            }

            _logger.LogError("Validation failed when adding a Vuln. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred adding a Vuln. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while creating the vulnerability. Please try again later.");
        }
    }
    
    [HttpPut]
    [HasPermission(Permissions.VulnsEdit)]
    public async Task<IActionResult> Edit([FromBody] VulnCreateViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (model.ProjectId != null && model.ProjectId != Guid.Empty)
                {
                    var user = projectUserManager.VerifyUser(model.ProjectId.Value, aspNetUserId);
                    if (user == null)
                    {
                        return Forbid("You do not have permission to access this project");
                    }
                }

                var vuln = vulnManager.GetById(model.Id);
                if (vuln.Id != Guid.Empty)
                {
                vuln.ProjectId = model.ProjectId;
                vuln.VulnCategoryId = model.VulnCategoryId;
                vuln.Name = Sanitizer.Sanitize(model.Name);
                vuln.Description = Sanitizer.Sanitize(model.Description);
                vuln.Risk = model.Risk;
                vuln.Status = model.Status;
                vuln.Template = model.Template;
                vuln.CreatedDate = DateTime.Now.ToUniversalTime();
                vuln.ModifiedDate = DateTime.Now.ToUniversalTime();
                vuln.cve = Sanitizer.Sanitize(model.cve);
                vuln.Language = model.Language;
                vuln.Remediation = Sanitizer.Sanitize(model.Remediation);
                vuln.JiraCreated = false;
                vuln.RemediationComplexity = model.RemediationComplexity;
                vuln.RemediationPriority = model.RemediationPriority;
                vuln.ProofOfConcept = Sanitizer.Sanitize(model.ProofOfConcept);
                vuln.Impact = Sanitizer.Sanitize(model.Impact);
                vuln.CVSS3 = model.CVSS3;
                if (string.IsNullOrEmpty(model.CVSSVector))
                {
                    vuln.CVSSVector = string.Empty;

                }
                else
                {
                    vuln.CVSSVector = model.CVSSVector;
                }
                vuln.OWASPImpact = model.OWASPImpact;
                vuln.OWASPLikehood = model.OWASPLikehood;
                vuln.OWASPRisk = model.OWASPRisk;
                vuln.OWASPVector = model.OWASPVector;
                if (model.MitreTechniques != null)
                {
                    StringBuilder mt = new StringBuilder();

                    foreach (var item in model.MitreTechniques)
                    {
                        mt.Append(item + ",");
                    }
                    vuln.MitreTechniques = mt.ToString();
                }
                else
                {
                    vuln.MitreTechniques = string.Empty;
                }
                
                if (model.MitreValues != null)
                {
                    StringBuilder mv = new StringBuilder();

                    foreach (var item in model.MitreValues)
                    {
                        mv.Append(item + ",");
                    } 
                    vuln.MitreValues = mv.ToString();

                }
                else
                {
                    vuln.MitreValues = string.Empty;
                }
                await vulnManager.Context.SaveChangesAsync();
                
                if (model.CweId != null)
                {
                    var cwes = vulnCweManager.GetAll().Where(x => x.VulnId == vuln.Id);
                    foreach (var cwe in cwes)
                    {
                        vulnCweManager.Remove(cwe);
                    }
                    await vulnCweManager.Context.SaveChangesAsync();
                    foreach (var cwe in model.CweId)
                    {
                        var vulnCwe = new VulnCwe();
                        vulnCwe.Id = Guid.NewGuid();
                        vulnCwe.VulnId = vuln.Id;
                        vulnCwe.CweId = cwe;
                        await vulnCweManager.AddAsync(vulnCwe);
                        
                    }
                    await vulnCweManager.Context.SaveChangesAsync();
                }
                
                if (model.TargetId != null)
                {
                    var targets = vulnTargetManager.GetAll().Where(x => x.VulnId == vuln.Id);
                    foreach (var tar in targets)
                    {
                        vulnTargetManager.Remove(tar);
                    }
                    await vulnTargetManager.Context.SaveChangesAsync();

                    foreach (var target in model.TargetId)
                    {
                        var vulnTarget = new VulnTargets();
                        vulnTarget.Id = Guid.NewGuid();
                        vulnTarget.VulnId = vuln.Id;
                        vulnTarget.TargetId = target;
                        await vulnTargetManager.AddAsync(vulnTarget);
                    }
                    await vulnTargetManager.Context.SaveChangesAsync();

                }

                // Handle custom field values
                if (model.CustomFieldValues != null && model.CustomFieldValues.Any())
                {
                    // Remove existing custom field values for this vulnerability
                    var existingValues = vulnCustomFieldValueManager.GetAll().Where(x => x.VulnId == vuln.Id);
                    foreach (var existingValue in existingValues)
                    {
                        vulnCustomFieldValueManager.Remove(existingValue);
                    }
                    await vulnCustomFieldValueManager.Context.SaveChangesAsync();

                    // Add new custom field values
                    foreach (var customFieldValue in model.CustomFieldValues)
                    {
                        if (!string.IsNullOrEmpty(customFieldValue.Value))
                        {
                            var vulnCustomFieldValue = new VulnCustomFieldValue
                            {
                                Id = Guid.NewGuid(),
                                VulnId = vuln.Id,
                                VulnCustomFieldId = customFieldValue.Key,
                                Value = Sanitizer.Sanitize(customFieldValue.Value),
                                UserId = aspNetUserId
                            };
                            await vulnCustomFieldValueManager.AddAsync(vulnCustomFieldValue);
                        }
                    }
                    await vulnCustomFieldValueManager.Context.SaveChangesAsync();
                }
  
                _logger.LogInformation("Vuln edited successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
                }
                
                _logger.LogError("Vulnerability not found for editing. User: {0}",
                    aspNetUserId);
                return NotFound("Vulnerability not found");
            }

            _logger.LogError("Validation failed when editing a Vuln. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred editing a Vuln. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while updating the vulnerability. Please try again later.");
        }
    }
    
    [HttpGet]
    [Route("Cwe/{id}")]
    [HasPermission(Permissions.VulnsRead)]
    public IEnumerable<CORE.Entities.VulnCwe> GetVulnCwes(Guid id)
    {
        try
        {
            IEnumerable<CORE.Entities.VulnCwe> model = vulnCweManager.GetAll().Where(x =>  x.VulnId == id).Include(x => x.Cwe).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting a CWE. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpGet]
    [Route("Targets/{id}")]
    [HasPermission(Permissions.VulnTargetsRead)]
    public IEnumerable<CORE.Entities.VulnTargets> GetVulnTargets(Guid id)
    {
        try
        {
            IEnumerable<CORE.Entities.VulnTargets> model = vulnTargetManager.GetAll().Where(x =>  x.VulnId == id).Include(x => x.Target).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting a Vuln targets. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpDelete]
    [Route("{id}")]
    [HasPermission(Permissions.VulnsDelete)]
    public async Task<IActionResult> DeleteVuln(Guid id)
    {
        try
        {
            var vuln = vulnManager.GetById(id);
            
            if (vuln.Name != null)
            {
                if (vuln.ProjectId != null && vuln.ProjectId != Guid.Empty)
                {
                    var user = projectUserManager.VerifyUser(vuln.ProjectId.Value, aspNetUserId);
                    if (user == null)
                    {
                        return Forbid("You do not have permission to access this project");
                    }
                }
                
                 vulnManager.Remove(vuln);
                 await vulnManager.Context.SaveChangesAsync();
                _logger.LogInformation("Vuln deleted successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
            }
   
            _logger.LogError("An error ocurred deleting a Vuln. User: {0}",
                aspNetUserId);
            return NotFound("Vulnerability not found or cannot be deleted");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting a Vuln. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while deleting the vulnerability. Please try again later.");
        }
    }
    
    [HttpGet]
    [Route("Notes/{id}")]
    [HasPermission(Permissions.VulnNotesRead)]
    public IEnumerable<CORE.Entities.VulnNote> GetVulnNotes(Guid id)
    {
        try
        {
            IEnumerable<CORE.Entities.VulnNote> model = vulnNoteManager.GetAll().Where(x =>  x.VulnId == id).Include(x => x.User).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting a Vuln notes. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpGet]
    [Route("Attachments/{id}")]
    [HasPermission(Permissions.VulnAttachmentsRead)]
    public IEnumerable<CORE.Entities.VulnAttachment> GetVulnAttachments(Guid id)
    {
        try
        {
            IEnumerable<CORE.Entities.VulnAttachment> model = vulnAttachmentManager.GetAll().Where(x =>  x.VulnId == id).Include(x => x.User).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting a Vuln attachments. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [NonAction]
    public CORE.Entities.VulnNote GetVulnNoteById(Guid vulnNoteId)
    {
        try
        {
            return vulnNoteManager.GetById(vulnNoteId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting a Vuln attachments. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [NonAction]
    public CORE.Entities.VulnAttachment GetVulnAttachmentById(Guid vulnAttachmentId)
    {
        try
        {
            return vulnAttachmentManager.GetById(vulnAttachmentId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting a Vuln attachments. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpPost]
    [Route("Note")]
    [HasPermission(Permissions.VulnNotesAdd)]
    public async Task<IActionResult> AddVulnNote([FromBody] VulnNoteViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var vuln = vulnManager.GetById(model.VulnId);
                if (vuln != null)
                {
                    if (vuln.Project != null)
                    {
                        var user = projectUserManager.VerifyUser(vuln.ProjectId.Value, aspNetUserId);
                        if (user == null)
                        {
                            return Forbid("You do not have permission to access this project");
                        }
                    }
                }
                
                var note = new VulnNote();
                note.Id = Guid.NewGuid();
                note.Name = Sanitizer.Sanitize(model.Name);
                note.Description = Sanitizer.Sanitize(model.Description);
                note.VulnId = model.VulnId;
                note.Visibility = Visibility.Public;
                note.UserId = aspNetUserId;

                await vulnNoteManager.AddAsync(note);
                await vulnNoteManager.Context.SaveChangesAsync();
  
                _logger.LogInformation("Vuln Note added successfully. User: {0}",
                    aspNetUserId);
                return Created($"/api/Vuln/Note/{note.Id}", note);
            }

            _logger.LogError("An error ocurred adding a Vuln Note. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding a Vuln Note. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while adding the vulnerability note. Please try again later.");
        }
    }
    
    [HttpDelete]
    [Route("Note/{id}")]
    [HasPermission(Permissions.VulnNotesDelete)]
    public async Task<IActionResult> DeleteVulnNote(Guid id)
    {
        try
        {
            var note = vulnNoteManager.GetById(id);
            
            if (note.Name != null)
            {
                var vuln = vulnManager.GetById(note.VulnId);
                if (vuln != null)
                {
                    if (vuln.Project != null)
                    {
                        var user = projectUserManager.VerifyUser(vuln.ProjectId.Value, aspNetUserId);
                        if (user == null)
                        {
                            return Forbid("You do not have permission to access this project");
                        }
                    }
                }
                
                vulnNoteManager.Remove(note);
                await vulnNoteManager.Context.SaveChangesAsync();
                _logger.LogInformation("Vuln Note deleted successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
            }
   
            _logger.LogError("An error ocurred deleting a Vuln Note. User: {0}",
                aspNetUserId);
            return NotFound("Vulnerability note not found or cannot be deleted");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting a Vuln Note. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while deleting the vulnerability note. Please try again later.");
        }
    }
    
    [HttpPut]
    [Route("Note")]
    [HasPermission(Permissions.VulnNotesEdit)]
    public async Task<IActionResult> EditNote([FromBody] VulnNoteEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var vuln = vulnManager.GetById(model.VulnId);
                if (vuln != null)
                {
                    if (vuln.Project != null)
                    {
                        var user = projectUserManager.VerifyUser(vuln.ProjectId.Value, aspNetUserId);
                        if (user == null)
                        {
                            return Forbid("You do not have permission to access this project");
                        }
                    }
                }
                
                var note = vulnNoteManager.GetById(model.Id);
                note.Id = model.Id;
                note.VulnId = model.VulnId;
                note.Name = Sanitizer.Sanitize(model.Name);
                note.Description = Sanitizer.Sanitize(model.Description);
                await vulnNoteManager.Context.SaveChangesAsync();
                _logger.LogInformation("Vuln Note edited successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
            }
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred editing a Vuln Note. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while updating the vulnerability note. Please try again later.");
        }
    }
    
    [HttpPost]
    [Route("Target")]
    [HasPermission(Permissions.VulnTargetsAdd)]
    public async Task<IActionResult> AddVulnTarget([FromBody] VulnTargetViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var vuln = vulnManager.GetById(model.VulnId);
                if (vuln != null)
                {
                    if (vuln.Project != null)
                    {
                        var user = projectUserManager.VerifyUser(vuln.ProjectId.Value, aspNetUserId);
                        if (user == null)
                        {
                            return Forbid("You do not have permission to access this project");
                        }
                    }
                }
                
                var result = vulnTargetManager.GetAll().Where(x => x.TargetId == model.TargetId && x.VulnId == model.VulnId);

                
                if (result.FirstOrDefault() == null)
                {
                    var tar = new VulnTargets();
                    tar.Id = Guid.NewGuid();
                    tar.TargetId = model.TargetId;
                    tar.VulnId = model.VulnId;


                    await vulnTargetManager.AddAsync(tar);
                    await vulnTargetManager.Context.SaveChangesAsync();

                    _logger.LogInformation("Vuln target added successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }
                else
                {
                    return Ok("TargetExists");
                }
                
            }

            _logger.LogError("An error ocurred adding a Vuln target. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding a Vuln target. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while adding the vulnerability target. Please try again later.");
        }
    }
    [HttpDelete]
    [Route("Target/{id}")]
    [HasPermission(Permissions.VulnTargetsDelete)]
    public async Task<IActionResult> DeleteVulnTarget(Guid id)
    {
        try
        {
            var tar = vulnTargetManager.GetById(id);
            
            if (tar != null)
            {
  
                    if (tar.Vuln.Project != null)
                    {
                        var user = projectUserManager.VerifyUser(tar.Vuln.ProjectId.Value, aspNetUserId);
                        if (user == null)
                        {
                            return Forbid("You do not have permission to access this project");
                        }
                    }
                
                vulnTargetManager.Remove(tar);
                await vulnTargetManager.Context.SaveChangesAsync();
                _logger.LogInformation("Vuln Target deleted successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
            }
   
            _logger.LogError("An error ocurred deleting a Vuln Target. User: {0}",
                aspNetUserId);
            return NotFound("Vulnerability target not found or cannot be deleted");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting a Vuln Target. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while deleting the vulnerability target. Please try again later.");
        }
    }
    
    [HttpPost]
    [Route("Attachment")]
    [HasPermission(Permissions.VulnAttachmentsAdd)]
    public async Task<IActionResult> AddAttachment(VulnAttachmentViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var vuln = vulnManager.GetById(model.VulnId);
                if (vuln != null)
                {
                    if (vuln.Project != null)
                    {
                        var user = projectUserManager.VerifyUser(vuln.ProjectId.Value, aspNetUserId);
                        if (user == null)
                        {
                            return Forbid("You do not have permission to access this project");
                        }
                    }
                }
                
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedSchemes.Add("data");
                var path = "";
                var unique = "";
                var file = "";
                if (model.FileContent != null)
                {
                    if (fileCheck.CheckFile(model.FileContent))
                    {
                        unique = Guid.NewGuid().ToString()+"."+fileCheck.GetExtension(model.FileContent);
                        path = $"{env.WebRootPath}/Attachments/Vuln/{model.VulnId}";
                        file = $"{env.WebRootPath}/Attachments/Vuln/{model.VulnId}/{unique}";
                        if (Directory.Exists(path))
                        {
                            var fs = System.IO.File.Create(file);
                            fs.Write(model.FileContent, 0,
                                model.FileContent.Length);
                            fs.Close();
                        }
                        else
                        {
                            Directory.CreateDirectory(path);
                            var fs = System.IO.File.Create(file);
                            fs.Write(model.FileContent, 0,
                                model.FileContent.Length);
                            fs.Close();
                        }
                    }
                    else
                    {
                        _logger.LogError("An error ocurred adding a Vuln Attachment filetype not admitted. User: {0}",
                            aspNetUserId);
                        return BadRequest("Invalid file type");
                    }
                    
                    
                }

                var attachment = new CORE.Entities.VulnAttachment();
                attachment.Id = Guid.NewGuid();
                attachment.VulnId = model.VulnId;
                attachment.UserId = aspNetUserId;
                attachment.Name = Sanitizer.Sanitize(model.Name);
                attachment.FilePath = "Attachments/Vuln/" + model.VulnId.ToString() + "/" + unique;

                await vulnAttachmentManager.AddAsync(attachment);
                await vulnAttachmentManager.Context.SaveChangesAsync();
                _logger.LogInformation("Vuln Attachment deleted successfully. User: {0}",
                    aspNetUserId);
                return Created($"/api/Vuln/Attachment/{attachment.Id}", attachment);
            }
            _logger.LogError("An error ocurred adding a Vuln Attachment. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding a Vuln Attachment. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while adding the vulnerability attachment. Please try again later.");
        }
    }
    [HttpDelete]
    [Route("Attachment/{id}")]
    [HasPermission(Permissions.VulnAttachmentsDelete)]
    public async Task<IActionResult> DeleteAttahcment(Guid id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var attachment = vulnAttachmentManager.GetById(id);
                if (attachment.Id != Guid.Empty)
                {
                    if (attachment.Vuln.Project != null)
                    {
                        var user = projectUserManager.VerifyUser(attachment.Vuln.ProjectId.Value, aspNetUserId);
                        if (user == null)
                        {
                            return Forbid("You do not have permission to access this project");
                        }
                    }
                    
                    vulnAttachmentManager.Remove(attachment);
                    await vulnAttachmentManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Vuln Attachment deleted successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
                }
            
                System.IO.File.Delete(attachment.FilePath);
                _logger.LogError("An error ocurred deleting a Vuln Attachment. User: {0}",
                    aspNetUserId);
                return NotFound("Vulnerability attachment not found or cannot be deleted");
            
            }
            _logger.LogError("An error ocurred deleting a Vuln Attachment. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error ocurred deleting a Vuln Attachment. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while deleting the vulnerability attachment. Please try again later.");
        }
        
    }
    
    [HttpPost]
    [Route("Import")]
    [HasPermission(Permissions.VulnsImport)]
    public async Task<IActionResult> Import(VulnImportViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (model.Project != null && model.Project != Guid.Empty)
                {
                    var user = projectUserManager.VerifyUser(model.Project.Value, aspNetUserId);
                    if (user == null)
                    {
                        return Forbid("You do not have permission to access this project");
                    }
                }
                
                var path = "";
                var unique = "";
                var file = "";
                if (model.FileContent != null)
                {
                    if (fileCheck.CheckFile(model.FileContent))
                    {
                        unique = Guid.NewGuid().ToString()+"."+fileCheck.GetExtension(model.FileContent);
                        path = $"{env.WebRootPath}/Attachments/Imports";
                        file = $"{env.WebRootPath}/Attachments/Imports/{unique}";
                        if (Directory.Exists(path))
                        {
                            var fs = System.IO.File.Create(file);
                            fs.Write(model.FileContent, 0,
                                model.FileContent.Length);
                            fs.Close();
                        }
                        else
                        {
                            Directory.CreateDirectory(path);
                            var fs = System.IO.File.Create(file);
                            fs.Write(model.FileContent, 0,
                                model.FileContent.Length);
                            fs.Close();
                        }

                    }
                    else
                    {
                        _logger.LogError("An error ocurred importing vulns. User: {{0}}\"",
                            aspNetUserId);
                        return BadRequest("Invalid file type");
                    }
                    
                    
                    if (model.Project != null)
                    {
                        switch (model.Type)
                        {
                            case VulnImportType.CSV:
                                csvParser.Parse(model.Project, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Pwndoc:
                                pwndocParser.Parse(model.Project, aspNetUserId,file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Burp:
                                burpParser.Parse(model.Project, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Nessus:
                                nessusParser.Parse(model.Project, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Zap:
                                zapParser.Parse(model.Project, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.OpenVAS:
                                openvasParser.Parse(model.Project, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Qualys:
                                qualysParser.Parse(model.Project, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Nuclei:
                                nucleiParser.Parse(model.Project, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Acunetix:
                                acunetixParser.Parse(model.Project, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Nikto:
                                niktoParser.Parse(model.Project, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Prowler:
                                prowlerParser.Parse(model.Project, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Trivy:
                                trivyParser.Parse(model.Project, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Bandit:
                                banditParser.Parse(model.Project, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Masscan:
                                masscanParser.Parse(model.Project, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.DependencyCheck:
                                dependencyCheckParser.Parse(model.Project, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                        }
                    }
                    else
                    {
                        switch (model.Type)
                        {
                            case VulnImportType.CSV:
                                csvParser.Parse(null, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Pwndoc:
                                pwndocParser.Parse(null, aspNetUserId,file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Burp:
                                burpParser.Parse(null, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Nessus:
                                nessusParser.Parse(null, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Zap:
                                zapParser.Parse(null, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.OpenVAS:
                                openvasParser.Parse(null, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Qualys:
                                qualysParser.Parse(null, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Nuclei:
                                nucleiParser.Parse(null, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Acunetix:
                                acunetixParser.Parse(null, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Nikto:
                                niktoParser.Parse(null, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Prowler:
                                prowlerParser.Parse(null, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Trivy:
                                trivyParser.Parse(null, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Bandit:
                                banditParser.Parse(null, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.Masscan:
                                masscanParser.Parse(null, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                            case VulnImportType.DependencyCheck:
                                dependencyCheckParser.Parse(null, aspNetUserId,
                                    file);
                                _logger.LogInformation("Vulns imported successfully. User: {0}",
                                    aspNetUserId);
                                return NoContent();
                        }
                    }
                    
                    
                }

                _logger.LogError("An error ocurred importing vulns. User: {0}",
                    aspNetUserId);
                return BadRequest("No file content provided or invalid import type");
            }
            _logger.LogError("An error ocurred importing vulns. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred importing vulns. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while importing vulnerabilities. Please try again later.");
        }
    }
    
      [HttpPost]
      [Route("Category")]
      [HasPermission(Permissions.VulnCategoriesAdd)]
    public async Task<IActionResult> AddCategory([FromBody] VulnCategoryCreate model)
    {
        try
        {
            if (ModelState.IsValid)
            {
              
                var category = new VulnCategory();
                category.Name = Sanitizer.Sanitize(model.Name);
                category.Description = Sanitizer.Sanitize(model.Description);
                category.Type = model.Type;
                await vulnCategoryManager.AddAsync(category);
                await vulnCategoryManager.Context.SaveChangesAsync();
                _logger.LogInformation("Vuln Category added successfully. User: {0}",
                    aspNetUserId);
                return Created($"/api/Vuln/Category/{category.Id}", category);
                
            }
            _logger.LogError( "An error ocurred adding a Vuln Category. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding a Vuln Category. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while adding the vulnerability category. Please try again later.");
        }
    }

    [NonAction]
    public CORE.Entities.VulnCategory GetCategoryById(Guid categoryId)
    {
        return vulnCategoryManager.GetById(categoryId);
    }
    
    [HttpDelete]
    [Route("Category/{categoryId}")]
    [HasPermission(Permissions.VulnCategoriesDelete)]
    public async Task<IActionResult> DeleteCategory(Guid categoryId)
    {
        try
        {
            var category = vulnCategoryManager.GetById(categoryId);
            if (category != null)
            {
                vulnCategoryManager.Remove(category);
                await vulnCategoryManager.Context.SaveChangesAsync();
                _logger.LogInformation("Vuln Category deleted successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
                
            }
            _logger.LogError( "An error ocurred deleting a Vuln Category. User: {0}",
                aspNetUserId);
            return NotFound("Vulnerability category not found or cannot be deleted");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting a Vuln Category. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while deleting the vulnerability category. Please try again later.");
        }
    }
    
    [HttpPut]
    [Route("Category")]
    [HasPermission(Permissions.VulnCategoriesEdit)]
    public async Task<IActionResult> EditCategory([FromBody] VulnCategoryEdit model)
    {
        try
        {
            var category = vulnCategoryManager.GetById(model.Id);
            if (category != null)
            {
                var sanitizer = new HtmlSanitizer();
              
                category.Name = Sanitizer.Sanitize(model.Name);
                category.Description = Sanitizer.Sanitize(model.Description);
                category.Type = model.Type;
                
                await vulnCategoryManager.Context.SaveChangesAsync();
                _logger.LogInformation("Vuln Category edited successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
                
            }
            _logger.LogError( "An error ocurred editing a Vuln Category. User: {0}",
                aspNetUserId);
            return NotFound("Vulnerability category not found or cannot be updated");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing a Vuln Category. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while updating the vulnerability category. Please try again later.");
        }
    }
    
    [HttpPost]
    [Route("UpdateStatus")]
    [HasPermission(Permissions.VulnsEdit)]
    public async Task<IActionResult> VulnStatusUpdate([FromBody] VulnStatusUpdate model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                foreach (var item in model.VulnIds)
                {
                    var vuln = vulnManager.GetById(item);
                    if (vuln != null)
                    {
                        vuln.Status = model.Status;
                    }
                }
                return NoContent();
            }

            _logger.LogError("An error ocurred updating a Vuln . User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred updating a Vuln. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while adding the vulnerability note. Please try again later.");
        }
    }
    
}