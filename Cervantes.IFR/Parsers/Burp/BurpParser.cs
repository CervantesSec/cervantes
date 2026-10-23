using System;
using System.Linq;
using System.Net;
using System.Xml;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Ganss.Xss;

namespace Cervantes.IFR.Parsers.Burp;

public class BurpParser: IBurpParser
{
    private ITargetManager targetManager = null;
    private IVulnManager vulnManager = null;
    // The finding ids of the import in progress; the project's ids are read once per import, not once per vuln.
    private FindingIdSequence findingIds = FindingIdSequence.Empty;
    private IVulnTargetManager vulnTargetManager = null;
    //private readonly IHostingEnvironment _appEnvironment;
    private IProjectManager projectManager = null;

    
    public BurpParser(ITargetManager targetManager, IVulnManager vulnManager, 
        IVulnTargetManager vulnTargetManager, IProjectManager projectManager )
    {
        this.targetManager = targetManager;
        this.vulnManager = vulnManager;
        this.vulnTargetManager = vulnTargetManager;
        this.projectManager = projectManager;
    }
    // A recorded request or response longer than this is cut: responses often hold whole pages or binaries
    private const int MaxMessageLength = 4000;

    /// <summary>
    /// The proof of concept of an issue as plain text: each <c>requestresponse</c> as <c>Request:</c> and
    /// <c>Response:</c> blocks, base64-decoded when <c>base64="true"</c> and cut to 4000 characters, plus a legacy
    /// <c>poc</c> element if present; "" when the issue recorded nothing.
    /// </summary>
    public static string PocOf(XmlNode issue)
    {
        static string Message(XmlNode? node)
        {
            if (node == null || string.IsNullOrEmpty(node.InnerText))
            {
                return "";
            }
            var text = node.InnerText;
            if (node.Attributes?["base64"]?.Value == "true")
            {
                try
                {
                    text = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(node.InnerText.Trim()));
                }
                catch (FormatException)
                {
                    // Keep the text as it is: a damaged message should not fail the whole import
                }
            }
            return text.Length > MaxMessageLength ? text[..MaxMessageLength] + "\n[…]" : text;
        }

