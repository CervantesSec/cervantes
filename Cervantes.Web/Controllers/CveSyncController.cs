using Cervantes.CORE.ViewModel;
using Cervantes.IFR.CveServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CveSyncController : ControllerBase
{
    private readonly ICveSyncService _cveSyncService;
    private readonly ILogger<CveSyncController> _logger;

    public CveSyncController(
        ICveSyncService cveSyncService,
        ILogger<CveSyncController> logger)
    {
        _cveSyncService = cveSyncService;
        _logger = logger;
    }

    /// <summary>
    /// Start CVE sync with custom options
    /// </summary>
    /// <param name="options">Sync options</param>
    /// <returns>Sync result</returns>
    [HttpPost("sync")]
    public async Task<ActionResult<CveSyncResult>> SyncCvesAsync([FromBody] CveSyncOptionsViewModel options)
    {
        try
        {
            // Validate options
            var validationErrors = options.Validate();
            if (validationErrors.Any())
            {
                return BadRequest(new { Errors = validationErrors });
            }

            _logger.LogInformation("Starting CVE sync with options: {Options}", 
                System.Text.Json.JsonSerializer.Serialize(options));

            var result = await _cveSyncService.SyncWithOptionsAsync(options);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(500, result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during CVE sync");
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Get CVE sync status for all sources
    /// </summary>
    /// <returns>Sync status list</returns>
    [HttpGet("status")]
    public async Task<ActionResult<List<CveSyncStatus>>> GetSyncStatusAsync()
    {
        try
        {
            var status = await _cveSyncService.GetSyncStatusAsync();
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting CVE sync status");
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Get quick sync presets
    /// </summary>
    /// <returns>Available presets</returns>
    [HttpGet("presets")]
    public ActionResult<Dictionary<string, string>> GetQuickPresets()
    {
        return Ok(CveSyncOptionsViewModel.AvailablePresets);
    }

    /// <summary>
    /// Get available severity options
    /// </summary>
    /// <returns>Available severities</returns>
    [HttpGet("severities")]
    public ActionResult<List<string>> GetSeverities()
    {
        return Ok(CveSyncOptionsViewModel.AvailableSeverities);
    }

    /// <summary>
    /// Validate sync options
    /// </summary>
    /// <param name="options">Options to validate</param>
    /// <returns>Validation result</returns>
    [HttpPost("validate")]
    public ActionResult<CveSyncValidationResult> ValidateOptions([FromBody] CveSyncOptionsViewModel options)
    {
        var errors = options.Validate();
        var estimatedCount = options.GetEstimatedCount();
        
        return Ok(new CveSyncValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors,
            EstimatedCount = estimatedCount,
            Warnings = GenerateWarnings(options, estimatedCount)
        });
    }

    /// <summary>
    /// Sync specific CVE by ID
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <returns>Sync result</returns>
    [HttpPost("sync/{cveId}")]
    public async Task<ActionResult<CveSyncResult>> SyncCveByIdAsync(string cveId)
    {
        try
        {
            if (string.IsNullOrEmpty(cveId) || !cveId.StartsWith("CVE-"))
            {
                return BadRequest("Invalid CVE ID format");
            }

            var result = await _cveSyncService.SyncCveByIdAsync(cveId, Guid.NewGuid());
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(500, result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing CVE {CveId}", cveId);
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Generate warnings for sync options
    /// </summary>
    /// <param name="options">Sync options</param>
    /// <param name="estimatedCount">Estimated CVE count</param>
    /// <returns>List of warnings</returns>
    [NonAction]
    private List<string> GenerateWarnings(CveSyncOptionsViewModel options, int? estimatedCount)
    {
        var warnings = new List<string>();

        if (estimatedCount.HasValue && estimatedCount > 10000)
        {
            warnings.Add($"Large sync detected ({estimatedCount:N0} estimated CVEs). This may take a long time.");
        }

        if (options.MaxTotalCves > 5000 && !options.SkipExisting)
        {
            warnings.Add("Syncing many CVEs without skipping existing ones may cause duplicates.");
        }

        if (!options.StartYear.HasValue && !options.EndYear.HasValue && 
            !options.PublishedDateStart.HasValue && !options.PublishedDateEnd.HasValue)
        {
            warnings.Add("No date filters specified. This may result in a very large sync.");
        }

        if (options.ResultsPerPage > 500)
        {
            warnings.Add("Large page size may impact performance and API rate limits.");
        }

        return warnings;
    }

    #region Internal Methods for Blazor Components

    /// <summary>
    /// Start CVE sync with options (internal method for Blazor)
    /// </summary>
    /// <param name="options">Sync options</param>
    /// <returns>Sync result</returns>
    [NonAction]
    internal async Task<CveSyncResult> SyncCvesInternalAsync(CveSyncOptionsViewModel options)
    {
        return await _cveSyncService.SyncWithOptionsAsync(options);
    }

    /// <summary>
    /// Get sync status (internal method for Blazor)
    /// </summary>
    /// <returns>Sync status</returns>
    [NonAction]
    internal async Task<List<CveSyncStatus>> GetSyncStatusInternalAsync()
    {
        return await _cveSyncService.GetSyncStatusAsync();
    }

    /// <summary>
    /// Validate options (internal method for Blazor)
    /// </summary>
    /// <param name="options">Options to validate</param>
    /// <returns>Validation result</returns>
    [NonAction]
    internal CveSyncValidationResult ValidateOptionsInternal(CveSyncOptionsViewModel options)
    {
        var errors = options.Validate();
        var estimatedCount = options.GetEstimatedCount();
        
        return new CveSyncValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors,
            EstimatedCount = estimatedCount,
            Warnings = GenerateWarnings(options, estimatedCount)
        };
    }

    #endregion
    
    /// <summary>
    /// Validation result for sync options
    /// </summary>
    public class CveSyncValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public int? EstimatedCount { get; set; }
    }
}