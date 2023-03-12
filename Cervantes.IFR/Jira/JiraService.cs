using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Cervantes.CORE;
using Atlassian;
using Atlassian.Jira;
using Cervantes.Contracts;
using Html2JiraMarkup;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Project = Atlassian.Jira.Project;

namespace Cervantes.IFR.Jira;

public class JiraService: IJIraService
{
    private readonly IJiraConfiguration _jiraConfiguration;
    private IVulnManager vulnManager = null;
    private IJiraManager jiraManager = null;
    private IJiraCommentManager jiraCommentManager = null;
    private readonly ILogger<JiraService> _logger = null;

    public JiraService(IJiraConfiguration jiraConfiguration, IVulnManager vulnManager, IJiraManager jiraManager, 
        IJiraCommentManager jiraCommentManager,ILogger<JiraService> logger)
    {
        _jiraConfiguration = jiraConfiguration;
        this.vulnManager = vulnManager;
        this.jiraManager = jiraManager;
        this.jiraCommentManager = jiraCommentManager;
        _logger = logger;
    }

    public bool CreateIssue(Guid vuln, string user)
    {
        try
        {
            if (_jiraConfiguration.Enabled == false)
            {
                return false;
            }

            var vulnerability = vulnManager.GetById(vuln);
            Converter converter = new Converter();


            Atlassian.Jira.Jira jira = null;
            switch (_jiraConfiguration.Auth)
            {
                case "Basic":
                    jira = Atlassian.Jira.Jira.CreateRestClient(_jiraConfiguration.Url, _jiraConfiguration.User,
                        _jiraConfiguration.Password);
                    break;
                case "OAuth":
                    jira = Atlassian.Jira.Jira.CreateOAuthRestClient(_jiraConfiguration.Url, _jiraConfiguration.ConsumerKey,_jiraConfiguration.ConsumerSecret,
                    _jiraConfiguration.OAuthAccessToken,_jiraConfiguration.OAuthTokenSecret);
                    break;
            }
            

            var issue = jira.CreateIssue(_jiraConfiguration.Project);
                issue.Type = "Error";
                switch (vulnerability.Risk)
                {
                    case VulnRisk.Critical:
                        issue.Priority = "Highest";
                        break;
                    case VulnRisk.High:
                        issue.Priority = "High";
                        break;
                    case VulnRisk.Medium:
                        issue.Priority = "Medium";
                        break;
                    case VulnRisk.Low:
                        issue.Priority = "Low";
                        break;
                    case VulnRisk.Info:
                        issue.Priority = "Lowest";
                        break;
                }
                issue.Summary = vulnerability.Name;
                
                //remove img tag on htmlDesc
                var htmlDesc = new HtmlDocument();
                htmlDesc.LoadHtml(vulnerability.Description);
                var nodes = htmlDesc.DocumentNode.SelectNodes("//img");
                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        node.Remove();
                    } 
                }
                HtmlNode refChildDesc = htmlDesc.DocumentNode.FirstChild;
                HtmlNode newChildDesc = HtmlNode.CreateNode("<h1> Description </h1>");
                htmlDesc.DocumentNode.InsertBefore(newChildDesc, refChildDesc);
                
                //remove img tag on htmlImpact
                var htmlImpact = new HtmlDocument();
                htmlImpact.LoadHtml(vulnerability.Impact);
                var nodes2 = htmlImpact.DocumentNode.SelectNodes("//img");
                if (nodes2 != null)
                {
                    foreach (var node in nodes2)
                    {
                        node.Remove();
                    } 
                }
                HtmlNode refChildImpact = htmlImpact.DocumentNode.FirstChild;
                HtmlNode newChildImpact = HtmlNode.CreateNode("<h1> Impact </h1>");
                htmlImpact.DocumentNode.InsertBefore(newChildImpact, refChildImpact);
                
