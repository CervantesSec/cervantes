using System.Collections.Generic;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModels;

public class DashboardViewModel
{
    public List<Project> Projects { get; set; }
    public List<Client> Clients { get; set; }
    public List<Vuln> Vulns { get; set; }
    public List<Document> Documents { get; set; }
    public double TotalProjects { get; set; }
    public double TotalClients { get; set; }
    public double TotalVulns { get; set; }
    public double TotalDocuments { get; set; }
    public double TotalTasks { get; set; }
    public double TotalUsers { get; set; }
    public double TotalReports { get; set; }
    public double TotalNotes { get; set; }
    public double ProjectArchived { get; set; }
    public double ProjectActive { get; set; }
    public double ProjectWaiting { get; set; }
    public double VulnsOpen { get; set; }
    public double VulnsResolved { get; set; }
    public double VulnsOut{ get; set; }
    public double VulnsAccepted { get; set; }
    public double VulnsConfirmed { get; set; }
    public double VulnsInvalid { get; set; }
    public double VulnsInfo { get; set; }
    public double VulnsLow { get; set; }
    public double VulnsMedium { get; set; }
    public double VulnsHigh { get; set; }
    public double VulnsCritical { get; set; }
    public double TasksBacklog { get; set; }
    public double TasksToDo { get; set; }
    public double TasksInProgress { get; set; }
    public double TasksBlocked { get; set; }
    public double TasksDone { get; set; }
}