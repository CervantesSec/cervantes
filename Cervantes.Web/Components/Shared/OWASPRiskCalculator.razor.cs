using Microsoft.AspNetCore.Components;

namespace Cervantes.Web.Components.Shared;

public partial class OWASPRiskCalculator: ComponentBase
{
    [Parameter] public string Risk { get; set; }
    [Parameter] public EventCallback<string> RiskChanged { get; set; }
    private string ValueRisk
    {
        get => Risk;
        set
        {
            if (value != Risk)
            {
                RiskChanged.InvokeAsync(value);        
            }
        }
    }
    
    [Parameter] public string Vector { get; set; }
    [Parameter] public EventCallback<string> VectorChanged { get; set; }
    private string ValueVector
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
    [Parameter] public string Likehood { get; set; } = String.Empty;
    [Parameter] public EventCallback<string> LikehoodChanged { get; set; }
    private string ValueLikehood
    {
        get => Likehood;
        set
        {
            if (value != Likehood)
            {
                LikehoodChanged.InvokeAsync(value);        
            }
        }
    }
    [Parameter] public string Impact { get; set; } = String.Empty;
    [Parameter] public EventCallback<string> ImpactChanged { get; set; }
    private string ValueImpact
    {
        get => Impact;
        set
        {
            if (value != Impact)
            {
                ImpactChanged.InvokeAsync(value);        
            }
        }
    }
    
    public string FLS = String.Empty;
    public string FIS = String.Empty;
    public double LS = 0;
    public double IS = 0;
    
    private int SelectedSkillLevel;
    private int SelectedMotive;
    private int SelectedOpportunity;
    private int SelectedSize;
    
    private int SelectedEaseDiscovery;
    private int SelectedEaseExploit;
    private int SelectedAwarness;
    private int SelectedIntrusionDetection;
    
    private int SelectedLossConfidentiality;
    private int SelectedLossIntegrity;
    private int SelectedLossAvailability;
    private int SelectedLossAccountability;
    
    private int SelectedFinancialDamage;
    private int SelectedReputationDamage;
    private int SelectedNonCompliance;
    private int SelectedPrivacyViolation;
    private int sl{ 
        get => SelectedSkillLevel;
        set
        {
            SelectedSkillLevel = value;
            CalculateBaseScore();
        }
    }
    private int m{ 
        get => SelectedMotive;
        set
        {
            SelectedMotive = value;
            CalculateBaseScore();
        }
    }
    private int o{ 
        get => SelectedOpportunity;
        set
        {
            SelectedOpportunity = value;
            CalculateBaseScore();
        }
    }
    private int s{ 
        get => SelectedSize;
        set
        {
            SelectedSize = value;
            CalculateBaseScore();
        }
    }
    private int ed{ 
        get => SelectedEaseDiscovery;
        set
        {
            SelectedEaseDiscovery = value;
            CalculateBaseScore();
        }
    }
    private int ee{ 
        get => SelectedEaseExploit;
        set
        {
            SelectedEaseExploit = value;
            CalculateBaseScore();
        }
    }
    private int a{ 
        get => SelectedAwarness;
        set
        {
            SelectedAwarness = value;
            CalculateBaseScore();
        }
    }
    private int id{ 
        get => SelectedIntrusionDetection;
        set
        {
            SelectedIntrusionDetection = value;
            CalculateBaseScore();
        }
    }
    
    private int lc{ 
        get => SelectedLossConfidentiality;
        set
        {
            SelectedLossConfidentiality = value;
            CalculateBaseScore();
        }
    }
    private int li{ 
        get => SelectedLossIntegrity;
        set
        {
            SelectedLossIntegrity = value;
            CalculateBaseScore();
        }
    }
    private int la{ 
        get => SelectedLossAvailability;
        set
        {
            SelectedLossAvailability = value;
            CalculateBaseScore();
        }
    }
    
    private int lac{ 
        get => SelectedLossAccountability;
        set
        {
            SelectedLossAccountability = value;
            CalculateBaseScore();
        }
    }
    
