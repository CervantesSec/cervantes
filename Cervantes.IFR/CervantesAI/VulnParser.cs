using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Collections.Generic;
using Cervantes.CORE.Entities;
using Markdig;

namespace Cervantes.IFR.CervantesAI;

public class VulnParser
{
    // The markdown comes from the AI provider, so every regex runs with a timeout to bound backtracking cost.
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);

    // Far above what AIConfiguration:MaxTokens produces; larger inputs are truncated before parsing.
    private const int MaxMarkdownLength = 512_000;

    private static readonly Regex[] TitleRegexes = new[]
    {
        @"###\s*Finding:\s*(.*?)(?=\n|$)",              // ### Finding: Title
        @"#\s+(.*?)(?=\n|$)",                          // # Title
        @"###\s+(.*?)(?=\n|$)",                        // ### Title
        @"##\s+(.*?)(?=\n|$)",                         // ## Title
    }.Select(p => new Regex(p, RegexOptions.Compiled, RegexTimeout)).ToArray();

    private static readonly Regex[] RiskRegexes = new[]
    {
        @"####\s*\*\*Risk Level\*\*\s*\n\*\*(.*?)\*\*",     // #### **Risk Level** \n**Critical**
        @"####\s*Risk Level\s*\n\*\*(.*?)\*\*",             // #### Risk Level \n**Critical**
        @"##\s*Risk Level\s*\n\*\*(.*?)\*\*",
        @"##\s*Risk Level\s*\n\*\*(.*?)\*\*",// ## Risk Level \n**Critical**
        @"Risk Level:\s*\*\*(.*?)\*\*",
        @"\*\*Risk Level:\*\*\s*(.*?)",

        @"####\s*\*\*Nivel de Riesgo\*\*\s*\n\*\*(.*?)\*\*",     // #### **Risk Level** \n**Critical**
        @"####\s*Nivel de Riesgo\s*\n\*\*(.*?)\*\*",             // #### Risk Level \n**Critical**
        @"##\s*Nivel de Riesgo\s*\n\*\*(.*?)\*\*",
        @"##\s*Nivel de Riesgo\s*\n\*\*(.*?)\*\*",// ## Risk Level \n**Critical**
        @"Nivel de Riesgo:\s*\*\*(.*?)\*\*",
        @"\*\*Nivel de Riesgo:\*\*\s*(.*?)",

        @"####\s*\*\*Nível de Risco\*\*\s*\n\*\*(.*?)\*\*",     // #### **Risk Level** \n**Critical**
        @"####\s*Nível de Riscoo\s*\n\*\*(.*?)\*\*",             // #### Risk Level \n**Critical**
        @"##\s*Nível de Risco\s*\n\*\*(.*?)\*\*",
        @"##\s*Nível de Risco\s*\n\*\*(.*?)\*\*",// ## Risk Level \n**Critical**
        @"Nível de Risco:\s*\*\*(.*?)\*\*",
        @"\*\*Nível de Risco:\*\*\s*(.*?)",
    }.Select(p => new Regex(p, RegexOptions.Singleline | RegexOptions.Compiled, RegexTimeout)).ToArray();

    // Section patterns embed the section name, so they are built once per name and cached.
    private static readonly ConcurrentDictionary<string, Regex[]> SectionRegexCache = new();

    private static readonly Regex BulletBoldHeaderRegex =
        new(@"^\*\*([^:]+):\*\*\s*(.+)$", RegexOptions.Compiled, RegexTimeout);

    private static readonly Regex InlineCodeRegex = new(@"`([^`]+)`", RegexOptions.Compiled, RegexTimeout);
    private static readonly Regex BoldRegex = new(@"\*\*([^*]+)\*\*", RegexOptions.Compiled, RegexTimeout);
    private static readonly Regex ItalicRegex = new(@"\*([^*]+)\*", RegexOptions.Compiled, RegexTimeout);
    private static readonly Regex BracketsRegex = new(@"\[([^\]]+)\]", RegexOptions.Compiled, RegexTimeout);

    public static VulnAiModel ParseSecurityReport(string markdown, Language language)
    {
        if (markdown is { Length: > MaxMarkdownLength })
        {
            markdown = markdown.Substring(0, MaxMarkdownLength);
        }

        var vuln = new VulnAiModel();
        switch (language)
        {
            case Language.English:
                vuln.Description = Markdown.ToHtml(ExtractSection(markdown, "Description"));
                vuln.Impact = Markdown.ToHtml(ExtractSection(markdown, "Impact"));
                var riskLevel = ExtractRiskLevel(markdown);
                switch (riskLevel)
                {
                    case "Critical":
                        vuln.Risk = VulnRisk.Critical;
                        break;
                    case "High":
                        vuln.Risk = VulnRisk.High;
                        break;
                    case "Medium":
                        vuln.Risk = VulnRisk.Medium;
                        break;
                    case "Low":
                        vuln.Risk = VulnRisk.Low;
                        break;
                    case "Info":
                        vuln.Risk = VulnRisk.Info;
                        break;
                }
                vuln.ProofOfConcept = Markdown.ToHtml(ExtractSection(markdown, "Proof of Concept"));
                vuln.Remediation = Markdown.ToHtml(ExtractSection(markdown, "Remediation"));
                break;
            case Language.Español:
                vuln.Description = Markdown.ToHtml(ExtractSection(markdown, "Descripción"));
                vuln.Impact = Markdown.ToHtml(ExtractSection(markdown, "Impacto"));
                var riesgo = ExtractRiskLevel(markdown);
                switch (riesgo)
                {
                    case "Crítico":
                        vuln.Risk = VulnRisk.Critical;
                        break;
                    case "Alto":
                        vuln.Risk = VulnRisk.High;
                        break;
                    case "Medio":
                        vuln.Risk = VulnRisk.Medium;
                        break;
                    case "Bajo":
                        vuln.Risk = VulnRisk.Low;
                        break;
                    case "Informativo":
                        vuln.Risk = VulnRisk.Info;
                        break;
                }
                vuln.ProofOfConcept = Markdown.ToHtml(ExtractSection(markdown, "Prueba de Concepto"));
                vuln.Remediation = Markdown.ToHtml(ExtractSection(markdown, "Remediación"));
                break;
            case Language.Português:
                vuln.Description = Markdown.ToHtml(ExtractSection(markdown, "Descrição"));
                vuln.Impact = Markdown.ToHtml(ExtractSection(markdown, "Impacto"));
                var risco = ExtractRiskLevel(markdown);
                switch (risco)
                {
                    case "Crítico":
                        vuln.Risk = VulnRisk.Critical;
                        break;
                    case "Alto":
                        vuln.Risk = VulnRisk.High;
                        break;
                    case "Médio":
                        vuln.Risk = VulnRisk.Medium;
                        break;
                    case "Baixo":
                        vuln.Risk = VulnRisk.Low;
                        break;
                    case "Informativo":
                        vuln.Risk = VulnRisk.Info;
                        break;
                }
                vuln.ProofOfConcept = Markdown.ToHtml(ExtractSection(markdown, "Prova de Conceito"));
                vuln.Remediation = Markdown.ToHtml(ExtractSection(markdown, "Remediação"));
                break;
        }

        return vuln;
    }
    
   public static string ExtractTitle(string markdown)
    {
        foreach (var regex in TitleRegexes)
        {
            var match = SafeMatch(regex, markdown);
            if (match.Success)
            {
                return CleanMarkdownText(match.Groups[1].Value);
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// Extracts the risk level with support for multiple formats
    /// </summary>
    public static string ExtractRiskLevel(string markdown)
    {
        foreach (var regex in RiskRegexes)
        {
            var match = SafeMatch(regex, markdown);
            if (match.Success)
            {
                return CleanMarkdownText(match.Groups[1].Value);
            }
        }
        return string.Empty;
    }

    public static string ExtractSection(string markdown, string sectionName)
    {
        var sectionRegexes = SectionRegexCache.GetOrAdd(sectionName, BuildSectionRegexes);

        foreach (var regex in sectionRegexes)
        {
            Match match;
            try
            {
                match = regex.Match(markdown);
            }
            catch (RegexMatchTimeoutException)
            {
                // The remaining patterns use similar lookaheads and would most likely time out too,
                // so give up on this section instead of spending the timeout once per pattern.
                return string.Empty;
            }

            if (match.Success)
            {
                return ProcessSectionContent(match.Groups[1].Value);
            }
        }
        return string.Empty;
    }

    private static Regex[] BuildSectionRegexes(string sectionName)
    {
        string[] sectionPatterns = new[]
        {
            // Format: #### **Section**
            $@"####\s*\*\*{Regex.Escape(sectionName)}\*\*\s*\n(.*?)(?=####|\z)",
            
            // Format: ## Section
            $@"##\s*{Regex.Escape(sectionName)}\s*\n(.*?)(?=##|\z)",
            
            // Format: #### Section
            $@"####\s*{Regex.Escape(sectionName)}\s*\n(.*?)(?=####|\z)",

            // Format: **Section:**
            $@"\*\*{Regex.Escape(sectionName)}:\*\*\s*(.*?)(?=\n\s*\*\*[^*]+:\*\*|\z)",
            $@"\*\*{Regex.Escape(sectionName)}:\*\*\s*\n(.*?)(?=\*\*[^*]+:\*\*|\z)",

            // Format: **Section**:
            $@"\*\*{Regex.Escape(sectionName)}:\*\*\s*(.*?)(?=\n\s*\*\*[^*]+:\*\*|\z)",
            // Bold section with colon
            //$@"\*\*{Regex.Escape(sectionName)}:\*\*\s*(.*?)(?=\n\s*\*\*|\z)",
            
            // Standard markdown headers with bold
            $@"(?:#{2,4})\s*\*\*{Regex.Escape(sectionName)}\*\*\s*(.*?)(?=(?:#{2,4}|\z))",
            
            // Standard markdown headers
            $@"(?:#{2,4})\s*{Regex.Escape(sectionName)}:?\s*(.*?)(?=(?:#{2,4}|\z))",
            
            // Bold section without colon
            $@"\*\*{Regex.Escape(sectionName)}\*\*\s*(.*?)(?=\n\s*\*\*|\z)",
            $@"\*\*{Regex.Escape(sectionName)}:\*\*\s*(.*?)(?=\n\s*\*\*|\z)",
            
            // Pattern for bold sections with colon
            $@"\*\*{Regex.Escape(sectionName)}:\*\*\s*(.*?)(?=\n\s*\*\*[^*]+:\*\*|\z)",
            
            // Pattern for headers with or without bold
            $@"(?:#{2,4})\s*\*?{Regex.Escape(sectionName)}\*?\s*:?\s*\n(.*?)(?=(?:#{2,4}[^#]|\*\*[^*]+:\*\*|\z))",
            
            // Pattern for bold sections without colon
            $@"\*\*{Regex.Escape(sectionName)}\*\*\s*\n(.*?)(?=\n\s*\*\*[^*]+:\*\*|\z)",
            
            // Backup patterns for edge cases
            $@"####\s*\*\*{Regex.Escape(sectionName)}\*\*\s*\n(.*?)(?=####|\*\*[^*]+:\*\*|\z)",
            $@"##\s*{Regex.Escape(sectionName)}\s*\n(.*?)(?=##|\*\*[^*]+:\*\*|\z)",
        };

        return sectionPatterns
            .Select(p => new Regex(p, RegexOptions.Singleline, RegexTimeout))
            .ToArray();
    }

 private static string ProcessSectionContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        var result = new StringBuilder();
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        bool inCodeBlock = false;
        var codeBlockContent = new StringBuilder();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].TrimStart();

            // Skip empty lines at the beginning
            if (i == 0 && string.IsNullOrWhiteSpace(line))
                continue;

            // Handle code blocks
            if (line.StartsWith("```"))
            {
                if (!inCodeBlock)
                {
                    inCodeBlock = true;
                    codeBlockContent.Clear();
                    result.AppendLine(line);
                }
                else
                {
                    inCodeBlock = false;
                    result.AppendLine(codeBlockContent.ToString());
                    result.AppendLine(line);
                }
                continue;
            }

            if (inCodeBlock)
            {
                codeBlockContent.AppendLine(line);
                continue;
            }

            // Handle bullet points
            if (line.StartsWith("* "))
            {
                string bulletContent = line.Substring(2);
                var boldHeaderMatch = SafeMatch(BulletBoldHeaderRegex, bulletContent);
                
                if (boldHeaderMatch.Success)
                {
                    string header = boldHeaderMatch.Groups[1].Value.Trim();
                    string description = boldHeaderMatch.Groups[2].Value.Trim();
                    
                    // Preserve inline code in description
                    description = PreserveInlineCode(description);
                    
                    result.AppendLine($"* {header}: {description}");
                }
                else
                {
                    // Handle regular bullet points
                    result.AppendLine($"* {PreserveInlineCode(bulletContent.Trim())}");
                }
            }
            // Handle continuation of bullet point content (indented lines)
            else if (!string.IsNullOrWhiteSpace(line))
            {
                // Preserve formatting for continued lines
                string processedLine = PreserveInlineCode(line);
                result.AppendLine($"  {processedLine}");
            }
            else
            {
                // Add empty line
                result.AppendLine();
            }
        }

        return result.ToString().Trim();
    }

    /// <summary>
    /// Preserves inline code blocks and cleans other markdown
    /// </summary>
    private static string PreserveInlineCode(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Store code blocks for preservation
        var codeBlocks = new System.Collections.Generic.Dictionary<string, string>();
        int counter = 0;

        // Preserve inline code
        text = SafeReplace(InlineCodeRegex, text, m =>
        {
            string placeholder = $"__CODE_{counter}__";
            codeBlocks[placeholder] = m.Groups[0].Value;
            counter++;
            return placeholder;
        });

        // Clean markdown formatting
        text = SafeReplace(BoldRegex, text, "$1");      // Remove bold
        text = SafeReplace(ItalicRegex, text, "$1");    // Remove italic
        text = SafeReplace(BracketsRegex, text, "$1");  // Remove brackets

        // Restore code blocks
        foreach (var block in codeBlocks)
        {
            text = text.Replace(block.Key, block.Value);
        }

        return text.Trim();
    }

    /// <summary>
    /// Cleans markdown formatting while preserving structure
    /// </summary>
    private static string CleanMarkdownText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Remove markdown formatting
        text = SafeReplace(BoldRegex, text, "$1");      // Remove bold
        text = SafeReplace(ItalicRegex, text, "$1");    // Remove italic
        text = SafeReplace(BracketsRegex, text, "$1");  // Remove brackets

        return text.Trim();
    }

    // A regex timeout is treated as "no match" so a pathological AI response degrades to an empty
    // or unformatted field instead of failing the whole vulnerability generation.
    private static Match SafeMatch(Regex regex, string input)
    {
        try
        {
            return regex.Match(input);
        }
        catch (RegexMatchTimeoutException)
        {
            return Match.Empty;
        }
    }

    private static string SafeReplace(Regex regex, string input, string replacement)
    {
        try
        {
            return regex.Replace(input, replacement);
        }
        catch (RegexMatchTimeoutException)
        {
            return input;
        }
    }

    private static string SafeReplace(Regex regex, string input, MatchEvaluator evaluator)
    {
        try
        {
            return regex.Replace(input, evaluator);
        }
        catch (RegexMatchTimeoutException)
        {
            return input;
        }
    }
}