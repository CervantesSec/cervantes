using System.Text.Json.Serialization;

namespace Cervantes.IFR.CveServices.Models;

/// <summary>
/// NVD API Response model
/// </summary>
public class NvdApiResponse
{
    [JsonPropertyName("resultsPerPage")]
    public int ResultsPerPage { get; set; }

    [JsonPropertyName("startIndex")]
    public int StartIndex { get; set; }

    [JsonPropertyName("totalResults")]
    public int TotalResults { get; set; }

    [JsonPropertyName("format")]
    public string Format { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("vulnerabilities")]
    public List<NvdVulnerability> Vulnerabilities { get; set; } = new();
}

/// <summary>
/// NVD Vulnerability wrapper
/// </summary>
public class NvdVulnerability
{
    [JsonPropertyName("cve")]
    public NvdCveItem Cve { get; set; }
}

/// <summary>
/// NVD CVE Item model
/// </summary>
public class NvdCveItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("sourceIdentifier")]
    public string SourceIdentifier { get; set; }

    [JsonPropertyName("published")]
    public DateTime Published { get; set; }

    [JsonPropertyName("lastModified")]
    public DateTime LastModified { get; set; }

    [JsonPropertyName("vulnStatus")]
    public string VulnStatus { get; set; }

    [JsonPropertyName("descriptions")]
    public List<NvdDescription> Descriptions { get; set; } = new();

    [JsonPropertyName("metrics")]
    public NvdMetrics Metrics { get; set; }

    [JsonPropertyName("weaknesses")]
    public List<NvdWeakness> Weaknesses { get; set; } = new();

    [JsonPropertyName("configurations")]
    public List<NvdConfiguration> Configurations { get; set; } = new();

    [JsonPropertyName("references")]
    public List<NvdReference> References { get; set; } = new();

    [JsonPropertyName("vendorComments")]
    public List<NvdVendorComment> VendorComments { get; set; } = new();
}

/// <summary>
/// NVD Description model
/// </summary>
public class NvdDescription
{
    [JsonPropertyName("lang")]
    public string Lang { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}

/// <summary>
/// NVD Metrics model
/// </summary>
public class NvdMetrics
{
    [JsonPropertyName("cvssMetricV31")]
    public List<NvdCvssV31> CvssMetricV31 { get; set; } = new();

    [JsonPropertyName("cvssMetricV30")]
    public List<NvdCvssV30> CvssMetricV30 { get; set; } = new();

    [JsonPropertyName("cvssMetricV2")]
    public List<NvdCvssV2> CvssMetricV2 { get; set; } = new();
}

/// <summary>
/// NVD CVSS v3.1 model
/// </summary>
public class NvdCvssV31
{
    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("cvssData")]
    public NvdCvssData CvssData { get; set; }

    [JsonPropertyName("exploitabilityScore")]
    public double ExploitabilityScore { get; set; }

    [JsonPropertyName("impactScore")]
    public double ImpactScore { get; set; }
}

/// <summary>
/// NVD CVSS v3.0 model
/// </summary>
public class NvdCvssV30
{
    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("cvssData")]
    public NvdCvssData CvssData { get; set; }

    [JsonPropertyName("exploitabilityScore")]
    public double ExploitabilityScore { get; set; }

    [JsonPropertyName("impactScore")]
    public double ImpactScore { get; set; }
}

/// <summary>
/// NVD CVSS v2 model
/// </summary>
public class NvdCvssV2
{
    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("cvssData")]
    public NvdCvssV2Data CvssData { get; set; }

    [JsonPropertyName("baseSeverity")]
    public string BaseSeverity { get; set; }

    [JsonPropertyName("exploitabilityScore")]
    public double ExploitabilityScore { get; set; }

    [JsonPropertyName("impactScore")]
    public double ImpactScore { get; set; }

    [JsonPropertyName("acInsufInfo")]
    public bool AcInsufInfo { get; set; }

    [JsonPropertyName("obtainAllPrivilege")]
    public bool ObtainAllPrivilege { get; set; }

    [JsonPropertyName("obtainUserPrivilege")]
    public bool ObtainUserPrivilege { get; set; }

    [JsonPropertyName("obtainOtherPrivilege")]
    public bool ObtainOtherPrivilege { get; set; }

    [JsonPropertyName("userInteractionRequired")]
    public bool UserInteractionRequired { get; set; }
}

