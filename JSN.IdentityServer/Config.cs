using IdentityServer4.Models;

namespace JSN.IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new()
        {
            Name = "role",
            UserClaims = new List<string> { "role" }
        }
    };

    public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
    {
        new("JsnAPI.read"),
        new("JsnAPI.write")
    };

    public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>
    {
        new("JsnAPI")
        {
            Scopes = { "JsnAPI.read", "JsnAPI.write" },
            ApiSecrets = { new Secret("ScopeSecret".Sha256()) },
            UserClaims = { "role" }
        }
    };

    public static IEnumerable<Client> Clients => new List<Client>
    {
        new()
        {
            ClientId = "m2m.client",
            ClientName = "Client Credentials Client",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("ClientSecret1".Sha256()) },
            AllowedScopes = { "JsnAPI.read", "JsnAPI.write" }
        },
        new()
        {
            ClientId = "interactive",
            ClientSecrets = { new Secret("ClientSecret1".Sha256()) },
            AllowedGrantTypes = GrantTypes.Code,
            RedirectUris = { "https://localhost:5444/signin-oidc" },
            FrontChannelLogoutUri = "https://localhost:5444/signout-oidc",
            PostLogoutRedirectUris = { "https://localhost:5444/signout-callback-oidc" },
            AllowOfflineAccess = true,
            AllowedScopes = { "openid", "profile", "JsnAPI.read" },
            RequirePkce = true,
            RequireConsent = true,
            AllowPlainTextPkce = false
        }
    };
}