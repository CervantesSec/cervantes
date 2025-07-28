using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Contracts;
using Cervantes.CORE;
using global::AuthPermissions.BaseCode.PermissionsCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using MudBlazor;
using System.Text.Json;
using Blazored.LocalStorage;
using Cervantes.Web.Localization;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Components.Pages.Cve;

[Authorize]
public partial class CveDashboard
{
    private List<BreadcrumbItem> breadcrumbs = new();
    private string selectedTimeRange = "30d";
    private string selectedSeverity = "all";
    private string selectedVendor = "all";
    private bool onlyKevCves = false;
    private bool loading = true;
    private string chartKey = Guid.NewGuid().ToString();
    
    // Dashboard data
    private int totalCves = 0;
    private int criticalCves = 0;
    private int kevCves = 0;
    private int newCvesToday = 0;
    private List<CORE.Entities.Cve> recentCves = new();
    private List<CveSubscription> activeSubscriptions = new();
    private List<CORE.Entities.Cve> allCves = new();
   const string cveSVG = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""24"" height=""24"" viewBox=""0 0 24 24""><path d=""M24 21.172l-5.66-5.659c1.047-1.581 1.66-3.475 1.66-5.513 0-5.523-4.477-10-10-10s-10 4.477-10 10 4.477 10 10 10c2.038 0 3.932-.613 5.512-1.66l5.66 5.66 2.828-2.828zm-22-11.172c0-4.411 3.589-8 8-8s8 3.589 8 8-3.589 8-8 8-8-3.589-8-8zm13 1.006c0 .239-.196.432-.439.432h-.995c-.231 0-.479.138-.532.473-.043.269.112.484.318.576l1.009.449c.221.098.318.354.22.572-.102.217-.361.314-.581.216l-1.068-.475c-.139-.063-.292-.029-.4.151-.446.735-1.31 1.464-2.532 1.6-1.246-.139-2.12-.894-2.557-1.643-.089-.152-.247-.164-.374-.107l-1.068.475c-.221.098-.479.001-.581-.216-.099-.218-.001-.474.22-.572l1.009-.449c.218-.097.359-.306.313-.609-.046-.285-.29-.44-.527-.44h-.996c-.243-.001-.439-.194-.439-.433s.196-.432.439-.432h1.001c.229 0 .47-.147.514-.462.034-.242-.114-.451-.322-.531l-.929-.362c-.226-.088-.337-.338-.248-.56.09-.222.345-.332.57-.244l.956.371c.123.047.298.032.392-.183.076-.169.16-.328.25-.477.713.416 1.497.624 2.377.624.867 0 1.661-.212 2.374-.625.095.154.183.318.259.494.081.183.249.221.386.168l.956-.371c.226-.088.48.021.57.244.089.222-.022.472-.248.56l-.929.361c-.2.077-.358.273-.32.556.038.274.272.438.512.438h1.001c.243-.001.439.192.439.431zm-6.587-4.847c.134.03.25.112.321.228.258.429-.078.796-.391 1.179.504.235 1.046.351 1.657.351.602 0 1.148-.119 1.654-.353-.308-.377-.644-.753-.389-1.177.071-.116.188-.198.321-.228.265-.058.465-.291.465-.573.001-.323-.265-.586-.595-.586-.393 0-.677.369-.575.74.038.141.018.29-.057.415-.169.279-.457.447-.824.449-.372-.002-.656-.173-.824-.449-.074-.125-.095-.274-.057-.415.102-.372-.182-.74-.575-.74-.33 0-.596.263-.596.586 0 .282.2.515.465.573z""/></svg>";
    const string cveNotSVG = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""24"" height=""24"" viewBox=""0 0 24 24""><path d=""M5.895 3.173c0-.648.534-1.173 1.192-1.173.785 0 1.355.736 1.15 1.48-.077.281-.035.58.116.829.334.553.903.895 1.647.898.733-.003 1.31-.34 1.647-.898.151-.25.193-.548.115-.829-.205-.743.364-1.48 1.151-1.48.658 0 1.192.525 1.192 1.173 0 .563-.402 1.029-.932 1.146-.268.059-.5.225-.64.457-.511.847.161 1.598.775 2.353-1.009.468-2.103.704-3.308.704-1.223 0-2.309-.231-3.312-.7.624-.767 1.296-1.502.779-2.358-.141-.232-.372-.397-.64-.457-.53-.116-.932-.582-.932-1.145zm4.105 12.827c0-3.327 2.042-6.184 4.939-7.389l-.189-.363c-1.429.827-3.017 1.252-4.75 1.252-1.761 0-3.329-.415-4.752-1.246-.181.299-.351.615-.5.954-.189.429-.539.46-.785.365l-1.913-.742c-.451-.176-.961.043-1.139.487-.178.444.044.946.495 1.121l1.86.722c.413.16.712.578.644 1.062-.088.631-.569.926-1.029.926h-2.003c-.485-.001-.878.386-.878.864 0 .477.393.864.878.864h1.989c.478 0 .966.31 1.055.88.093.607-.19 1.024-.626 1.218l-2.017.898c-.442.197-.638.709-.438 1.144s.721.628 1.162.431l2.134-.95c.256-.114.573-.088.75.216.871 1.497 2.62 3.009 5.113 3.286.783-.087 1.484-.306 2.117-.598-1.31-1.425-2.117-3.319-2.117-5.402zm14 0c0 3.314-2.686 6-6 6s-6-2.686-6-6 2.686-6 6-6 6 2.686 6 6zm-2.142 1h-1.858v-2h1.858c-.364-1.399-1.459-2.494-2.858-2.858v1.858h-2v-1.858c-1.399.364-2.494 1.459-2.858 2.858h1.858v2h-1.858c.364 1.399 1.459 2.494 2.858 2.858v-1.858h2v1.858c1.399-.364 2.494-1.459 2.858-2.858z""/></svg>";
    const string cveSubSVG = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""24"" height=""24"" viewBox=""0 0 24 24""><path d=""M13 9.406c0 .215-.177.389-.396.389h-.896c-.208 0-.431.125-.479.426-.038.241.102.435.287.519l.907.404c.199.088.287.318.198.514-.092.196-.325.283-.522.194l-.962-.427c-.125-.057-.263-.026-.36.136-.4.661-1.177 1.316-2.277 1.439-1.121-.125-1.908-.805-2.301-1.479-.08-.137-.223-.148-.337-.097l-.962.428c-.198.088-.431.001-.522-.194-.089-.196-.001-.426.198-.514l.907-.404c.196-.088.323-.275.282-.548-.04-.257-.26-.398-.474-.398h-.895c-.219 0-.396-.173-.396-.388 0-.216.177-.389.396-.389h.901c.206 0 .422-.132.462-.416.03-.217-.103-.406-.29-.478l-.836-.326c-.203-.079-.303-.304-.224-.504.081-.2.311-.298.514-.219l.86.334c.111.042.269.028.353-.165.068-.152.144-.294.224-.429.642.374 1.348.561 2.14.561.78 0 1.495-.191 2.137-.563.085.139.164.287.232.445.073.164.225.199.348.151l.86-.334c.203-.079.433.019.514.219.079.2-.021.425-.224.504l-.836.325c-.18.069-.322.246-.288.5.034.247.245.394.46.394h.901c.219.001.396.174.396.39zm-5.928-4.363c.12.027.225.101.289.205.232.386-.07.717-.351 1.062.452.211.94.315 1.49.315.541 0 1.033-.107 1.489-.318-.277-.339-.579-.677-.35-1.058.064-.104.169-.179.289-.206.238-.052.419-.262.419-.516 0-.29-.24-.527-.536-.527-.354 0-.61.332-.518.666.034.127.016.261-.051.373-.152.251-.412.402-.742.404-.335-.002-.591-.155-.742-.404-.067-.112-.085-.246-.051-.373.092-.334-.164-.666-.518-.666-.296 0-.536.237-.536.528 0 .253.181.463.419.515zm14.097 14.711c.522-.79.831-1.735.831-2.754 0-2.761-2.238-5-5-5s-5 2.239-5 5 2.238 5 5 5c1.019 0 1.964-.309 2.755-.832l2.831 2.832 1.414-1.414-2.831-2.832zm-4.169.246c-1.654 0-3-1.346-3-3s1.346-3 3-3 3 1.346 3 3-1.346 3-3 3zm-4.89 2h-7.11l2.599-3h2.696c.345 1.152.976 2.18 1.815 3zm-2.11-5h-10v-17h22v12.11c-.574-.586-1.251-1.068-2-1.425v-8.685h-18v13h8.295c-.19.634-.295 1.305-.295 2z""/></svg>";

