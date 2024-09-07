using Cervantes.CORE.Entities;
using Microsoft.SemanticKernel;

namespace Cervantes.IFR.CervantesAI;

public interface IAiService
{
    bool IsEnabled();
    Task<VulnAiModel?> GenerateVuln(string name, Language language);
    Task<string> GenerateCustom(string prompt);
    Task<string> GenerateExecutive(Project project);
}