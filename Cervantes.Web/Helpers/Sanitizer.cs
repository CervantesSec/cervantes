using System.Text.RegularExpressions;
using System.Web;
using Ganss.Xss;

namespace Cervantes.Web.Helpers;

public class Sanitizer
{
    private readonly HtmlSanitizer _sanitizer;
    private static readonly Regex _htmlEntityRegex = new Regex(@"&(?:#\d+|#x[a-fA-F0-9]+|[a-zA-Z]+);", RegexOptions.Compiled);
    private static readonly Regex _htmlTagRegex = new Regex(@"</?[a-zA-Z]+(?:\s+[a-zA-Z]+(?:\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)*\s*/?>", RegexOptions.Compiled);
    
    // Regex to match pre/code blocks with their content
    private static readonly Regex _codeBlockRegex = new Regex(
        @"<pre([^>]*)><code([^>]*)>(.*?)</code></pre>",
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
    
    private static readonly Regex _preBlockRegex = new Regex(
        @"<pre([^>]*)>(.*?)</pre>",
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

    public Sanitizer()
    {
        _sanitizer = new HtmlSanitizer
        {
            KeepChildNodes = true
        };
        _sanitizer.AllowedAttributes.Add("class");
        _sanitizer.AllowedAttributes.Add("tabindex");
        _sanitizer.AllowedAttributes.Add("data-language");
        _sanitizer.AllowedAttributes.Add("style");
        _sanitizer.AllowedSchemes.Add("data");
        _sanitizer.AllowedTags.Add("pre");
        _sanitizer.AllowedTags.Add("code");
        
        // Allow data attributes
        foreach (var attr in new[] { "data-line", "data-language", "data-prismjs-copy", "data-src" })
        {
            _sanitizer.AllowedAttributes.Add(attr);
        }
    }

    public string Sanitize(string html)
    {
        if (string.IsNullOrEmpty(html))
            return string.Empty;

        // Step 1: Extract and protect code blocks before any processing
        var codeBlocks = new Dictionary<string, string>();
        var placeholderIndex = 0;
        
        // Replace code blocks with placeholders
        html = _codeBlockRegex.Replace(html, match =>
        {
            var placeholder = $"___CODE_BLOCK_{placeholderIndex}___";
            codeBlocks[placeholder] = match.Value;
            placeholderIndex++;
            return placeholder;
        });
        
        // Also handle standalone pre blocks
        html = _preBlockRegex.Replace(html, match =>
        {
            // Skip if this pre block was already handled as part of a pre/code pair
            if (match.Value.Contains("<code"))
                return match.Value;
                
            var placeholder = $"___PRE_BLOCK_{placeholderIndex}___";
            codeBlocks[placeholder] = match.Value;
            placeholderIndex++;
            return placeholder;
        });

        // Step 2: Decode the non-code content
        string decodedHtml = html;
        string previousDecoded;
        do 
        {
            previousDecoded = decodedHtml;
            decodedHtml = HttpUtility.HtmlDecode(previousDecoded);
        } while (decodedHtml != previousDecoded);

        // Step 3: Sanitize the content (without code blocks)
        string sanitized = _sanitizer.Sanitize(decodedHtml);
        
        // Step 4: Restore the protected code blocks
        foreach (var kvp in codeBlocks)
        {
            sanitized = sanitized.Replace(kvp.Key, kvp.Value);
        }
        
        return sanitized;
    }

    private static bool IsHtmlEncoded(string input)
    {
        return !string.IsNullOrEmpty(input) && _htmlEntityRegex.IsMatch(input);
    }

    public static bool IsHtmlCode(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (_htmlTagRegex.IsMatch(input))
            return true;

        if (_htmlEntityRegex.IsMatch(input))
            return true;

        if (input.Trim().StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
            return true;

        if (input.Contains("<!--") && input.Contains("-->"))
            return true;

        return false;
    }
    
    private string ProcessCodeSamples(string content)
    {
        var languages = new[] { "markup", "javascript", "css", "php", "ruby", "python", "java", "c", "csharp", "cpp" };

        foreach (var lang in languages)
        {
            var pattern = $@"<pre class=""language-{lang}"">";
            var replacement = $@"<pre class=""language-{lang}"" style=""{GetLanguageSpecificStyle(lang)}"" data-language=""{lang}"">";
            content = Regex.Replace(content, pattern, replacement);
        }

        return content;
    }

    private string GetLanguageSpecificStyle(string language)
    {
        var baseStyle = "background-color: #f4f4f4; color: #666; page-break-inside: avoid; font-family: monospace; font-size: 15px; line-height: 1.6; margin-bottom: 1.6em; max-width: 100%; overflow: auto; padding: 1em 1.5em; display: block; word-wrap: break-word; border: 1px solid #ddd; border-left: 3px solid {0};";
        
        return string.Format(baseStyle, language switch
        {
            "markup" => "#e34c26",  // HTML/XML: Orange (HTML5 color)
            "javascript" => "#f0db4f",  // JavaScript: Yellow
            "css" => "#264de4",  // CSS: Blue
            "php" => "#777bb3",  // PHP: Purple
            "ruby" => "#cc342d",  // Ruby: Red
            "python" => "#306998",  // Python: Blue
            "java" => "#5382a1",  // Java: Blue
            "c" => "#a8b9cc",  // C: Light Blue
            "csharp" => "#68217a",  // C#: Purple
            "cpp" => "#00599c",  // C++: Dark Blue
            _ => "#f36d33"  // Default: Orange
        });
    }
}