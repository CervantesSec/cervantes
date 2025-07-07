namespace Cervantes.IFR.Ldap;

public interface ILdapConfiguration
{
    bool Enabled { get; set; }
    string Server { get; set; }
    int Port { get; set; }
    bool UseSSL { get; set; }
    string BaseDN { get; set; }
    string UserDN { get; set; }
    string UserSearchFilter { get; set; }
    string GroupSearchBase { get; set; }
    string GroupSearchFilter { get; set; }
    string AdminUsername { get; set; }
    string AdminPassword { get; set; }
    string UserAttribute { get; set; }
    string EmailAttribute { get; set; }
    string DisplayNameAttribute { get; set; }
    string FirstNameAttribute { get; set; }
    string LastNameAttribute { get; set; }
    string DefaultRole { get; set; }
    Dictionary<string, string> GroupRoleMapping { get; set; }
}