                //remove img tag on htmlExploit
                var htmlExploit = new HtmlDocument();
                htmlExploit.LoadHtml(vulnerability.ProofOfConcept);
                var nodes3 = htmlExploit.DocumentNode.SelectNodes("//img");
                if (nodes3 != null)
                {
                    foreach (var node in nodes3)
                    {
                        node.Remove();
                    }
                }
                HtmlNode refChildExploit = htmlExploit.DocumentNode.FirstChild;
                HtmlNode newChildExploit = HtmlNode.CreateNode("<h1> Proof of Concept </h1>");
                htmlExploit.DocumentNode.InsertBefore(newChildExploit, refChildExploit);
                
                //remove img tag on htmlReme
                var htmlReme = new HtmlDocument();
                htmlReme.LoadHtml(vulnerability.Remediation);
                var nodes4 = htmlReme.DocumentNode.SelectNodes("//img");
                if (nodes4 != null)
                {
                    foreach (var node in nodes4)
                    {
                        node.Remove();
                    }
                }
                HtmlNode refChildReme = htmlReme.DocumentNode.FirstChild;
                HtmlNode newChildReme = HtmlNode.CreateNode("<h1> Remediation </h1>");
                htmlReme.DocumentNode.InsertBefore(newChildReme, refChildReme);
		

                //convert from html to jira and add it to issu deswcription
                issue.Description = converter.ConvertHtmlToJira(htmlDesc.DocumentNode.InnerHtml) + 
                                    converter.ConvertHtmlToJira(htmlImpact.DocumentNode.InnerHtml) +
                                    converter.ConvertHtmlToJira(htmlExploit.DocumentNode.InnerHtml)+
                                    converter.ConvertHtmlToJira(htmlReme.DocumentNode.InnerHtml);
                issue.SaveChangesAsync().Wait();

                //check for images in htmlDesc and add image as attachment in Jira
                if (nodes != null)
                {
                    int nodei = 0;
                    foreach (var node in nodes)
                    {
                        var base64Image = node.GetAttributeValue("src", "");
                        var offset = base64Image.IndexOf(',') + 1;
                        var imageInBytes = Convert.FromBase64String(base64Image[offset..^0]);

                        var extension = base64Image.Split(';')[0].Split('/')[1];
                        issue.AddAttachment("description"+nodei+"."+extension,imageInBytes);
                        nodei++;
                    }
                }
                //check for images in htmlImpact and add image as attachment in Jira
                if (nodes2 != null)
                {
                    int node2i = 0;
                    foreach (var node in nodes2)
                    {
                        var base64Image = node.GetAttributeValue("src", "");
                        var offset = base64Image.IndexOf(',') + 1;
                        var imageInBytes = Convert.FromBase64String(base64Image[offset..^0]);

                        var extension = base64Image.Split(';')[0].Split('/')[1];
                        issue.AddAttachment("impact"+node2i+"."+extension,imageInBytes);
                        node2i++;
                    }
                }
                //check for images in htmlExploit and add image as attachment in Jira
                if (nodes3 != null)
                {
                    int node3i = 0;
                    foreach (var node in nodes3)
                    {
                        var base64Image = node.GetAttributeValue("src", "");
                        var offset = base64Image.IndexOf(',') + 1;
                        var imageInBytes = Convert.FromBase64String(base64Image[offset..^0]);

                        var extension = base64Image.Split(';')[0].Split('/')[1];
                        issue.AddAttachment("poc"+node3i+"."+extension,imageInBytes);
                        node3i++;
                    }
                }
                //check for images in htmlExploit and add image as attachment in Jira
                if (nodes4 != null)
                {
                    int node4i = 0;
                    foreach (var node in nodes4)
                    {
                        var base64Image = node.GetAttributeValue("src", "");
                        var offset = base64Image.IndexOf(',') + 1;
                        var imageInBytes = Convert.FromBase64String(base64Image[offset..^0]);

                        var extension = base64Image.Split(';')[0].Split('/')[1];
                        issue.AddAttachment("remediation"+node4i+"."+extension,imageInBytes);
                        node4i++;
                    }
                }

