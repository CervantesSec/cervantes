using Cervantes.CORE.Entities;
using Cervantes.IFR.Parsers;

namespace Cervantes.Tests.IFR;

public class ImportScopeTests
{
    [Theory]
    [InlineData(VulnImportType.CSV)]
    [InlineData(VulnImportType.Pwndoc)]
    [InlineData(VulnImportType.Burp)]
    [InlineData(VulnImportType.Nessus)]
    public void RequiresProject_TemplateCapableTypes(VulnImportType type)
    {
        Assert.False(ImportScope.RequiresProject(type));
    }

    [Theory]
    [InlineData(VulnImportType.Zap)]
    [InlineData(VulnImportType.OpenVAS)]
    [InlineData(VulnImportType.Qualys)]
    [InlineData(VulnImportType.Nuclei)]
    [InlineData(VulnImportType.Acunetix)]
    [InlineData(VulnImportType.Nikto)]
    [InlineData(VulnImportType.Prowler)]
    [InlineData(VulnImportType.Trivy)]
    [InlineData(VulnImportType.Bandit)]
    [InlineData(VulnImportType.Masscan)]
    [InlineData(VulnImportType.DependencyCheck)]
    public void RequiresProject_Others(VulnImportType type)
    {
        Assert.True(ImportScope.RequiresProject(type));
    }
}
