using System.Text.RegularExpressions;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Anthropic;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.HuggingFace;
using Microsoft.SemanticKernel.Connectors.MistralAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextGeneration;

namespace Cervantes.IFR.CervantesAI;

public class AiService : IAiService
{
    private readonly IAiConfiguration _aiConfiguration;
    private IProjectUserManager projectUserManager;
    private IVulnManager vulnManager;
    private ITargetManager targetManager;
    private IVulnTargetManager vulnTargetManager;

    public AiService(IAiConfiguration aiConfiguration, IProjectUserManager projectUserManager, IVulnManager vulnManager,
        ITargetManager targetManager,
        IVulnTargetManager vulnTargetManager)
    {
        _aiConfiguration = aiConfiguration;
        this.projectUserManager = projectUserManager;
        this.vulnManager = vulnManager;
        this.targetManager = targetManager;
        this.vulnTargetManager = vulnTargetManager;
    }

    public bool IsEnabled()
    {
        return _aiConfiguration.Enabled;
    }

    public async Task<VulnAiModel?> GenerateVuln(string name, Language language)
    {
        try
        {
            if (IsEnabled())
            {
                var builder = TypeBuilder(_aiConfiguration.Type);
                VulnAiModel vulnAiModel = new VulnAiModel();

                var prompt = @"";
                FunctionResult result;
                switch (language)
                {
                    case Language.English:
                        prompt = PromptHelper.VulnEnglish;
                        break;
                    case Language.Español:
                        prompt = PromptHelper.VulnSpanish;
                        break;
                    case Language.Português:
                        prompt = PromptHelper.VulnPortuguese;
                        break;
                }

                result = await PromptExecution(prompt, _aiConfiguration.Type, name, builder);

                vulnAiModel.Name = name;
                vulnAiModel.Language = language;
                var vuln = VulnParser.ParseSecurityReport(result.ToString(),language);
                vulnAiModel.Description = vuln.Description;
                vulnAiModel.Impact = vuln.Impact;
                vulnAiModel.ProofOfConcept = vuln.ProofOfConcept;
                vulnAiModel.Remediation = vuln.Remediation;
                vulnAiModel.Risk = vuln.Risk;
                
                return vulnAiModel;
            }

            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<string> GenerateCustom(string prompt)
    {
        try
        {
            if (IsEnabled())
            {
                var builder = TypeBuilder(_aiConfiguration.Type);
                var result = await PromptExecution(prompt, _aiConfiguration.Type, null, builder);
                return result.ToString();
            }

            return String.Empty;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<string> GenerateExecutive(Project project)
    {
        try
        {
            if (IsEnabled())
            {
                var builder = TypeBuilder(_aiConfiguration.Type);
                var prompt = @"";
                switch (project.Language)
                {
                    case Language.English:
                        string message = PromptHelper.ExecutiveEnglish;
                        var membersList = projectUserManager.GetAll().Where(x => x.ProjectId == project.Id)
                            .Select(x => x.User.FullName).ToList();
                        string members = "";
                        foreach (var mem in membersList)
                        {
                            members += mem + ", ";
                        }

                        var targetsList = targetManager.GetAll().Where(x => x.ProjectId == project.Id).ToList();
                        string targets = "";
                        foreach (var tar in targetsList)
                        {
                            targets += tar.Name + ", ";
                        }

                        var vulnsList = vulnManager.GetAll().Where(x => x.ProjectId == project.Id).ToList();
                        string vulns = "";
                        foreach (var vul in vulnsList)
                        {
                            var tar = vulnTargetManager.GetAll().Where(X => X.VulnId == vul.Id).ToList();
                            var targetsVuln = "";
                            foreach (var t in tar)
                            {
                                targetsVuln += t.Target.Name + ", ";
                            }

                            vulns += vul.Name + ", Risk: " + vul.Risk.ToString() + " , Assets Affected:" +
                                     targetsVuln + ". ";
                        }

                        prompt = message.Replace("{Client}", project.Client.Name).Replace("{Project}", project.Name)
                            .Replace("{StartDate}", project.StartDate.ToShortDateString())
                            .Replace("{EndDate}", project.EndDate.ToShortDateString())
                            .Replace("{ProjectDescription}", project.Description).Replace("{Members}", members)
                            .Replace("{Scope}", targets).Replace("{Vulns}", vulns);

                        break;
                    case Language.Español:
                        string message2 = PromptHelper.ExecutiveSpanish;
                        var membersList2 = projectUserManager.GetAll().Where(x => x.ProjectId == project.Id)
                            .Select(x => x.User.FullName).ToList();
                        string members2 = "";
                        foreach (var mem in membersList2)
                        {
                            members2 += mem + ", ";
                        }

                        var targetsList2 = targetManager.GetAll().Where(x => x.ProjectId == project.Id).ToList();
                        string targets2 = "";
                        foreach (var tar in targetsList2)
                        {
                            targets2 += tar.Name + ", ";
                        }

                        var vulnsList2 = vulnManager.GetAll().Where(x => x.ProjectId == project.Id).ToList();
                        string vulns2 = "";
                        foreach (var vul in vulnsList2)
                        {
                            var tar = vulnTargetManager.GetAll().Where(X => X.VulnId == vul.Id).ToList();
                            var targetsVuln = "";
                            foreach (var t in tar)
                            {
                                targetsVuln += t.Target.Name + ", ";
                            }

                            vulns2 += vul.Name + ", Risk: " + vul.Risk.ToString() + " , Assets Affected:" +
                                      targetsVuln + ". ";
                        }

                        prompt = message2.Replace("{Client}", project.Client.Name)
                            .Replace("{Project}", project.Name)
                            .Replace("{StartDate}", project.StartDate.ToShortDateString())
                            .Replace("{EndDate}", project.EndDate.ToShortDateString())
                            .Replace("{ProjectDescription}", project.Description).Replace("{Members}", members2)
                            .Replace("{Scope}", targets2).Replace("{Vulns}", vulns2);

                        break;
                    case Language.Português:
                        string message3 = PromptHelper.ExecutivePortuguese;

                        var membersList3 = projectUserManager.GetAll().Where(x => x.ProjectId == project.Id)
                            .Select(x => x.User.FullName).ToList();
                        string members3 = "";
                        foreach (var mem in membersList3)
                        {
                            members3 += mem + ", ";
                        }

                        var targetsList3 = targetManager.GetAll().Where(x => x.ProjectId == project.Id).ToList();
                        string targets3 = "";
                        foreach (var tar in targetsList3)
                        {
                            targets3 += tar.Name + ", ";
                        }

                        var vulnsList3 = vulnManager.GetAll().Where(x => x.ProjectId == project.Id).ToList();
                        string vulns3 = "";
                        foreach (var vul in vulnsList3)
                        {
                            var tar = vulnTargetManager.GetAll().Where(X => X.VulnId == vul.Id).ToList();
                            var targetsVuln = "";
                            foreach (var t in tar)
                            {
                                targetsVuln += t.Target.Name + ", ";
                            }

                            vulns3 += vul.Name + ", Risk: " + vul.Risk.ToString() + " , Assets Affected:" +
                                      targetsVuln + ". ";
                        }

                        prompt = message3.Replace("{Client}", project.Client.Name)
                            .Replace("{Project}", project.Name)
                            .Replace("{StartDate}", project.StartDate.ToShortDateString())
                            .Replace("{EndDate}", project.EndDate.ToShortDateString())
                            .Replace("{ProjectDescription}", project.Description).Replace("{Members}", members3)
                            .Replace("{Scope}", targets3).Replace("{Vulns}", vulns3);
                        break;
                }

                var result = await PromptExecution(prompt, _aiConfiguration.Type, null, builder);

                return result.ToString();
            }

            return String.Empty;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<FunctionResult> PromptExecution(string prompt, string type, string? vuln, IKernelBuilder builder)
    {
        var kernel = builder.Build();
        KernelFunction summarize;
        FunctionResult result;
        switch (type)
        {
            case "OpenAI":
                summarize = kernel.CreateFunctionFromPrompt(prompt,
                    executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = _aiConfiguration.MaxTokens });
                if (vuln != null)
                {
                    result = await kernel.InvokeAsync(summarize, new() { ["input"] = vuln });
                }
                else
                {
                    result = await kernel.InvokeAsync(summarize);
                }

                break;
            case "Azure":
                summarize = kernel.CreateFunctionFromPrompt(prompt,
                    executionSettings: new AzureOpenAIPromptExecutionSettings()
                        { MaxTokens = _aiConfiguration.MaxTokens });
                if (vuln != null)
                {
                    result = await kernel.InvokeAsync(summarize, new() { ["input"] = vuln });
                }
                else
                {
                    result = await kernel.InvokeAsync(summarize);
                }

                break;
            case "Google":
#pragma warning disable SKEXP0070
                summarize = kernel.CreateFunctionFromPrompt(prompt,
                    executionSettings: new GeminiPromptExecutionSettings() { MaxTokens = _aiConfiguration.MaxTokens });
                if (vuln != null)
                {
                    result = await kernel.InvokeAsync(summarize, new() { ["input"] = vuln });
                }
                else
                {
                    result = await kernel.InvokeAsync(summarize);
                }

                break;
            case "GoogleVertex":
#pragma warning disable SKEXP0070
                summarize = kernel.CreateFunctionFromPrompt(prompt,
                    executionSettings: new GeminiPromptExecutionSettings() { MaxTokens = _aiConfiguration.MaxTokens });
                if (vuln != null)
                {
                    result = await kernel.InvokeAsync(summarize, new() { ["input"] = vuln });
                }
                else
                {
                    result = await kernel.InvokeAsync(summarize);
                }

                break;
            case "Mistral":
                summarize = kernel.CreateFunctionFromPrompt(prompt,
                    executionSettings: new MistralAIPromptExecutionSettings()
                        { MaxTokens = _aiConfiguration.MaxTokens });
                if (vuln != null)
                {
                    result = await kernel.InvokeAsync(summarize, new() { ["input"] = vuln });
                }
                else
                {
                    result = await kernel.InvokeAsync(summarize);
                }

                break;
            case "HuggingFace":
                summarize = kernel.CreateFunctionFromPrompt(prompt,
                    executionSettings: new HuggingFacePromptExecutionSettings()
                        { MaxTokens = _aiConfiguration.MaxTokens });
                if (vuln != null)
                {
                    result = await kernel.InvokeAsync(summarize, new() { ["input"] = vuln });
                }
                else
                {
                    result = await kernel.InvokeAsync(summarize);
                }

                break;
            case "Custom":
                summarize = kernel.CreateFunctionFromPrompt(prompt,
                    executionSettings: new OpenAIPromptExecutionSettings() { MaxTokens = _aiConfiguration.MaxTokens });
                if (vuln != null)
                {
                    result = await kernel.InvokeAsync(summarize, new() { ["input"] = vuln });
                }
                else
                {
                    result = await kernel.InvokeAsync(summarize);
                }

                break;
            case "Anthropic":
                summarize = kernel.CreateFunctionFromPrompt(prompt);
                if (vuln != null)
                {
                    result = await kernel.InvokeAsync(summarize, new() { ["input"] = vuln });
                }
                else
                {
                    result = await kernel.InvokeAsync(summarize);
                }

                break;
            default:
                throw new Exception("Invalid AI Type");
        }

        return result;
    }

    public IKernelBuilder TypeBuilder(string type)
    {
        var builder = Kernel.CreateBuilder();
        switch (type)
        {
            case "OpenAI":
                builder.AddOpenAIChatCompletion(
                    _aiConfiguration.Model,
                    _aiConfiguration.ApiKey);
                break;
            case "Azure":
                builder.AddAzureOpenAIChatCompletion(
                    _aiConfiguration.Model,
                    _aiConfiguration.Endpoint,
                    _aiConfiguration.ApiKey);
                break;
            case "Google":
#pragma warning disable SKEXP0070
                builder.AddGoogleAIGeminiChatCompletion(
                    _aiConfiguration.Model,
                    _aiConfiguration.ApiKey);
                break;
            case "GoogleVertex":
#pragma warning disable SKEXP0070
                builder.AddVertexAIGeminiChatCompletion(
                    _aiConfiguration.Model,
                    _aiConfiguration.ApiKey, _aiConfiguration.Location, _aiConfiguration.ProjectId);
                break;
            case "Mistral":
                if (_aiConfiguration.Endpoint != "")
                {
                    Uri endpoint = new Uri(_aiConfiguration.Endpoint);
                    builder.AddMistralChatCompletion(
                        _aiConfiguration.Model,
                        _aiConfiguration.ApiKey, endpoint, _aiConfiguration.ProjectId);
                }
                else
                {
                    builder.AddMistralChatCompletion(
                        _aiConfiguration.Model,
                        _aiConfiguration.ApiKey);
                }

                break;
            case "HuggingFace":
                builder.AddHuggingFaceChatCompletion(_aiConfiguration.Model, new Uri(_aiConfiguration.Endpoint),
                    _aiConfiguration.ApiKey, _aiConfiguration.ProjectId);
                break;
            case "Custom":
                builder.AddOpenAIChatCompletion(
                    _aiConfiguration.Model,
                    _aiConfiguration.ApiKey,
                    httpClient: new HttpClient(new LocalHostServer(_aiConfiguration.Endpoint))
                );
                break;
            case "Anthropic":
                builder.AddAnthropicChatCompletion(
                    _aiConfiguration.Model,
                    _aiConfiguration.ApiKey);
                break;
            default:
                throw new Exception("Invalid AI Type");
        }

        return builder;
    }

    public class LocalHostServer(string url) : HttpClientHandler
    {
        private readonly Uri uri = new Uri(url);

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.RequestUri = uri;
            return base.SendAsync(request, cancellationToken);
        }
    }
}