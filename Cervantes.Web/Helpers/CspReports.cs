using System.Net;
using System.Net.Sockets;
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

    /// <summary>Total: any body that cannot be read as a report (bad JSON, lone surrogates, wrong types) gives null.</summary>
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
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// MudBlazor.Extensions tries eval, which the CSP blocks (there is no 'unsafe-eval') and the browser reports. It is
    /// expected noise from our own pages, so it is dropped.
    /// </summary>
    public static bool IsIgnored(CspViolation violation) =>
        violation.BlockedUri == "eval" && violation.Directive.StartsWith("script-src", StringComparison.Ordinal);

    public static async Task<string?> ReadLimitedAsync(Stream body, int maxBytes, CancellationToken cancellationToken = default)
    {
        var buffer = new byte[maxBytes + 1];
        var total = 0;
        while (total < buffer.Length)
        {
            var read = await body.ReadAsync(buffer.AsMemory(total), cancellationToken);
            if (read == 0)
            {
                break;
            }
            total += read;
        }
        return total > maxBytes ? null : Encoding.UTF8.GetString(buffer, 0, total);
    }

    /// <summary>
    /// Rate-limit partition of a client: IPv4 (and IPv4-mapped IPv6) by address, other IPv6 by its /64 prefix, because
    /// a client usually controls a whole /64 and would otherwise get a fresh bucket per address.
    /// </summary>
    public static string PartitionKey(IPAddress? address)
    {
        if (address == null)
        {
            return "unknown";
        }
        if (address.IsIPv4MappedToIPv6)
        {
            address = address.MapToIPv4();
        }
        if (address.AddressFamily == AddressFamily.InterNetworkV6)
        {
            return new IPAddress(address.GetAddressBytes().Take(8).Concat(new byte[8]).ToArray()) + "/64";
        }
        return address.ToString();
    }

    private static string? GetString(JsonElement element, string name) =>
        element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

    private static string? FirstToken(string? value) =>
        value?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

    private static string Cut(string value)
    {
        if (value.Length > MaxFieldLength)
        {
            // Do not split a surrogate pair.
            var length = char.IsHighSurrogate(value[MaxFieldLength - 1]) ? MaxFieldLength - 1 : MaxFieldLength;
            value = value[..length];
        }
        return value.Any(char.IsControl)
            ? new string(value.Select(c => char.IsControl(c) ? '?' : c).ToArray())
            : value;
    }
}
