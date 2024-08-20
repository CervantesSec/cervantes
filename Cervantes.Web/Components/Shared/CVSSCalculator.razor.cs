using Microsoft.AspNetCore.Components;

namespace Cervantes.Web.Components.Shared;

public partial class CVSSCalculator: ComponentBase
{
    [Parameter] public string Vector { get; set; }
    [Parameter] public EventCallback<string> VectorChanged { get; set; }

    private string SelectedVector
    {
        get => Vector;
        set
        {
            if (value != Vector)
            {
                VectorChanged.InvokeAsync(value);        
            }
        }
    }

    private double SelectedScore
    {
        get => Score;
        set
        {
            if (value != Score)
            {
                ScoreChanged.InvokeAsync(value);        
            }
        }
    }
    private async Task OnVectorChanged(string value)
        => await this.VectorChanged.InvokeAsync(value);
    [Parameter] public double Score { get; set; }
    [Parameter] public EventCallback<double> ScoreChanged { get; set; }
    private async Task OnScoreChanged(double value)
        => await this.ScoreChanged.InvokeAsync(value);
    
    private string? SelectedAttackVector;
    private string? SelectedAttackComplexity;
    private string? SelectedPrivilegesRequired;
    private string? SelectedUserInteraction;
    private string? SelectedScope;
    private string? SelectedConfidentiality ;
    private string? SelectedIntegrity;
    private string? SelectedAvailability;
    
   
    private string av{ 
        get => SelectedAttackVector;
        set
        {
            SelectedAttackVector = value;
            CalculateBaseScore();
        }
    }
    private string ac{ 
        get => SelectedAttackComplexity;
        set
        {
            SelectedAttackComplexity = value;
            CalculateBaseScore();
        }
    }
    private string pr{ 
        get => SelectedPrivilegesRequired;
        set
        {
            SelectedPrivilegesRequired = value;
            CalculateBaseScore();
        }
    }
    private string ui{ 
        get => SelectedUserInteraction;
        set
        {
            SelectedUserInteraction = value;
            CalculateBaseScore();
        }
    }
    private string s{ 
        get => SelectedScope;
        set
        {
            SelectedScope = value;
            CalculateBaseScore();
        }
    }
    private string c{ 
        get => SelectedConfidentiality;
        set
        {
            SelectedConfidentiality = value;
            CalculateBaseScore();
        }
    }
    private string i{ 
        get => SelectedIntegrity;
        set
        {
            SelectedIntegrity = value;
            CalculateBaseScore();
        }
    }
    private string a{ 
        get => SelectedAvailability;
        set
        {
            SelectedAvailability = value;
            CalculateBaseScore();
        }
    }
    //private string vector = "CVSS:3.1/AV:" + @SelectedAttackVector + "/AC:" + @SelectedAttackComplexity + "/PR:" + @SelectedPrivilegesRequired + "/UI:" + @SelectedUserInteraction + "/S:" + @SelectedScope + "/C:+" + @SelectedConfidentiality + "/I:" + @SelectedIntegrity + "/A:" + @SelectedAvailability;

    protected override void OnParametersSet()
    {
        /*SelectedVector = Vector;
        if (SelectedVector != String.Empty || SelectedVector != null || SelectedVector != "")
        {
            var metrics = ParseCvssVector(SelectedVector);
            SelectedAttackVector = metrics.AttackVector;
            SelectedAttackComplexity = metrics.AttackComplexity;
            SelectedPrivilegesRequired = metrics.PrivilegesRequired;
            SelectedUserInteraction = metrics.UserInteraction;
            SelectedScope = metrics.Scope;
            SelectedConfidentiality = metrics.Confidentiality;
            SelectedIntegrity = metrics.Integrity;
            SelectedAvailability = metrics.Availability;
            CalculateBaseScore();
            
        }
        else
        {
            SelectedAttackVector = "N";
            SelectedAttackComplexity = "L";
            SelectedPrivilegesRequired = "N";
            SelectedUserInteraction = "N";
            SelectedScope = "C";
            SelectedConfidentiality = "H";
            SelectedIntegrity = "H";
            SelectedAvailability= "H";
            SelectedScore = 10;
            CalculateBaseScore();
        }*/
        var metrics = ParseCvssVector(Vector);
        SelectedAttackVector = metrics.AttackVector;
        SelectedAttackComplexity = metrics.AttackComplexity;
        SelectedPrivilegesRequired = metrics.PrivilegesRequired;
        SelectedUserInteraction = metrics.UserInteraction;
        SelectedScope = metrics.Scope;
        SelectedConfidentiality = metrics.Confidentiality;
        SelectedIntegrity = metrics.Integrity;
        SelectedAvailability = metrics.Availability;
        //CalculateBaseScore();
    }

