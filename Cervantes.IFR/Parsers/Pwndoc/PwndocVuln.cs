using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Cervantes.CORE;

namespace Cervantes.IFR.Parsers.Pwndoc;

// One item of a Pwndoc vulnerabilities export (details, cvssv3, cvssv4, priority, remediationComplexity, category).
public class PwndocVuln
{
    public string Cvssv3 { get; set; }
    public string Cvssv4 { get; set; }
    // Optional in real exports
    public int? Priority { get; set; }
    public int RemediationComplexity { get; set; }
    public string Category { get; set; }
    public List<Detail> Details { get; set; }
}

public class Detail
{
    public List<string> References { get; set; }
    public List<PwndocCustomField> CustomFields { get; set; }
    public string Locale { get; set; }
    public string Title { get; set; }
    public string VulnType { get; set; }
    public string Description { get; set; }
    public string Observation { get; set; }
    public string Remediation { get; set; }
}

// A custom field value: the id of the field definition and its value (text, or a list for multi-value fields).
public class PwndocCustomField
{
    public string CustomField { get; set; }
    public object Text { get; set; }
}
