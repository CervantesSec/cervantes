namespace Cervantes.IFR.Ldap;

public class LdapConfiguration : ILdapConfiguration
{
    public bool Enabled { get; set; } = false;
    public string Server { get; set; } = string.Empty;
    public int Port { get; set; } = 389;
    public bool UseSSL { get; set; } = false;
    public string BaseDN { get; set; } = string.Empty;
    public string UserDN { get; set; } = string.Empty;
    public string UserSearchFilter { get; set; } = "(&(objectClass=person)(sAMAccountName={0}))";
    public string GroupSearchBase { get; set; } = string.Empty;
    public string GroupSearchFilter { get; set; } = "(&(objectClass=group)(member={0}))";
    public string AdminUsername { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
    public string UserAttribute { get; set; } = "sAMAccountName";
    public string EmailAttribute { get; set; } = "mail";
    public string DisplayNameAttribute { get; set; } = "displayName";
    public string FirstNameAttribute { get; set; } = "givenName";
    public string LastNameAttribute { get; set; } = "sn";
    public string DefaultRole { get; set; } = "User";
    public Dictionary<string, string> GroupRoleMapping { get; set; } = new();
}