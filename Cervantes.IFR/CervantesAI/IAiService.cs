using Cervantes.CORE.Entities;
using Microsoft.SemanticKernel;

namespace Cervantes.IFR.CervantesAI;

public interface IAiService
{
    bool IsEnabled();
    Task<VulnAiModel?> GenerateVuln(string name, Language language);
    Task<FunctionResult?> GenerateCustom(string prompt);
    Task<FunctionResult?> GenerateExecutive(Project project);
}