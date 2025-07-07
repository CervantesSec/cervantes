namespace Cervantes.IFR.Ldap;

public interface ILdapService
{
    bool LdapEnabled();
    Task<bool> ValidateUserAsync(string username, string password);
    Task<LdapUser?> GetUserInfoAsync(string username);
    Task<List<string>> GetUserGroupsAsync(string userDN);
    Task<bool> TestConnectionAsync();
}