    private void CalculateBaseScore()
    {
        var exploitabilityCoefficient = 8.22;
        var scopeCoefficient = 1.08;
        double avValue = GetAttackVectorValue(SelectedAttackVector);
        double acValue = GetAttackComplexityValue(SelectedAttackComplexity);
        double prValue = GetPrivilegesRequiredValue(SelectedPrivilegesRequired);
        double uiValue = GetUserInteractionValue(SelectedUserInteraction);
        double sValue = GetScopeValue(SelectedScope);
        double cValue = GetConfidentialityValue(SelectedConfidentiality);
        double iValue = GetIntegrityValue(SelectedIntegrity);
        double aValue = GetAvailabilityValue(SelectedAvailability);

        //double baseScore = 0.6 * avValue + 0.4 * (acValue * prValue * uiValue) + sValue;
        var impactSubScoreMultiplier = (1 - ((1 - cValue) * (1 - iValue) * (1 - aValue)));
        var impactSubScore = 0.0;
        var baseScore = 0.0;
        if (SelectedScope == "U") {
            impactSubScore = sValue * impactSubScoreMultiplier;
        } 
        else {
            impactSubScore = sValue * (impactSubScoreMultiplier - 0.029) - 3.25 * Math.Pow(impactSubScoreMultiplier - 0.02, 15);
        }
        
        var exploitabalitySubScore = exploitabilityCoefficient * avValue * acValue * prValue * uiValue;
        
        if (impactSubScore <= 0) {
            baseScore = 0;
        } 
        else {
            if (SelectedScope == "U") {
                baseScore = Roundup(Math.Min((exploitabalitySubScore + impactSubScore), 10));
            }
            else
            {
                baseScore = Roundup(Math.Min((exploitabalitySubScore + impactSubScore) * scopeCoefficient, 10));
            }
        }
        
        SelectedScore = Math.Round(baseScore,1);
        SelectedVector = "CVSS:3.1/AV:" + @SelectedAttackVector + "/AC:" + @SelectedAttackComplexity + "/PR:" + @SelectedPrivilegesRequired + "/UI:" + @SelectedUserInteraction + "/S:" + @SelectedScope + "/C:" + @SelectedConfidentiality + "/I:" + @SelectedIntegrity + "/A:" + @SelectedAvailability;
    }

    private double GetAttackVectorValue(string value)
    {
        switch (value)
        {
            case "N":
                return 0.85;
            case "A":
                return 0.62;
            case "L":
                return 0.55;
            case "P":
                return 0.20;
            default:
                return 0.0;
        }
    }

    private double GetAttackComplexityValue(string value)
    {
        switch (value)
        {
            case "L":
                return 0.77;
            case "H":
                return 0.44;
            default:
                return 0.0;
        }
    }

    private double GetPrivilegesRequiredValue(string value)
    {
        if (SelectedScope == "U")
        {
            switch (value)
            {
                case "N":
                    return 0.85;
                case "L":
                    return 0.62;
                case "H":
                    return 0.27;
                default:
                    return 0.0;
            }
        }

        switch (value)
        {
            case "N":
                return 0.85;
            case "L":
                return 0.68;
            case "H":
                return 0.50;
            default:
                return 0.0;
        }
    }

    private double GetUserInteractionValue(string value)
    {
        switch (value)
        {
            case "N":
                return 0.85;
            case "R":
                return 0.62;
            default:
                return 0.0;
        }
    }

    private double GetScopeValue(string value)
    {
        switch (value)
        {
            case "U":
                return 6.42;
            case "C":
                return 7.52;
            default:
                return 0.0;
        }
    }
    
    private double GetConfidentialityValue(string value)
    {
        switch (value)
        {
            case "N":
                return 0;
            case "L":
                return 0.22;
            case "H":
                return 0.56;
            default:
                return 0.0;
        }
    }
    
    private double GetIntegrityValue(string value)
    {
        switch (value)
        {
            case "N":
                return 0;
            case "L":
                return 0.22;
            case "H":
                return 0.56;
            default:
                return 0.0;
        }
    }
    
    private double GetAvailabilityValue(string value)
    {
        switch (value)
        {
            case "N":
                return 0;
            case "L":
                return 0.22;
            case "H":
                return 0.56;
            default:
                return 0.0;
        }
    }
    
    public static double Roundup(double input)
    {
        var intInput = Math.Round(input * 100000);
        
        if (intInput % 10000 == 0)
        {
            return intInput / 100000.0;
        }
        else
        {
            return (Math.Floor(intInput / 10000.0) + 1) / 10.0;
        }
    }
    public class CvssMetrics
    {
        //public string? Version { get; set; }
        public string? AttackVector { get; set; }
        public string? AttackComplexity { get; set; }
        public string? PrivilegesRequired { get; set; }
        public string? UserInteraction { get; set; }
        public string? Scope { get; set; }
        public string? Confidentiality { get; set; }
        public string? Integrity { get; set; }
        public string? Availability { get; set; }
    }
    public static CvssMetrics ParseCvssVector(string vector)
    {
        var metrics = new CvssMetrics();

        // Remove "CVSS:" prefix if present
        var vectorString = vector;
        //vectorString = vectorString.Replace("CVSS:3.1", "");

        var parts = vectorString.Split('/');

        //metrics.Version = parts[0];

        foreach (var part in parts.Skip(1))
        {
            var keyValue = part.Split(':');
            var key = keyValue[0];
            var value = keyValue[1];

            switch (key)
            {
                case "AV":
                    metrics.AttackVector = value;
                    break;
                case "AC":
                    metrics.AttackComplexity = value;
                    break;
                case "PR":
                    metrics.PrivilegesRequired = value;
                    break;
                case "UI":
                    metrics.UserInteraction = value;
                    break;
                case "S":
                    metrics.Scope = value;
                    break;
                case "C":
                    metrics.Confidentiality = value;
                    break;
                case "I":
                    metrics.Integrity = value;
                    break;
                case "A":
                    metrics.Availability = value;
                    break;
                default:
                    // Handle unsupported keys
                    break;
            }
        }

        return metrics;
    }
}