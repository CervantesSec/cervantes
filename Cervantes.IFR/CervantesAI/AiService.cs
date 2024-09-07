using System.Text.RegularExpressions;
using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextGeneration;

namespace Cervantes.IFR.CervantesAI;

public class AiService: IAiService
{
    private readonly IAiConfiguration _aiConfiguration;
    private IProjectUserManager projectUserManager;
    private IVulnManager vulnManager;
    private ITargetManager targetManager;
    private IVulnTargetManager vulnTargetManager;

    public AiService(IAiConfiguration aiConfiguration, IProjectUserManager projectUserManager, IVulnManager vulnManager, ITargetManager targetManager,
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
                var builder = Kernel.CreateBuilder();
                VulnAiModel vulnAiModel = new VulnAiModel();

                switch (_aiConfiguration.Type)
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
                  case "Custom":
                        builder.AddOpenAIChatCompletion(
                            _aiConfiguration.Model,  
                            _aiConfiguration.ApiKey, 
                            httpClient: new HttpClient( new LocalHostServer(_aiConfiguration.Endpoint))
                            );
                        break;
                  case "Anthropic":
                        break;
                    default:
                        throw new Exception("Invalid AI Type");
                }

                if (_aiConfiguration.Type != "Anthropic")
                {
                    var kernel = builder.Build();
                    var prompt = @"";
                    switch (language)
                    {
                        case Language.English:
                            prompt =
                                @"You are a penetration tester writing report findings for a client. You are writing a finding for the following vulnerability: {{$input}}
                        . The finding should be written in the following format: Description, Impact, Risk Level (Critical, High, Medium, Low, Info), Proof of Concept, Remediation";
                            break;
                        case Language.Español:
                            prompt = @"Eres un pentester redactando los hallazgos de un informe para un cliente. Estás redactando un hallazgo para la siguiente vulnerabilidad: {{$input}}
                        . El hallazgo debe ser redactado en el siguiente formato: Descripción, Impacto, Nivel de Riesgo (Crítico, Alto, Medio, Bajo, Informativo), Prueba de Concepto y Remediación."; ;
                            break;
                    }

                    var summarize = kernel.CreateFunctionFromPrompt(prompt, executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = _aiConfiguration.MaxTokens });
                     var result = await kernel.InvokeAsync(summarize, new() { ["input"] = name });
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
                         description = Regex.Match(result.ToString(), descriptionPattern, RegexOptions.Singleline).Groups[1].Value;
                         impact = Regex.Match(result.ToString(), impactPattern, RegexOptions.Singleline).Groups[1].Value;
                         riskLevel = Regex.Match(result.ToString(), riskLevelPattern).Groups[1].Value;
                         proofOfConcept = Regex.Match(result.ToString(), proofOfConceptPattern, RegexOptions.Singleline).Groups[1].Value;
                         remediation = Regex.Match(result.ToString(), remediationPattern, RegexOptions.Singleline).Groups[1].Value;
                        
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
                
