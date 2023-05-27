using IdentityServer4.Models;

namespace IdentityServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            IdentityResource customProfile = new(name: "profile", userClaims: new[] { "name" });
            IdentityResource roles = new(name: "roles", userClaims: new[] { "role" });

            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                customProfile,
                roles
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("security") { UserClaims = { "name" } },
                new ApiScope("data") { UserClaims = { "name" } },
                new ApiScope("reporting") { UserClaims = { "name" } },
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("medibase")
                {
                    UserClaims = { "name", "role" },
                    Scopes = { "security", "data", "reporting" }
                }
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration config)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "spa",

                    AllowedGrantTypes = GrantTypes.Code,
                    AllowOfflineAccess = true,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = {
                        config["IdentityServer:Client:RedirectUris:Callback"],
                    },
                    PostLogoutRedirectUris = {
                        config["IdentityServer:Client:PostLogoutRedirectUris:Callback"],
                    },
                    AllowedCorsOrigins = { config["Cors"] },

                    AllowedScopes = { "openid", "profile", "roles", "security", "data", "reporting", "offline_access"},

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AccessTokenLifetime = 300
                },
            };
        }

    }
}
