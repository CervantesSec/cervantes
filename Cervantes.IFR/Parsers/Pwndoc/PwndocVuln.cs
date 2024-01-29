using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Cervantes.CORE;

namespace Cervantes.IFR.Parsers.Pwndoc;

public class PwndocVuln
{
    public string Cvssv3 { get; set; }
    public string CvssScore { get; set; }
    public string CvssSeverity { get; set; }
    public int Priority { get; set; }
    public int RemediationComplexity { get; set; }
    public string Category { get; set; }
    public List<Detail> Details { get; set; }
}

public class Detail
{
    public List<string> References { get; set; }
    public List<string> CustomFields { get; set; }
    public string Locale { get; set; }
    public string Title { get; set; }
    public string VulnType { get; set; }
    public string Description { get; set; }
    public string Observation { get; set; }
    public string Remediation { get; set; }
}
