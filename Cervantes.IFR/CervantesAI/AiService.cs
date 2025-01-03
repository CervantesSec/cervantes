using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using DocumentFormat.OpenXml.EMMA;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Anthropic;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.HuggingFace;
using Microsoft.SemanticKernel.Connectors.MistralAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextGeneration;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.EntityFrameworkCore;


namespace Cervantes.IFR.CervantesAI;

public class AiService : IAiService
{
    private readonly IAiConfiguration _aiConfiguration;
    private IProjectUserManager projectUserManager;
    private IVulnManager vulnManager;
    private ITargetManager targetManager;
    private IVulnTargetManager vulnTargetManager;
    private ITaskManager taskManager;
    private IProjectManager projectManager;
    private IChatManager chatManager;
    private IChatMessageManager chatMessageManager;
    private IVaultManager vaultManager;
    private ITaskTargetManager taskTargetManager;
    private readonly IConfiguration configuration;

    public AiService(IAiConfiguration aiConfiguration, IProjectUserManager projectUserManager, IVulnManager vulnManager,
        ITargetManager targetManager,
        IVulnTargetManager vulnTargetManager, IChatManager chatManager, IChatMessageManager chatMessageManager,
        IConfiguration configuration, ITaskManager taskManager, IProjectManager projectManager, IVaultManager vaultManager, ITaskTargetManager taskTargetManager)
    {
        _aiConfiguration = aiConfiguration;
        this.projectUserManager = projectUserManager;
        this.vulnManager = vulnManager;
        this.targetManager = targetManager;
        this.vulnTargetManager = vulnTargetManager;
        this.chatManager = chatManager;
        this.chatMessageManager = chatMessageManager;
        this.configuration = configuration;
        this.taskManager = taskManager;
        this.projectManager = projectManager;
        this.vaultManager = vaultManager;
        this.taskTargetManager = taskTargetManager;
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
    
    public async Task<string> SendMessage(ChatMessageCreateViewModel message)
    {
        try
        {
            if (IsEnabled())
            {
                List<ChatMessage> _chatHistory = new List<ChatMessage>();
                var builder = TypeBuilder(_aiConfiguration.Type);
                var memory = MemoryBuilder(_aiConfiguration.TextEmbedding.TextEmbeddingType);
                var kernel = builder.Build();
                var _memory = memory.Build();
                _chatHistory = chatMessageManager.GetAll().Where(x => x.ChatId == message.ChatId).ToList();
                
                /*// Add user message to history
                ChatMessage msg = new ChatMessage();
                msg.Id = Guid.NewGuid();
                msg.ChatId = message.ChatId;
                msg.Content = message.Content;
                if (message.IsUser)
                {
                    msg.Role = "User";
                }
                else
                {
                    msg.Role = "Assistant";
                }   
                _chatHistory.Add(msg);*/

                // Search relevant memories
                // Create the filter for the specific chatId
                var filter = MemoryFilters.ByTag("chatId", message.ChatId.ToString());
                var searchResults = await _memory.SearchAsync(message.Content, filter: filter);
                /*var relevantContext = string.Join("\n", searchResults.Results
                    .Select(r => $"{r.Partitions.First().Text}"));*/
                // Build context from search results
                var contextBuilder = new StringBuilder();
                foreach (var result2 in searchResults.Results)
                {
                    foreach (var item in result2.Partitions)
                    {
                        contextBuilder.AppendLine(item.Text);
                    }
                }

                string context = contextBuilder.ToString();
                // Create chat prompt
                /*var prompt = $@"Previous relevant context:
                {context}

                Chat Context:
                {FormatChatHistory(_chatHistory)}

                User: {message.Content}

                About you: Your name is Cide you are an expert in cybersecurity and pentesting professional and provide a relevant and helpful response
                Instructions: You will gather information about the context and chat history and provide a relevant and helpful response";*/
                
                var prompt = $@"Previous relevant context:
                {context}

                User: {message.Content}

                About you: Your name is Cide you are an expert in cybersecurity and pentesting professional and provide a relevant and helpful response
                Instructions: You will gather information about the context and chat history and provide a relevant and helpful response";
                // Get AI response using Semantic Kernel
                var chatFunction = kernel.CreateFunctionFromPrompt(prompt);
                var result = await kernel.InvokeAsync(chatFunction);
                var response = result.GetValue<string>();
                

                // Store the interaction in memory
                await _memory.ImportTextAsync(
                    text: $"User: {message.Content}\nAssistant: {response}",
                    documentId: Guid.NewGuid().ToString(),
                    tags: new TagCollection {
                        { "chatId", message.ChatId.ToString() },
                        { "messageId", message.Id.ToString() }
                    });
                
                return response.ToString();
            }

            return String.Empty;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private string FormatChatHistory(List<ChatMessage> chatHistory)
    {
        return string.Join("\n", chatHistory.Select(m => 
            $"{(m.Role == "user" ? "User" : "Assistant")}: {m.Content}"));
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

    public KernelMemoryBuilder MemoryBuilder(string type)
    {
        var memory = new KernelMemoryBuilder();
        string connectionString = configuration["ConnectionStrings:DefaultConnection"];
        switch (type)
        {
            case "Azure":
                var azAIConfig = new AzureOpenAIConfig();
                if (_aiConfiguration.TextEmbedding.TextEmbeddingEndpoint != "")
                {
                    azAIConfig.Endpoint = _aiConfiguration.TextEmbedding.TextEmbeddingEndpoint;
                }
                azAIConfig.APIKey = _aiConfiguration.TextEmbedding.TextEmbeddingApiKey;
                memory.WithPostgresMemoryDb(new PostgresConfig
                    {
                        ConnectionString = connectionString,
                        TableNamePrefix = "sk_"
                    })
                    .WithAzureOpenAITextEmbeddingGeneration()
                    .Build();
                break;
            case "OpenAI":
                var openAIConfig = new OpenAIConfig();
                if (!string.IsNullOrEmpty(_aiConfiguration.TextEmbedding.TextEmbeddingEndpoint))
                {
                    openAIConfig.Endpoint = _aiConfiguration.TextEmbedding.TextEmbeddingEndpoint;
                }
                openAIConfig.APIKey = _aiConfiguration.TextEmbedding.TextEmbeddingApiKey;
                openAIConfig.EmbeddingModel = _aiConfiguration.TextEmbedding.TextEmbeddingModel;
                
                var openAIConfig2 = new OpenAIConfig();
                if (!string.IsNullOrEmpty(_aiConfiguration.Endpoint))
                {
                    openAIConfig2.Endpoint = _aiConfiguration.Endpoint;
                }
                openAIConfig2.APIKey = _aiConfiguration.ApiKey;
                openAIConfig2.TextModel = _aiConfiguration.Model;
                
                memory.WithPostgresMemoryDb(new PostgresConfig
                    {
                        ConnectionString = connectionString,
                        TableNamePrefix = "sk_"
                    })
                    .WithOpenAITextGeneration(openAIConfig2)
                    .WithOpenAITextEmbeddingGeneration(openAIConfig)
                    .Build();
                break;
            case "Onnx":
                break;
            case "Google":
                break;
            case "Mistral":
                break;
            case "Custom":
                var customAIConfig = new OpenAIConfig();
                customAIConfig.Endpoint = _aiConfiguration.TextEmbedding.TextEmbeddingEndpoint;
                customAIConfig.APIKey = _aiConfiguration.TextEmbedding.TextEmbeddingApiKey;
                customAIConfig.EmbeddingModel = _aiConfiguration.TextEmbedding.TextEmbeddingModel;
                
                var customAIConfig2 = new OpenAIConfig();
                customAIConfig2.Endpoint = _aiConfiguration.Endpoint;
                customAIConfig2.APIKey = _aiConfiguration.ApiKey;
                customAIConfig2.TextModel = _aiConfiguration.Model;
                
                memory.WithPostgresMemoryDb(new PostgresConfig
                    {
                        ConnectionString = connectionString,
                        TableNamePrefix = "sk_"
                    })
                    .WithOpenAITextGeneration(customAIConfig2,httpClient: new HttpClient(new LocalHostServer(_aiConfiguration.Endpoint)))
                    .WithOpenAITextEmbeddingGeneration(customAIConfig,httpClient: new HttpClient(new LocalHostServer(_aiConfiguration.TextEmbedding.TextEmbeddingEndpoint)))
                    .Build();
                break;
                
        }

        return memory;
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
    
    public static string ExtractTextPdf(string pdfPath)
    {
        try
        {
            StringBuilder text = new StringBuilder();
            using (PdfReader pdfReader = new PdfReader(pdfPath))
            using (PdfDocument pdfDoc = new PdfDocument(pdfReader))
            {
                int numberOfPages = pdfDoc.GetNumberOfPages();
                
                for (int i = 1; i <= numberOfPages; i++)
                {
                    var page = pdfDoc.GetPage(i);
                    var strategy = new LocationTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(page, strategy);
                    text.AppendLine(currentText);
                }
            }
            return text.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error extracting text from PDF: {ex.Message}", ex);
        }
    }
    public async Task<bool> AddDocument(Guid chatId, string path)
    {
        try
        {
            if (IsEnabled())
            {
                var builder = TypeBuilder(_aiConfiguration.Type);
                var memory = MemoryBuilder(_aiConfiguration.TextEmbedding.TextEmbeddingType);
                var kernel = builder.Build();
                var _memory = memory.Build();
                // Store the interaction in memory
                await _memory.ImportDocumentAsync(
                    path ,
                    documentId: Guid.NewGuid().ToString(),
                    tags: new TagCollection {
                        { "chatId", chatId.ToString() },
                        { "messageId", Guid.NewGuid().ToString() }
                    });
                
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<bool> AddProject(Guid chatId, Guid projectId)
    {
        try
        {
            if (IsEnabled())
            {
                var builder = TypeBuilder(_aiConfiguration.Type);
                var memory = MemoryBuilder(_aiConfiguration.TextEmbedding.TextEmbeddingType);
                var kernel = builder.Build();
                var _memory = memory.Build();
                var project = projectManager.GetAll().Where(x => x.Id == projectId).Include(x => x.Client).Include(x => x.User).Select(
                    p => new
                    {
                        Project = new
                        {
                            Name = p.Name,
                            Description = p.Description,
                            StartDate = p.StartDate,
                            EndDate = p.EndDate,
                            Score = p.Score,
                            Status = p.Status.ToString(),
                            Language = p.Language.ToString(),
                            ProjectType = p.ProjectType.ToString(),
                            ExecutiveSummary = p.ExecutiveSummary,
                            CreatedBy = new
                            {
                                FullName = p.User.FullName,
                                Position = p.User.Position,
                                Email = p.User.Email
                            },
                            Client = new
                            {
                                Name = p.Client.Name,
                                Description = p.Client.Description,
                                Email = p.Client.ContactEmail,
                                Phone = p.Client.ContactPhone,
                                Url = p.Client.Url,
                                Address = p.Client.ContactName,
                                CreatedDate = p.Client.CreatedDate
                            }
                        },
                            
                    }
                    ).FirstOrDefault();
                var projectUsers = projectUserManager.GetAll().Where(x => x.ProjectId == projectId)
                    .Include(x => x.User).Select(pu => new
                    {
                        ProjectMembers = new
                        {
                            FullName = pu.User.FullName,
                            Position = pu.User.Position,
                            Email = pu.User.Email
                        }
                    }).ToList();
                var taskIds = taskManager.GetAll().Where(x => x.ProjectId == projectId).Select(y => y.Id).ToList();
                var tasks = taskTargetManager.GetAll().Where(x => taskIds.Contains(x.TaskId)).Include(x=> x.Task).Include(x => x.Target)
                    .Select(pu => new
                    {
                        Task = new {
                        
                            Name = pu.Task.Name,
                            Description = pu.Task.Description,
                            StartDate = pu.Task.StartDate,
                            EndDate = pu.Task.EndDate,
                            Status = pu.Task.Status,
                            AsignedTo = new
                            {
                                FullName = pu.Task.AsignedUser.FullName,
                                Position = pu.Task.AsignedUser.Position,
                                Email = pu.Task.AsignedUser.Email
                            },
                            CreatedBy = new
                            {
                                FullName = pu.Task.CreatedUser.FullName,
                                Position = pu.Task.CreatedUser.Position,
                                Email = pu.Task.CreatedUser.Email
                            },
                            Targets = new
                            {
                                Name = pu.Target.Name,
                            }
                        }
                    }).ToList();
                var vulns = vulnManager.GetAll().Where(x => x.ProjectId == projectId).Include(x => x.VulnCategory)
                    .Include(x=> x.VulnCwes)
                    .Include(x => x.VulnTargets).Include(x => x.User).Select(pu => new
                {
                    Vulnerability = new
                    {
                        Name = pu.Name,
                        FindingId = pu.FindingId,
                        Language = pu.Language,
                        Description = pu.Description,
                        Impact = pu.Impact,
                        Risk = pu.Risk,
                        ProofOfConcept = pu.ProofOfConcept,
                        Remediation = pu.Remediation,
                        RemediationComplexity = pu.RemediationComplexity.ToString(),
                        RemediationPriority = pu.RemediationPriority.ToString(),
                        CreatedDate = pu.CreatedDate,
                        ModifiedDate = pu.ModifiedDate,
                        Category = pu.VulnCategory.Name,
                        Status = pu.Status.ToString(),
                        CVE = pu.cve,
                        CVSS3Score = pu.CVSS3,
                        CVSS3Vector = pu.CVSSVector,
                        Mitre = pu.MitreTechniques,
                        Cwes = pu.VulnCwes.Select(x => x.CweId+"-"+x.Cwe.Name).ToList(),
                        Targets = pu.VulnTargets.Select(x => x.Target.Name).ToList(),
                        CreatedBy = new
                        {
                            FullName = pu.User.FullName,
                            Position = pu.User.Position,
                            Email = pu.User.Email
                        }
                    }
                }).ToList();
                var vaults = vaultManager.GetAll().Where(x => x.ProjectId == projectId).Include(x => x.User).Select(pu => new
                    {
                        Name = pu.Name,
                        Description = pu.Description,
                        Type = pu.Type.ToString(),
                        CreatedDate = pu.CreatedDate,
                        CreatedBy = new
                        {
                            FullName = pu.User.FullName,
                            Position = pu.User.Position,
                            Email = pu.User.Email
                        }
                    }).ToList();
                var targets = targetManager.GetAll().Where(x => x.ProjectId == projectId).Include(x => x.User)
                    .Select(pu => new
                    {
                        Target = new
                        {
                            Name = pu.Name,
                            Description = pu.Description,
                            Type = pu.Type.ToString(),
                            CreatedBy = new
                            {
                                FullName = pu.User.FullName,
                                Position = pu.User.Position,
                                Email = pu.User.Email
                            }
                        }
                    }).ToList();
                var converter = new ReverseMarkdown.Converter();
                StringBuilder membersInfo = new StringBuilder();
                foreach (var item in projectUsers)
                {
                    membersInfo.AppendLine("## Member");
                    membersInfo.Append($"- Name:{item.ProjectMembers.FullName}\n- Position:{item.ProjectMembers.Position}\n- Email:{item.ProjectMembers.Email}\n");
                }
                StringBuilder tasksInfo = new StringBuilder();
                foreach (var item in tasks)
                {
                    tasksInfo.Append($"## Task\n");
                    tasksInfo.Append($"### Name\n{item.Task.Name}\n");
                    tasksInfo.Append($"### Description\n{converter.Convert(item.Task.Description)}\n");
                    tasksInfo.Append($"### Start Date\n{item.Task.StartDate.ToShortDateString()}\n");
                    tasksInfo.Append($"### End Date\n{item.Task.EndDate.ToShortDateString()}\n");
                    tasksInfo.Append($"### Status\n{item.Task.Status}\n");
                    tasksInfo.Append($"### Asigned To\n- {item.Task.AsignedTo.FullName}\n- {item.Task.AsignedTo.Position}\n- {item.Task.AsignedTo.Email}\n");
                    tasksInfo.Append($"### Created By\n- {item.Task.CreatedBy.FullName}\n- {item.Task.CreatedBy.Position}\n- {item.Task.CreatedBy.Email}\n");
                }
                StringBuilder vulnsInfo = new StringBuilder();
                foreach (var item in vulns)
                {
                    tasksInfo.Append($"## Vuln: {item.Vulnerability.FindingId}\n");
                    vulnsInfo.Append($"### Name\n{item.Vulnerability.Name}\n");
                    //vulnsInfo.Append($"### Finding Id\n{item.Vulnerability.FindingId}\n");
                    vulnsInfo.Append($"### Description\n{converter.Convert(item.Vulnerability.Description)}\n");
                    vulnsInfo.Append($"### Impact\n{converter.Convert(item.Vulnerability.Impact)}\n");
                    vulnsInfo.Append($"### Risk\n{item.Vulnerability.Risk}\n");
                    vulnsInfo.Append($"### Proof Of Concept\n{converter.Convert(item.Vulnerability.ProofOfConcept)}\n");
                    vulnsInfo.Append($"### Remediation\n{converter.Convert(item.Vulnerability.Remediation)}\n");
                    vulnsInfo.Append($"### Remediation Complexity\n{item.Vulnerability.RemediationComplexity.ToString()}\n");
                    vulnsInfo.Append($"### Remediation Priority\n{item.Vulnerability.RemediationPriority.ToString()}\n");
                    vulnsInfo.Append($"### Created Date\n{item.Vulnerability.CreatedDate.ToShortDateString()}\n");
                    vulnsInfo.Append($"### Modified Date\n{item.Vulnerability.ModifiedDate.ToShortDateString()}\n");
                    vulnsInfo.Append($"### Category\n{item.Vulnerability.Category}\n");
                    vulnsInfo.Append($"### Status\n{item.Vulnerability.Status.ToString()}\n");
                    vulnsInfo.Append($"### CVE\n{item.Vulnerability.CVE}\n");
                    vulnsInfo.Append($"### CVSS3 Score\n{item.Vulnerability.CVSS3Score}\n");
                    vulnsInfo.Append($"### CVSS3 Vector\n{item.Vulnerability.CVSS3Vector}\n");
                    vulnsInfo.Append($"### Mitre Techniques\n{item.Vulnerability.Mitre}\n");
                    vulnsInfo.Append($"### Cwes\n{string.Join(", ", item.Vulnerability.Cwes)}\n");
                    vulnsInfo.Append($"### Targets\n{string.Join(", ", item.Vulnerability.Targets)}\n");
                    vulnsInfo.Append($"### Created By\n- {item.Vulnerability.CreatedBy.FullName}\n- {item.Vulnerability.CreatedBy.Position}\n- {item.Vulnerability.CreatedBy.Email}\n");
                    
                }
                StringBuilder vaultsInfo = new StringBuilder();
                foreach (var item in vaults)
                {
                    tasksInfo.Append($"## Vault\n");
                    vaultsInfo.Append($"### Name\n{item.Name}\n");
                    vaultsInfo.Append($"### Description\n{converter.Convert(item.Description)}\n");
                    vaultsInfo.Append($"### Type\n{item.Type}\n");
                    vaultsInfo.Append($"### Created Date\n{item.CreatedDate.ToShortDateString()}\n");
                    vaultsInfo.Append($"### Created By\n- {item.CreatedBy.FullName}\n- {item.CreatedBy.Position}\n- {item.CreatedBy.Email}\n");
                }
                StringBuilder targetsInfo = new StringBuilder();
                foreach (var item in targets)
                {
                    tasksInfo.Append($"## Target\n");
                    targetsInfo.Append($"### Name\n{item.Target.Name}\n");
                    targetsInfo.Append($"### Description\n{converter.Convert(item.Target.Description)}\n");
                    targetsInfo.Append($"### Type\n{item.Target.Type}\n");
                    targetsInfo.Append($"### Created By\n- {item.Target.CreatedBy.FullName}\n- {item.Target.CreatedBy.Position}\n- {item.Target.CreatedBy.Email}\n");
                }
                
                var info = $@"
                The context is provided below in markdown format:

                #Project
                ## Name 
                {project.Project.Name}
                ## Description: 
                {converter.Convert(project.Project.Description)};
                ## Start Date:
                {project.Project.StartDate.ToShortDateString()}
                ## End Date:    
                {project.Project.EndDate.ToShortDateString()}
                ## Score:
                {project.Project.Score}
                ## Status:
                {project.Project.Status.ToString()}
                ## Language:
                {project.Project.Language.ToString()}
                ## Project Type:
                {project.Project.ProjectType.ToString()}
                ##  Project Created By:
                - {project.Project.CreatedBy.FullName}
                - {project.Project.CreatedBy.Position}
                - {project.Project.CreatedBy.Email}
                # Project Executive Summary:
                {converter.Convert(project.Project.ExecutiveSummary)}
                # Client
                ## Name
                {project.Project.Client.Name}
                ## Description
                {converter.Convert(project.Project.Client.Description)}
                ## Email
                {project.Project.Client.Email}
                ## Phone
                {project.Project.Client.Phone}
                ## Url
                {project.Project.Client.Url}
                ## Address
                {project.Project.Client.Address}
                ## Created Date
                {project.Project.Client.CreatedDate.ToShortDateString()}
            
                # Project Members
                {membersInfo}
                # Tasks
                {tasksInfo}
                # Vulnerabilities
                {vulnsInfo}
                # Vaults
                {vaultsInfo}
                # Targets
                {targetsInfo}
                ";
                // Store the interaction in memory
                await _memory.ImportTextAsync(
                    text: info,
                    documentId: Guid.NewGuid().ToString(),
                    tags: new TagCollection {
                        { "chatId", chatId.ToString() },
                        { "messageId", Guid.NewGuid().ToString() }
                    });
                
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<bool> DeleteContext(Guid chatId)
    {
        try
        {
            if (IsEnabled())
            {
                var builder = TypeBuilder(_aiConfiguration.Type);
                var memory = MemoryBuilder(_aiConfiguration.TextEmbedding.TextEmbeddingType);
                var kernel = builder.Build();
                var _memory = memory.Build();
                // Store the interaction in memory
                var filter = MemoryFilters.ByTag("chatId", chatId.ToString());
                var records = await _memory.SearchAsync(
                    query: "", // Empty query to get all records
                    filter: filter);

                foreach (var record in records.Results)
                {
                    await _memory.DeleteDocumentAsync(record.DocumentId);
                }

                
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
}

