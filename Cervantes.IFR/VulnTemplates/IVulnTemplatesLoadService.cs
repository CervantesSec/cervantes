namespace Cervantes.IFR.VulnTemplates;

public interface IVulnTemplatesLoadService
{
    public Task CreateEnglish(string type);
    public Task CreateSpanish(string type);
    public Task CreatePortuguese(string type);
}