        var blocks = new System.Collections.Generic.List<string>();
        foreach (XmlNode pair in issue.SelectNodes("requestresponse")!)
        {
            var request = Message(pair.SelectSingleNode("request"));
            var response = Message(pair.SelectSingleNode("response"));
            if (request.Length > 0)
            {
                blocks.Add("Request:\n" + request);
            }
            if (response.Length > 0)
            {
                blocks.Add("Response:\n" + response);
            }
        }
        if (issue.SelectSingleNode("poc")?.InnerText is { Length: > 0 } legacy)
        {
            blocks.Add(legacy);
        }
        return string.Join("\n\n", blocks);
    }

    /// <summary>The proof of concept as stored: <see cref="PocOf"/> encoded inside <c>&lt;pre&gt;</c>, so markup in a response shows as text.</summary>
    public static string PocHtml(XmlNode issue)
    {
        var poc = PocOf(issue);
        return poc.Length > 0 ? "<pre>" + WebUtility.HtmlEncode(poc) + "</pre>" : "";
    }

    public void Parse(Guid? project, string user, string path)
    {
        findingIds = FindingIdSequence.Empty;
        // Burp stores HTML in CDATA, so InnerText is markup. Sanitize it, and do not HtmlDecode first:
        // payloads quoted in issueDetail are entity-encoded on purpose and must stay text.
        var sanitizer = new HtmlSanitizer().AllowDataImagesOnly();

        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        XmlNodeList issues = doc.GetElementsByTagName("issue");

        if (project != null)
        {
            var pro = projectManager.GetById((Guid) project);
            findingIds = FindingIdGenerator.Start(vulnManager, pro);
            foreach (XmlNode issue in issues)
            {
                string name = "";
                if(issue.SelectSingleNode("name") != null){name = issue.SelectSingleNode("name").InnerText;}
                
                string severity = "";
                if(issue.SelectSingleNode("severity") != null){severity = issue.SelectSingleNode("severity").InnerText;}
                
                string remediation = "";
                if(issue.SelectSingleNode("remediationBackground") != null){remediation = issue.SelectSingleNode("remediationBackground").InnerText;}
                
                string remediationDet = "";
                if(issue.SelectSingleNode("remediationDetail") != null){remediationDet = issue.SelectSingleNode("remediationDetail").InnerText;}
                
                string references = "";
                if(issue.SelectSingleNode("references") != null){references = issue.SelectSingleNode("references").InnerText;}
                
                string issueBack = "";
                if(issue.SelectSingleNode("issueBackground") != null){issueBack = issue.SelectSingleNode("issueBackground").InnerText;}
                
                string issueDet = "";
                if(issue.SelectSingleNode("issueDetail") != null){issueDet = issue.SelectSingleNode("issueDetail").InnerText;}
                
                string host = "";
                if(issue.SelectSingleNode("host") != null){host = issue.SelectSingleNode("host").InnerText;}
                
                string location = "";
                if(issue.SelectSingleNode("location") != null){location = issue.SelectSingleNode("location").InnerText;}
                
                string pathIssue = "";
                if(issue.SelectSingleNode("path") != null){pathIssue = issue.SelectSingleNode("path").InnerText;}
                
                string classifications = "";
                if(issue.SelectSingleNode("vulnerabilityClassifications") != null){classifications = issue.SelectSingleNode("vulnerabilityClassifications").InnerText;}
                
                string detailItems = string.Join("<br>", issue.SelectNodes("issueDetailItems/issueDetailItem")!.Cast<XmlNode>()
                    .Select(x => WebUtility.HtmlEncode(x.InnerText)));


                host = ImportedTarget.Name(sanitizer, host);
                var tar = targetManager.GetByName(host, project);
                Target target = null;
                if (tar == null)
                {
                    target = new Target
                    {
                        UserId = user,
                        ProjectId = project,
                        Name = host,
                        Description = "Host created from Burp Suite Scan Import",
                        Type = TargetType.URL
                    };

                    targetManager.Add(target);
                    targetManager.Context.SaveChanges();
                }


                var vuln = new Vuln();
                vuln.FindingId = findingIds.Next();
                vuln.Template = false;
                vuln.Name = sanitizer.Sanitize(name);
                vuln.CreatedDate = DateTime.Now.ToUniversalTime();
                vuln.ModifiedDate = DateTime.Now.ToUniversalTime();
                vuln.UserId = user;
                vuln.ProjectId = project;
                vuln.VulnCategoryId = null;
                switch (severity)
                {
                    case "Information":
                        vuln.Risk = VulnRisk.Info;
                        break;
                    case "Low":
                        vuln.Risk = VulnRisk.Low;
                        break;
                    case "Medium":
                        vuln.Risk = VulnRisk.Medium;
                        break;
                    case "High":
                        vuln.Risk = VulnRisk.High;
                        break;
                    case "Critical":
                        vuln.Risk = VulnRisk.Critical;
                        break;
                }
                vuln.Status = VulnStatus.Open;
                vuln.cve = "None";
                // path and location are plain text, so encode them before they are placed in HTML.
                vuln.Description = sanitizer.Sanitize(issueBack + "<br><p>Path: " + WebUtility.HtmlEncode(pathIssue) +
                    "</p><p>Location: " + WebUtility.HtmlEncode(location) + "</p><br><p>References</p>" +
                    references + classifications);
                vuln.ProofOfConcept = sanitizer.Sanitize(PocHtml(issue));
                vuln.Impact = sanitizer.Sanitize(detailItems.Length > 0 ? issueDet + "<p>" + detailItems + "</p>" : issueDet);
                vuln.CVSS3 = 0;
                vuln.CVSSVector = "";
                vuln.Remediation = sanitizer.Sanitize(remediation + remediationDet);
                vuln.RemediationComplexity = RemediationComplexity.Low;
                vuln.RemediationPriority = RemediationPriority.Low;
                vuln.JiraCreated = false;
                vuln.OWASPRisk = "No Data";
                vuln.OWASPImpact = "No Data";
                vuln.OWASPLikehood = "No Data";
                vuln.OWASPVector = "No Data";
                vuln.MitreTechniques = "";
                vuln.MitreValues = "";

                vulnManager.Add(vuln);
                vulnManager.Context.SaveChanges();


                if (tar == null)
                {
                    var vulnTarget = new VulnTargets
                    {
                        VulnId = vuln.Id,
                        TargetId = target.Id
                    };
                    vulnTargetManager.Add(vulnTarget);
                    vulnTargetManager.Context.SaveChanges();
                }
                else
                {
                    var vulnTarget = new VulnTargets
                    {
                        VulnId = vuln.Id,
                        TargetId = tar.Id
                    };
                    vulnTargetManager.Add(vulnTarget);
                    vulnTargetManager.Context.SaveChanges();
                }

                

            }
        }
        else
        {
             foreach (XmlNode issue in issues)
            {
                string name = "";
                if(issue.SelectSingleNode("name") != null){name = issue.SelectSingleNode("name").InnerText;}
                
                string severity = "";
                if(issue.SelectSingleNode("severity") != null){severity = issue.SelectSingleNode("severity").InnerText;}
                
                string remediation = "";
                if(issue.SelectSingleNode("remediationBackground") != null){remediation = issue.SelectSingleNode("remediationBackground").InnerText;}
                
                string remediationDet = "";
                if(issue.SelectSingleNode("remediationDetail") != null){remediationDet = issue.SelectSingleNode("remediationDetail").InnerText;}
                
                string references = "";
                if(issue.SelectSingleNode("references") != null){references = issue.SelectSingleNode("references").InnerText;}
                
                string issueBack = "";
                if(issue.SelectSingleNode("issueBackground") != null){issueBack = issue.SelectSingleNode("issueBackground").InnerText;}
                
                string issueDet = "";
                if(issue.SelectSingleNode("issueDetail") != null){issueDet = issue.SelectSingleNode("issueDetail").InnerText;}
                
                string host = "";
                if(issue.SelectSingleNode("host") != null){host = issue.SelectSingleNode("host").InnerText;}
                
                string location = "";
                if(issue.SelectSingleNode("location") != null){location = issue.SelectSingleNode("location").InnerText;}
                
                string pathIssue = "";
                if(issue.SelectSingleNode("path") != null){pathIssue = issue.SelectSingleNode("path").InnerText;}
                
                string classifications = "";
                if(issue.SelectSingleNode("vulnerabilityClassifications") != null){classifications = issue.SelectSingleNode("vulnerabilityClassifications").InnerText;}
                
                string detailItems = string.Join("<br>", issue.SelectNodes("issueDetailItems/issueDetailItem")!.Cast<XmlNode>()
                    .Select(x => WebUtility.HtmlEncode(x.InnerText)));
                
                var vuln = new Vuln();
                vuln.Template = true; // no project: imported as a template
                vuln.Name = sanitizer.Sanitize(name);
                vuln.FindingId = "No Project";
                vuln.CreatedDate = DateTime.Now.ToUniversalTime();
                vuln.ModifiedDate = DateTime.Now.ToUniversalTime();
                vuln.UserId = user;
                vuln.ProjectId = null;
                vuln.VulnCategoryId = null;
                switch (severity)
                {
                    case "Information":
                        vuln.Risk = VulnRisk.Info;
                        break;
                    case "Low":
                        vuln.Risk = VulnRisk.Low;
                        break;
                    case "Medium":
                        vuln.Risk = VulnRisk.Medium;
                        break;
                    case "High":
                        vuln.Risk = VulnRisk.High;
                        break;
                    case "Critical":
                        vuln.Risk = VulnRisk.Critical;
                        break;
                }
                vuln.Status = VulnStatus.Open;
                vuln.cve = "None";
                // path and location are plain text, so encode them before they are placed in HTML.
                vuln.Description = sanitizer.Sanitize(issueBack + "<br><p>Path: " + WebUtility.HtmlEncode(pathIssue) +
                    "</p><p>Location: " + WebUtility.HtmlEncode(location) + "</p><br><p>References</p>" +
                    references + classifications);
                vuln.ProofOfConcept = sanitizer.Sanitize(PocHtml(issue));
                vuln.Impact = sanitizer.Sanitize(detailItems.Length > 0 ? issueDet + "<p>" + detailItems + "</p>" : issueDet);
                vuln.CVSS3 = 0;
                vuln.CVSSVector = "";
                vuln.Remediation = sanitizer.Sanitize(remediation + remediationDet);
                vuln.RemediationComplexity = RemediationComplexity.Low;
                vuln.RemediationPriority = RemediationPriority.Low;
                vuln.JiraCreated = false;
                vuln.OWASPRisk = "No Data";
                vuln.OWASPImpact = "No Data";
                vuln.OWASPLikehood = "No Data";
                vuln.OWASPVector = "No Data";
                vuln.MitreTechniques = "";
                vuln.MitreValues = "";
                vulnManager.Add(vuln);
                vulnManager.Context.SaveChanges();

            }
        }

       
    }
}