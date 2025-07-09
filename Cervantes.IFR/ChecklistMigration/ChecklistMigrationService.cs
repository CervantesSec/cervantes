using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.IFR.ChecklistMigration;

/// <summary>
/// Service to migrate existing WSTG and MASTG systems to the new custom checklist system
/// </summary>
public class ChecklistMigrationService
{
    private readonly IChecklistTemplateManager _checklistTemplateManager;
    private readonly ILogger<ChecklistMigrationService> _logger;

    public ChecklistMigrationService(
        IChecklistTemplateManager checklistTemplateManager,
        ILogger<ChecklistMigrationService> logger)
    {
        _checklistTemplateManager = checklistTemplateManager;
        _logger = logger;
    }

    /// <summary>
    /// Creates WSTG system template
    /// </summary>
    /// <param name="userId">System user ID for creating the template</param>
    /// <returns>Created WSTG template</returns>
    public async Task<ChecklistTemplate> CreateWSTGSystemTemplate(string userId)
    {
        try
        {
            var template = new ChecklistTemplate
            {
                Id = Guid.NewGuid(),
                Name = "OWASP Web Security Testing Guide (WSTG) v4.2",
                Description = "Official OWASP Web Security Testing Guide checklist for comprehensive web application security testing",
                Version = "4.2",
                UserId = userId,
                OrganizationId = null, // Global template
                IsSystemTemplate = true,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            // Add WSTG Categories and Items
            CreateWSTGCategories(template);

            var createdTemplate = await _checklistTemplateManager.AddAsync(template);
            await _checklistTemplateManager.Context.SaveChangesAsync();
            _logger.LogInformation("Created WSTG system template with ID: {TemplateId}", createdTemplate.Id);
            
            return createdTemplate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating WSTG system template");
            throw;
        }
    }

    /// <summary>
    /// Creates MASTG system template
    /// </summary>
    /// <param name="userId">System user ID for creating the template</param>
    /// <returns>Created MASTG template</returns>
    public async Task<ChecklistTemplate> CreateMASTGSystemTemplate(string userId)
    {
        try
        {
            var template = new ChecklistTemplate
            {
                Id = Guid.NewGuid(),
                Name = "OWASP Mobile Application Security Testing Guide (MASTG)",
                Description = "Official OWASP Mobile Application Security Testing Guide checklist for comprehensive mobile application security testing",
                Version = "1.0",
                UserId = userId,
                OrganizationId = null, // Global template
                IsSystemTemplate = true,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            // Add MASTG Categories and Items
            CreateMASTGCategories(template);

            var createdTemplate = await _checklistTemplateManager.AddAsync(template);
            await _checklistTemplateManager.Context.SaveChangesAsync();
            _logger.LogInformation("Created MASTG system template with ID: {TemplateId}", createdTemplate.Id);
            
            return createdTemplate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating MASTG system template");
            throw;
        }
    }

    private void CreateWSTGCategories(ChecklistTemplate template)
    {
        try
        {
            var wstgData = LoadWSTGData();
            if (wstgData == null)
            {
                _logger.LogWarning("Could not load WSTG data, falling back to basic categories");
                CreateBasicWSTGCategories(template);
                return;
            }

            int order = 1;
            foreach (var kvp in wstgData.categories)
            {
                var categoryName = kvp.Key;
                var categoryData = kvp.Value;

                var category = new ChecklistCategory
                {
                    Id = Guid.NewGuid(),
                    Name = categoryName,
                    Description = $"WSTG {categoryName} testing category",
                    Order = order++,
                    ChecklistTemplateId = template.Id
                };

                // Add items from JSON data
                CreateWSTGItemsFromJson(category, categoryData);
                template.Categories.Add(category);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating WSTG categories from JSON, falling back to basic categories");
            CreateBasicWSTGCategories(template);
        }
    }

    private WstgJsonRoot? LoadWSTGData()
    {
        try
        {
            var possiblePaths = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChecklistMigration", "wstg.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "ChecklistMigration", "wstg.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "Cervantes.IFR", "ChecklistMigration", "wstg.json"),
                Path.Combine(Environment.CurrentDirectory, "ChecklistMigration", "wstg.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wstg.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "wstg.json")
            };

            string? validPath = null;
            foreach (var path in possiblePaths)
            {
                if (System.IO.File.Exists(path))
                {
                    validPath = path;
                    break;
                }
            }

            if (validPath == null)
            {
                _logger.LogWarning("WSTG JSON file not found in any of the following paths: {Paths}", string.Join(", ", possiblePaths));
                return null;
            }

            _logger.LogInformation("Loading WSTG data from: {JsonPath}", validPath);
            var jsonContent = System.IO.File.ReadAllText(validPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            return JsonSerializer.Deserialize<WstgJsonRoot>(jsonContent, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading WSTG JSON data");
            return null;
        }
    }

    private void CreateWSTGItemsFromJson(ChecklistCategory category, WstgCategory categoryData)
    {
        int order = 1;
        foreach (var test in categoryData.tests)
        {
            var item = new ChecklistItem
            {
                Id = Guid.NewGuid(),
                Code = test.id,
                Name = test.name,
                Description = test.name,
                Objectives = test.objectives != null && test.objectives.Any() 
                    ? string.Join("\n", test.objectives)
                    : "Complete the security test as defined in the WSTG documentation",
                TestProcedure = $"Follow the OWASP WSTG guidelines for {test.id}. Reference: {test.reference}",
                PassCriteria = "Security control is properly implemented and no vulnerabilities are identified",
                Order = order++,
                IsRequired = true,
                Severity = 3,
                References = test.reference,
                ChecklistCategoryId = category.Id
            };

            category.Items.Add(item);
        }
    }

    private void CreateBasicWSTGCategories(ChecklistTemplate template)
    {
        var categories = new[]
        {
            new { Name = "Information Gathering", Description = "Gather information about the target application", Order = 1, Prefix = "INFO" },
            new { Name = "Configuration and Deployment Management Testing", Description = "Test configuration and deployment settings", Order = 2, Prefix = "CONF" },
            new { Name = "Identity Management Testing", Description = "Test identity management mechanisms", Order = 3, Prefix = "IDNT" },
            new { Name = "Authentication Testing", Description = "Test authentication mechanisms", Order = 4, Prefix = "ATHN" },
            new { Name = "Authorization Testing", Description = "Test authorization controls", Order = 5, Prefix = "ATHZ" },
            new { Name = "Session Management Testing", Description = "Test session management controls", Order = 6, Prefix = "SESS" },
            new { Name = "Input Validation Testing", Description = "Test input validation mechanisms", Order = 7, Prefix = "INPV" },
            new { Name = "Error Handling", Description = "Test error handling mechanisms", Order = 8, Prefix = "ERRH" },
            new { Name = "Cryptography", Description = "Test cryptographic implementations", Order = 9, Prefix = "CRYP" },
            new { Name = "Business Logic Testing", Description = "Test business logic implementation", Order = 10, Prefix = "BUSL" },
            new { Name = "Client-side Testing", Description = "Test client-side security controls", Order = 11, Prefix = "CLNT" },
            new { Name = "API Testing", Description = "Test API security controls", Order = 12, Prefix = "APIT" }
        };

        foreach (var cat in categories)
        {
            var category = new ChecklistCategory
            {
                Id = Guid.NewGuid(),
                Name = cat.Name,
                Description = cat.Description,
                Order = cat.Order,
                ChecklistTemplateId = template.Id
            };

            // Add items based on category
            CreateWSTGItems(category, cat.Prefix);
            template.Categories.Add(category);
        }
    }

    private void CreateWSTGItems(ChecklistCategory category, string prefix)
    {
        // This is a simplified example - in a real implementation, you would have
        // detailed definitions for each WSTG test case
        var itemCounts = new Dictionary<string, int>
        {
            ["INFO"] = 10,
            ["CONF"] = 11,
            ["IDNT"] = 5,
            ["ATHN"] = 10,
            ["ATHZ"] = 4,
            ["SESS"] = 9,
            ["INPV"] = 19,
            ["ERRH"] = 2,
            ["CRYP"] = 4,
            ["BUSL"] = 9,
            ["CLNT"] = 13,
            ["APIT"] = 1
        };

        var count = itemCounts.GetValueOrDefault(prefix, 1);
        
        for (int i = 1; i <= count; i++)
        {
            var item = new ChecklistItem
            {
                Id = Guid.NewGuid(),
                Code = $"WSTG-{prefix}-{i:D2}",
                Name = $"{category.Name} Test {i}",
                Description = $"Test case {i} for {category.Name}",
                Objectives = $"Verify security controls for {category.Name.ToLower()}",
                TestProcedure = "Follow OWASP WSTG guidelines for this test case",
                PassCriteria = "Security control is properly implemented and effective",
                Order = i,
                IsRequired = true,
                Severity = 3,
                References = "https://owasp.org/www-project-web-security-testing-guide/",
                ChecklistCategoryId = category.Id
            };

            category.Items.Add(item);
        }
    }

    private void CreateMASTGCategories(ChecklistTemplate template)
    {
        try
        {
            var mastgData = LoadMASTGData();
            if (mastgData == null)
            {
                _logger.LogWarning("Could not load MASTG data, falling back to basic categories");
                CreateBasicMASTGCategories(template);
                return;
            }

            int order = 1;
            foreach (var kvp in mastgData.categories)
            {
                var categoryName = kvp.Key;
                var categoryData = kvp.Value;

                var category = new ChecklistCategory
                {
                    Id = Guid.NewGuid(),
                    Name = categoryName,
                    Description = $"MASTG {categoryName} testing category",
                    Order = order++,
                    ChecklistTemplateId = template.Id
                };

                // Add items from JSON data
                CreateMASTGItemsFromJson(category, categoryData);
                template.Categories.Add(category);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating MASTG categories from JSON, falling back to basic categories");
            CreateBasicMASTGCategories(template);
        }
    }

    private WstgJsonRoot? LoadMASTGData()
    {
        try
        {
            var possiblePaths = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChecklistMigration", "mastg.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "ChecklistMigration", "mastg.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "Cervantes.IFR", "ChecklistMigration", "mastg.json"),
                Path.Combine(Environment.CurrentDirectory, "ChecklistMigration", "mastg.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mastg.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "mastg.json")
            };

            string? validPath = null;
            foreach (var path in possiblePaths)
            {
                if (System.IO.File.Exists(path))
                {
                    validPath = path;
                    break;
                }
            }

            if (validPath == null)
            {
                _logger.LogWarning("MASTG JSON file not found in any of the following paths: {Paths}", string.Join(", ", possiblePaths));
                return null;
            }

            _logger.LogInformation("Loading MASTG data from: {JsonPath}", validPath);
            var jsonContent = System.IO.File.ReadAllText(validPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            return JsonSerializer.Deserialize<WstgJsonRoot>(jsonContent, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading MASTG JSON data");
            return null;
        }
    }

    private void CreateMASTGItemsFromJson(ChecklistCategory category, WstgCategory categoryData)
    {
        int order = 1;
        foreach (var test in categoryData.tests)
        {
            var item = new ChecklistItem
            {
                Id = Guid.NewGuid(),
                Code = test.id,
                Name = test.name,
                Description = test.name,
                Objectives = test.objectives != null && test.objectives.Any() 
                    ? string.Join("\n", test.objectives)
                    : "Complete the security test as defined in the MASTG documentation",
                TestProcedure = $"Follow the OWASP MASTG guidelines for {test.id}. Reference: {test.reference}",
                PassCriteria = "Security control is properly implemented and no vulnerabilities are identified",
                Order = order++,
                IsRequired = true,
                Severity = 3,
                References = test.reference,
                ChecklistCategoryId = category.Id
            };

            category.Items.Add(item);
        }
    }

    private void CreateBasicMASTGCategories(ChecklistTemplate template)
    {
        var categories = new[]
        {
            new { Name = "Data Storage", Description = "Test data storage security", Order = 1, Prefix = "STORAGE" },
            new { Name = "Cryptography", Description = "Test cryptographic implementations", Order = 2, Prefix = "CRYPTO" },
            new { Name = "Authentication", Description = "Test authentication mechanisms", Order = 3, Prefix = "AUTH" },
            new { Name = "Network Communication", Description = "Test network security", Order = 4, Prefix = "NETWORK" },
            new { Name = "Platform Interaction", Description = "Test platform-specific security", Order = 5, Prefix = "PLATFORM" },
            new { Name = "Code Quality", Description = "Test code quality and security", Order = 6, Prefix = "CODE" },
            new { Name = "Resilience", Description = "Test application resilience", Order = 7, Prefix = "RESILIENCE" }
        };

        foreach (var cat in categories)
        {
            var category = new ChecklistCategory
            {
                Id = Guid.NewGuid(),
                Name = cat.Name,
                Description = cat.Description,
                Order = cat.Order,
                ChecklistTemplateId = template.Id
            };

            // Add items based on category
            CreateMASTGItems(category, cat.Prefix);
            template.Categories.Add(category);
        }
    }

    private void CreateMASTGItems(ChecklistCategory category, string prefix)
    {
        // This is a simplified example - in a real implementation, you would have
        // detailed definitions for each MASTG test case
        var itemCounts = new Dictionary<string, int>
        {
            ["STORAGE"] = 14,
            ["CRYPTO"] = 6,
            ["AUTH"] = 13,
            ["NETWORK"] = 9,
            ["PLATFORM"] = 25,
            ["CODE"] = 15,
            ["RESILIENCE"] = 11
        };

        var count = itemCounts.GetValueOrDefault(prefix, 1);
        
        for (int i = 1; i <= count; i++)
        {
            var item = new ChecklistItem
            {
                Id = Guid.NewGuid(),
                Code = $"MASTG-{prefix}-{i:D2}",
                Name = $"{category.Name} Test {i}",
                Description = $"Mobile test case {i} for {category.Name}",
                Objectives = $"Verify mobile security controls for {category.Name.ToLower()}",
                TestProcedure = "Follow OWASP MASTG guidelines for this test case",
                PassCriteria = "Security control is properly implemented and effective",
                Order = i,
                IsRequired = true,
                Severity = 3,
                References = "https://mas.owasp.org/MASTG/",
                ChecklistCategoryId = category.Id
            };

            category.Items.Add(item);
        }
    }

    /// <summary>
    /// Initializes system templates if they don't exist
    /// </summary>
    /// <param name="systemUserId">System user ID</param>
    /// <returns>Task</returns>
    public async Task InitializeSystemTemplates(string systemUserId)
    {
        try
        {
            var existingTemplates = _checklistTemplateManager.GetSystemTemplates().ToList();

            // Check if WSTG template exists
            if (!existingTemplates.Any(t => t.Name.Contains("WSTG")))
            {
                await CreateWSTGSystemTemplate(systemUserId);
            }

            // Check if MASTG template exists
            if (!existingTemplates.Any(t => t.Name.Contains("MASTG")))
            {
                await CreateMASTGSystemTemplate(systemUserId);
            }

            _logger.LogInformation("System templates initialization completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing system templates");
            throw;
        }
    }
}