    private int fd{ 
        get => SelectedFinancialDamage;
        set
        {
            SelectedFinancialDamage = value;
            CalculateBaseScore();
        }
    }
    private int rd{ 
        get => SelectedReputationDamage;
        set
        {
            SelectedReputationDamage = value;
            CalculateBaseScore();
        }
    }
    private int nc{ 
        get => SelectedNonCompliance;
        set
        {
            SelectedNonCompliance = value;
            CalculateBaseScore();
        }
    }
    private int pv{ 
        get => SelectedPrivacyViolation;
        set
        {
            SelectedPrivacyViolation = value;
            CalculateBaseScore();
        }
    }
    
    protected override void OnParametersSet()
    {

            var metrics = ParseOWASPVector(Vector);
            SelectedSkillLevel = metrics.SkillLevel;
            SelectedMotive = metrics.Motive;
            SelectedOpportunity = metrics.Opportunity;
            SelectedSize = metrics.Size;
            SelectedEaseDiscovery = metrics.EaseDiscovery;
            SelectedEaseExploit = metrics.EaseExploit;
            SelectedAwarness = metrics.Awareness;
            SelectedIntrusionDetection = metrics.IntrusionDetection;
            SelectedLossConfidentiality = metrics.LossConfidentiality;
            SelectedLossIntegrity = metrics.LossIntegrity;
            SelectedLossAvailability = metrics.LossAvailability;
            SelectedLossAccountability = metrics.LossAccountability;
            SelectedFinancialDamage = metrics.FinancialDamage;
            SelectedReputationDamage = metrics.ReputationDamage;
            SelectedNonCompliance = metrics.NonCompliance;
            SelectedPrivacyViolation = metrics.PrivacyViolation;
            
            /*
            sl = metrics.SkillLevel;
            m = metrics.Motive;
            o = metrics.Opportunity;
            s = metrics.Size;
            ed = metrics.EaseDiscovery;
            ee = metrics.EaseExploit;
            a = metrics.Awareness;
            id = metrics.IntrusionDetection;
            lc = metrics.LossConfidentiality;
            li = metrics.LossIntegrity;
            la = metrics.LossAvailability;
            lac = metrics.LossAccountability;
            fd = metrics.FinancialDamage;
            rd = metrics.ReputationDamage;
            nc = metrics.NonCompliance;
            pv = metrics.PrivacyViolation;
            */
            
            //CalculateBaseScore();
        
    }
    
     private void CalculateBaseScore()
    {
         LS = SelectedSkillLevel + SelectedMotive + SelectedOpportunity + SelectedSize + SelectedEaseDiscovery + 
                 SelectedEaseExploit + SelectedAwarness + SelectedIntrusionDetection + 0;
         IS= SelectedLossConfidentiality + SelectedLossIntegrity + SelectedLossAvailability + SelectedLossAccountability + 
                SelectedFinancialDamage + SelectedReputationDamage + SelectedNonCompliance + SelectedPrivacyViolation + 0;
         /*LS = sl + m + o + s + ed + 
              ee + a + id + 0;
         IS= lc + SelectedLossIntegrity + la + lac + 
             fd + rd + nc + pv + 0;*/
        LS = Math.Round(LS / 8,3);
        IS = Math.Round(IS / 8,3);
        FLS = GetRisk(LS);
        FIS = GetRisk(IS);
        ValueLikehood = FLS + " "+ LS;
        ValueImpact = FIS + " "+ IS;
        ValueRisk = GetCriticaly(FLS, FIS);
        ValueVector = "(SL:"+sl+"/M:"+m+"/O:"+o+"/S:"+s+"/ED:"+ed+"/EE:"+ee+"/A:"+a+"/ID:"+id+"/LC:"+lc+"/LI:"+li+"/LAV:"+la+
                      "/LAC:"+lac+"/FD:"+fd+"/RD:"+rd+"/NC:"+nc+"/PV:"+pv+")";
        /*sl = SelectedSkillLevel;
        m = SelectedMotive;
        o = SelectedOpportunity;
        s = SelectedSize;
        ed = SelectedEaseDiscovery;
        ee = SelectedEaseExploit;
        a = SelectedAwarness;
        id = SelectedIntrusionDetection;
        lc = SelectedLossConfidentiality;
        li = SelectedLossIntegrity;
        la = SelectedLossAvailability;
        lac = SelectedLossAccountability;
        fd = SelectedFinancialDamage;
        rd = SelectedReputationDamage;
        nc = SelectedNonCompliance;
        pv = SelectedPrivacyViolation;*/
    }
     
