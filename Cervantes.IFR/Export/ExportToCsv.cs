using System.Globalization;
using Cervantes.CORE.Entities;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;

namespace Cervantes.IFR.Export;

public class ExportToCsv : IExportToCsv
{
    private readonly IWebHostEnvironment env;

    public ExportToCsv(IWebHostEnvironment env)
    {
        this.env = env;
    }
    public string ExportClients(List<ClientExport> records)
    {
        var file = "Clients-"+Guid.NewGuid()+".csv";
        var path = $"{env.WebRootPath}/Attachments/Export/{file}";
        using (var writer = new StreamWriter(path))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(records);
        }

        return "Attachments/Export/"+file;
    }
    
    public string ExportProjects(List<ProjectExport> records)
    {
        var file = "Projects-"+Guid.NewGuid()+".csv";
        var path = $"{env.WebRootPath}/Attachments/Export/{file}";
        using (var writer = new StreamWriter(path))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(records);
        }

        return "Attachments/Export/"+file;
    }
    
    public string ExportLogs(List<Log> records)
    {
        var file = "Logs-"+Guid.NewGuid()+".csv";
        var path = $"{env.WebRootPath}/Attachments/Export/{file}";
        using (var writer = new StreamWriter(path))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(records);
        }

        return "Attachments/Export/"+file;
    }
    
    public string ExportTasks(List<TaskExport> records)
    {
        try
        {
            var file = "Tasks-"+Guid.NewGuid()+".csv";
            var path = $"{env.WebRootPath}/Attachments/Export/{file}";
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }

            return "Attachments/Export/"+file;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public string ExportVulns(List<VulnExport> records)
    {
        try
        {
            var file = "Vulns-"+Guid.NewGuid()+".csv";
            var path = $"{env.WebRootPath}/Attachments/Export/{file}";
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }

            return "Attachments/Export/"+file;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    /// <summary>
    /// How long an export stays on disk after the page asked for it to be removed.
    /// </summary>
    internal static TimeSpan DeleteDelay = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Removes an export once the browser has had time to download it. Callers hand the path to the browser
    /// (downloadFile, which only starts a navigation) and call this right away, so deleting at once would make
    /// the download request find nothing.
    /// </summary>
    public void DeleteFile(string path)
    {
        _ = DeleteFileLater(path);
    }

    // The delayed delete itself, returned so that a test can observe how it ends.
    internal System.Threading.Tasks.Task DeleteFileLater(string path)
    {
        var exportPath = $"{env.WebRootPath}/{path}";
        return System.Threading.Tasks.Task.Run(async () =>
        {
            await System.Threading.Tasks.Task.Delay(DeleteDelay);
            try
            {
                System.IO.File.Delete(exportPath);
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                // Still open or already gone: a leftover export is harmless, it only takes disk space.
            }
        });
    }
    
    

}