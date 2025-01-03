using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Microsoft.SemanticKernel;

namespace Cervantes.IFR.CervantesAI;

public interface IAiService
{
    bool IsEnabled();
    Task<VulnAiModel?> GenerateVuln(string name, Language language);
    Task<string> GenerateCustom(string prompt);
    Task<string> GenerateExecutive(Project project);
    Task<string> SendMessage(ChatMessageCreateViewModel message);
    Task<bool> AddDocument(Guid chatId, string path);
    Task<bool> AddProject(Guid chatId, Guid projectId);
    Task<bool> DeleteContext(Guid chatId);
}