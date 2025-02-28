using System.DirectoryServices.AccountManagement;
using System.Net;
using System.Security.Principal;

namespace WinAuth.Client.Tests;

public class WinAuthHttpTest
{
    [Fact]
    public async Task EnsureTheClientSendsCredentialsTest()
    {
        var handler = new HttpClientHandler { UseDefaultCredentials = true };
        using var httpClient = new HttpClient(handler);

        var response = await httpClient.GetAsync("http://localhost:5278");
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);
        //rerun the test to see the user name
    }


    [Fact]
    public async Task ClientSendsTestUserTest()
    {
        //TODO: create a test user and Change the user name and password 
        var handler = new HttpClientHandler
        {
            Credentials = new NetworkCredential("test", "test") //,"domain"
        };
        using var httpClient = new HttpClient(handler);

        var response = await httpClient.GetAsync("http://localhost:5278");
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);
        //return the test to see test user name
    }

    [Fact]
    public async Task ClientSendsNoUserTest()
    {
        var handler = new HttpClientHandler { UseDefaultCredentials = false };
        using var httpClient = new HttpClient(handler);

        var response = await httpClient.GetAsync("http://localhost:5278");
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);
        //return the test to see no user name
    }

    [Fact]
    public async Task AddTheClientSendsCredentialsInHeaderTest()
    {
        var handler = new HttpClientHandler { UseDefaultCredentials = true };
        using var httpClient = new HttpClient(handler);
        var identity = WindowsIdentity.GetCurrent();
        if (identity != null)
        {
            httpClient.DefaultRequestHeaders.Add("X-User-Name", identity.Name);
        }
        var response = await httpClient.GetAsync("http://localhost:5278");
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);
    }

    [Fact]
    public async Task GetCurrentUserInfo()
    {
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);

        Console.WriteLine($"User: {identity.Name}");
        if (principal.IsInRole("Administrators"))
        {
            Console.WriteLine("User is an Administrator.");
        }

        Console.WriteLine("Groups:");

        foreach (var group in identity.Groups)
        {
            try
            {
                var groupName = group.Translate(typeof(NTAccount)).ToString();
                Console.WriteLine($" - {groupName}");
            }
            catch
            {
                Console.WriteLine($" - {group.Value} (Could not resolve name)");
            }
        }
    }
    [Fact]
    public async Task GetCurrentUserInfo2()
    {
        using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
        using (UserPrincipal user = UserPrincipal.FindByIdentity(context, Environment.UserName))
        {
            if (user != null)
            {
                Console.WriteLine($"User: {user.SamAccountName}");
                Console.WriteLine("Groups:");

                foreach (var group in user.GetAuthorizationGroups())
                {
                    Console.WriteLine($" - {group.Name}");
                }
            }
        }
    }
}
