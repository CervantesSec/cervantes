using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Web.Helpers;

/// <summary>
/// Re-runs <see cref="Sanitizer"/> over every stored HTML field that a component renders with MarkupString.
/// Write paths sanitize new data; this cleans rows stored before a sanitizer fix or restored from a backup.
/// A value that is already clean sanitizes to itself, so a second run changes nothing.
/// Custom field values are covered for every field type, not only Textarea: a field's type can be changed later, and
/// a raw value of an Input field would then render as HTML. The write paths sanitize every type the same way.
/// Not covered: Report.HtmlCode (ReportDialog renders it in a sandboxed iframe), Vault.Value (a secret), and role
/// descriptions (they live in the AuthPermissions tables, outside <see cref="IApplicationDbContext"/>).
/// </summary>
public class StoredHtmlResanitizer
{
    private readonly IApplicationDbContext context;
    private readonly Sanitizer sanitizer;
    private readonly ILogger<StoredHtmlResanitizer> logger;

    public StoredHtmlResanitizer(IApplicationDbContext context, Sanitizer sanitizer, ILogger<StoredHtmlResanitizer> logger)
    {
        this.context = context;
        this.sanitizer = sanitizer;
        this.logger = logger;
    }

    /// <returns>The number of fields that changed.</returns>
    public async Task<int> RunAsync()
    {
        var changed = 0;
        changed += Resanitize<Vuln>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<Vuln>(x => x.Impact, (x, v) => x.Impact = v);
        changed += Resanitize<Vuln>(x => x.ProofOfConcept, (x, v) => x.ProofOfConcept = v);
        changed += Resanitize<Vuln>(x => x.Remediation, (x, v) => x.Remediation = v);
        changed += Resanitize<VulnCustomFieldValue>(x => x.Value, (x, v) => x.Value = v);
        changed += Resanitize<VulnNote>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<VulnCategory>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<Client>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<Document>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<Project>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<Project>(x => x.ExecutiveSummary, (x, v) => x.ExecutiveSummary = v);
        changed += Resanitize<ProjectNote>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<CORE.Entities.Task>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<TaskNote>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<Note>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<Target>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<TargetServices>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<Vault>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<KnowledgeBase>(x => x.Content, (x, v) => x.Content = v);
        changed += Resanitize<KnowledgeBaseCategories>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<ApplicationUser>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<ReportTemplate>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<Report>(x => x.Description, (x, v) => x.Description = v);
        changed += Resanitize<ChatMessage>(x => x.Content, (x, v) => x.Content = v);
        changed += Resanitize<ChecklistTemplate>(x => x.Description, (x, v) => x.Description = v);

        // Maintenance pass: do not write one audit row per cleaned field.
        await context.SaveChangesNoAuditAsync();
        logger.LogInformation("Stored HTML re-sanitize pass finished, {Changed} fields changed", changed);
        return changed;
    }

    private int Resanitize<T>(Func<T, string?> get, Action<T, string> set) where T : class
    {
        var changed = 0;
        foreach (var entity in context.Set<T>().ToList())
        {
            var value = get(entity);
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            var clean = sanitizer.Sanitize(value);
            if (clean != value)
            {
                set(entity, clean);
                changed++;
            }
        }

        return changed;
    }
}