/// <summary>
/// NVD CVSS Data model
/// </summary>
public class NvdCvssData
{
    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("vectorString")]
    public string VectorString { get; set; }

    [JsonPropertyName("attackVector")]
    public string AttackVector { get; set; }

    [JsonPropertyName("attackComplexity")]
    public string AttackComplexity { get; set; }

    [JsonPropertyName("privilegesRequired")]
    public string PrivilegesRequired { get; set; }

    [JsonPropertyName("userInteraction")]
    public string UserInteraction { get; set; }

    [JsonPropertyName("scope")]
    public string Scope { get; set; }

    [JsonPropertyName("confidentialityImpact")]
    public string ConfidentialityImpact { get; set; }

    [JsonPropertyName("integrityImpact")]
    public string IntegrityImpact { get; set; }

    [JsonPropertyName("availabilityImpact")]
    public string AvailabilityImpact { get; set; }

    [JsonPropertyName("baseScore")]
    public double BaseScore { get; set; }

    [JsonPropertyName("baseSeverity")]
    public string BaseSeverity { get; set; }
}

/// <summary>
/// NVD CVSS v2 Data model
/// </summary>
public class NvdCvssV2Data
{
    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("vectorString")]
    public string VectorString { get; set; }

    [JsonPropertyName("accessVector")]
    public string AccessVector { get; set; }

    [JsonPropertyName("accessComplexity")]
    public string AccessComplexity { get; set; }

    [JsonPropertyName("authentication")]
    public string Authentication { get; set; }

    [JsonPropertyName("confidentialityImpact")]
    public string ConfidentialityImpact { get; set; }

    [JsonPropertyName("integrityImpact")]
    public string IntegrityImpact { get; set; }

    [JsonPropertyName("availabilityImpact")]
    public string AvailabilityImpact { get; set; }

    [JsonPropertyName("baseScore")]
    public double BaseScore { get; set; }
}

/// <summary>
/// NVD Weakness model
/// </summary>
public class NvdWeakness
{
    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("description")]
    public List<NvdDescription> Description { get; set; } = new();
}

/// <summary>
/// NVD Configuration model
/// </summary>
public class NvdConfiguration
{
    [JsonPropertyName("nodes")]
    public List<NvdConfigurationNode> Nodes { get; set; } = new();
}

/// <summary>
/// NVD Configuration Node model
/// </summary>
public class NvdConfigurationNode
{
    [JsonPropertyName("operator")]
    public string Operator { get; set; }

    [JsonPropertyName("negate")]
    public bool Negate { get; set; }

    [JsonPropertyName("cpeMatch")]
    public List<NvdCpeMatch> CpeMatch { get; set; } = new();
}

/// <summary>
/// NVD CPE Match model
/// </summary>
public class NvdCpeMatch
{
    [JsonPropertyName("vulnerable")]
    public bool Vulnerable { get; set; }

    [JsonPropertyName("criteria")]
    public string Criteria { get; set; }

    [JsonPropertyName("matchCriteriaId")]
    public string MatchCriteriaId { get; set; }

    [JsonPropertyName("versionStartIncluding")]
    public string VersionStartIncluding { get; set; }

    [JsonPropertyName("versionStartExcluding")]
    public string VersionStartExcluding { get; set; }

    [JsonPropertyName("versionEndIncluding")]
    public string VersionEndIncluding { get; set; }

    [JsonPropertyName("versionEndExcluding")]
    public string VersionEndExcluding { get; set; }
}

/// <summary>
/// NVD Reference model
/// </summary>
public class NvdReference
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// NVD Vendor Comment model
/// </summary>
public class NvdVendorComment
{
    [JsonPropertyName("organization")]
    public string Organization { get; set; }

    [JsonPropertyName("comment")]
    public string Comment { get; set; }

    [JsonPropertyName("lastModified")]
    public DateTime LastModified { get; set; }
}