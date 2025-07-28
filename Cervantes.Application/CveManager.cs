using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Cervantes.Application;

/// <summary>
/// CVE Manager implementation
/// </summary>
public class CveManager : GenericManager<Cve>, ICveManager
{
    public CveManager(IApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get CVE by CVE identifier
    /// </summary>
    /// <param name="cveId">CVE identifier (e.g., CVE-2023-1234)</param>
    /// <returns>CVE entity</returns>
    public async Task<Cve> GetByCveIdAsync(string cveId)
    {
        return await Context.Set<Cve>()
            .Include(c => c.Configurations)
            .Include(c => c.References)
            .Include(c => c.Tags)
            .Include(c => c.CweMappings)
            .Include(c => c.ProjectMappings)
            .FirstOrDefaultAsync(c => c.CveId == cveId);
    }

    /// <summary>
    /// Get CVEs by multiple CVE identifiers
    /// </summary>
    /// <param name="cveIds">Collection of CVE identifiers</param>
    /// <returns>List of CVE entities</returns>
    public async Task<List<Cve>> GetCvesByIdsAsync(IEnumerable<string> cveIds)
    {
        var cveIdList = cveIds.ToList();
        if (!cveIdList.Any())
            return new List<Cve>();

        return await Context.Set<Cve>()
            .Where(c => cveIdList.Contains(c.CveId))
            .ToListAsync();
    }

    /// <summary>
    /// Get CVEs by CVSS score range
    /// </summary>
    /// <param name="minScore">Minimum CVSS score</param>
    /// <param name="maxScore">Maximum CVSS score</param>
    /// <returns>List of CVE entities</returns>
    public async Task<List<Cve>> GetByCvssScoreRangeAsync(double minScore, double maxScore)
    {
        return await Context.Set<Cve>()
            .Where(c => c.CvssV3BaseScore >= minScore && c.CvssV3BaseScore <= maxScore)
            .OrderByDescending(c => c.CvssV3BaseScore)
            .ToListAsync();
    }

    /// <summary>
    /// Get CVEs by severity
    /// </summary>
    /// <param name="severity">CVSS severity (LOW, MEDIUM, HIGH, CRITICAL)</param>
    /// <returns>List of CVE entities</returns>
    public async Task<List<Cve>> GetBySeverityAsync(string severity)
    {
        return await Context.Set<Cve>()
            .Where(c => c.CvssV3Severity == severity.ToUpper())
            .OrderByDescending(c => c.PublishedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get CVEs published within date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of CVE entities</returns>
    public async Task<List<Cve>> GetByPublishedDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await Context.Set<Cve>()
            .Where(c => c.PublishedDate >= startDate && c.PublishedDate <= endDate)
            .OrderByDescending(c => c.PublishedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get CVEs modified within date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of CVE entities</returns>
    public async Task<List<Cve>> GetByModifiedDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await Context.Set<Cve>()
            .Where(c => c.LastModifiedDate >= startDate && c.LastModifiedDate <= endDate)
            .OrderByDescending(c => c.LastModifiedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get CVEs by vendor
    /// </summary>
    /// <param name="vendor">Vendor name</param>
    /// <returns>List of CVE entities</returns>
    public async Task<List<Cve>> GetByVendorAsync(string vendor)
    {
        return await Context.Set<Cve>()
            .Include(c => c.Configurations)
            .Where(c => c.Configurations.Any(config => config.Vendor.ToLower().Contains(vendor.ToLower())))
            .OrderByDescending(c => c.PublishedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get CVEs by product
    /// </summary>
    /// <param name="product">Product name</param>
    /// <returns>List of CVE entities</returns>
    public async Task<List<Cve>> GetByProductAsync(string product)
    {
        return await Context.Set<Cve>()
            .Include(c => c.Configurations)
            .Where(c => c.Configurations.Any(config => config.Product.ToLower().Contains(product.ToLower())))
            .OrderByDescending(c => c.PublishedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get CVEs by CWE ID
    /// </summary>
    /// <param name="cweId">CWE identifier</param>
    /// <returns>List of CVE entities</returns>
    public async Task<List<Cve>> GetByCweIdAsync(string cweId)
    {
        return await Context.Set<Cve>()
            .Where(c => c.PrimaryCweId == cweId || c.CweMappings.Any(cwe => cwe.Cwe.Id.ToString() == cweId))
            .OrderByDescending(c => c.PublishedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get known exploited CVEs (CISA KEV)
    /// </summary>
    /// <returns>List of CVE entities</returns>
    public async Task<List<Cve>> GetKnownExploitedAsync()
    {
        return await Context.Set<Cve>()
            .Where(c => c.IsKnownExploited)
            .OrderByDescending(c => c.KevDueDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get CVEs by EPSS score range
    /// </summary>
    /// <param name="minScore">Minimum EPSS score</param>
    /// <param name="maxScore">Maximum EPSS score</param>
    /// <returns>List of CVE entities</returns>
    public async Task<List<Cve>> GetByEpssScoreRangeAsync(double minScore, double maxScore)
    {
        return await Context.Set<Cve>()
            .Where(c => c.EpssScore >= minScore && c.EpssScore <= maxScore)
            .OrderByDescending(c => c.EpssScore)
            .ToListAsync();
    }

    /// <summary>
    /// Search CVEs by keyword
    /// </summary>
    /// <param name="keyword">Search keyword</param>
    /// <returns>List of CVE entities</returns>
    public async Task<List<Cve>> SearchAsync(string keyword)
    {
        return await Context.Set<Cve>()
            .Where(c => c.CveId.Contains(keyword) || 
                       c.Title.Contains(keyword) || 
                       c.Description.Contains(keyword))
            .OrderByDescending(c => c.PublishedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get CVEs with advanced search criteria
    /// </summary>
    /// <param name="searchModel">Search criteria</param>
    /// <returns>List of CVE entities</returns>
    public async Task<List<Cve>> GetWithAdvancedSearchAsync(CveSearchViewModel searchModel)
    {
        var query = Context.Set<Cve>().AsQueryable();

        // Apply search filters
        if (!string.IsNullOrEmpty(searchModel.SearchTerm))
        {
            query = query.Where(c => c.CveId.Contains(searchModel.SearchTerm) ||
                                    c.Title.Contains(searchModel.SearchTerm) ||
                                    c.Description.Contains(searchModel.SearchTerm));
        }

        if (!string.IsNullOrEmpty(searchModel.CveId))
        {
            query = query.Where(c => c.CveId == searchModel.CveId);
        }

        if (searchModel.MinCvssScore.HasValue)
        {
            query = query.Where(c => c.CvssV3BaseScore >= searchModel.MinCvssScore.Value);
        }

        if (searchModel.MaxCvssScore.HasValue)
        {
            query = query.Where(c => c.CvssV3BaseScore <= searchModel.MaxCvssScore.Value);
        }

        if (searchModel.Severities?.Any() == true)
        {
            query = query.Where(c => searchModel.Severities.Contains(c.CvssV3Severity));
        }

        if (searchModel.MinEpssScore.HasValue)
        {
            query = query.Where(c => c.EpssScore >= searchModel.MinEpssScore.Value);
        }

        if (searchModel.MaxEpssScore.HasValue)
        {
            query = query.Where(c => c.EpssScore <= searchModel.MaxEpssScore.Value);
        }

        if (searchModel.PublishedDateStart.HasValue)
        {
            query = query.Where(c => c.PublishedDate >= searchModel.PublishedDateStart.Value);
        }

        if (searchModel.PublishedDateEnd.HasValue)
        {
            query = query.Where(c => c.PublishedDate <= searchModel.PublishedDateEnd.Value);
        }

        if (searchModel.ModifiedDateStart.HasValue)
        {
            query = query.Where(c => c.LastModifiedDate >= searchModel.ModifiedDateStart.Value);
        }

        if (searchModel.ModifiedDateEnd.HasValue)
        {
            query = query.Where(c => c.LastModifiedDate <= searchModel.ModifiedDateEnd.Value);
        }

        if (searchModel.KnownExploited.HasValue)
        {
            query = query.Where(c => c.IsKnownExploited == searchModel.KnownExploited.Value);
        }

        if (searchModel.HasKevDueDate.HasValue)
        {
            query = searchModel.HasKevDueDate.Value 
                ? query.Where(c => c.KevDueDate.HasValue)
                : query.Where(c => !c.KevDueDate.HasValue);
        }

        if (searchModel.States?.Any() == true)
        {
            query = query.Where(c => searchModel.States.Contains(c.State));
        }

        if (searchModel.IsFavorite.HasValue)
        {
            query = query.Where(c => c.IsFavorite == searchModel.IsFavorite.Value);
        }

        if (searchModel.IsRead.HasValue)
        {
            query = query.Where(c => c.IsRead == searchModel.IsRead.Value);
        }

        if (searchModel.IsArchived.HasValue)
        {
            query = query.Where(c => c.IsArchived == searchModel.IsArchived.Value);
        }

        if (searchModel.HasNotes.HasValue)
        {
            query = searchModel.HasNotes.Value 
                ? query.Where(c => !string.IsNullOrEmpty(c.Notes))
                : query.Where(c => string.IsNullOrEmpty(c.Notes));
        }

        // Apply sorting
        query = searchModel.SortBy?.ToLower() switch
        {
            "cveid" => searchModel.SortDirection?.ToLower() == "desc" 
                ? query.OrderByDescending(c => c.CveId)
                : query.OrderBy(c => c.CveId),
            "title" => searchModel.SortDirection?.ToLower() == "desc" 
                ? query.OrderByDescending(c => c.Title)
                : query.OrderBy(c => c.Title),
            "cvssv3basescore" => searchModel.SortDirection?.ToLower() == "desc" 
                ? query.OrderByDescending(c => c.CvssV3BaseScore)
                : query.OrderBy(c => c.CvssV3BaseScore),
            "epsscore" => searchModel.SortDirection?.ToLower() == "desc" 
                ? query.OrderByDescending(c => c.EpssScore)
                : query.OrderBy(c => c.EpssScore),
            "lastmodifieddate" => searchModel.SortDirection?.ToLower() == "desc" 
                ? query.OrderByDescending(c => c.LastModifiedDate)
                : query.OrderBy(c => c.LastModifiedDate),
            _ => searchModel.SortDirection?.ToLower() == "desc" 
                ? query.OrderByDescending(c => c.PublishedDate)
                : query.OrderBy(c => c.PublishedDate)
        };

        // Apply pagination
        if (searchModel.Page > 1)
        {
            query = query.Skip((searchModel.Page - 1) * searchModel.PageSize);
        }

        query = query.Take(searchModel.PageSize);

        return await query.ToListAsync();
    }

    /// <summary>
    /// Get CVE statistics using efficient database aggregations
    /// </summary>
    /// <returns>CVE statistics</returns>
    public async Task<CveStatistics> GetStatisticsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var cveSet = Context.Set<Cve>();

        // Use efficient database aggregations instead of loading all data into memory
        var statistics = new CveStatistics
        {
            TotalCves = await cveSet.CountAsync(),
            CriticalCves = await cveSet.CountAsync(c => c.CvssV3Severity == "CRITICAL"),
            HighCves = await cveSet.CountAsync(c => c.CvssV3Severity == "HIGH"),
            MediumCves = await cveSet.CountAsync(c => c.CvssV3Severity == "MEDIUM"),
            LowCves = await cveSet.CountAsync(c => c.CvssV3Severity == "LOW"),
            KnownExploitedCves = await cveSet.CountAsync(c => c.IsKnownExploited),
            NewCvesToday = await cveSet.CountAsync(c => c.PublishedDate.Date == today),
            NewCvesThisWeek = await cveSet.CountAsync(c => c.PublishedDate >= today.AddDays(-7)),
            NewCvesThisMonth = await cveSet.CountAsync(c => c.PublishedDate >= today.AddDays(-30))
        };

        // Get average scores - only execute if there are records with scores
        var cvssScores = cveSet.Where(c => c.CvssV3BaseScore.HasValue);
        statistics.AverageCvssScore = await cvssScores.AnyAsync() ? await cvssScores.AverageAsync(c => c.CvssV3BaseScore!.Value) : 0;

        var epssScores = cveSet.Where(c => c.EpssScore.HasValue);
        statistics.AverageEpssScore = await epssScores.AnyAsync() ? await epssScores.AverageAsync(c => c.EpssScore!.Value) : 0;

        // Get last update date
        statistics.LastUpdateDate = await cveSet.AnyAsync() ? await cveSet.MaxAsync(c => c.ModifiedDate) : DateTime.MinValue;

        return statistics;
    }

    /// <summary>
    /// Get CVE count by severity
    /// </summary>
    /// <returns>Dictionary of severity counts</returns>
    public async Task<Dictionary<string, int>> GetCountBySeverityAsync()
    {
        return await Context.Set<Cve>()
            .GroupBy(c => c.CvssV3Severity)
            .ToDictionaryAsync(g => g.Key ?? "Unknown", g => g.Count());
    }

    /// <summary>
    /// Get CVE count by date range
    /// </summary>
    /// <param name="days">Number of days</param>
    /// <returns>Dictionary of date counts</returns>
    public async Task<Dictionary<DateTime, int>> GetCountByDateRangeAsync(int days)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-days);
        
        return await Context.Set<Cve>()
            .Where(c => c.PublishedDate >= startDate)
            .GroupBy(c => c.PublishedDate.Date)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    /// <summary>
    /// Get top affected vendors
    /// </summary>
    /// <param name="limit">Number of vendors to return</param>
    /// <returns>Dictionary of vendor counts</returns>
    public async Task<Dictionary<string, int>> GetTopVendorsAsync(int limit = 10)
    {
        return await Context.Set<CveConfiguration>()
            .GroupBy(c => c.Vendor)
            .OrderByDescending(g => g.Count())
            .Take(limit)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    /// <summary>
    /// Get top affected products
    /// </summary>
    /// <param name="limit">Number of products to return</param>
    /// <returns>Dictionary of product counts</returns>
    public async Task<Dictionary<string, int>> GetTopProductsAsync(int limit = 10)
    {
        return await Context.Set<CveConfiguration>()
            .GroupBy(c => c.Product)
            .OrderByDescending(g => g.Count())
            .Take(limit)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    /// <summary>
    /// Mark CVE as read
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="userId">User identifier</param>
    /// <returns>True if successful</returns>
    public async Task<bool> MarkAsReadAsync(Guid cveId, string userId)
    {
        var cve = await Context.Set<Cve>().FindAsync(cveId);
        if (cve == null) return false;

        cve.IsRead = true;
        cve.ModifiedDate = DateTime.UtcNow;
        
        await Context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Mark CVE as favorite
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="userId">User identifier</param>
    /// <returns>True if successful</returns>
    public async Task<bool> MarkAsFavoriteAsync(Guid cveId, string userId)
    {
        var cve = await Context.Set<Cve>().FindAsync(cveId);
        if (cve == null) return false;

        cve.IsFavorite = !cve.IsFavorite;
        cve.ModifiedDate = DateTime.UtcNow;
        
        await Context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Archive CVE
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="userId">User identifier</param>
    /// <returns>True if successful</returns>
    public async Task<bool> ArchiveAsync(Guid cveId, string userId)
    {
        var cve = await Context.Set<Cve>().FindAsync(cveId);
        if (cve == null) return false;

        cve.IsArchived = !cve.IsArchived;
        cve.ModifiedDate = DateTime.UtcNow;
        
        await Context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Add or update CVE notes
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="notes">Notes text</param>
    /// <param name="userId">User identifier</param>
    /// <returns>True if successful</returns>
    public async Task<bool> UpdateNotesAsync(Guid cveId, string notes, string userId)
    {
        var cve = await Context.Set<Cve>().FindAsync(cveId);
        if (cve == null) return false;

        cve.Notes = notes;
        cve.ModifiedDate = DateTime.UtcNow;
        
        await Context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Get CVEs for specific project
    /// </summary>
    /// <param name="projectId">Project identifier</param>
    /// <returns>List of CVE entities</returns>
    public async Task<List<Cve>> GetByProjectAsync(Guid projectId)
    {
        return await Context.Set<Cve>()
            .Include(c => c.ProjectMappings)
            .Where(c => c.ProjectMappings.Any(pm => pm.ProjectId == projectId))
            .OrderByDescending(c => c.PublishedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get CVEs for user subscriptions
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>List of CVE entities</returns>
    public async Task<List<Cve>> GetByUserSubscriptionsAsync(string userId)
    {
        var subscriptions = await Context.Set<CveSubscription>()
            .Where(s => s.UserId == userId && s.IsActive)
            .ToListAsync();

        // This would need more complex logic to match CVEs against subscription criteria
        // For now, returning recent CVEs
        return await Context.Set<Cve>()
            .OrderByDescending(c => c.PublishedDate)
            .Take(100)
            .ToListAsync();
    }

    /// <summary>
    /// Create or update CVE from external source
    /// </summary>
    /// <param name="cve">CVE entity</param>
    /// <param name="sourceId">Source identifier</param>
    /// <returns>Created or updated CVE</returns>
    public async Task<Cve> CreateOrUpdateFromSourceAsync(Cve cve, Guid sourceId)
    {
        var existingCve = await GetByCveIdAsync(cve.CveId);
        
        if (existingCve != null)
        {
            // Update existing CVE
            existingCve.Title = cve.Title;
            existingCve.Description = cve.Description;
            existingCve.LastModifiedDate = DateTime.SpecifyKind(cve.LastModifiedDate, DateTimeKind.Utc);
            existingCve.CvssV3BaseScore = cve.CvssV3BaseScore;
            existingCve.CvssV3Vector = cve.CvssV3Vector;
            existingCve.CvssV3Severity = cve.CvssV3Severity;
            existingCve.EpssScore = cve.EpssScore;
            existingCve.EpssPercentile = cve.EpssPercentile;
            existingCve.IsKnownExploited = cve.IsKnownExploited;
            existingCve.KevDueDate = cve.KevDueDate.HasValue ? DateTime.SpecifyKind(cve.KevDueDate.Value, DateTimeKind.Utc) : null;
            existingCve.ModifiedDate = DateTime.UtcNow;
            
            await Context.SaveChangesNoAuditAsync();
            return existingCve;
        }
        else
        {
            // Create new CVE
            var newCve = await AddAsync(cve);
            await Context.SaveChangesNoAuditAsync();
            return newCve;
        }
    }

    /// <summary>
    /// Bulk import CVEs
    /// </summary>
    /// <param name="cves">List of CVE entities</param>
    /// <param name="sourceId">Source identifier</param>
    /// <returns>Import result</returns>
    public async Task<CveImportResult> BulkImportAsync(List<Cve> cves, Guid sourceId)
    {
        var result = new CveImportResult
        {
            IsSuccess = true,
            ProcessedCount = cves.Count
        };

        try
        {
            foreach (var cve in cves)
            {
                try
                {
                    var existingCve = await GetByCveIdAsync(cve.CveId);
                    
                    if (existingCve != null)
                    {
                        // Update existing
                        await CreateOrUpdateFromSourceAsync(cve, sourceId);
                        result.UpdatedCount++;
                    }
                    else
                    {
                        // Create new
                        await AddAsync(cve);
                        await Context.SaveChangesNoAuditAsync();
                        result.NewCount++;
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    result.Errors.Add($"Error processing CVE {cve.CveId}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// Delete multiple CVEs by their IDs
    /// </summary>
    /// <param name="cveIds">List of CVE IDs to delete</param>
    /// <returns>Number of deleted CVEs</returns>
    public async Task<int> DeleteMultipleAsync(List<Guid> cveIds)
    {
        if (cveIds == null || !cveIds.Any())
            return 0;

        try
        {
            var cvesToDelete = await Context.Set<Cve>()
                .Where(c => cveIds.Contains(c.Id))
                .ToListAsync();

            if (!cvesToDelete.Any())
                return 0;

            Context.Set<Cve>().RemoveRange(cvesToDelete);
            await Context.SaveChangesNoAuditAsync();

            return cvesToDelete.Count;
        }
        catch (Exception)
        {
            throw;
        }
    }
}