    public string GetRisk(double score)
    {
         if(score == 0) return "NOTE";
         if(score < 3) return "LOW";
         if(score < 6) return "MEDIUM";
         if(score <= 9) return "HIGH";
         return "N/A";
    }
    
    public string GetCriticaly( string L, string I){
        //NOTE
        if(L == "LOW" && I == "LOW") return "NOTE";

        //LOW
        if(L == "LOW" && I == "MEDIUM") return "LOW";
        if(L == "MEDIUM" && I == "LOW") return "LOW";
  
        //MEDIUM
        if(L == "LOW" && I == "HIGH") return "MEDIUM";
        if(L == "MEDIUM" && I == "MEDIUM") return "MEDIUM";
        if(L == "HIGH" && I == "LOW") return "MEDIUM";

        //HIGH
        if(L == "HIGH" && I == "MEDIUM") return "HIGH";
        if(L == "MEDIUM" && I == "HIGH") return "HIGH";

        //CRITICAL
        if(L == "HIGH" && I == "HIGH") return "CRITICAL";
        return "N/A";
    }
     
     public class OWASPMetrics
     {
         public int SkillLevel { get; set; }
         public int Motive { get; set; }
         public int Opportunity { get; set; }
         public int Size { get; set; }
         public int EaseDiscovery { get; set; }
         public int EaseExploit { get; set; }
         public int Awareness { get; set; }
         public int IntrusionDetection { get; set; }
         public int LossConfidentiality { get; set; }
         public int LossIntegrity { get; set; }
         public int LossAvailability { get; set; }
         public int LossAccountability { get; set; }
         public int FinancialDamage { get; set; }
         public int ReputationDamage { get; set; }
         public int NonCompliance { get; set; }
         public int PrivacyViolation { get; set; }
     }

     private static OWASPMetrics ParseOWASPVector(string vectorString)
     {
         var metrics = new OWASPMetrics();

         vectorString = vectorString.Replace("(", "");
         vectorString = vectorString.Replace(")", "");
         var parts = vectorString.Split('/');
         
         foreach (var part in parts)
         {
             var keyValue = part.Split(':');
             var key = keyValue[0];
             var value = keyValue[1];

             switch (key)
             {
                 case "SL":
                     metrics.SkillLevel = int.Parse(value);
                     break;
                 case "M":
                     metrics.Motive = int.Parse(value);
                     break;
                 case "O":
                     metrics.Opportunity = int.Parse(value);
                     break;
                 case "S":
                     metrics.Size = int.Parse(value);
                     break;
                 case "ED":
                     metrics.EaseDiscovery = int.Parse(value);
                     break;
                 case "EE":
                     metrics.EaseExploit = int.Parse(value);
                     break;
                 case "A":
                     metrics.Awareness = int.Parse(value);
                     break;
                 case "ID":
                     metrics.IntrusionDetection = int.Parse(value);
                     break;
                    case "LC":  
                     metrics.LossConfidentiality = int.Parse(value);
                     break;
                    case "LI":  
                     metrics.LossIntegrity = int.Parse(value);
                     break;
                    case "LAV":  
                     metrics.LossAvailability = int.Parse(value);
                     break;
                    case "LAC":  
                     metrics.LossAccountability = int.Parse(value);
                     break;
                    case "FD":  
                     metrics.FinancialDamage = int.Parse(value);
                     break;
                    case "RD":  
                     metrics.ReputationDamage = int.Parse(value);
                     break;
                    case "NC":  
                     metrics.NonCompliance = int.Parse(value);
                     break;
                    case "PV":  
                     metrics.PrivacyViolation = int.Parse(value);
                     break;
                 default:
                     // Handle unsupported keys
                     break;
             }
         }

         return metrics;
     }
}