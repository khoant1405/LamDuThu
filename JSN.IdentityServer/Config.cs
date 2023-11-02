using IdentityServer4.Models;

namespace JSN.IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource>
    {
        // Tài nguyên OpenID được sử dụng để xác định danh tính của người dùng.
        new IdentityResources.OpenId(),

        // Tài nguyên Profile chứa thông tin hồ sơ của người dùng, chẳng hạn như tên và hình ảnh.
        new IdentityResources.Profile(),

        // Tạo một tài nguyên tùy chỉnh có tên 'role' và gắn liên kết với tên quyền 'role'.
        new()
        {
            Name = "role",
            UserClaims = new List<string> { "role" }
        }
    };

    public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
    {
        // Định nghĩa một phạm vi tài nguyên có tên 'JsnAPI.read'.
        new("JsnAPI.read"),

        // Định nghĩa một phạm vi tài nguyên có tên 'JsnAPI.write'.
        new("JsnAPI.write")
    };

    public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>
    {
        // Định nghĩa một tài nguyên API có tên 'JsnAPI'.
        new("JsnAPI")
        {
            // Xác định các phạm vi tài nguyên mà tài nguyên API này có thể sử dụng.
            Scopes = { "JsnAPI.read", "JsnAPI.write" },

            // Định nghĩa các mã bí mật được sử dụng để xác thực với tài nguyên API.
            ApiSecrets = { new Secret("ScopeSecret".Sha256()) },

            // Liệt kê các quyền mà người dùng có thể có khi truy cập tài nguyên API này.
            UserClaims = { "role" }
        }
    };

    public static IEnumerable<Client> Clients => new List<Client>
    {
        // Định nghĩa một khách hàng (client) có tên 'm2m.client' sử dụng phương thức Client Credentials.
        new()
        {
            ClientId = "m2m.client",
            ClientName = "Client Credentials Client",
            AllowedGrantTypes = GrantTypes.ClientCredentials,

            // Định nghĩa mã bí mật của khách hàng.
            ClientSecrets = { new Secret("ClientSecret1".Sha256()) },

            // Xác định các phạm vi mà khách hàng này được phép truy cập.
            AllowedScopes = { "JsnAPI.read", "JsnAPI.write" }
        },

        // Định nghĩa một khách hàng tương tác có tên 'interactive' sử dụng phương thức Authorization Code.
        new()
        {
            ClientId = "interactive",

            // Định nghĩa mã bí mật của khách hàng.
            ClientSecrets = { new Secret("ClientSecret1".Sha256()) },

            AllowedGrantTypes = GrantTypes.Code,

            // Xác định các URI để xử lý đăng nhập và đăng xuất.
            //RedirectUris = { "https://localhost:5444/signin-oidc" },
            //FrontChannelLogoutUri = "https://localhost:5444/signout-oidc",
            //PostLogoutRedirectUris = { "https://localhost:5444/signout-callback-oidc" },

            // Cho phép truy cập offline để lấy mã làm mới (refresh token).
            AllowOfflineAccess = true,

            // Xác định các phạm vi mà khách hàng này được phép truy cập.
            AllowedScopes = { "openid", "profile", "JsnAPI.read" },

            // Yêu cầu Proof Key for Code Exchange (PKCE) để bảo mật.
            RequirePkce = true,

            // Yêu cầu sự đồng tình của người dùng (consent) khi truy cập.
            RequireConsent = true,

            // Không cho phép PKCE dạng văn bản thô.
            AllowPlainTextPkce = false
        }
    };
}