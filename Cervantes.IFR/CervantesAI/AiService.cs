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

                string descriptionPattern;
                string impactPattern;
                string riskLevelPattern;
                string proofOfConceptPattern;
                string remediationPattern;


                string description;
                string impact;
                string riskLevel;
                string proofOfConcept;
                string remediation;

                vulnAiModel.Name = name;
                vulnAiModel.Language = language;
                switch (language)
                {
                    case Language.English:
                        descriptionPattern = @"Description:\s*(.*?)\s*Impact:";
                        impactPattern = @"Impact:\s*(.*?)\s*Risk Level:";
                        riskLevelPattern = @"Risk Level:\s*(Critical|High|Medium|Low|Info)";
                        proofOfConceptPattern = @"Proof of Concept:\s*(.*?)\s*Remediation:";
                        remediationPattern = @"Remediation:\s*(.*)";
                        description = Regex.Match(result.ToString(), descriptionPattern, RegexOptions.Singleline)
                            .Groups[1].Value;
                        impact = Regex.Match(result.ToString(), impactPattern, RegexOptions.Singleline).Groups[1].Value;
                        riskLevel = Regex.Match(result.ToString(), riskLevelPattern).Groups[1].Value;
                        proofOfConcept = Regex.Match(result.ToString(), proofOfConceptPattern, RegexOptions.Singleline)
                            .Groups[1].Value;
                        remediation = Regex.Match(result.ToString(), remediationPattern, RegexOptions.Singleline)
                            .Groups[1].Value;

                        vulnAiModel.Description = description;
                        vulnAiModel.Impact = impact;
                        vulnAiModel.ProofOfConcept = proofOfConcept;
                        vulnAiModel.Remediation = remediation;
                        switch (riskLevel)
                        {
                            case "Critical":
                                vulnAiModel.Risk = VulnRisk.Critical;
                                break;
                            case "High":
                                vulnAiModel.Risk = VulnRisk.High;
                                break;
                            case "Medium":
                                vulnAiModel.Risk = VulnRisk.Medium;
                                break;
                            case "Low":
                                vulnAiModel.Risk = VulnRisk.Low;
                                break;
                            case "Info":
                                vulnAiModel.Risk = VulnRisk.Info;
                                break;
                        }

                        break;
                    case Language.Español:
                        descriptionPattern = @"Descripción:\s*(.*?)\s*Impacto:";
                        impactPattern = @"Impacto:\s*(.*?)\s*Nivel de Riesgo:";
                        riskLevelPattern = @"Nivel de Riesgo:\s*(Crítico|Alto|Medio|Bajo|Informativo)";
                        proofOfConceptPattern = @"Prueba de Concepto:\s*(.*?)\s*Remediación:";
                        remediationPattern = @"Remediación:\s*(.*)";

                        description = Regex.Match(result.ToString(), descriptionPattern, RegexOptions.Singleline)
                            .Groups[1].Value;
                        impact = Regex.Match(result.ToString(), impactPattern, RegexOptions.Singleline).Groups[1].Value;
                        riskLevel = Regex.Match(result.ToString(), riskLevelPattern).Groups[1].Value;
                        proofOfConcept = Regex.Match(result.ToString(), proofOfConceptPattern, RegexOptions.Singleline)
                            .Groups[1].Value;
                        remediation = Regex.Match(result.ToString(), remediationPattern, RegexOptions.Singleline)
                            .Groups[1].Value;

                        vulnAiModel.Description = description;
                        vulnAiModel.Impact = impact;
                        vulnAiModel.ProofOfConcept = proofOfConcept;
                        vulnAiModel.Remediation = remediation;
                        switch (riskLevel)
                        {
                            case "Crítico":
                                vulnAiModel.Risk = VulnRisk.Critical;
                                break;
                            case "Alto":
                                vulnAiModel.Risk = VulnRisk.High;
                                break;
                            case "Medio":
                                vulnAiModel.Risk = VulnRisk.Medium;
                                break;
                            case "Bajo":
                                vulnAiModel.Risk = VulnRisk.Low;
                                break;
                            case "Informativo":
                                vulnAiModel.Risk = VulnRisk.Info;
                                break;
                        }

                        break;
                    case Language.Português:
                        descriptionPattern = @"Descrição:\s*(.*?)\s*Impacto:";
                        impactPattern = @"Impacto:\s*(.*?)\s*Nível de Risco:";
                        riskLevelPattern = @"Nível de Risco:\s*(Crítico|Alto|Médio|Baixo|Informativo)";
                        proofOfConceptPattern = @"Prova de Conceito:\s*(.*?)\s*Remediação:";
                        remediationPattern = @"Remediação:\s*(.*)";

                        description = Regex.Match(result.ToString(), descriptionPattern, RegexOptions.Singleline)
                            .Groups[1].Value;
                        impact = Regex.Match(result.ToString(), impactPattern, RegexOptions.Singleline).Groups[1].Value;
                        riskLevel = Regex.Match(result.ToString(), riskLevelPattern).Groups[1].Value;
                        proofOfConcept = Regex.Match(result.ToString(), proofOfConceptPattern, RegexOptions.Singleline)
                            .Groups[1].Value;
                        remediation = Regex.Match(result.ToString(), remediationPattern, RegexOptions.Singleline)
                            .Groups[1].Value;

                        vulnAiModel.Description = description;
                        vulnAiModel.Impact = impact;
                        vulnAiModel.ProofOfConcept = proofOfConcept;
                        vulnAiModel.Remediation = remediation;
                        switch (riskLevel)
                        {
                            case "Crítico":
                                vulnAiModel.Risk = VulnRisk.Critical;
                                break;
                            case "Alto":
                                vulnAiModel.Risk = VulnRisk.High;
                                break;
                            case "Médio":
                                vulnAiModel.Risk = VulnRisk.Medium;
                                break;
                            case "Baixo":
                                vulnAiModel.Risk = VulnRisk.Low;
                                break;
                            case "Informativo":
                                vulnAiModel.Risk = VulnRisk.Info;
                                break;
                        }

                        break;
                }


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