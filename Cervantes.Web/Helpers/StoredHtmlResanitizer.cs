using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cervantes.Web.Helpers;

/// <summary>
/// Re-runs <see cref="Sanitizer"/> over every stored HTML field that a component renders with MarkupString.
/// Write paths sanitize new data; this cleans rows stored before a sanitizer fix or restored from a backup.
/// A value that is already clean sanitizes to itself, so a second run changes nothing.
/// The pass commits per batch of rows (see <see cref="BatchSize"/>), so it is idempotent and safe to repeat: a run
/// that stops halfway keeps the batches already saved, and running it again finishes the rest.
/// Custom field values are covered for every field type, not only Textarea: a field's type can be changed later, and
/// a raw value of an Input field would then render as HTML. The write paths sanitize every type the same way.
/// Not covered: Report.HtmlCode (ReportDialog renders it in a sandboxed iframe), Vault.Value (a secret), and role
/// descriptions (they live in the AuthPermissions tables, outside <see cref="IApplicationDbContext"/>).
/// </summary>
public class StoredHtmlResanitizer
{
    // Rows read, cleaned and saved per round trip: bounds memory on large tables.
    private const int BatchSize = 200;

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
        changed += await Resanitize<Vuln>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<Vuln>(x => x.Impact, (x, v) => x.Impact = v);
        changed += await Resanitize<Vuln>(x => x.ProofOfConcept, (x, v) => x.ProofOfConcept = v);
        changed += await Resanitize<Vuln>(x => x.Remediation, (x, v) => x.Remediation = v);
        changed += await Resanitize<VulnCustomFieldValue>(x => x.Value, (x, v) => x.Value = v);
        changed += await Resanitize<VulnNote>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<VulnCategory>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<Client>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<Document>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<Project>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<Project>(x => x.ExecutiveSummary, (x, v) => x.ExecutiveSummary = v);
        changed += await Resanitize<ProjectNote>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<CORE.Entities.Task>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<TaskNote>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<Note>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<Target>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<TargetServices>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<Vault>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<KnowledgeBase>(x => x.Content, (x, v) => x.Content = v);
        changed += await Resanitize<KnowledgeBaseCategories>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<ApplicationUser, string>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<ReportTemplate>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<Report>(x => x.Description, (x, v) => x.Description = v);
        changed += await Resanitize<ChatMessage>(x => x.Content, (x, v) => x.Content = v);
        changed += await Resanitize<ChecklistTemplate>(x => x.Description, (x, v) => x.Description = v);

        // Each batch is saved as it is cleaned (no audit rows: this is a maintenance pass), so there is nothing left to save here.
        logger.LogInformation("Stored HTML re-sanitize pass finished, {Changed} fields changed", changed);
        return changed;
    }

    // Every entity except ApplicationUser (string key, from IdentityUser) has a Guid Id.
    private Task<int> Resanitize<T>(Func<T, string?> get, Action<T, string> set) where T : class
        => Resanitize<T, Guid>(get, set);

    // Keyset pagination on "Id": WHERE Id > last ORDER BY Id LIMIT BatchSize. Cleaned rows are saved and detached
    // after each batch, so the change tracker never holds more than BatchSize entities. A second run changes nothing.
    private async Task<int> Resanitize<T, TKey>(Func<T, string?> get, Action<T, string> set)
        where T : class where TKey : IComparable<TKey>
    {
        var changed = 0;
        var first = true;
        TKey? last = default;
        while (true)
        {
            var query = context.Set<T>().AsQueryable();
            if (!first)
            {
                var after = last!;
                query = query.Where(x => EF.Property<TKey>(x, "Id").CompareTo(after) > 0);
            }

            var batch = await query.OrderBy(x => EF.Property<TKey>(x, "Id")).Take(BatchSize).ToListAsync();
            if (batch.Count == 0)
            {
                break;
            }

            last = (TKey)context.Entry(batch[^1]).Property("Id").CurrentValue!;
            first = false;

            var batchChanged = 0;
            foreach (var entity in batch)
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
                    batchChanged++;
                }
            }

            if (batchChanged > 0)
            {
                await context.SaveChangesNoAuditAsync();
                changed += batchChanged;
            }

            foreach (var entity in batch)
            {
                context.Entry(entity).State = EntityState.Detached;
            }

            if (batch.Count < BatchSize)
            {
                break;
            }
        }

        return changed;
    }
}