                var assignee = "No Assigned";
                if (issue.Assignee != null) { assignee = issue.Assignee;}
                
                StringBuilder components = new StringBuilder();
                if (issue.Components.Count == 0)
                {
                    components.Append("No Components");
                }
                else
                {
                    foreach (var component in issue.Components)
                    {
                        components.Append(component.Name+",");
                    }
                }

                DateTime? dueDate = null;
                if (issue.DueDate != null)
                {
                    dueDate = issue.DueDate.Value.ToUniversalTime();

                }

                StringBuilder labels = new StringBuilder();
                if (issue.Labels.Count == 0)
                {
                    labels.Append("No Labels");
                }
                else
                {
                    foreach (var label in issue.Labels)
                    {
                        labels.Append(label.ToString()+",");
                    }
                }

                string resolution = "No Resolved";
                if (issue.Resolution != null) { resolution = issue.Resolution.Name;}

                DateTime? resolutionDate = null;
                if (issue.ResolutionDate != null)
                {
                    resolutionDate = issue.ResolutionDate.Value.ToUniversalTime();

                }

                string securitylevel = "No Security Level";
                if (issue.SecurityLevel != null) { securitylevel = issue.SecurityLevel.Name;}
                

                // create CORE.Jira object
                var jiraTicket = new CORE.Jira
                {
                    VulnId = vuln,
                    UserId = user,
                    CreatedDate = DateTime.Now.ToUniversalTime(),
                    JiraIdentifier = issue.JiraIdentifier,
                    JiraKey = issue.Key.Value,
                    Name = issue.Summary,
                    Reporter = issue.Reporter,
                    Assignee = assignee,
                    JiraType = issue.Type.Name,
                    Label = labels.ToString(),
                    Votes = issue.Votes,
                    JiraCreatedDate = issue.Created.Value.ToUniversalTime(),
                    JiraUpdatedDate = issue.Updated.Value.ToUniversalTime(),
                    JiraStatus = issue.Status.Name,
                    JiraComponent = components.ToString(),
                    Priority = issue.Priority.Name,
                    JiraProject = issue.Project,
                    Resolution = resolution,
                    ResolutionDate = resolutionDate,
                    DueDate = dueDate,
                    SecurityLevel = securitylevel
                };
                jiraManager.Add(jiraTicket);
                jiraManager.Context.SaveChanges();

