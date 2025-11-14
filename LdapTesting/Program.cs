using Novell.Directory.Ldap;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LdapTesting;

class Program
{
    static async Task Main(string[] args)
    {
        string ldapHost = "108.136.163.238";        // Your LDAP server hostname or IP
        int ldapPort = 389;                      // LDAP default port (use 636 for LDAPS)
        string loginDN = "CN=Administrator,CN=Users,DC=masba,DC=local";  // Bind DN (adjust if needed)
        string password = ".BSwtVlDxI*?;rhX6qKWouprzNt;TtyK";    // Bind password
        string userId = "erwin.barus";           // The sAMAccountName or uid you want to search

        foreach (var role in await GetUserGroups(ldapHost, loginDN, password, userId))
        {
            Console.WriteLine(role);
        }
    }


    public static async Task<List<string>> GetUserGroups(
        string ldapHost,
        string loginDN,
        string password,
        string userId)
    {
        List<string> groups = [];

        try
        {
            using var ldapConn = new LdapConnection();

            // Connect to the LDAP server and bind (authenticate)
            await ldapConn.ConnectAsync(ldapHost, 389);
            await ldapConn.BindAsync(loginDN, password);

            // Base DN for your domain
            string searchBase = GetSearchBaseFromLoginDN(loginDN);

            // LDAP filter to find user by sAMAccountName or uid
            string searchFilter = $"(&(objectClass=user)(|(sAMAccountName={userId})(uid={userId})))";

            // Attributes to retrieve
            string[] attributesToReturn = { "memberOf" };

            LdapSearchQueue searchQueue = await ldapConn.SearchAsync(
                searchBase,
                LdapConnection.ScopeSub,
                searchFilter,
                attributesToReturn,
                false,
                null as LdapSearchQueue
            );

            LdapMessage message;
            while ((message = searchQueue.GetResponse()) != null)
            {
                if (message is LdapSearchResult searchResult)
                {
                    LdapEntry entry = searchResult.Entry;
                    LdapAttribute memberAttribute = entry.GetAttributeSet().GetAttribute("memberOf");

                    if (memberAttribute != null)
                    {
                        foreach (string memberDn in memberAttribute.StringValueArray)
                        {
                            // Extract the CN (Common Name) from the member's Distinguished Name
                            Match match = Regex.Match(
                                memberDn,
                                @"^CN=([^,]*BA[^,]*),",
                                RegexOptions.IgnoreCase,
                                TimeSpan.FromMilliseconds(500)
                            );
                            if (match.Success)
                            {
                                groups.Add(match.Groups[1].Value);
                            }
                            else
                            {
                                groups.Add(memberDn); // Add full DN if CN extraction fails
                            }
                        }
                    }

                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return groups;
    }

    public static string GetSearchBaseFromLoginDN(string loginDN)
    {
        // Extract DC components and build search base
        var dcMatches = Regex.Matches(loginDN, @"DC=([^,]+)", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(500));
        if (dcMatches.Count == 0) return string.Empty;

        var searchBase = string.Join(",", dcMatches.Select(m => $"DC={m.Groups[1].Value}"));
        return searchBase;
    }
}
