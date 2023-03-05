using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE;

public class MASTG
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
    public string ArchNote01 {get; set;}
public string ArchNote02 {get; set;}
public string ArchNote03 {get; set;}
public string ArchNote04 {get; set;}
public string ArchNote05 {get; set;}
public string ArchNote06 {get; set;}
public string ArchNote07 {get; set;}
public string ArchNote08 {get; set;}
public string ArchNote09 {get; set;}
public string ArchNote10 {get; set;}
public string ArchNote11 {get; set;}
public string ArchNote12 {get; set;}
public string StorageNote01 {get; set;}
public string StorageNote02 {get; set;}
public string StorageNote03 {get; set;}
public string StorageNote04 {get; set;}
public string StorageNote05 {get; set;}
public string StorageNote06 {get; set;}
public string StorageNote07 {get; set;}
public string StorageNote08 {get; set;}
public string StorageNote09 {get; set;}
public string StorageNote10 {get; set;}
public string StorageNote11 {get; set;}
public string StorageNote12 {get; set;}
public string StorageNote13 {get; set;}
public string StorageNote14 {get; set;}
public string StorageNote15 {get; set;}
public string CryptoNote01 {get; set;}
public string CryptoNote02 {get; set;}
public string CryptoNote03 {get; set;}
public string CryptoNote04 {get; set;}
public string CryptoNote05 {get; set;}
public string CryptoNote06 {get; set;}
public string AuthNote01 {get; set;}
public string AuthNote02 {get; set;}
public string AuthNote03 {get; set;}
public string AuthNote04 {get; set;}
public string AuthNote05 {get; set;}
public string AuthNote06 {get; set;}
public string AuthNote07 {get; set;}
public string AuthNote08 {get; set;}
public string AuthNote09 {get; set;}
public string AuthNote10 {get; set;}
public string AuthNote11 {get; set;}
public string AuthNote12 {get; set;}
public string NetworkNote01 {get; set;}
public string NetworkNote02 {get; set;}
public string NetworkNote03 {get; set;}
public string NetworkNote04 {get; set;}
public string NetworkNote05 {get; set;}
public string NetworkNote06 {get; set;}
public string NetworkNote07 {get; set;}
public string NetworkNote08 {get; set;}
public string NetworkNote09 {get; set;}
public string NetworkNote10 {get; set;}
public string NetworkNote11 {get; set;}
public string NetworkNote12 {get; set;}
public string NetworkNote13 {get; set;}
public string NetworkNote14 {get; set;}
public string NetworkNote15 {get; set;}
public string NetworkNote16 {get; set;}
public string NetworkNote17 {get; set;}
public string CodeNote01 {get; set;}
public string CodeNote02 {get; set;}
public string CodeNote03 {get; set;}
public string CodeNote04 {get; set;}
public string CodeNote05 {get; set;}
public string CodeNote06 {get; set;}
public string CodeNote07 {get; set;}
public string CodeNote08 {get; set;}
public string CodeNote09 {get; set;}
public string ResilienceNote01 {get; set;}
public string ResilienceNote02 {get; set;}
public string ResilienceNote03 {get; set;}
public string ResilienceNote04 {get; set;}
public string ResilienceNote05 {get; set;}
public string ResilienceNote06 {get; set;}
public string ResilienceNote07 {get; set;}
public string ResilienceNote08 {get; set;}
public string ResilienceNote09 {get; set;}
public string ResilienceNote10 {get; set;}
public string ResilienceNote11 {get; set;}
public string ResilienceNote12 {get; set;}
public string ResilienceNote13 {get; set;}
public string PlatformNote01 {get; set;}
public string PlatformNote02 {get; set;}
public string PlatformNote03 {get; set;}
public string PlatformNote04 {get; set;}
public string PlatformNote05 {get; set;}
public string PlatformNote06 {get; set;}
public string PlatformNote07 {get; set;}
public string PlatformNote08 {get; set;}
public string PlatformNote09 {get; set;}
public string PlatformNote10 {get; set;}
public string PlatformNote11 {get; set;}
public MASTGStatus ArchStatus01 {get; set;}
public MASTGStatus ArchStatus02 {get; set;}
public MASTGStatus ArchStatus03 {get; set;}
public MASTGStatus ArchStatus04 {get; set;}
public MASTGStatus ArchStatus05 {get; set;}
public MASTGStatus ArchStatus06 {get; set;}
public MASTGStatus ArchStatus07 {get; set;}
public MASTGStatus ArchStatus08 {get; set;}
public MASTGStatus ArchStatus09 {get; set;}
public MASTGStatus ArchStatus10 {get; set;}
public MASTGStatus ArchStatus11 {get; set;}
public MASTGStatus ArchStatus12 {get; set;}
public MASTGStatus StorageStatus01 {get; set;}
public MASTGStatus StorageStatus02 {get; set;}
public MASTGStatus StorageStatus03 {get; set;}
public MASTGStatus StorageStatus04 {get; set;}
public MASTGStatus StorageStatus05 {get; set;}
public MASTGStatus StorageStatus06 {get; set;}
public MASTGStatus StorageStatus07 {get; set;}
public MASTGStatus StorageStatus08 {get; set;}
public MASTGStatus StorageStatus09 {get; set;}
public MASTGStatus StorageStatus10 {get; set;}
public MASTGStatus StorageStatus11 {get; set;}
public MASTGStatus StorageStatus12 {get; set;}
public MASTGStatus StorageStatus13 {get; set;}
public MASTGStatus StorageStatus14 {get; set;}
public MASTGStatus StorageStatus15 {get; set;}
public MASTGStatus CryptoStatus01 {get; set;}
public MASTGStatus CryptoStatus02 {get; set;}
public MASTGStatus CryptoStatus03 {get; set;}
public MASTGStatus CryptoStatus04 {get; set;}
public MASTGStatus CryptoStatus05 {get; set;}
public MASTGStatus CryptoStatus06 {get; set;}
public MASTGStatus AuthStatus01 {get; set;}
public MASTGStatus AuthStatus02 {get; set;}
public MASTGStatus AuthStatus03 {get; set;}
public MASTGStatus AuthStatus04 {get; set;}
public MASTGStatus AuthStatus05 {get; set;}
public MASTGStatus AuthStatus06 {get; set;}
public MASTGStatus AuthStatus07 {get; set;}
public MASTGStatus AuthStatus08 {get; set;}
public MASTGStatus AuthStatus09 {get; set;}
public MASTGStatus AuthStatus10 {get; set;}
public MASTGStatus AuthStatus11 {get; set;}
public MASTGStatus AuthStatus12 {get; set;}
public MASTGStatus NetworkStatus01 {get; set;}
public MASTGStatus NetworkStatus02 {get; set;}
public MASTGStatus NetworkStatus03 {get; set;}
public MASTGStatus NetworkStatus04 {get; set;}
public MASTGStatus NetworkStatus05 {get; set;}
public MASTGStatus NetworkStatus06 {get; set;}
public MASTGStatus NetworkStatus07 {get; set;}
public MASTGStatus NetworkStatus08 {get; set;}
public MASTGStatus NetworkStatus09 {get; set;}
public MASTGStatus NetworkStatus10 {get; set;}
public MASTGStatus NetworkStatus11 {get; set;}
public MASTGStatus NetworkStatus12 {get; set;}
public MASTGStatus NetworkStatus13 {get; set;}
public MASTGStatus NetworkStatus14 {get; set;}
public MASTGStatus NetworkStatus15 {get; set;}
public MASTGStatus NetworkStatus16 {get; set;}
public MASTGStatus NetworkStatus17 {get; set;}
public MASTGStatus CodeStatus01 {get; set;}
public MASTGStatus CodeStatus02 {get; set;}
public MASTGStatus CodeStatus03 {get; set;}
public MASTGStatus CodeStatus04 {get; set;}
public MASTGStatus CodeStatus05 {get; set;}
public MASTGStatus CodeStatus06 {get; set;}
public MASTGStatus CodeStatus07 {get; set;}
public MASTGStatus CodeStatus08 {get; set;}
public MASTGStatus CodeStatus09 {get; set;}
public MASTGStatus ResilienceStatus01 {get; set;}
public MASTGStatus ResilienceStatus02 {get; set;}
public MASTGStatus ResilienceStatus03 {get; set;}
public MASTGStatus ResilienceStatus04 {get; set;}
public MASTGStatus ResilienceStatus05 {get; set;}
public MASTGStatus ResilienceStatus06 {get; set;}
public MASTGStatus ResilienceStatus07 {get; set;}
public MASTGStatus ResilienceStatus08 {get; set;}
public MASTGStatus ResilienceStatus09 {get; set;}
public MASTGStatus ResilienceStatus10 {get; set;}
public MASTGStatus ResilienceStatus11 {get; set;}
public MASTGStatus ResilienceStatus12 {get; set;}
public MASTGStatus ResilienceStatus13 {get; set;}
public MASTGStatus PlatformStatus01 {get; set;}
public MASTGStatus PlatformStatus02 {get; set;}
public MASTGStatus PlatformStatus03 {get; set;}
public MASTGStatus PlatformStatus04 {get; set;}
public MASTGStatus PlatformStatus05 {get; set;}
public MASTGStatus PlatformStatus06 {get; set;}
public MASTGStatus PlatformStatus07 {get; set;}
public MASTGStatus PlatformStatus08 {get; set;}
public MASTGStatus PlatformStatus09 {get; set;}
public MASTGStatus PlatformStatus10 {get; set;}
public MASTGStatus PlatformStatus11 {get; set;}
}