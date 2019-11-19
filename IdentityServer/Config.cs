using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResource("role", new List<string> {"role"}),
                new IdentityResource("email", new List<string> {"email"})
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("api", "DGHA API", new List<string> {"role"}),
                new ApiResource("testapi", "API Resource set for testing")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedScopes = { "testapi" }
                },

                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { 
                        "api",
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.OpenId
                    },

                    AllowOfflineAccess = true
                },

                new Client
                {
                    ClientId = "AdminPortal",
                    ClientName = "Admin Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireConsent = false,
                    //RequireClientSecret = false,

                    RedirectUris =           { "http://localhost:4200/callback", "https://dgha-admin.azurewebsites.net/callback" },
                    PostLogoutRedirectUris = { "http://localhost:4200/home", "https://dgha-admin.azurewebsites.net/home" },
                    AllowedCorsOrigins =     { "http://localhost:4200", "https://dgha-admin.azurewebsites.net" },

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api",
                        "role",
                        "email"
                    },
                    AlwaysIncludeUserClaimsInIdToken = true,

                    AllowOfflineAccess = true
                }
            };
        }

        //public static List<TestUser> GetTestUsers()
        //{
        //    return new List<TestUser>
        //        {
        //            new TestUser
        //            {
        //                SubjectId = "1",
        //                Username = "admin",
        //                Password = "admin"
        //            }
        //        };
        //}
    }
}