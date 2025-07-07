using System.DirectoryServices.Protocols;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Cervantes.IFR.Ldap;

public class LdapService : ILdapService
{
    private readonly ILdapConfiguration _config;
    private readonly ILogger<LdapService> _logger;

    public LdapService(ILdapConfiguration config, ILogger<LdapService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public bool LdapEnabled()
    {
        return _config.Enabled;
    }

    public async Task<bool> ValidateUserAsync(string username, string password)
    {
        if (!_config.Enabled || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return false;
        }

        try
        {
            using var connection = CreateConnection();
            
            // First, bind with admin credentials to search for the user
            connection.Credential = new NetworkCredential(_config.AdminUsername, _config.AdminPassword);
            connection.Bind();

            // Search for the user
            var userDN = await FindUserDNAsync(connection, username);
            if (string.IsNullOrEmpty(userDN))
            {
                _logger.LogWarning("User {Username} not found in LDAP", username);
                return false;
            }

            // Try to bind with user credentials
            using var userConnection = CreateConnection();
            userConnection.Credential = new NetworkCredential(userDN, password);
            userConnection.Bind();

            _logger.LogInformation("LDAP authentication successful for user {Username}", username);
            return true;
        }
        catch (DirectoryOperationException ex)
        {
            _logger.LogWarning("LDAP authentication failed for user {Username}: {Error}", username, ex.Message);
            return false;
        }
        catch (LdapException ex)
        {
            _logger.LogError(ex, "LDAP connection error during authentication for user {Username}", username);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during LDAP authentication for user {Username}", username);
            return false;
        }
    }

    public async Task<LdapUser?> GetUserInfoAsync(string username)
    {
        if (!_config.Enabled || string.IsNullOrEmpty(username))
        {
            return null;
        }

        try
        {
            using var connection = CreateConnection();
            connection.Credential = new NetworkCredential(_config.AdminUsername, _config.AdminPassword);
            connection.Bind();

            var searchFilter = string.Format(_config.UserSearchFilter, username);
            var searchRequest = new SearchRequest(
                _config.UserDN,
                searchFilter,
                SearchScope.Subtree,
                _config.UserAttribute,
                _config.EmailAttribute,
                _config.DisplayNameAttribute,
                _config.FirstNameAttribute,
                _config.LastNameAttribute
            );

            var searchResponse = (SearchResponse)connection.SendRequest(searchRequest);

            if (searchResponse.Entries.Count == 0)
            {
                _logger.LogWarning("User {Username} not found in LDAP directory", username);
                return null;
            }

            var entry = searchResponse.Entries[0];
            var ldapUser = new LdapUser
            {
                Username = GetAttributeValue(entry, _config.UserAttribute) ?? username,
                Email = GetAttributeValue(entry, _config.EmailAttribute) ?? string.Empty,
                DisplayName = GetAttributeValue(entry, _config.DisplayNameAttribute) ?? string.Empty,
                FirstName = GetAttributeValue(entry, _config.FirstNameAttribute) ?? string.Empty,
                LastName = GetAttributeValue(entry, _config.LastNameAttribute) ?? string.Empty,
                DistinguishedName = entry.DistinguishedName
            };

            // Get user groups
            ldapUser.Groups = await GetUserGroupsAsync(entry.DistinguishedName);

            // Store all attributes
            foreach (string attributeName in entry.Attributes.AttributeNames)
            {
                var value = GetAttributeValue(entry, attributeName);
                if (!string.IsNullOrEmpty(value))
                {
                    ldapUser.Attributes[attributeName] = value;
                }
            }

            _logger.LogInformation("Retrieved LDAP user info for {Username}", username);
            return ldapUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving LDAP user info for {Username}", username);
            return null;
        }
    }

    public async Task<List<string>> GetUserGroupsAsync(string userDN)
    {
        var groups = new List<string>();

        if (!_config.Enabled || string.IsNullOrEmpty(userDN) || string.IsNullOrEmpty(_config.GroupSearchBase))
        {
            return groups;
        }

        try
        {
            using var connection = CreateConnection();
            connection.Credential = new NetworkCredential(_config.AdminUsername, _config.AdminPassword);
            connection.Bind();

            var searchFilter = string.Format(_config.GroupSearchFilter, userDN);
            var searchRequest = new SearchRequest(
                _config.GroupSearchBase,
                searchFilter,
                SearchScope.Subtree,
                "cn"
            );

            var searchResponse = (SearchResponse)connection.SendRequest(searchRequest);

            foreach (SearchResultEntry entry in searchResponse.Entries)
            {
                var groupName = GetAttributeValue(entry, "cn");
                if (!string.IsNullOrEmpty(groupName))
                {
                    groups.Add(groupName);
                }
            }

            _logger.LogInformation("Retrieved {GroupCount} groups for user DN {UserDN}", groups.Count, userDN);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving LDAP groups for user DN {UserDN}", userDN);
        }

        return groups;
    }

    public async Task<bool> TestConnectionAsync()
    {
        if (!_config.Enabled)
        {
            return false;
        }

        try
        {
            using var connection = CreateConnection();
            connection.Credential = new NetworkCredential(_config.AdminUsername, _config.AdminPassword);
            connection.Bind();

            _logger.LogInformation("LDAP connection test successful");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LDAP connection test failed");
            return false;
        }
    }

    private LdapConnection CreateConnection()
    {
        var identifier = new LdapDirectoryIdentifier(_config.Server, _config.Port);
        var connection = new LdapConnection(identifier);

        if (_config.UseSSL)
        {
            connection.SessionOptions.SecureSocketLayer = true;
        }

        connection.SessionOptions.ProtocolVersion = 3;
        connection.Timeout = TimeSpan.FromSeconds(30);
        connection.AuthType = AuthType.Basic;

        return connection;
    }

    private Task<string?> FindUserDNAsync(LdapConnection connection, string username)
    {
        try
        {
            var searchFilter = string.Format(_config.UserSearchFilter, username);
            _logger.LogInformation("Searching for user {Username} with filter {Filter} in base {Base}", username, searchFilter, _config.UserDN);
            
            var searchRequest = new SearchRequest(
                _config.UserDN,
                searchFilter,
                SearchScope.Subtree,
                "distinguishedName"
            );

            var searchResponse = (SearchResponse)connection.SendRequest(searchRequest);
            _logger.LogInformation("LDAP search returned {Count} entries for user {Username}", searchResponse.Entries.Count, username);

            if (searchResponse.Entries.Count > 0)
            {
                var userDN = searchResponse.Entries[0].DistinguishedName;
                _logger.LogInformation("Found user DN: {UserDN}", userDN);
                return Task.FromResult<string?>(userDN);
            }
            else
            {
                _logger.LogWarning("No entries found for user {Username}", username);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding user DN for {Username}", username);
        }

        return Task.FromResult<string?>(null);
    }

    private static string? GetAttributeValue(SearchResultEntry entry, string attributeName)
    {
        if (entry.Attributes.Contains(attributeName))
        {
            var attribute = entry.Attributes[attributeName];
            if (attribute.Count > 0)
            {
                if (attribute[0] is byte[] bytes)
                {
                    return Encoding.UTF8.GetString(bytes);
                }
                return attribute[0]?.ToString();
            }
        }
        return null;
    }
}