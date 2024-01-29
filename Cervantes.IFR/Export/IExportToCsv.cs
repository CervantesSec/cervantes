using Cervantes.CORE.Entities;

namespace Cervantes.IFR.Export;

public interface IExportToCsv
{
    
    string ExportClients(List<ClientExport> records);
    string ExportTasks(List<TaskExport> records);
    string ExportVulns(List<VulnExport> records);
    void DeleteFile(string path);
    string ExportLogs(List<Log> records);

    string ExportProjects(List<ProjectExport> records);
}