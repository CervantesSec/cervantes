using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE;

public class WSTG
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [ForeignKey("TargetId")]
    public virtual Target Target { get; set; }

    /// <summary>
    /// Id target
    /// </summary>
    public Guid TargetId { get; set; }
    /// <summary>
    /// User who created 
    /// </summary>
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public string UserId { get; set; }
    
    /// <summary>
    /// Project Associated
    /// </summary>
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    public Guid ProjectId { get; set; }
    
    /// <summary>
    ///  Created Date
    /// </summary>
    public DateTime CreatedDate { get; set; }
    
    public string Info01Note {get; set;}
public string Info02Note {get; set;}
public string Info03Note {get; set;}
public string Info04Note {get; set;}
public string Info05Note {get; set;}
public string Info06Note {get; set;}
public string Info07Note {get; set;}
public string Info08Note {get; set;}
public string Info09Note {get; set;}
public string Info10Note {get; set;}
public string Conf01Note {get; set;}
public string Conf02Note {get; set;}
public string Conf03Note {get; set;}
public string Conf04Note {get; set;}
public string Conf05Note {get; set;}
public string Conf06Note {get; set;}
public string Conf07Note {get; set;}
public string Conf08Note {get; set;}
public string Conf09Note {get; set;}
public string Conf10Note {get; set;}
public string Conf11Note {get; set;}
public string Idnt1Note {get; set;}
public string Idnt2Note {get; set;}
public string Idnt3Note {get; set;}
public string Idnt4Note {get; set;}
public string Idnt5Note {get; set;}
public string Athn01Note {get; set;}
public string Athn02Note {get; set;}
public string Athn03Note {get; set;}
public string Athn04Note {get; set;}
public string Athn05Note {get; set;}
public string Athn06Note {get; set;}
public string Athn07Note {get; set;}
public string Athn08Note {get; set;}
public string Athn09Note {get; set;}
public string Athn10Note {get; set;}
public string Athz01Note {get; set;}
public string Athz02Note {get; set;}
public string Athz03Note {get; set;}
public string Athz04Note {get; set;}
public string Sess01Note {get; set;}
public string Sess02Note {get; set;}
public string Sess03Note {get; set;}
public string Sess04Note {get; set;}
public string Sess05Note {get; set;}
public string Sess06Note {get; set;}
public string Sess07Note {get; set;}
public string Sess08Note {get; set;}
public string Sess09Note {get; set;}
public string Inpv01Note {get; set;}
public string Inpv02Note {get; set;}
public string Inpv03Note {get; set;}
public string Inpv04Note {get; set;}
public string Inpv05Note {get; set;}
public string Inpv06Note {get; set;}
public string Inpv07Note {get; set;}
public string Inpv08Note {get; set;}
public string Inpv09Note {get; set;}
public string Inpv10Note {get; set;}
public string Inpv11Note {get; set;}
public string Inpv12Note {get; set;}
public string Inpv13Note {get; set;}
public string Inpv14Note {get; set;}
public string Inpv15Note {get; set;}
public string Inpv16Note {get; set;}
public string Inpv17Note {get; set;}
public string Inpv18Note {get; set;}
public string Inpv19Note {get; set;}
public string Errh01Note {get; set;}
public string Errh02Note {get; set;}
public string Cryp01Note {get; set;}
public string Cryp02Note {get; set;}
public string Cryp03Note {get; set;}
public string Cryp04Note {get; set;}
public string Busl01Note {get; set;}
public string Busl02Note {get; set;}
public string Busl03Note {get; set;}
public string Busl04Note {get; set;}
public string Busl05Note {get; set;}
public string Busl06Note {get; set;}
public string Busl07Note {get; set;}
public string Busl08Note {get; set;}
public string Busl09Note {get; set;}
public string Clnt01Note {get; set;}
public string Clnt02Note {get; set;}
public string Clnt03Note {get; set;}
public string Clnt04Note {get; set;}
public string Clnt05Note {get; set;}
public string Clnt06Note {get; set;}
public string Clnt07Note {get; set;}
public string Clnt08Note {get; set;}
public string Clnt09Note {get; set;}
public string Clnt10Note {get; set;}
public string Clnt11Note {get; set;}
public string Clnt12Note {get; set;}
public string Clnt13Note {get; set;}
public string Apit01Note {get; set;}
public WSTGStatus Info01Status {get; set;}
public WSTGStatus Info02Status {get; set;}
public WSTGStatus Info03Status {get; set;}
public WSTGStatus Info04Status {get; set;}
public WSTGStatus Info05Status {get; set;}
public WSTGStatus Info06Status {get; set;}
public WSTGStatus Info07Status {get; set;}
public WSTGStatus Info08Status {get; set;}
public WSTGStatus Info09Status {get; set;}
public WSTGStatus Info10Status {get; set;}
public WSTGStatus Conf01Status {get; set;}
public WSTGStatus Conf02Status {get; set;}
public WSTGStatus Conf03Status {get; set;}
public WSTGStatus Conf04Status {get; set;}
public WSTGStatus Conf05Status {get; set;}
public WSTGStatus Conf06Status {get; set;}
public WSTGStatus Conf07Status {get; set;}
public WSTGStatus Conf08Status {get; set;}
public WSTGStatus Conf09Status {get; set;}
public WSTGStatus Conf10Status {get; set;}
public WSTGStatus Conf11Status {get; set;}
public WSTGStatus Idnt1Status {get; set;}
public WSTGStatus Idnt2Status {get; set;}
public WSTGStatus Idnt3Status {get; set;}
public WSTGStatus Idnt4Status {get; set;}
public WSTGStatus Idnt5Status {get; set;}
public WSTGStatus Athn01Status {get; set;}
public WSTGStatus Athn02Status {get; set;}
public WSTGStatus Athn03Status {get; set;}
public WSTGStatus Athn04Status {get; set;}
public WSTGStatus Athn05Status {get; set;}
public WSTGStatus Athn06Status {get; set;}
public WSTGStatus Athn07Status {get; set;}
public WSTGStatus Athn08Status {get; set;}
public WSTGStatus Athn09Status {get; set;}
public WSTGStatus Athn10Status {get; set;}
public WSTGStatus Athz01Status {get; set;}
public WSTGStatus Athz02Status {get; set;}
public WSTGStatus Athz03Status {get; set;}
public WSTGStatus Athz04Status {get; set;}
public WSTGStatus Sess01Status {get; set;}
public WSTGStatus Sess02Status {get; set;}
public WSTGStatus Sess03Status {get; set;}
public WSTGStatus Sess04Status {get; set;}
public WSTGStatus Sess05Status {get; set;}
public WSTGStatus Sess06Status {get; set;}
public WSTGStatus Sess07Status {get; set;}
public WSTGStatus Sess08Status {get; set;}
public WSTGStatus Sess09Status {get; set;}
public WSTGStatus Inpv01Status {get; set;}
public WSTGStatus Inpv02Status {get; set;}
public WSTGStatus Inpv03Status {get; set;}
public WSTGStatus Inpv04Status {get; set;}
public WSTGStatus Inpv05Status {get; set;}
public WSTGStatus Inpv06Status {get; set;}
public WSTGStatus Inpv07Status {get; set;}
public WSTGStatus Inpv08Status {get; set;}
public WSTGStatus Inpv09Status {get; set;}
public WSTGStatus Inpv10Status {get; set;}
public WSTGStatus Inpv11Status {get; set;}
public WSTGStatus Inpv12Status {get; set;}
public WSTGStatus Inpv13Status {get; set;}
public WSTGStatus Inpv14Status {get; set;}
public WSTGStatus Inpv15Status {get; set;}
public WSTGStatus Inpv16Status {get; set;}
public WSTGStatus Inpv17Status {get; set;}
public WSTGStatus Inpv18Status {get; set;}
public WSTGStatus Inpv19Status {get; set;}
public WSTGStatus Errh01Status {get; set;}
public WSTGStatus Errh02Status {get; set;}
public WSTGStatus Cryp01Status {get; set;}
public WSTGStatus Cryp02Status {get; set;}
public WSTGStatus Cryp03Status {get; set;}
public WSTGStatus Cryp04Status {get; set;}
public WSTGStatus Busl01Status {get; set;}
public WSTGStatus Busl02Status {get; set;}
public WSTGStatus Busl03Status {get; set;}
public WSTGStatus Busl04Status {get; set;}
public WSTGStatus Busl05Status {get; set;}
public WSTGStatus Busl06Status {get; set;}
public WSTGStatus Busl07Status {get; set;}
public WSTGStatus Busl08Status {get; set;}
public WSTGStatus Busl09Status {get; set;}
public WSTGStatus Clnt01Status {get; set;}
public WSTGStatus Clnt02Status {get; set;}
public WSTGStatus Clnt03Status {get; set;}
public WSTGStatus Clnt04Status {get; set;}
public WSTGStatus Clnt05Status {get; set;}
public WSTGStatus Clnt06Status {get; set;}
public WSTGStatus Clnt07Status {get; set;}
public WSTGStatus Clnt08Status {get; set;}
public WSTGStatus Clnt09Status {get; set;}
public WSTGStatus Clnt10Status {get; set;}
public WSTGStatus Clnt11Status {get; set;}
public WSTGStatus Clnt12Status {get; set;}
public WSTGStatus Clnt13Status {get; set;}
public WSTGStatus Apit01Status {get; set;}
}