    protected override async Task OnInitializedAsync()
    {
        breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem(localizer["cveDashboard"], href: null, disabled: true,icon: cveSVG)
        };
        
        await LoadDashboardData();
    }

    private async Task OnFiltersChanged()
    {
        await LoadDashboardData();
    }

    private async Task RefreshDashboard()
    {
        await LoadDashboardData();
        StateHasChanged();
    }

    private async Task ResetFilters()
    {
        selectedTimeRange = "30d";
        selectedSeverity = "all";
        selectedVendor = "all";
        onlyKevCves = false;
        await LoadDashboardData();
    }
    
    private async Task LoadDashboardData()
    {
        loading = true;
        try
        {
            // Load CVE statistics using controller
            try
            {
                var statisticsResult = await CveController.GetStatisticsForComponentsAsync();
                if (statisticsResult != null)
                {
                    totalCves = statisticsResult.TotalCves;
                    criticalCves = statisticsResult.CriticalCves;
                    kevCves = statisticsResult.KnownExploitedCves;
                    newCvesToday = statisticsResult.NewCvesToday;
                }
                else
                {
                    // No statistics available
                    totalCves = 0;
                    criticalCves = 0;
                    kevCves = 0;
                    newCvesToday = 0;
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading CVE statistics: {ex.Message}", Severity.Error);
                // Set default values when error occurs
                totalCves = 0;
                criticalCves = 0;
                kevCves = 0;
                newCvesToday = 0;
            }
            
            // Load recent CVEs
            try
            {
                var cveResult = CveController.Get();
                if (cveResult != null && cveResult.Any())
                {
                    var filteredCves = ApplyFilters(cveResult.ToList());
                    allCves = filteredCves;
                    recentCves = allCves
                        .OrderByDescending(c => c.PublishedDate)
                        .Take(10)
                        .ToList();
                        
                    // Recalculate statistics based on filtered data
                    RecalculateStatistics();
                }
                else
                {
                    allCves = new List<CORE.Entities.Cve>();
                    recentCves = new List<CORE.Entities.Cve>();
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading CVE data: {ex.Message}", Severity.Error);
                allCves = new List<CORE.Entities.Cve>();
                recentCves = new List<CORE.Entities.Cve>();
            }
            
            // Load user subscriptions
            try
            {
                var subscriptionResult = await CveController.GetUserSubscriptions();
                if (subscriptionResult != null)
                {
                    // Extract the value from ActionResult
                    if (subscriptionResult.Result is OkObjectResult okResult && okResult.Value is IEnumerable<CveSubscription> subscriptions)
                    {
                        activeSubscriptions = subscriptions.Where(s => s.IsActive).ToList();
                    }
                    else if (subscriptionResult.Value != null)
                    {
                        activeSubscriptions = subscriptionResult.Value.Where(s => s.IsActive).ToList();
                    }
                    else
                    {
                        activeSubscriptions = new List<CveSubscription>();
                    }
                }
                else
                {
                    activeSubscriptions = new List<CveSubscription>();
                }
            }
            catch (Exception ex)
            {
                // Handle gracefully if subscriptions can't be loaded
                activeSubscriptions = new List<CveSubscription>();
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading dashboard data: {ex.Message}", Severity.Error);
            // Set default values when error occurs
            totalCves = 0;
            criticalCves = 0;
            kevCves = 0;
            newCvesToday = 0;
            allCves = new List<CORE.Entities.Cve>();
            recentCves = new List<CORE.Entities.Cve>();
        }
        finally
        {
            loading = false;
            chartKey = Guid.NewGuid().ToString(); // Force chart re-render
            StateHasChanged();
        }
    }
    
    
    private void RecalculateStatistics()
    {
        if (allCves == null || !allCves.Any())
        {
            totalCves = 0;
            criticalCves = 0;
            kevCves = 0;
            newCvesToday = 0;
            return;
        }
        
        totalCves = allCves.Count;
        criticalCves = allCves.Count(c => c.CvssV3BaseScore >= 9.0);
        kevCves = allCves.Count(c => c.IsKnownExploited);
        newCvesToday = allCves.Count(c => c.PublishedDate.Date == DateTime.Today);
    }
    
    
    
    private string GetCurrentUserId()
    {
        // For now, return a default user ID. In a real implementation, 
        // you would get this from the authentication context
        try
        {
            var authState = AuthenticationStateProvider.GetAuthenticationStateAsync().Result;
            var userId = authState.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return userId ?? "default-user";
        }
        catch
        {
            return "default-user";
        }
    }
    
    private Color GetSeverityColor(double? baseScore)
    {
        if (baseScore == null) return Color.Default;
        
        return baseScore switch
        {
            >= 9.0 => Color.Error,
            >= 7.0 => Color.Warning,
            >= 4.0 => Color.Info,
            _ => Color.Success
        };
    }
    
    // Chart data methods for Severity Distribution (Donut Chart)
    private double[] GetSeverityChartData()
    {
        if (allCves == null || !allCves.Any())
        {
            return new double[] { 0, 0, 0, 0, 0 };
        }
        
        var critical = allCves.Count(c => c.CvssV3BaseScore >= 9.0);
        var high = allCves.Count(c => c.CvssV3BaseScore >= 7.0 && c.CvssV3BaseScore < 9.0);
        var medium = allCves.Count(c => c.CvssV3BaseScore >= 4.0 && c.CvssV3BaseScore < 7.0);
        var low = allCves.Count(c => c.CvssV3BaseScore < 4.0 && c.CvssV3BaseScore > 0);
        var none = allCves.Count(c => c.CvssV3BaseScore == null || c.CvssV3BaseScore == 0);
        
        return new double[] { critical, high, medium, low, none };
    }
    
    private string[] GetSeverityChartLabels()
    {
        return new[] { "Critical (9.0-10.0)", "High (7.0-8.9)", "Medium (4.0-6.9)", "Low (0.1-3.9)", "None (0.0)" };
    }
    
    private ChartOptions GetSeverityChartOptions()
    {
        return new ChartOptions
        {
            ChartPalette = new[] { "#F44336", "#FF9800", "#FFC107", "#4CAF50", "#9E9E9E" }
        };
    }
    
    // Chart data methods for CVE Trend (Line Chart)
    private List<ChartSeries> GetTrendChartSeries()
    {
        if (allCves == null || !allCves.Any())
        {
            var emptyData = new List<double>();
            for (int i = 0; i < 30; i++) emptyData.Add(0);
            return new List<ChartSeries>
            {
                new ChartSeries { Name = "New CVEs", Data = emptyData.ToArray() }
            };
        }
        
        var last30Days = GetLast30Days();
        var trendData = new List<double>();
        
        foreach (var date in last30Days)
        {
            var count = allCves.Count(c => c.PublishedDate.Date == date.Date);
            trendData.Add(count);
        }
        
        
        return new List<ChartSeries>
        {
            new ChartSeries { Name = "New CVEs", Data = trendData.ToArray() }
        };
    }
    
    private string[] GetTrendChartLabels()
    {
        return GetLast30Days().Select(d => d.ToString("MMM dd")).ToArray();
    }
    
    private ChartOptions GetTrendChartOptions()
    {
        return new ChartOptions
        {
            ChartPalette = new[] { "#2196F3" }
        };
    }
    
    private List<DateTime> GetLast30Days()
    {
        var dates = new List<DateTime>();
        for (int i = 29; i >= 0; i--)
        {
            dates.Add(DateTime.Today.AddDays(-i));
        }
        return dates;
    }
    
    // Chart data methods for Top Vendors (Bar Chart)
    private List<ChartSeries> GetVendorsChartSeries()
    {
        if (allCves == null || !allCves.Any())
        {
            return new List<ChartSeries>
            {
                new ChartSeries { Name = "CVE Count", Data = new double[] { 0, 0, 0, 0, 0 } }
            };
        }
        
        var vendorGroups = allCves
            .Where(c => !string.IsNullOrEmpty(c.CveId))
            .GroupBy(c => ExtractVendor(c.Description))
            .Where(g => !string.IsNullOrEmpty(g.Key))
            .OrderByDescending(g => g.Count())
            .Take(10)
            .ToList();
        
        var data = vendorGroups.Select(g => (double)g.Count()).ToArray();
        
        // Return empty array if no data
        if (data.Length == 0)
        {
            data = new double[0];
        }
        
        
        return new List<ChartSeries>
        {
            new ChartSeries { Name = "CVE Count", Data = data }
        };
    }
    
    private string[] GetVendorsChartLabels()
    {
        if (allCves == null || !allCves.Any())
        {
            return new[] { "Microsoft", "Adobe", "Apple", "Google", "Oracle" };
        }
        
        var labels = allCves
            .Where(c => !string.IsNullOrEmpty(c.CveId))
            .GroupBy(c => ExtractVendor(c.Description))
            .Where(g => !string.IsNullOrEmpty(g.Key))
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Select(g => g.Key)
            .ToArray();
            
        // Return empty array if no data
        if (labels.Length == 0)
        {
            labels = new string[0];
        }
        
        return labels;
    }
    
    private ChartOptions GetVendorsChartOptions()
    {
        return new ChartOptions
        {
            ChartPalette = new[] { "#2196F3", "#4CAF50", "#FF9800", "#9C27B0", "#F44336", "#00BCD4", "#FFEB3B", "#795548", "#607D8B", "#E91E63" }
        };
    }
    
    private AxisChartOptions GetVendorsAxisChartOptions()
    {
        return new AxisChartOptions
        {
            MatchBoundsToSize = true
        };
    }
    
    private string ExtractVendor(string description)
    {
        // Simple vendor extraction logic - in real implementation, 
        // you might have a dedicated vendor field or more sophisticated parsing
        if (string.IsNullOrEmpty(description)) return "Unknown";
        
        var commonVendors = new[] { "Microsoft", "Adobe", "Apple", "Google", "Oracle", "IBM", "Cisco", "VMware" };
        foreach (var vendor in commonVendors)
        {
            if (description.Contains(vendor, StringComparison.OrdinalIgnoreCase))
                return vendor;
        }
        return "Other";
    }
    
    // Chart data methods for EPSS Score Distribution (Pie Chart)
    private double[] GetEpssChartData()
    {
        if (allCves == null || !allCves.Any())
        {
            return new double[] { 0, 0, 0, 0, 0, 0 };
        }
        
        var veryHigh = allCves.Count(c => c.EpssScore >= 0.8);
        var high = allCves.Count(c => c.EpssScore >= 0.6 && c.EpssScore < 0.8);
        var medium = allCves.Count(c => c.EpssScore >= 0.4 && c.EpssScore < 0.6);
        var low = allCves.Count(c => c.EpssScore >= 0.2 && c.EpssScore < 0.4);
        var veryLow = allCves.Count(c => c.EpssScore < 0.2 && c.EpssScore > 0);
        var noScore = allCves.Count(c => c.EpssScore == null || c.EpssScore == 0);
        
        return new double[] { veryHigh, high, medium, low, veryLow, noScore };
    }
    
    private string[] GetEpssChartLabels()
    {
        return new[] { "Very High (0.8-1.0)", "High (0.6-0.8)", "Medium (0.4-0.6)", "Low (0.2-0.4)", "Very Low (0.0-0.2)", "No Score" };
    }
    
    private ChartOptions GetEpssChartOptions()
    {
        return new ChartOptions
        {
            ChartPalette = new[] { "#D32F2F", "#F57C00", "#FBC02D", "#689F38", "#388E3C", "#9E9E9E" }
        };
    }
    
    private List<CORE.Entities.Cve> ApplyFilters(List<CORE.Entities.Cve> cves)
    {
        var filtered = cves.AsQueryable();
        
        // Apply time range filter
        var cutoffDate = GetCutoffDateForTimeRange();
        if (cutoffDate.HasValue)
        {
            filtered = filtered.Where(c => c.PublishedDate >= cutoffDate.Value);
        }
        
        // Apply severity filter
        if (selectedSeverity != "all")
        {
            filtered = selectedSeverity switch
            {
                "critical" => filtered.Where(c => c.CvssV3BaseScore >= 9.0),
                "high" => filtered.Where(c => c.CvssV3BaseScore >= 7.0 && c.CvssV3BaseScore < 9.0),
                "medium" => filtered.Where(c => c.CvssV3BaseScore >= 4.0 && c.CvssV3BaseScore < 7.0),
                "low" => filtered.Where(c => c.CvssV3BaseScore < 4.0 && c.CvssV3BaseScore > 0),
                "none" => filtered.Where(c => c.CvssV3BaseScore == null || c.CvssV3BaseScore == 0),
                _ => filtered
            };
        }
        
        // Apply vendor filter
        if (selectedVendor != "all")
        {
            filtered = filtered.Where(c => ExtractVendor(c.Description).Equals(selectedVendor, StringComparison.OrdinalIgnoreCase));
        }
        
        // Apply KEV filter
        if (onlyKevCves)
        {
            filtered = filtered.Where(c => c.IsKnownExploited);
        }
        
        return filtered.ToList();
    }
    
    private DateTime? GetCutoffDateForTimeRange()
    {
        return selectedTimeRange switch
        {
            "24h" => DateTime.Today.AddDays(-1),
            "7d" => DateTime.Today.AddDays(-7),
            "30d" => DateTime.Today.AddDays(-30),
            "90d" => DateTime.Today.AddDays(-90),
            "1y" => DateTime.Today.AddYears(-1),
            _ => null
        };
    }
    
    private List<string> GetAvailableVendors()
    {
        if (allCves == null || !allCves.Any())
        {
            return new List<string>();
        }
        
        return allCves
            .Select(c => ExtractVendor(c.Description))
            .Where(v => !string.IsNullOrEmpty(v) && v != "Unknown" && v != "Other")
            .Distinct()
            .OrderBy(v => v)
            .Take(20)
            .ToList();
    }
}