using System.Text;
using System.Text.Json;

namespace Cervantes.Web.Helpers;

/// <summary>
/// Reading and filtering of CSP violation reports (legacy report-uri format, "csp-report" object). Reports come from
/// browsers but anyone can post them, so every value is length-capped and only the path of the document is kept.
/// </summary>
public static class CspReports
{
    public const int MaxBodyBytes = 8 * 1024;
    public const string Path = "/csp-report";
    private const int MaxFieldLength = 500;

    public sealed record CspViolation(string Directive, string BlockedUri, string DocumentPath, string? SourceFile, int? Line);

    public static CspViolation? Parse(string body)
    {
        try
        {
            using var document = JsonDocument.Parse(body);
            if (document.RootElement.ValueKind != JsonValueKind.Object ||
                !document.RootElement.TryGetProperty("csp-report", out var report) ||
                report.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            var directive = FirstToken(GetString(report, "effective-directive")) ?? FirstToken(GetString(report, "violated-directive"));
            if (string.IsNullOrEmpty(directive))
            {
                return null;
            }

            var documentPath = "";
            var documentUri = GetString(report, "document-uri");
            if (documentUri != null && Uri.TryCreate(documentUri, UriKind.Absolute, out var uri))
            {
                documentPath = Cut(uri.AbsolutePath);
            }

            int? line = null;
            if (report.TryGetProperty("line-number", out var lineElement) && lineElement.ValueKind == JsonValueKind.Number &&
                lineElement.TryGetInt32(out var lineNumber))
            {
                line = lineNumber;
            }

            return new CspViolation(Cut(directive), Cut(GetString(report, "blocked-uri") ?? ""), documentPath,
                GetString(report, "source-file") is { } source ? Cut(source) : null, line);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>MudBlazor.Extensions needs eval, which the CSP allows by report only; that noise is dropped.</summary>
    public static bool IsIgnored(CspViolation violation) =>
        violation.BlockedUri == "eval" && violation.Directive.StartsWith("script-src", StringComparison.Ordinal);

    public static async Task<string?> ReadLimitedAsync(Stream body, int maxBytes)
    {
        var buffer = new byte[maxBytes + 1];
        var total = 0;
        while (total < buffer.Length)
        {
            var read = await body.ReadAsync(buffer.AsMemory(total));
            if (read == 0)
            {
                break;
            }
            total += read;
        }
        return total > maxBytes ? null : Encoding.UTF8.GetString(buffer, 0, total);
    }

    private static string? GetString(JsonElement element, string name) =>
        element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

    private static string? FirstToken(string? value) =>
        value?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

    private static string Cut(string value) => value.Length > MaxFieldLength ? value[..MaxFieldLength] : value;
}
