using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

public class MASTG
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [ForeignKey("TargetId")]
    [JsonIgnore]
    public virtual Target Target { get; set; }

    /// <summary>
    /// Id target
    /// </summary>
    public Guid TargetId { get; set; }
    /// <summary>
    /// User who created 
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public string? UserId { get; set; }
    
    /// <summary>
    /// Project Associated
    /// </summary>
    [ForeignKey("ProjectId")]
    [JsonIgnore]
    public virtual Project Project { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    public Guid ProjectId { get; set; }
    
    /// <summary>
    ///  Created Date
    /// </summary>
    public DateTime CreatedDate { get; set; }
    
    public MobileAppPlatform MobilePlatform { get; set; }
    
    public string? Storage1Note {get; set;}
    public MASTGStatus Storage1Status {get; set;}
    public string? Storage1Note1 {get; set;}
    public string? Storage1Note2 {get; set;}
    public string? Storage1Note3 {get; set;}
    public MASTGStatus Storage1Status1 {get; set;}
    public MASTGStatus Storage1Status2 {get; set;}
    public MASTGStatus Storage1Status3 {get; set;}
    public string? Storage2Note {get; set;}
    public MASTGStatus Storage2Status {get; set;}
    public string? Storage2Note1 {get; set;}
    public string? Storage2Note2 {get; set;}
    public string? Storage2Note3 {get; set;}
    public string? Storage2Note4 {get; set;}
    public string? Storage2Note5 {get; set;}
    public string? Storage2Note6 {get; set;}
    public string? Storage2Note7 {get; set;}
    public string? Storage2Note8 {get; set;}
    public string? Storage2Note9 {get; set;}
    public string? Storage2Note10 {get; set;}
    public string? Storage2Note11 {get; set;}
    public MASTGStatus Storage2Status1 {get; set;}
    public MASTGStatus Storage2Status2 {get; set;}
    public MASTGStatus Storage2Status3 {get; set;}
    public MASTGStatus Storage2Status4 {get; set;}
    public MASTGStatus Storage2Status5 {get; set;}
    public MASTGStatus Storage2Status6 {get; set;}
    public MASTGStatus Storage2Status7 {get; set;}
    public MASTGStatus Storage2Status8 {get; set;}
    public MASTGStatus Storage2Status9 {get; set;}
    public MASTGStatus Storage2Status10 {get; set;}
    public MASTGStatus Storage2Status11 {get; set;}
    
    public string? Crypto1Note {get; set;}
    public string? Crypto1Note1 {get; set;}
    public string? Crypto1Note2 {get; set;}
    public string? Crypto1Note3 {get; set;}
    public string? Crypto1Note4 {get; set;}
    public string? Crypto1Note5 {get; set;}
    public MASTGStatus Crypto1Status {get; set;}
    public MASTGStatus Crypto1Status1 {get; set;}
    public MASTGStatus Crypto1Status2 {get; set;}
    public MASTGStatus Crypto1Status3 {get; set;}
    public MASTGStatus Crypto1Status4 {get; set;}
    public MASTGStatus Crypto1Status5 {get; set;}
    public string? Crypto2Note {get; set;}
    public string? Crypto2Note1 {get; set;}
    public string? Crypto2Note2 {get; set;}
    public MASTGStatus Crypto2Status {get; set;}
    public MASTGStatus Crypto2Status1 {get; set;}
    public MASTGStatus Crypto2Status2 {get; set;}
    
    public string? Auth1Note {get; set;}
    public MASTGStatus Auth1Status {get; set;}
    public string? Auth2Note {get; set;}
    public string? Auth2Note1 {get; set;}
    public string? Auth2Note2 {get; set;}
    public string? Auth2Note3 {get; set;}
    public MASTGStatus Auth2Status {get; set;}
    public MASTGStatus Auth2Status1 {get; set;}
    public MASTGStatus Auth2Status2 {get; set;}
    public MASTGStatus Auth2Status3 {get; set;}
    public string? Auth3Note {get; set;}
    public MASTGStatus Auth3Status {get; set;}
    
    public string? Network1Note {get; set;}
    public MASTGStatus Network1Status {get; set;}
    public string? Network1Note1 {get; set;}
    public string? Network1Note2 {get; set;}
    public string? Network1Note3 {get; set;}
    public string? Network1Note4 {get; set;}
    public string? Network1Note5 {get; set;}
    public string? Network1Note6 {get; set;}
    public string? Network1Note7 {get; set;}
    public MASTGStatus Network1Status1 {get; set;}
    public MASTGStatus Network1Status2 {get; set;}
    public MASTGStatus Network1Status3 {get; set;}
    public MASTGStatus Network1Status4 {get; set;}
    public MASTGStatus Network1Status5 {get; set;}
    public MASTGStatus Network1Status6 {get; set;}
    public MASTGStatus Network1Status7 {get; set;}
    public string? Network2Note {get; set;}
    public string? Network2Note1 {get; set;}
    public string? Network2Note2 {get; set;}
    public MASTGStatus Network2Status {get; set;}
    public MASTGStatus Network2Status1 {get; set;}
    public MASTGStatus Network2Status2 {get; set;}
    
    public string? Platform1Note {get; set;}
    public MASTGStatus Platform1Status {get; set;}
    public string? Platform1Note1 {get; set;}
    public string? Platform1Note2 {get; set;}
    public string? Platform1Note3 {get; set;}
    public string? Platform1Note4 {get; set;}
    public string? Platform1Note5 {get; set;}
    public string? Platform1Note6 {get; set;}
    public string? Platform1Note7 {get; set;}
    public string? Platform1Note8 {get; set;}
    public string? Platform1Note9 {get; set;}
    public string? Platform1Note10 {get; set;}
    public string? Platform1Note11 {get; set;}
    public string? Platform1Note12 {get; set;}
    public string? Platform1Note13 {get; set;}
    public MASTGStatus Platform1Status1 {get; set;}
    public MASTGStatus Platform1Status2 {get; set;}
    public MASTGStatus Platform1Status3 {get; set;}
    public MASTGStatus Platform1Status4 {get; set;}
    public MASTGStatus Platform1Status5 {get; set;}
    public MASTGStatus Platform1Status6 {get; set;}
    public MASTGStatus Platform1Status7 {get; set;}
    public MASTGStatus Platform1Status8 {get; set;}
    public MASTGStatus Platform1Status9 {get; set;}
    public MASTGStatus Platform1Status10 {get; set;}
    public MASTGStatus Platform1Status11 {get; set;}
    public MASTGStatus Platform1Status12 {get; set;}
    public MASTGStatus Platform1Status13 {get; set;}
    public string? Platform2Note {get; set;}
    public MASTGStatus Platform2Status {get; set;}
    public string? Platform2Note1 {get; set;}
    public string? Platform2Note2 {get; set;}
    public string? Platform2Note3 {get; set;}
    public string? Platform2Note4 {get; set;}
    public string? Platform2Note5 {get; set;}
    public string? Platform2Note6 {get; set;}
    public string? Platform2Note7 {get; set;}
    public MASTGStatus Platform2Status1 {get; set;}
    public MASTGStatus Platform2Status2 {get; set;}
    public MASTGStatus Platform2Status3 {get; set;}
    public MASTGStatus Platform2Status4 {get; set;}
    public MASTGStatus Platform2Status5 {get; set;}
    public MASTGStatus Platform2Status6 {get; set;}
    public MASTGStatus Platform2Status7 {get; set;}
    public string? Platform3Note {get; set;}
    public MASTGStatus Platform3Status {get; set;}
    public string? Platform3Note1 {get; set;}
    public string? Platform3Note2 {get; set;}
    public string? Platform3Note3 {get; set;}
    public string? Platform3Note4 {get; set;}
    public string? Platform3Note5 {get; set;}
    public MASTGStatus Platform3Status1 {get; set;}
    public MASTGStatus Platform3Status2 {get; set;}
    public MASTGStatus Platform3Status3 {get; set;}
    public MASTGStatus Platform3Status4 {get; set;}
    public MASTGStatus Platform3Status5 {get; set;}
    
    public string? Code1Note {get; set;}
    public MASTGStatus Code1Status {get; set;}
    public string? Code2Note {get; set;}
    public MASTGStatus Code2Status {get; set;}
    public string? Code2Note1 {get; set;}
    public string? Code2Note2 {get; set;}
    public MASTGStatus Code2Status1 {get; set;}
    public MASTGStatus Code2Status2 { get; set; }
    public string? Code3Note1 {get; set;}
    public string? Code3Note2 {get; set;}
    public string? Code3Note {get; set;}
    public MASTGStatus Code3Status {get; set;}
    public MASTGStatus Code3Status1 {get; set;}
    public MASTGStatus Code3Status2 {get; set;}
    public string? Code4Note {get; set;}
    public MASTGStatus Code4Status {get; set;}
    public string? Code4Note1 {get; set;}
    public string? Code4Note2 {get; set;}
    public string? Code4Note3 {get; set;}
    public string? Code4Note4 {get; set;}
    public string? Code4Note5 {get; set;}
    public string? Code4Note6 {get; set;}
    public string? Code4Note7 {get; set;}
    public string? Code4Note8 {get; set;}
    public string? Code4Note9 {get; set;}
    public string? Code4Note10 {get; set;}
    public MASTGStatus Code4Status1 {get; set;}
    public MASTGStatus Code4Status2 {get; set;}
    public MASTGStatus Code4Status3 {get; set;}
    public MASTGStatus Code4Status4 {get; set;}
    public MASTGStatus Code4Status5 {get; set;}
    public MASTGStatus Code4Status6 {get; set;}
    public MASTGStatus Code4Status7 {get; set;}
    public MASTGStatus Code4Status8 {get; set;}
    public MASTGStatus Code4Status9 {get; set;}
    public MASTGStatus Code4Status10 {get; set;}
    
    public string? Resilience1Note {get; set;}
    public MASTGStatus Resilience1Status {get; set;}
    public string? Resilience1Note1 {get; set;}
    public string? Resilience1Note2 {get; set;}
    public string? Resilience1Note3 {get; set;}
    public string? Resilience1Note4 {get; set;}
    public MASTGStatus Resilience1Status1 {get; set;}
    public MASTGStatus Resilience1Status2 {get; set;}
    public MASTGStatus Resilience1Status3 {get; set;}
    public MASTGStatus Resilience1Status4 {get; set;}
    public string? Resilience2Note {get; set;}
    public MASTGStatus Resilience2Status {get; set;}
    public string? Resilience2Note1 {get; set;}
    public string? Resilience2Note2 {get; set;}
    public string? Resilience2Note3 {get; set;}
    public string? Resilience2Note4 {get; set;}
    public string? Resilience2Note5 {get; set;}
    public MASTGStatus Resilience2Status1 {get; set;}
    public MASTGStatus Resilience2Status2 {get; set;}
    public MASTGStatus Resilience2Status3 {get; set;}
    public MASTGStatus Resilience2Status4 {get; set;}
    public MASTGStatus Resilience2Status5 {get; set;}
    public string? Resilience3Note {get; set;}
    public MASTGStatus Resilience3Status {get; set;}
    public string? Resilience3Note1 {get; set;}
    public string? Resilience3Note2 {get; set;}
    public string? Resilience3Note3 {get; set;}
    public string? Resilience3Note4 {get; set;}
    public string? Resilience3Note5 {get; set;}
    public string? Resilience3Note6 {get; set;}
    public MASTGStatus Resilience3Status1 {get; set;}
    public MASTGStatus Resilience3Status2 {get; set;}
    public MASTGStatus Resilience3Status3 {get; set;}
    public MASTGStatus Resilience3Status4 {get; set;}
    public MASTGStatus Resilience3Status5 {get; set;}
    public MASTGStatus Resilience3Status6 {get; set;}
    public string? Resilience4Note {get; set;}
    public MASTGStatus Resilience4Status {get; set;}
    public string? Resilience4Note1 {get; set;}
    public string? Resilience4Note2 {get; set;}
    public string? Resilience4Note3 {get; set;}
    public string? Resilience4Note4 {get; set;}
    public string? Resilience4Note5 {get; set;}
    public string? Resilience4Note6 {get; set;}
    public MASTGStatus Resilience4Status1 {get; set;}
    public MASTGStatus Resilience4Status2 {get; set;}
    public MASTGStatus Resilience4Status3 {get; set;}
    public MASTGStatus Resilience4Status4 {get; set;}
    public MASTGStatus Resilience4Status5 {get; set;}
    public MASTGStatus Resilience4Status6 {get; set;}
}