                         description = Regex.Match(result.ToString(), descriptionPattern, RegexOptions.Singleline).Groups[1].Value;
                         impact = Regex.Match(result.ToString(), impactPattern, RegexOptions.Singleline).Groups[1].Value;
                         riskLevel = Regex.Match(result.ToString(), riskLevelPattern).Groups[1].Value;
                         proofOfConcept = Regex.Match(result.ToString(), proofOfConceptPattern, RegexOptions.Singleline).Groups[1].Value;
                         remediation = Regex.Match(result.ToString(), remediationPattern, RegexOptions.Singleline).Groups[1].Value;
                        
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
                }
                }
                else
                {
                    var client = new AnthropicClient(_aiConfiguration.ApiKey);
                    var prompt = @"";
                    switch (language)
                    {
                        case Language.English:
                            prompt =
                                $@"You are a penetration tester writing report findings for a client. You are writing a finding for the following vulnerability: {name}
                        . The finding should be written in the following format: Description, Impact, Risk Level (Critical, High, Medium, Low, Info), Proof of Concept, Remediation";
                            break;
                        case Language.Español:
                            prompt = $@"Eres un pentester redactando los hallazgos de un informe para un cliente. Estás redactando un hallazgo para la siguiente vulnerabilidad: {name}
                        . El hallazgo debe ser redactado en el siguiente formato: Descripción, Impacto, Nivel de Riesgo (Crítico, Alto, Medio, Bajo, Informativo), Prueba de Concepto y Remediación."; ;
                            break;
                    }
                    
                    var messages = new List<Message>()
                    {
                        new Message(RoleType.User, prompt),
                    };
                    
                    var parameters = new MessageParameters()
                    {
                        Messages = messages,
                        MaxTokens = _aiConfiguration.MaxTokens,
                        Model = AnthropicModels.Claude35Sonnet,
                        Stream = false,
                        Temperature = 1.0m,
                    };
                    var result = await client.Messages.GetClaudeMessageAsync(parameters);
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
                         description = Regex.Match(result.Message.ToString(), descriptionPattern, RegexOptions.Singleline).Groups[1].Value;
                         impact = Regex.Match(result.Message.ToString(), impactPattern, RegexOptions.Singleline).Groups[1].Value;
                         riskLevel = Regex.Match(result.Message.ToString(), riskLevelPattern).Groups[1].Value;
                         proofOfConcept = Regex.Match(result.Message.ToString(), proofOfConceptPattern, RegexOptions.Singleline).Groups[1].Value;
                         remediation = Regex.Match(result.Message.ToString(), remediationPattern, RegexOptions.Singleline).Groups[1].Value;
                        
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
                
                         description = Regex.Match(result.Message.ToString(), descriptionPattern, RegexOptions.Singleline).Groups[1].Value;
                         impact = Regex.Match(result.Message.ToString(), impactPattern, RegexOptions.Singleline).Groups[1].Value;
                         riskLevel = Regex.Match(result.Message.ToString(), riskLevelPattern).Groups[1].Value;
                         proofOfConcept = Regex.Match(result.Message.ToString(), proofOfConceptPattern, RegexOptions.Singleline).Groups[1].Value;
                         remediation = Regex.Match(result.Message.ToString(), remediationPattern, RegexOptions.Singleline).Groups[1].Value;
                        
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
                }
                    
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
                var builder = Kernel.CreateBuilder();

                switch (_aiConfiguration.Type)
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
                    case "Custom":
                        builder.AddOpenAIChatCompletion(
                            _aiConfiguration.Model,  
                            _aiConfiguration.ApiKey, 
                            httpClient: new HttpClient( new LocalHostServer(_aiConfiguration.Endpoint))
                        );
                        break;
                    case "Anthropic":
                        break;
                    default:
                        throw new Exception("Invalid AI Type");
                }

                if (_aiConfiguration.Type != "Anthropic")
                {
                    var kernel = builder.Build();
              

                    var summarize = kernel.CreateFunctionFromPrompt(prompt, executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = _aiConfiguration.MaxTokens });
                    var result = await kernel.InvokeAsync(summarize);
                
                    return result.ToString();
                }
                else
                {
                    var client = new AnthropicClient(_aiConfiguration.ApiKey);
                    
                    var messages = new List<Message>()
                    {
                        new Message(RoleType.User, prompt),
                    };
                    
                    var parameters = new MessageParameters()
                    {
                        Messages = messages,
                        MaxTokens = _aiConfiguration.MaxTokens,
                        Model = AnthropicModels.Claude35Sonnet,
                        Stream = false,
                        Temperature = 1.0m,
                    };
                    var result = await client.Messages.GetClaudeMessageAsync(parameters);
                    return result.Message.ToString();
                }
                
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
                var builder = Kernel.CreateBuilder();

                switch (_aiConfiguration.Type)
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
                    case "Custom":
                        builder.AddOpenAIChatCompletion(
                            _aiConfiguration.Model,  
                            _aiConfiguration.ApiKey, 
                            httpClient: new HttpClient( new LocalHostServer(_aiConfiguration.Endpoint))
                        );
                        break;
                    case "Anthropic":
                        break;
                    default:
                        throw new Exception("Invalid AI Type");
                }

                if (_aiConfiguration.Type != "Anthropic")
                {

                    var kernel = builder.Build();
                    var prompt = @"";
                    switch (project.Language)
                    {
                        case Language.English:
                            string message =
                                @"You are a penetration tester writing the executive summary of a report for a client. 
This should provide a high-level overview of the key findings and recommendations in a concise and easily understandable manner and the response should be in HTML.  This executive summary should include:

                            Introduction. Briefly explain the purpose of the penetration test. Mention the systems or areas tested.

                            Scope and Objectives (Outline the scope of the penetration test, including the systems or networks tested. Summarize the objectives set for the testing)

                            Key Findings (Highlight the most critical vulnerabilities and weaknesses discovered. Provide a brief description of the severity and potential impact)

                            Overall Risk Profile( Summarize the overall risk ranking or score for the organization.Include a high-level explanation of the risk factors considered.)

                            Successes and Challenges ( Briefly mention the successes in terms of breaching security controls (if applicable).Highlight any challenges encountered during the testing)

                            Recommendations(Summarize the main recommendations for addressing identified vulnerabilities. Provide an overview of the suggested remediation measures)

                            Strategic Roadmap ( Outline the strategic roadmap for addressing security issues. Emphasize key milestones and priorities)

                            Conclusion (Provide a brief concluding statement summarizing the overall security posture. Mention any notable achievements or improvements)

                            Next Steps (Outline the immediate actions that need to be taken post-assessment.
                            Mention any follow-up activities or ongoing monitoring)

                            The Project information is:

                            Client Name: {Client}
                            Project name: Project}
                            Project Start Date: {StartDate}
                            Project End Date: {EndDate}
                            Project Description: {ProjectDescription}
                            Members: {Members}
                            Scope: {Scope}
                            Vulnerabilities: {Vulns}.
                            ";

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
                            string message2 =
                                @"Eres un pentester que esta redactando el resumen ejecutivo de un informe para un cliente. Este debe proporcionar una descripción general de alto nivel de los hallazgos clave y recomendaciones de manera concisa y fácilmente comprensible y el formato debe ser en HTML. Este resumen ejecutivo debe incluir:
                        Introducción: Explica brevemente el propósito de la prueba de penetración. Menciona los sistemas o áreas evaluadas.

                        Alcance y objetivos: Esquematiza el alcance de la prueba de penetración, incluyendo los sistemas o redes evaluadas. Resume los objetivos establecidos para las pruebas.

                        Hallazgos clave: Destaca las vulnerabilidades y debilidades más críticas descubiertas. Proporciona una breve descripción de la gravedad y el impacto potencial.

                        Perfil de riesgo general: Resume la clasificación o puntuación de riesgo general para la organización. Incluye una explicación de alto nivel de los factores de riesgo considerados.

                        Éxitos y desafíos: Menciona brevemente los éxitos en términos de violación de controles de seguridad (si es aplicable). Destaca cualquier desafío encontrado durante las pruebas.

                        Recomendaciones: Resume las principales recomendaciones para abordar las vulnerabilidades identificadas. Proporciona una descripción general de las medidas de remediación sugeridas.

                        Hoja de ruta estratégica: Esquematiza la hoja de ruta estratégica para abordar problemas de seguridad. Destaca hitos clave y prioridades.

                        Conclusión: Proporciona una breve declaración de conclusión resumiendo la postura general de seguridad. Menciona cualquier logro o mejora notable.

                        Próximos pasos: Esquematiza las acciones inmediatas que deben tomarse después de la evaluación. Menciona cualquier actividad de seguimiento o monitoreo continuo.

                        La información del proyecto es:

                        Nombre del cliente: {Client}
                        Nombre del proyecto: {Project}
                        Fecha de inicio del proyecto: {StartDate}
                        Fecha de finalización del proyecto: {EndDate}
                        Descripción del proyecto: {ProjectDescription}
                        Miembros: {Members}
                        Alcance: {Scope}
                        Vulnerabilidades: {Vulns}
                        El resultado debe ser en formato HTML.";
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
                    }

                    var summarize = kernel.CreateFunctionFromPrompt(prompt,
                        executionSettings: new OpenAIPromptExecutionSettings
                            { MaxTokens = _aiConfiguration.MaxTokens });
                    var result = await kernel.InvokeAsync(summarize);

                    return result.ToString();
                }
                else
                {
                    var client = new AnthropicClient(_aiConfiguration.ApiKey);
                     var prompt = @"";
                    switch (project.Language)
                    {
                        case Language.English:
                            string message =
                                @"You are a penetration tester writing the executive summary of a report for a client. 
This should provide a high-level overview of the key findings and recommendations in a concise and easily understandable manner and the response should be in HTML.  This executive summary should include:

                            Introduction. Briefly explain the purpose of the penetration test. Mention the systems or areas tested.

                            Scope and Objectives (Outline the scope of the penetration test, including the systems or networks tested. Summarize the objectives set for the testing)

                            Key Findings (Highlight the most critical vulnerabilities and weaknesses discovered. Provide a brief description of the severity and potential impact)

                            Overall Risk Profile( Summarize the overall risk ranking or score for the organization.Include a high-level explanation of the risk factors considered.)

                            Successes and Challenges ( Briefly mention the successes in terms of breaching security controls (if applicable).Highlight any challenges encountered during the testing)

                            Recommendations(Summarize the main recommendations for addressing identified vulnerabilities. Provide an overview of the suggested remediation measures)

                            Strategic Roadmap ( Outline the strategic roadmap for addressing security issues. Emphasize key milestones and priorities)

                            Conclusion (Provide a brief concluding statement summarizing the overall security posture. Mention any notable achievements or improvements)

                            Next Steps (Outline the immediate actions that need to be taken post-assessment.
                            Mention any follow-up activities or ongoing monitoring)

                            The Project information is:

                            Client Name: {Client}
                            Project name: Project}
                            Project Start Date: {StartDate}
                            Project End Date: {EndDate}
                            Project Description: {ProjectDescription}
                            Members: {Members}
                            Scope: {Scope}
                            Vulnerabilities: {Vulns}.
                            ";

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
                            string message2 =
                                @"Eres un pentester que esta redactando el resumen ejecutivo de un informe para un cliente. Este debe proporcionar una descripción general de alto nivel de los hallazgos clave y recomendaciones de manera concisa y fácilmente comprensible y el formato debe ser en HTML. Este resumen ejecutivo debe incluir:
                        Introducción: Explica brevemente el propósito de la prueba de penetración. Menciona los sistemas o áreas evaluadas.

                        Alcance y objetivos: Esquematiza el alcance de la prueba de penetración, incluyendo los sistemas o redes evaluadas. Resume los objetivos establecidos para las pruebas.

                        Hallazgos clave: Destaca las vulnerabilidades y debilidades más críticas descubiertas. Proporciona una breve descripción de la gravedad y el impacto potencial.

                        Perfil de riesgo general: Resume la clasificación o puntuación de riesgo general para la organización. Incluye una explicación de alto nivel de los factores de riesgo considerados.

                        Éxitos y desafíos: Menciona brevemente los éxitos en términos de violación de controles de seguridad (si es aplicable). Destaca cualquier desafío encontrado durante las pruebas.

                        Recomendaciones: Resume las principales recomendaciones para abordar las vulnerabilidades identificadas. Proporciona una descripción general de las medidas de remediación sugeridas.

                        Hoja de ruta estratégica: Esquematiza la hoja de ruta estratégica para abordar problemas de seguridad. Destaca hitos clave y prioridades.

                        Conclusión: Proporciona una breve declaración de conclusión resumiendo la postura general de seguridad. Menciona cualquier logro o mejora notable.

                        Próximos pasos: Esquematiza las acciones inmediatas que deben tomarse después de la evaluación. Menciona cualquier actividad de seguimiento o monitoreo continuo.

                        La información del proyecto es:

                        Nombre del cliente: {Client}
                        Nombre del proyecto: {Project}
                        Fecha de inicio del proyecto: {StartDate}
                        Fecha de finalización del proyecto: {EndDate}
                        Descripción del proyecto: {ProjectDescription}
                        Miembros: {Members}
                        Alcance: {Scope}
                        Vulnerabilidades: {Vulns}
                        El resultado debe ser en formato HTML.";
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
                    }
                    
                    var messages = new List<Message>()
                    {
                        new Message(RoleType.User, prompt),
                    };
                    
                    var parameters = new MessageParameters()
                    {
                        Messages = messages,
                        MaxTokens = _aiConfiguration.MaxTokens,
                        Model = AnthropicModels.Claude35Sonnet,
                        Stream = false,
                        Temperature = 1.0m,
                    };
                    var result = await client.Messages.GetClaudeMessageAsync(parameters);
                    return result.Message.ToString();
                    
                   
                }

            }

            return String.Empty;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public class LocalHostServer(string url) : HttpClientHandler
    {
        private readonly Uri uri = new Uri(url);
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.RequestUri = uri;
            return base.SendAsync(request, cancellationToken);
        }
    }
    
    public class LocalHostServerDocker(string url) : HttpClientHandler
    {
        private readonly Uri uri = new Uri(url);
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.RequestUri = uri;
            return base.SendAsync(request, cancellationToken);
        }
    }
    
}