using System.Globalization;
using Cervantes.IFR.Parsers.CSV;
using CsvHelper;
using CsvHelper.Configuration;

namespace Cervantes.Tests.IFR;

public class CsvImportTests
{
    [Fact]
    public void Csv_Cvss31StaysThreePointOne()
    {
        const string csvText =
            "Name;Template;VulnCategory;Risk;Status;CVE;Description;Poc;Impact;CVSS3;CVSSVector;Remediation;RemediationComplexity;RemediationPriority;OwaspRisk;OwaspVector;OwaspLikehood;OwaspImpact;MitreTechniques;MitreValues\n" +
            "Weak TLS;False;Network;Low;Open;None;d;p;i;3.1;;r;Low;Low;;;;;;\n";
        using var csv = new CsvReader(new StringReader(csvText), new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" });

        var row = Assert.Single(csv.GetRecords<VulnImportCsv>().ToList());

        Assert.Equal<double?>(3.1, row.CVSS3);
    }
}
