using IdentityServer4.Models;

namespace IdentityServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            IdentityResource customProfile = new(name: "profile", userClaims: new[] { "name" });

            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                customProfile
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
                    UserClaims = { "name" },
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

                    AllowedScopes = { "openid", "profile", "security", "data", "reporting", "offline_access"},

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AccessTokenLifetime = 300
                },
            };
        }

    }
}
