using InsuranceApp.IntegrationTests.Authentication;

namespace InsuranceApp.IntegrationTests.Extensions;

public static class HttpClientAuthExtensions
{
    public static void AuthenticateAs(this HttpClient client, params string[] roles)
    {
        RemoveDefaultHeaders(client);

        AddRolesHeader(client, roles);
    }

    public static void AuthenticateAs(this HttpClient client, string userId, string userName, params string[] roles)
    {
        RemoveDefaultHeaders(client);

        client.DefaultRequestHeaders.Add(TestAuthHeaders.UserId, userId);
        client.DefaultRequestHeaders.Add(TestAuthHeaders.UserName, userName);

        AddRolesHeader(client, roles);   
    }

    public static void AuthenticateAsAnonymous(this HttpClient client)
    {
        RemoveDefaultHeaders(client);

        client.DefaultRequestHeaders.Add(TestAuthHeaders.Anonymous, "true");
    }

    private static void AddRolesHeader(HttpClient client, params string[] roles)
    {
        if (roles.Length > 0)
        {
            client.DefaultRequestHeaders.Add(TestAuthHeaders.Roles, string.Join(",", roles));
        }
    }

    private static void RemoveDefaultHeaders(HttpClient client)
    {
        client.DefaultRequestHeaders.Remove(TestAuthHeaders.Anonymous);
        client.DefaultRequestHeaders.Remove(TestAuthHeaders.Roles);
        client.DefaultRequestHeaders.Remove(TestAuthHeaders.UserName);
        client.DefaultRequestHeaders.Remove(TestAuthHeaders.UserId);
    }
}
