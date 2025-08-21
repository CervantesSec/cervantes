using System.Security.Claims;
using AuthPermissions.AspNetCore;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.CveServices;
using Cervantes.Web.Helpers;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CveController : ControllerBase
{
    private readonly ICveManager _cveManager;
    private readonly ICveSubscriptionManager _cveSubscriptionManager;
    private readonly ICveNotificationManager _cveNotificationManager;
    private readonly ICveSyncService _cveSyncService;
    private readonly IProjectManager _projectManager;
    private readonly IProjectUserManager _projectUserManager;
    private readonly ILogger<CveController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Sanitizer _sanitizer;
    private readonly string _aspNetUserId;

    public CveController(
        ICveManager cveManager,
        ICveSubscriptionManager cveSubscriptionManager,
        ICveNotificationManager cveNotificationManager,
        ICveSyncService cveSyncService,
        IProjectManager projectManager,
        IProjectUserManager projectUserManager,
        ILogger<CveController> logger,
        IHttpContextAccessor httpContextAccessor,
        Sanitizer sanitizer)
    {
        _cveManager = cveManager;
        _cveSubscriptionManager = cveSubscriptionManager;
        _cveNotificationManager = cveNotificationManager;
        _cveSyncService = cveSyncService;
        _projectManager = projectManager;
        _projectUserManager = projectUserManager;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _sanitizer = sanitizer;
        _aspNetUserId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    #region CVE Management

    /// <summary>
    /// Get all CVEs with pagination
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paginated list of CVEs</returns>
    [HttpGet]
    [HasPermission(Permissions.CveRead)]
    public async Task<ActionResult<IEnumerable<Cve>>> GetCves(int page = 1, int pageSize = 50)
    {
        try
        {
            var cves = _cveManager.GetAll().ToList();
            var pagedCves = cves.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            _logger.LogInformation("Retrieved {Count} CVEs for page {Page}", pagedCves.Count, page);
            
            return Ok(pagedCves);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving CVEs");
            return StatusCode(500, "An error occurred while retrieving CVEs");
        }
    }

    /// <summary>
    /// Get CVE by ID
    /// </summary>
    /// <param name="id">CVE ID</param>
    /// <returns>CVE details</returns>
    [HttpGet("{id}")]
    [HasPermission(Permissions.CveRead)]
    public async Task<ActionResult<Cve>> GetCve(Guid id)
    {
        try
        {
            var cve = _cveManager.GetById(id);
            
            if (cve == null)
            {
                return NotFound($"CVE with ID {id} not found");
            }

            // Mark as read for the current user
            await _cveManager.MarkAsReadAsync(id, _aspNetUserId);
            
            return Ok(cve);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving CVE {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the CVE");
        }
    }

    /// <summary>
    /// Get CVE by CVE identifier
    /// </summary>
    /// <param name="cveId">CVE identifier (e.g., CVE-2023-1234)</param>
    /// <returns>CVE details</returns>
    [HttpGet("byCveId/{cveId}")]
    [HasPermission(Permissions.CveRead)]
    public async Task<ActionResult<Cve>> GetCveByCveId(string cveId)
    {
        try
        {
            var cve = await _cveManager.GetByCveIdAsync(cveId);
            
            if (cve == null)
            {
                return NotFound($"CVE {cveId} not found");
            }

            // Mark as read for the current user
            await _cveManager.MarkAsReadAsync(cve.Id, _aspNetUserId);
            
            return Ok(cve);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving CVE {CveId}", cveId);
            return StatusCode(500, "An error occurred while retrieving the CVE");
        }
    }

    /// <summary>
    /// Search CVEs with advanced criteria
    /// </summary>
    /// <param name="searchModel">Search criteria</param>
    /// <returns>List of matching CVEs</returns>
    [HttpPost("search")]
    [HasPermission(Permissions.CveRead)]
    public async Task<ActionResult<IEnumerable<Cve>>> SearchCves([FromBody] CveSearchViewModel searchModel)
    {
        try
        {
            if (searchModel == null)
            {
                return BadRequest("Search model is required");
            }

            var validationErrors = searchModel.Validate();
            if (validationErrors.Any())
            {
                return BadRequest(validationErrors);
            }

            searchModel.UserId = _aspNetUserId;
            var cves = await _cveManager.GetWithAdvancedSearchAsync(searchModel);
            
            _logger.LogInformation("Advanced search returned {Count} CVEs", cves.Count);
            
            return Ok(cves);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing CVE search");
            return StatusCode(500, "An error occurred while searching CVEs");
        }
    }

    /// <summary>
    /// Get CVE statistics
    /// </summary>
    /// <returns>CVE statistics</returns>
    [HttpGet("statistics")]
    [HasPermission(Permissions.CveRead)]
    public async Task<ActionResult<CveStatistics>> GetStatistics()
    {
        try
        {
            var statistics = await _cveManager.GetStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving CVE statistics");
            return StatusCode(500, "An error occurred while retrieving CVE statistics");
        }
    }

    /// <summary>
    /// Get CVE count by severity
    /// </summary>
    /// <returns>Dictionary of severity counts</returns>
    [HttpGet("countBySeverity")]
    [HasPermission(Permissions.CveRead)]
    public async Task<ActionResult<Dictionary<string, int>>> GetCountBySeverity()
    {
        try
        {
            var counts = await _cveManager.GetCountBySeverityAsync();
            return Ok(counts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving CVE count by severity");
            return StatusCode(500, "An error occurred while retrieving CVE count by severity");
        }
    }

    /// <summary>
    /// Get CVE count by date range
    /// </summary>
    /// <param name="days">Number of days</param>
    /// <returns>Dictionary of date counts</returns>
    [HttpGet("countByDate/{days}")]
    [HasPermission(Permissions.CveRead)]
    public async Task<ActionResult<Dictionary<DateTime, int>>> GetCountByDate(int days = 30)
    {
        try
        {
            var counts = await _cveManager.GetCountByDateRangeAsync(days);
            return Ok(counts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving CVE count by date range");
            return StatusCode(500, "An error occurred while retrieving CVE count by date range");
        }
    }

    /// <summary>
    /// Create new CVE
    /// </summary>
    /// <param name="model">CVE creation model</param>
    /// <returns>Created CVE</returns>
    [HttpPost]
    [HasPermission(Permissions.CveCreate)]
    public async Task<ActionResult<Cve>> CreateCve([FromBody] CveCreateViewModel model)
    {
        try
        {
            if (model == null)
            {
                return BadRequest("CVE model is required");
            }

            var validationErrors = model.Validate();
            if (validationErrors.Any())
            {
                return BadRequest(validationErrors);
            }

            // Check if CVE already exists
            var existingCve = await _cveManager.GetByCveIdAsync(model.CveId);
            if (existingCve != null)
            {
                return Conflict($"CVE {model.CveId} already exists");
            }

            var cve = new Cve
            {
                Id = Guid.NewGuid(),
                CveId = _sanitizer.Sanitize(model.CveId),
                Title = _sanitizer.Sanitize(model.Title),
                Description = _sanitizer.Sanitize(model.Description),
                PublishedDate = model.PublishedDate,
                LastModifiedDate = model.LastModifiedDate ?? model.PublishedDate,
                CvssV3BaseScore = model.CvssV3BaseScore,
                CvssV3Vector = _sanitizer.Sanitize(model.CvssV3Vector),
                CvssV3Severity = _sanitizer.Sanitize(model.CvssV3Severity),
                CvssV2BaseScore = model.CvssV2BaseScore,
                CvssV2Vector = _sanitizer.Sanitize(model.CvssV2Vector),
                CvssV2Severity = _sanitizer.Sanitize(model.CvssV2Severity),
                EpssScore = model.EpssScore,
                EpssPercentile = model.EpssPercentile,
                IsKnownExploited = model.IsKnownExploited,
                KevDueDate = model.KevDueDate,
                PrimaryCweId = _sanitizer.Sanitize(model.PrimaryCweId),
                PrimaryCweName = _sanitizer.Sanitize(model.PrimaryCweName),
                State = _sanitizer.Sanitize(model.State),
                AssignerOrgId = _sanitizer.Sanitize(model.AssignerOrgId),
                SourceIdentifier = _sanitizer.Sanitize(model.SourceIdentifier),
                Notes = _sanitizer.Sanitize(model.Notes),
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                UserId = _aspNetUserId
            };

            var createdCve = await _cveManager.AddAsync(cve);
            await _cveManager.Context.SaveChangesAsync();
            
            _logger.LogInformation("CVE {CveId} created successfully", model.CveId);
            
            return CreatedAtAction(nameof(GetCve), new { id = createdCve.Id }, createdCve);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating CVE");
            return StatusCode(500, "An error occurred while creating the CVE");
        }
    }

    /// <summary>
    /// Update CVE notes
    /// </summary>
    /// <param name="id">CVE ID</param>
    /// <param name="notes">Notes text</param>
    /// <returns>Success result</returns>
    [HttpPut("{id}/notes")]
    [HasPermission(Permissions.CveEdit)]
    public async Task<ActionResult> UpdateCveNotes(Guid id, [FromBody] string notes)
    {
        try
        {
            var sanitizedNotes = _sanitizer.Sanitize(notes);
            var success = await _cveManager.UpdateNotesAsync(id, sanitizedNotes, _aspNetUserId);
            
            if (!success)
            {
                return NotFound($"CVE with ID {id} not found");
            }

            _logger.LogInformation("CVE {Id} notes updated successfully", id);
            
            return Ok(new { Message = "CVE notes updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating CVE notes for {Id}", id);
            return StatusCode(500, "An error occurred while updating CVE notes");
        }
    }

    /// <summary>
    /// Mark CVE as favorite
    /// </summary>
    /// <param name="id">CVE ID</param>
    /// <returns>Success result</returns>
    [HttpPut("{id}/favorite")]
    [HasPermission(Permissions.CveEdit)]
    public async Task<ActionResult> MarkAsFavorite(Guid id)
    {
        try
        {
            var success = await _cveManager.MarkAsFavoriteAsync(id, _aspNetUserId);
            
            if (!success)
            {
                return NotFound($"CVE with ID {id} not found");
            }

            _logger.LogInformation("CVE {Id} marked as favorite", id);
            
            return Ok(new { Message = "CVE marked as favorite" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking CVE {Id} as favorite", id);
            return StatusCode(500, "An error occurred while marking CVE as favorite");
        }
    }

    /// <summary>
    /// Archive CVE
    /// </summary>
    /// <param name="id">CVE ID</param>
    /// <returns>Success result</returns>
    [HttpPut("{id}/archive")]
    [HasPermission(Permissions.CveEdit)]
    public async Task<ActionResult> ArchiveCve(Guid id)
    {
        try
        {
            var success = await _cveManager.ArchiveAsync(id, _aspNetUserId);
            
            if (!success)
            {
                return NotFound($"CVE with ID {id} not found");
            }

            _logger.LogInformation("CVE {Id} archived", id);
            
            return Ok(new { Message = "CVE archived successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving CVE {Id}", id);
            return StatusCode(500, "An error occurred while archiving CVE");
        }
    }

    /// <summary>
    /// Get CVEs for specific project
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <returns>List of CVEs</returns>
    [HttpGet("project/{projectId}")]
    [HasPermission(Permissions.CveRead)]
    public async Task<ActionResult<IEnumerable<Cve>>> GetCvesByProject(Guid projectId)
    {
        try
        {
            // Check if user has access to the project
            var projectUser = _projectUserManager.VerifyUser(projectId, _aspNetUserId);
            if (projectUser == null)
            {
                return Forbid("You don't have access to this project");
            }

            var cves = await _cveManager.GetByProjectAsync(projectId);
            
            _logger.LogInformation("Retrieved {Count} CVEs for project {ProjectId}", cves.Count, projectId);
            
            return Ok(cves);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving CVEs for project {ProjectId}", projectId);
            return StatusCode(500, "An error occurred while retrieving CVEs for project");
        }
    }

    #endregion

    #region CVE Subscriptions

    /// <summary>
    /// Get user's CVE subscriptions
    /// </summary>
    /// <returns>List of CVE subscriptions</returns>
    [HttpGet("subscriptions")]
    [HasPermission(Permissions.CveRead)]
    public async Task<ActionResult<IEnumerable<CveSubscription>>> GetUserSubscriptions()
    {
        try
        {
            var subscriptions = await _cveSubscriptionManager.GetByUserIdAsync(_aspNetUserId);
            return Ok(subscriptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user subscriptions");
            return StatusCode(500, "An error occurred while retrieving subscriptions");
        }
    }

    /// <summary>
    /// Create new CVE subscription
    /// </summary>
    /// <param name="subscription">CVE subscription</param>
    /// <returns>Created subscription</returns>
    [HttpPost("subscriptions")]
    [HasPermission(Permissions.CveCreate)]
    public async Task<ActionResult<CveSubscription>> CreateSubscription([FromBody] CveSubscription subscription)
    {
        try
        {
            if (subscription == null)
            {
                return BadRequest("Subscription model is required");
            }

            subscription.Id = Guid.NewGuid();
            subscription.UserId = _aspNetUserId;
            subscription.CreatedDate = DateTime.UtcNow;
            subscription.ModifiedDate = DateTime.UtcNow;

            // Sanitize input
            subscription.Name = _sanitizer.Sanitize(subscription.Name);
            subscription.Description = _sanitizer.Sanitize(subscription.Description);
            subscription.Vendor = _sanitizer.Sanitize(subscription.Vendor);
            subscription.Product = _sanitizer.Sanitize(subscription.Product);
            
            // Ensure all string fields are not null
            if (string.IsNullOrEmpty(subscription.Keywords))
                subscription.Keywords = "[]";
            if (string.IsNullOrEmpty(subscription.CweFilter))
                subscription.CweFilter = "[]";
            if (subscription.Vendor == null)
                subscription.Vendor = "";
            if (subscription.Product == null)
                subscription.Product = "";
            if (subscription.Description == null)
                subscription.Description = "";
            if (subscription.WebhookUrl == null)
                subscription.WebhookUrl = "";

            var createdSubscription = await _cveSubscriptionManager.AddAsync(subscription);
            await _cveSubscriptionManager.Context.SaveChangesAsync();
            
            _logger.LogInformation("CVE subscription {Name} created successfully", subscription.Name);
            
            return CreatedAtAction(nameof(GetSubscription), new { id = createdSubscription.Id }, createdSubscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating CVE subscription");
            return StatusCode(500, "An error occurred while creating the subscription");
        }
    }

    /// <summary>
    /// Get CVE subscription by ID
    /// </summary>
    /// <param name="id">Subscription ID</param>
    /// <returns>CVE subscription</returns>
    [HttpGet("subscriptions/{id}")]
    [HasPermission(Permissions.CveRead)]
    public async Task<ActionResult<CveSubscription>> GetSubscription(Guid id)
    {
        try
        {
            var subscription = _cveSubscriptionManager.GetById(id);
            
            if (subscription == null)
            {
                return NotFound($"Subscription with ID {id} not found");
            }

            // Check if user owns the subscription
            if (subscription.UserId != _aspNetUserId)
            {
                return Forbid("You don't have access to this subscription");
            }

            return Ok(subscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving subscription {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the subscription");
        }
    }

    /// <summary>
    /// Update CVE subscription
    /// </summary>
    /// <param name="id">Subscription ID</param>
    /// <param name="subscription">Updated subscription</param>
    /// <returns>Updated subscription</returns>
    [HttpPut("subscriptions/{id}")]
    [HasPermission(Permissions.CveEdit)]
    public async Task<ActionResult<CveSubscription>> UpdateSubscription(Guid id, [FromBody] CveSubscription subscription)
    {
        try
        {
            if (subscription == null)
            {
                return BadRequest("Subscription model is required");
            }

            var existingSubscription = _cveSubscriptionManager.GetById(id);
            if (existingSubscription == null)
            {
                return NotFound($"Subscription with ID {id} not found");
            }

            // Check if user owns the subscription
            if (existingSubscription.UserId != _aspNetUserId)
            {
                return Forbid("You don't have access to this subscription");
            }

            // Update fields
            existingSubscription.Name = _sanitizer.Sanitize(subscription.Name);
            existingSubscription.Description = _sanitizer.Sanitize(subscription.Description);
            existingSubscription.IsActive = subscription.IsActive;
            existingSubscription.Vendor = _sanitizer.Sanitize(subscription.Vendor);
            existingSubscription.Product = _sanitizer.Sanitize(subscription.Product);
            existingSubscription.Keywords = subscription.Keywords;
            existingSubscription.MinCvssScore = subscription.MinCvssScore;
            existingSubscription.MaxCvssScore = subscription.MaxCvssScore;
            existingSubscription.MinEpssScore = subscription.MinEpssScore;
            existingSubscription.OnlyKnownExploited = subscription.OnlyKnownExploited;
            existingSubscription.CweFilter = subscription.CweFilter;
            existingSubscription.NotificationFrequency = subscription.NotificationFrequency;
            existingSubscription.NotificationMethod = subscription.NotificationMethod;
            existingSubscription.WebhookUrl = _sanitizer.Sanitize(subscription.WebhookUrl);
            existingSubscription.ModifiedDate = DateTime.UtcNow;
            
            // Ensure Keywords and CweFilter are not null
            if (string.IsNullOrEmpty(existingSubscription.Keywords))
                existingSubscription.Keywords = "[]";
            if (string.IsNullOrEmpty(existingSubscription.CweFilter))
                existingSubscription.CweFilter = "[]";

            var updatedSubscription = _cveSubscriptionManager.Update(existingSubscription);
            await _cveSubscriptionManager.Context.SaveChangesAsync();
            
            _logger.LogInformation("CVE subscription {Id} updated successfully", id);
            
            return Ok(updatedSubscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating subscription {Id}", id);
            return StatusCode(500, "An error occurred while updating the subscription");
        }
    }

    /// <summary>
    /// Delete CVE subscription
    /// </summary>
    /// <param name="id">Subscription ID</param>
    /// <returns>Success result</returns>
    [HttpDelete("subscriptions/{id}")]
    [HasPermission(Permissions.CveDelete)]
    public async Task<ActionResult> DeleteSubscription(Guid id)
    {
        try
        {
            var subscription = _cveSubscriptionManager.GetById(id);
            if (subscription == null)
            {
                return NotFound($"Subscription with ID {id} not found");
            }

            // Check if user owns the subscription
            if (subscription.UserId != _aspNetUserId)
            {
                return Forbid("You don't have access to this subscription");
            }

            var subscriptionToDelete = _cveSubscriptionManager.GetById(id);
            if (subscriptionToDelete != null)
            {
                _cveSubscriptionManager.Remove(subscriptionToDelete);
                await _cveSubscriptionManager.Context.SaveChangesAsync();
            }
            
            _logger.LogInformation("CVE subscription {Id} deleted successfully", id);
            
            return Ok(new { Message = "Subscription deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting subscription {Id}", id);
            return StatusCode(500, "An error occurred while deleting the subscription");
        }
    }

    #endregion

    #region CVE Notifications

    /// <summary>
    /// Get user's CVE notifications
    /// </summary>
    /// <param name="unreadOnly">Get only unread notifications</param>
    /// <returns>List of CVE notifications</returns>
    [HttpGet("notifications")]
    [HasPermission(Permissions.CveRead)]
    public async Task<ActionResult<IEnumerable<CveNotification>>> GetUserNotifications(bool unreadOnly = false)
    {
        try
        {
            var notifications = unreadOnly 
                ? await _cveNotificationManager.GetUnreadByUserIdAsync(_aspNetUserId)
                : await _cveNotificationManager.GetByUserIdAsync(_aspNetUserId);
                
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user notifications");
            return StatusCode(500, "An error occurred while retrieving notifications");
        }
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <returns>Success result</returns>
    [HttpPut("notifications/{id}/read")]
    [HasPermission(Permissions.CveEdit)]
    public async Task<ActionResult> MarkNotificationAsRead(Guid id)
    {
        try
        {
            var success = await _cveNotificationManager.MarkAsReadAsync(id, _aspNetUserId);
            
            if (!success)
            {
                return NotFound($"Notification with ID {id} not found");
            }

            _logger.LogInformation("Notification {Id} marked as read", id);
            
            return Ok(new { Message = "Notification marked as read" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {Id} as read", id);
            return StatusCode(500, "An error occurred while marking notification as read");
        }
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    /// <returns>Success result</returns>
    [HttpPut("notifications/markAllRead")]
    [HasPermission(Permissions.CveEdit)]
    public async Task<ActionResult> MarkAllNotificationsAsRead()
    {
        try
        {
            var count = await _cveNotificationManager.MarkAllAsReadAsync(_aspNetUserId);
            
            _logger.LogInformation("{Count} notifications marked as read", count);
            
            return Ok(new { Message = $"{count} notifications marked as read" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read");
            return StatusCode(500, "An error occurred while marking all notifications as read");
        }
    }

    #endregion

    #region CVE Synchronization

    /// <summary>
    /// Get CVE synchronization status
    /// </summary>
    /// <returns>Synchronization status</returns>
    [HttpGet("sync/status")]
    [HasPermission(Permissions.CveRead)]
    public async Task<ActionResult<List<CveSyncStatus>>> GetSyncStatus()
    {
        try
        {
            var status = await _cveSyncService.GetSyncStatusAsync();
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sync status");
            return StatusCode(500, "An error occurred while retrieving sync status");
        }
    }

    /// <summary>
    /// Trigger manual CVE synchronization
    /// </summary>
    /// <returns>Synchronization result</returns>
    [HttpPost("sync/trigger")]
    [HasPermission(Permissions.CveEdit)]
    public async Task<ActionResult<CveSyncResult>> TriggerSync()
    {
        try
        {
            var result = await _cveSyncService.SyncAllSourcesAsync();
            
            _logger.LogInformation("Manual CVE sync triggered, processed {Count} CVEs", result.ProcessedCount);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering CVE sync");
            return StatusCode(500, "An error occurred while triggering CVE sync");
        }
    }

    #endregion

    #region Internal Methods for Blazor Components

    /// <summary>
    /// Get all CVEs for Blazor components (non-API method)
    /// </summary>
    /// <returns>List of CVE entities</returns>
    [NonAction]
    public IEnumerable<Cve> Get()
    {
        try
        {
            return _cveManager.GetAll().ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving CVEs for Blazor component");
            throw;
        }
    }

    /// <summary>
    /// Get CVE statistics for Blazor components (non-API method)
    /// </summary>
    /// <returns>CVE statistics</returns>
    [NonAction]
    public async Task<CveStatistics> GetStatisticsForComponentsAsync()
    {
        try
        {
            return await _cveManager.GetStatisticsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving CVE statistics for Blazor component");
            throw;
        }
    }

    /// <summary>
    /// Search CVEs for Blazor components (non-API method)
    /// </summary>
    /// <param name="searchModel">Search criteria</param>
    /// <returns>List of matching CVEs</returns>
    [NonAction]
    public async Task<List<Cve>> SearchForComponentsAsync(CveSearchViewModel searchModel)
    {
        try
        {
            searchModel.UserId = _aspNetUserId;
            return await _cveManager.GetWithAdvancedSearchAsync(searchModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching CVEs for Blazor component");
            throw;
        }
    }

    /// <summary>
    /// Trigger CVE synchronization for Blazor components (non-API method)
    /// </summary>
    /// <returns>Synchronization result</returns>
    [NonAction]
    public async Task<CveSyncResult> TriggerSyncForComponentsAsync()
    {
        try
        {
            return await _cveSyncService.SyncAllSourcesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering CVE sync for Blazor component");
            throw;
        }
    }

    /// <summary>
    /// Mark CVE as favorite for Blazor components (non-API method)
    /// </summary>
    /// <param name="id">CVE ID</param>
    /// <returns>Success status</returns>
    [NonAction]
    public async Task<bool> MarkAsFavoriteForComponentsAsync(Guid id)
    {
        try
        {
            return await _cveManager.MarkAsFavoriteAsync(id, _aspNetUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking CVE as favorite for Blazor component");
            throw;
        }
    }

    /// <summary>
    /// Archive CVE for Blazor components (non-API method)
    /// </summary>
    /// <param name="id">CVE ID</param>
    /// <returns>Success status</returns>
    [NonAction]
    public async Task<bool> ArchiveForComponentsAsync(Guid id)
    {
        try
        {
            return await _cveManager.ArchiveAsync(id, _aspNetUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving CVE for Blazor component");
            throw;
        }
    }

    
    /// <summary>
    /// Get user subscriptions for Blazor components (non-API method)
    /// </summary>
    /// <returns>List of CVE subscriptions</returns>
    [NonAction]
    public async Task<List<CveSubscription>> GetUserSubscriptionsForComponentsAsync()
    {
        try
        {
            var subscriptions = await _cveSubscriptionManager.GetByUserIdAsync(_aspNetUserId);
            return subscriptions.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user subscriptions for Blazor component");
            throw;
        }
    }

    /// <summary>
    /// Get user subscription summary for Blazor components (non-API method)
    /// </summary>
    /// <returns>User subscription summary</returns>
    [NonAction]
    public async Task<CveUserSubscriptionSummary> GetUserSubscriptionSummaryForComponentsAsync()
    {
        try
        {
            return await _cveSubscriptionManager.GetUserSummaryAsync(_aspNetUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user subscription summary for Blazor component");
            throw;
        }
    }

    /// <summary>
    /// Toggle subscription status for Blazor components (non-API method)
    /// </summary>
    /// <param name="subscriptionId">Subscription ID</param>
    /// <returns>Success status</returns>
    [NonAction]
    public async Task<bool> ToggleSubscriptionForComponentsAsync(Guid subscriptionId)
    {
        try
        {
            var subscription = _cveSubscriptionManager.GetById(subscriptionId);
            if (subscription == null || subscription.UserId != _aspNetUserId)
            {
                return false;
            }

            if (subscription.IsActive)
            {
                await _cveSubscriptionManager.DeactivateAsync(subscriptionId, _aspNetUserId);
            }
            else
            {
                await _cveSubscriptionManager.ActivateAsync(subscriptionId, _aspNetUserId);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling subscription for Blazor component");
            throw;
        }
    }

    /// <summary>
    /// Delete subscription for Blazor components (non-API method)
    /// </summary>
    /// <param name="subscriptionId">Subscription ID</param>
    /// <returns>Success status</returns>
    [NonAction]
    public async Task<bool> DeleteSubscriptionForComponentsAsync(Guid subscriptionId)
    {
        try
        {
            var subscription = _cveSubscriptionManager.GetById(subscriptionId);
            if (subscription == null || subscription.UserId != _aspNetUserId)
            {
                return false;
            }

            _cveSubscriptionManager.Remove(subscription);
            await _cveSubscriptionManager.Context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting subscription for Blazor component");
            throw;
        }
    }

    /// <summary>
    /// Create subscription for Blazor components (non-API method)
    /// </summary>
    /// <param name="subscription">CVE subscription</param>
    /// <returns>Created subscription</returns>
    [NonAction]
    public async Task<CveSubscription> CreateSubscriptionForComponentsAsync(CveSubscription subscription)
    {
        try
        {
            subscription.Id = Guid.NewGuid();
            subscription.UserId = _aspNetUserId;
            subscription.CreatedDate = DateTime.UtcNow;
            subscription.ModifiedDate = DateTime.UtcNow;

            // Sanitize input
            subscription.Name = _sanitizer.Sanitize(subscription.Name);
            subscription.Description = _sanitizer.Sanitize(subscription.Description);
            subscription.Vendor = _sanitizer.Sanitize(subscription.Vendor);
            subscription.Product = _sanitizer.Sanitize(subscription.Product);
            
            // Ensure all string fields are not null
            if (string.IsNullOrEmpty(subscription.Keywords))
                subscription.Keywords = "[]";
            if (string.IsNullOrEmpty(subscription.CweFilter))
                subscription.CweFilter = "[]";
            if (subscription.Vendor == null)
                subscription.Vendor = "";
            if (subscription.Product == null)
                subscription.Product = "";
            if (subscription.Description == null)
                subscription.Description = "";
            if (subscription.WebhookUrl == null)
                subscription.WebhookUrl = "";

            var createdSubscription = await _cveSubscriptionManager.AddAsync(subscription);
            await _cveSubscriptionManager.Context.SaveChangesAsync();
            
            _logger.LogInformation("CVE subscription {Name} created successfully for component", subscription.Name);
            
            return createdSubscription;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating CVE subscription for component");
            throw;
        }
    }

    /// <summary>
    /// Update subscription for Blazor components (non-API method)
    /// </summary>
    /// <param name="subscription">CVE subscription</param>
    /// <returns>Updated subscription</returns>
    [NonAction]
    public async Task<CveSubscription> UpdateSubscriptionForComponentsAsync(CveSubscription subscription)
    {
        try
        {
            var existingSubscription = _cveSubscriptionManager.GetById(subscription.Id);
            if (existingSubscription == null)
            {
                throw new InvalidOperationException($"Subscription with ID {subscription.Id} not found");
            }

            // Check if user owns the subscription
            if (existingSubscription.UserId != _aspNetUserId)
            {
                throw new UnauthorizedAccessException("You don't have access to this subscription");
            }

            // Update fields
            existingSubscription.Name = _sanitizer.Sanitize(subscription.Name);
            existingSubscription.Description = _sanitizer.Sanitize(subscription.Description);
            existingSubscription.IsActive = subscription.IsActive;
            existingSubscription.Vendor = _sanitizer.Sanitize(subscription.Vendor);
            existingSubscription.Product = _sanitizer.Sanitize(subscription.Product);
            existingSubscription.Keywords = subscription.Keywords;
            existingSubscription.MinCvssScore = subscription.MinCvssScore;
            existingSubscription.MaxCvssScore = subscription.MaxCvssScore;
            existingSubscription.MinEpssScore = subscription.MinEpssScore;
            existingSubscription.OnlyKnownExploited = subscription.OnlyKnownExploited;
            existingSubscription.CweFilter = subscription.CweFilter;
            existingSubscription.NotificationFrequency = subscription.NotificationFrequency;
            existingSubscription.NotificationMethod = subscription.NotificationMethod;
            existingSubscription.WebhookUrl = _sanitizer.Sanitize(subscription.WebhookUrl);
            existingSubscription.ModifiedDate = DateTime.UtcNow;
            
            // Ensure Keywords and CweFilter are not null
            if (string.IsNullOrEmpty(existingSubscription.Keywords))
                existingSubscription.Keywords = "[]";
            if (string.IsNullOrEmpty(existingSubscription.CweFilter))
                existingSubscription.CweFilter = "[]";

            var updatedSubscription = _cveSubscriptionManager.Update(existingSubscription);
            await _cveSubscriptionManager.Context.SaveChangesAsync();
            
            _logger.LogInformation("CVE subscription {Id} updated successfully for component", subscription.Id);
            
            return updatedSubscription;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating subscription for component");
            throw;
        }
    }

    /// <summary>
    /// Create default subscriptions for user (non-API method)
    /// </summary>
    /// <returns>List of created subscriptions</returns>
    [NonAction]
    public async Task<List<CveSubscription>> CreateDefaultSubscriptionsForComponentsAsync()
    {
        try
        {
            var subscriptions = await _cveSubscriptionManager.CreateDefaultSubscriptionsAsync(_aspNetUserId);
            await _cveSubscriptionManager.Context.SaveChangesAsync();
            
            _logger.LogInformation("Default CVE subscriptions created for user {UserId}", _aspNetUserId);
            
            return subscriptions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating default subscriptions for component");
            throw;
        }
    }

    /// <summary>
    /// Test notification for a specific subscription (non-API method)
    /// </summary>
    /// <param name="subscriptionId">Subscription ID</param>
    /// <returns>Test result</returns>
    [NonAction]
    public async Task<CveNotificationTestResult> TestSubscriptionNotificationForComponentsAsync(Guid subscriptionId)
    {
        try
        {
            var subscription = _cveSubscriptionManager.GetById(subscriptionId);
            if (subscription == null || subscription.UserId != _aspNetUserId)
            {
                return new CveNotificationTestResult 
                { 
                    IsSuccess = false, 
                    ErrorMessage = "Subscription not found or access denied" 
                };
            }

            // Try to get or create a test CVE for notifications
            const string testCveId = "CVE-TEST-NOTIF"; // Truncated to fit 20 char limit
            var testCve = _cveManager.GetAll().FirstOrDefault(c => c.CveId == testCveId);
            
            if (testCve == null)
            {
                // Create a special test CVE for notification testing
                testCve = new Cve
                {
                    Id = Guid.NewGuid(),
                    CveId = testCveId,
                    Title = TruncateString("Test CVE for Notification Testing", 500),
                    Description = "This is a special CVE created for testing notification functionality. It is not a real vulnerability.",
                    PublishedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow,
                    CvssV3BaseScore = 5.0,
                    CvssV3Severity = TruncateString("Medium", 20),
                    State = TruncateString("PUBLISHED", 20),
                    SourceIdentifier = TruncateString("test@cervantes.local", 100),
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    UserId = _aspNetUserId
                };
                
                await _cveManager.AddAsync(testCve);
                await _cveManager.Context.SaveChangesNoAuditAsync();
                
                _logger.LogInformation("Created test CVE for notification testing");
            }

            // Create a test CVE notification using the test CVE
            var title = $"TEST: {subscription.Name} Notification";
            var message = $"This is a test notification for your subscription '{subscription.Name}'. " +
                         $"Your notification settings: {subscription.NotificationMethod} - {subscription.NotificationFrequency}. " +
                         $"This test uses {testCveId} as a sample CVE for testing purposes.";
            
            // Log values for debugging
            _logger.LogDebug("Creating test notification with values: Title={Title}, NotificationType={NotificationType}, Priority={Priority}, Method={Method}, Status={Status}, UserId={UserId}", 
                            title, "Test", "Medium", subscription.NotificationMethod ?? "InApp", "Pending", _aspNetUserId);
            
            var testNotification = new CveNotification
            {
                Id = Guid.NewGuid(),
                CveId = testCve.Id, // Use test CVE for foreign key
                SubscriptionId = subscriptionId,
                UserId = _aspNetUserId,
                Title = TruncateString(title, 200),
                Message = message,
                NotificationType = TruncateString("Test", 20),
                Priority = TruncateString("Medium", 20), 
                Method = TruncateString(subscription.NotificationMethod ?? "InApp", 20),
                Status = TruncateString("Pending", 20),
                ErrorMessage = "",
                IsSent = false,
                IsRead = false,
                RetryCount = 0,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            await _cveNotificationManager.AddAsync(testNotification);
            await _cveNotificationManager.Context.SaveChangesNoAuditAsync();

            _logger.LogInformation("Test notification created for subscription {SubscriptionId}", subscriptionId);
            
            return new CveNotificationTestResult 
            { 
                IsSuccess = true, 
                Message = "Test notification created successfully",
                NotificationId = testNotification.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating test notification for subscription {SubscriptionId}", subscriptionId);
            return new CveNotificationTestResult 
            { 
                IsSuccess = false, 
                ErrorMessage = ex.Message 
            };
        }
    }

    /// <summary>
    /// Test all active subscriptions for current user (non-API method)
    /// </summary>
    /// <returns>Test results</returns>
    [NonAction]
    public async Task<List<CveNotificationTestResult>> TestAllActiveSubscriptionsForComponentsAsync()
    {
        try
        {
            var activeSubscriptions = await _cveSubscriptionManager.GetByUserIdAsync(_aspNetUserId);
            var results = new List<CveNotificationTestResult>();

            foreach (var subscription in activeSubscriptions.Where(s => s.IsActive))
            {
                var result = await TestSubscriptionNotificationForComponentsAsync(subscription.Id);
                result.SubscriptionName = subscription.Name;
                results.Add(result);
            }

            _logger.LogInformation("Tested {Count} active subscriptions for user {UserId}", results.Count, _aspNetUserId);
            
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing active subscriptions for user {UserId}", _aspNetUserId);
            throw;
        }
    }

    /// <summary>
    /// Delete multiple CVEs for Blazor components (non-API method)
    /// </summary>
    /// <param name="cveIds">List of CVE IDs to delete</param>
    /// <returns>Number of deleted CVEs</returns>
    [NonAction]
    public async Task<int> DeleteMultipleForComponentsAsync(List<Guid> cveIds)
    {
        try
        {
            var deletedCount = await _cveManager.DeleteMultipleAsync(cveIds);
            
            _logger.LogInformation("Deleted {Count} CVEs for Blazor component", deletedCount);
            
            return deletedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting multiple CVEs for Blazor component");
            throw;
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Truncate string to specified length
    /// </summary>
    /// <param name="value">String to truncate</param>
    /// <param name="maxLength">Maximum length</param>
    /// <returns>Truncated string</returns>
    [NonAction]
    private static string TruncateString(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
            
        return value.Length > maxLength ? value.Substring(0, maxLength) : value;
    }

    #endregion
}