                return true;
                
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred in Jira Service Creating Jira Issue Vuln: {0}",
                vuln);
            return false;
        }
        
    }

    public void UpdateIssue(string key)
    {
        Atlassian.Jira.Jira jira = null;
        switch (_jiraConfiguration.Auth)
        {
            case "Basic":
                jira = Atlassian.Jira.Jira.CreateRestClient(_jiraConfiguration.Url, _jiraConfiguration.User,
                    _jiraConfiguration.Password);
                break;
            case "OAuth":
                jira = Atlassian.Jira.Jira.CreateOAuthRestClient(_jiraConfiguration.Url, _jiraConfiguration.ConsumerKey,_jiraConfiguration.ConsumerSecret,
                    _jiraConfiguration.OAuthAccessToken,_jiraConfiguration.OAuthTokenSecret);
                break;
        }
        var issue =  jira.Issues.GetIssueAsync(key).Result;
        var ticket = jiraManager.GetByJiraKey(key);

        if (issue != null)
        {
            var vulnerability = vulnManager.GetById(ticket.VulnId);
            Converter converter = new Converter();
            switch (vulnerability.Risk)
                {
                    case VulnRisk.Critical:
                        issue.Priority = "Highest";
                        break;
                    case VulnRisk.High:
                        issue.Priority = "High";
                        break;
                    case VulnRisk.Medium:
                        issue.Priority = "Medium";
                        break;
                    case VulnRisk.Low:
                        issue.Priority = "Low";
                        break;
                    case VulnRisk.Info:
                        issue.Priority = "Lowest";
                        break;
                }
                issue.Summary = vulnerability.Name;
                
                //remove img tag on htmlDesc
                var htmlDesc = new HtmlDocument();
                htmlDesc.LoadHtml(vulnerability.Description);
                var nodes = htmlDesc.DocumentNode.SelectNodes("//img");
                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        node.Remove();
                    } 
                }
                HtmlNode refChildDesc = htmlDesc.DocumentNode.FirstChild;
                HtmlNode newChildDesc = HtmlNode.CreateNode("<h1> Description </h1>");
                htmlDesc.DocumentNode.InsertBefore(newChildDesc, refChildDesc);
                
                //remove img tag on htmlImpact
                var htmlImpact = new HtmlDocument();
                htmlImpact.LoadHtml(vulnerability.Impact);
                var nodes2 = htmlImpact.DocumentNode.SelectNodes("//img");
                if (nodes2 != null)
                {
                    foreach (var node in nodes2)
                    {
                        node.Remove();
                    } 
                }
                HtmlNode refChildImpact = htmlImpact.DocumentNode.FirstChild;
                HtmlNode newChildImpact = HtmlNode.CreateNode("<h1> Impact </h1>");
                htmlImpact.DocumentNode.InsertBefore(newChildImpact, refChildImpact);
                
                //remove img tag on htmlExploit
                var htmlExploit = new HtmlDocument();
                htmlExploit.LoadHtml(vulnerability.ProofOfConcept);
                var nodes3 = htmlExploit.DocumentNode.SelectNodes("//img");
                if (nodes3 != null)
                {
                    foreach (var node in nodes3)
                    {
                        node.Remove();
                    }
                }
                HtmlNode refChildExploit = htmlExploit.DocumentNode.FirstChild;
                HtmlNode newChildExploit = HtmlNode.CreateNode("<h1> Proof of Concept </h1>");
                htmlExploit.DocumentNode.InsertBefore(newChildExploit, refChildExploit);
                
                //remove img tag on htmlReme
                var htmlReme = new HtmlDocument();
                htmlReme.LoadHtml(vulnerability.Remediation);
                var nodes4 = htmlReme.DocumentNode.SelectNodes("//img");
                if (nodes4 != null)
                {
                    foreach (var node in nodes4)
                    {
                        node.Remove();
                    }
                }
                HtmlNode refChildReme = htmlReme.DocumentNode.FirstChild;
                HtmlNode newChildReme = HtmlNode.CreateNode("<h1> Remediation </h1>");
                htmlReme.DocumentNode.InsertBefore(newChildReme, refChildReme);
		

                //convert from html to jira and add it to issu deswcription
                issue.Description = converter.ConvertHtmlToJira(htmlDesc.DocumentNode.InnerHtml) + 
                                    converter.ConvertHtmlToJira(htmlImpact.DocumentNode.InnerHtml) +
                                    converter.ConvertHtmlToJira(htmlExploit.DocumentNode.InnerHtml)+
                                    converter.ConvertHtmlToJira(htmlReme.DocumentNode.InnerHtml);
                
                issue.SaveChangesAsync().Wait();


                var attList = issue.GetAttachmentsAsync();
                if (attList.Result.Count() != 0)
                {
                    foreach (var att in attList.Result)
                    {
                        issue.DeleteAttachmentAsync(att).Wait();
                    }
                }
                
                
                //check for images in htmlDesc and add image as attachment in Jira
                if (nodes != null)
                {
                    int nodei = 0;
                    foreach (var node in nodes)
                    {
                        var base64Image = node.GetAttributeValue("src", "");
                        var offset = base64Image.IndexOf(',') + 1;
                        var imageInBytes = Convert.FromBase64String(base64Image[offset..^0]);

                        var extension = base64Image.Split(';')[0].Split('/')[1];
                        issue.AddAttachment("description"+nodei+"."+extension,imageInBytes);
                        nodei++;
                    }
                }
                //check for images in htmlImpact and add image as attachment in Jira
                if (nodes2 != null)
                {
                    int node2i = 0;
                    foreach (var node in nodes2)
                    {
                        var base64Image = node.GetAttributeValue("src", "");
                        var offset = base64Image.IndexOf(',') + 1;
                        var imageInBytes = Convert.FromBase64String(base64Image[offset..^0]);

                        var extension = base64Image.Split(';')[0].Split('/')[1];
                        issue.AddAttachment("impact"+node2i+"."+extension,imageInBytes);
                        node2i++;
                    }
                }
                //check for images in htmlExploit and add image as attachment in Jira
                if (nodes3 != null)
                {
                    int node3i = 0;
                    foreach (var node in nodes3)
                    {
                        var base64Image = node.GetAttributeValue("src", "");
                        var offset = base64Image.IndexOf(',') + 1;
                        var imageInBytes = Convert.FromBase64String(base64Image[offset..^0]);

                        var extension = base64Image.Split(';')[0].Split('/')[1];
                        issue.AddAttachment("poc"+node3i+"."+extension,imageInBytes);
                        node3i++;
                    }
                }
                //check for images in htmlExploit and add image as attachment in Jira
                if (nodes4 != null)
                {
                    int node4i = 0;
                    foreach (var node in nodes4)
                    {
                        var base64Image = node.GetAttributeValue("src", "");
                        var offset = base64Image.IndexOf(',') + 1;
                        var imageInBytes = Convert.FromBase64String(base64Image[offset..^0]);

                        var extension = base64Image.Split(';')[0].Split('/')[1];
                        issue.AddAttachment("remediation"+node4i+"."+extension,imageInBytes);
                        node4i++;
                    }
                }

                var assignee = "No Assigned";
                if (issue.Assignee != null) { assignee = issue.Assignee;}
                
                StringBuilder components = new StringBuilder();
                if (issue.Components.Count == 0)
                {
                    components.Append("No Components");
                }
                else
                {
                    foreach (var component in issue.Components)
                    {
                        components.Append(component.Name+",");
                    }
                }

                DateTime? dueDate = null;
                if (issue.DueDate != null)
                {
                    dueDate = issue.DueDate.Value.ToUniversalTime();

                }

                StringBuilder labels = new StringBuilder();
                if (issue.Labels.Count == 0)
                {
                    labels.Append("No Labels");
                }
                else
                {
                    foreach (var label in issue.Labels)
                    {
                        labels.Append(label.ToString()+",");
                    }
                }

                string resolution = "No Resolved";
                if (issue.Resolution != null) { resolution = issue.Resolution.Name;}

                DateTime? resolutionDate = null;
                if (issue.ResolutionDate != null)
                {
                    resolutionDate = issue.ResolutionDate.Value.ToUniversalTime();

                }

                string securitylevel = "No Security Level";
                if (issue.SecurityLevel != null) { securitylevel = issue.SecurityLevel.Name;}
                

                
                jiraManager.Context.SaveChanges();
                
            
            
            
            
            if(ticket == null){return;}

            ticket.Reporter = issue.Reporter;
            ticket.Assignee = issue.Assignee;
            ticket.Name = issue.Summary;
            ticket.JiraType = issue.Type.ToString();
            ticket.Label = issue.Labels.Count == 0 ? "No Labels" : string.Join(",", issue.Labels);
            ticket.Votes = issue.Votes;
            ticket.JiraCreatedDate = issue.Created.Value.ToUniversalTime();
            ticket.JiraUpdatedDate = issue.Updated.Value.ToUniversalTime();
            ticket.JiraStatus = issue.Status.ToString();
            ticket.JiraComponent = issue.Components.Count == 0 ? "No Component" : string.Join(",", issue.Components);
            ticket.Priority = issue.Priority.ToString();
            ticket.JiraProject = issue.Project;
            ticket.Resolution = issue.Resolution == null ? "No resolved" : issue.Resolution.ToString();
            ticket.ResolutionDate = issue.ResolutionDate == null ? null : issue.ResolutionDate.Value.ToUniversalTime();
            ticket.DueDate = issue.DueDate == null ? null : issue.DueDate.Value.ToUniversalTime();
            ticket.SecurityLevel = issue.SecurityLevel == null ? "No Security Level" : issue.SecurityLevel.ToString();
            jiraManager.Context.SaveChanges();
            
        

        

            var comments = issue.GetCommentsAsync().Result;
            if(comments == null ){return;}

            foreach (var comment in comments)
            {
                var c =jiraCommentManager.GetByJiraId(comment.Id);
                if (c != null)
                {
                    continue;
                }

                var jc = new JiraComments
                {
                    JiraId = ticket.Id,
                    JiraIdComment = comment.Id,
                    Author = comment.Author,
                    Body = comment.Body,
                    GroupLevel = comment.GroupLevel,
                    RoleLevel = comment.RoleLevel,
                    CreatedDate = comment.CreatedDate.Value.ToUniversalTime(),
                    UpdatedDate = comment.UpdatedDate.Value.ToUniversalTime(),
                    UpdateAuthor = comment.UpdateAuthor
                };
                jiraCommentManager.Add(jc);
                jiraCommentManager.Context.SaveChanges();

            }
        }
        

    }

    public bool JiraEnabled()
    {
        return _jiraConfiguration.Enabled;
    }
    
    public bool DeleteIssue(string key)
    {
        try
        {
            if (_jiraConfiguration.Enabled == false)
            {
                return false;
            }

            Atlassian.Jira.Jira jira = null;
            switch (_jiraConfiguration.Auth)
            {
                case "Basic":
                    jira = Atlassian.Jira.Jira.CreateRestClient(_jiraConfiguration.Url, _jiraConfiguration.User,
                        _jiraConfiguration.Password);
                    break;
                case "OAuth":
                    jira = Atlassian.Jira.Jira.CreateOAuthRestClient(_jiraConfiguration.Url, _jiraConfiguration.ConsumerKey,_jiraConfiguration.ConsumerSecret,
                        _jiraConfiguration.OAuthAccessToken,_jiraConfiguration.OAuthTokenSecret);
                    break;
            }
            
                jira.Issues.DeleteIssueAsync(key).Wait();
                return true;

        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred in Jira Service deleting Jira Issue : {0}",
                key);
            return false;
        }
        
    }
    
    public bool AddComment(string key, string comment)
    {
        try
        {
            if (_jiraConfiguration.Enabled == false)
            {
                return false;
            }

            Atlassian.Jira.Jira jira = null;
            switch (_jiraConfiguration.Auth)
            {
                case "Basic":
                    jira = Atlassian.Jira.Jira.CreateRestClient(_jiraConfiguration.Url, _jiraConfiguration.User,
                        _jiraConfiguration.Password);
                    break;
                case "OAuth":
                    jira = Atlassian.Jira.Jira.CreateOAuthRestClient(_jiraConfiguration.Url, _jiraConfiguration.ConsumerKey,_jiraConfiguration.ConsumerSecret,
                        _jiraConfiguration.OAuthAccessToken,_jiraConfiguration.OAuthTokenSecret);
                    break;
            }
            
            var issue = jira.Issues.GetIssueAsync(key).Result;
            Converter converter = new Converter();
            var com = converter.ConvertHtmlToJira(comment);
            issue.AddCommentAsync(com).Wait();
            UpdateIssue(key);
            return true;
           
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred in Jira Service deleting Jira Issue : {0}",
                key);
            return false;
        }
        
    }
}