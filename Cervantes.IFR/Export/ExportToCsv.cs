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
    
    public void DeleteFile(string path)
    {
        var imagePath = $"{env.WebRootPath}/{path}";
        System.IO.File.Delete(imagePath);
    }
    
    

}