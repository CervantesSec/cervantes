using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace Cervantes.CORE.Entities;

public class AuditEntry
{
    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }
    
    public EntityEntry Entry { get; }
    public string UserId { get; set; }
    public string TableName { get; set; }
    public string IpAddress { get; set; }
    public string Browser { get; set; }
    public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
    public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
    public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
    public AuditType AuditType { get; set; }
    public List<string> ChangedColumns { get; } = new List<string>();
    
    public Audit ToAudit()
    {
        var audit = new Audit();
        audit.UserId = UserId;
        audit.Type = AuditType.ToString();
        audit.TableName = TableName;
        audit.IpAddress = IpAddress;
        audit.Browser = Browser;
        audit.DateTime = DateTime.Now.ToUniversalTime();
        audit.PrimaryKey = JsonConvert.SerializeObject(KeyValues);
        var keyOldCopy = new Dictionary<string, object>(OldValues);
        keyOldCopy.Remove("PasswordHash");
        audit.OldValues = OldValues.Count == 0 ? "null" : JsonConvert.SerializeObject(keyOldCopy);
        var keyNewCopy = new Dictionary<string, object>(NewValues);
        keyNewCopy.Remove("PasswordHash");
        audit.NewValues = NewValues.Count == 0 ? "null" : JsonConvert.SerializeObject(keyNewCopy);
        audit.AffectedColumns = ChangedColumns.Count == 0 ? "null" : JsonConvert.SerializeObject(ChangedColumns);
        return audit;
    }
}