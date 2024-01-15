﻿using IdentityModel;
using IdentityServer4.Test;
using IdentityServer4;
using System.Security.Claims;
using System.Text.Json;
using IdentityServer4.Models;

namespace IDP
{
    public static class Config
    {
        #region Users
        public static List<TestUser> Users
        {
            get
            {
                var address = new
                {
                    street_address = "Mohamed Metwally El-Sharawy",
                    locality = "Cairo",
                    postal_code = 13515,
                    country = "Egypt"
                };

                return new List<TestUser>
        {
            new TestUser
          {
            SubjectId = "666666",
            Username = "ramadan",
            Password = "ramadan",
            Claims =
            {
              new Claim(JwtClaimTypes.Name, "Abdelazez Ramadan"),
              new Claim(JwtClaimTypes.GivenName, "Abdelazez"),
              new Claim(JwtClaimTypes.FamilyName, "Ramadan"),
              new Claim(JwtClaimTypes.Email, "zezo.ramadan77@gmail.com"),
              new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
              new Claim(JwtClaimTypes.Role, "admin"),
              new Claim(JwtClaimTypes.WebSite, "http://ramadan.com"),
              new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                IdentityServerConstants.ClaimValueTypes.Json)
            }
          },
          new TestUser
          {
            SubjectId = "818727",
            Username = "alice",
            Password = "alice",
            Claims =
            {
              new Claim(JwtClaimTypes.Name, "Alice Smith"),
              new Claim(JwtClaimTypes.GivenName, "Alice"),
              new Claim(JwtClaimTypes.FamilyName, "Smith"),
              new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
              new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
              new Claim(JwtClaimTypes.Role, "admin"),
              new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
              new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                IdentityServerConstants.ClaimValueTypes.Json)
            }
          },
          new TestUser
          {
            SubjectId = "88421113",
            Username = "bob",
            Password = "bob",
            Claims =
            {
              new Claim(JwtClaimTypes.Name, "Bob Smith"),
              new Claim(JwtClaimTypes.GivenName, "Bob"),
              new Claim(JwtClaimTypes.FamilyName, "Smith"),
              new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
              new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
              new Claim(JwtClaimTypes.Role, "user"),
              new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
              new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                IdentityServerConstants.ClaimValueTypes.Json)
            }
          }
        };
            }
        }
        #endregion

        public static IEnumerable<IdentityResource> IdentityResources => new[]
        {
            new IdentityResources.OpenId(),

            //new IdentityResource(
            //name: "openid",
            //userClaims: new[] { "sub" },
            //displayName: "Your user identifier"),

            new IdentityResource
            {
                Name = "role",
                UserClaims = new List<string> {"role"}
            },

            new IdentityResource(
                name: "profile",
                userClaims: new[] { "name", "email", "website","given_name","family_name","address"},
                displayName: "Your profile data"){ Enabled=true, ShowInDiscoveryDocument=true},
            //new IdentityResources.Profile(),
        };

        public static IEnumerable<ApiScope> ApiScopes => new[]
        {
            new ApiScope("api.read","read"),
            new ApiScope("weatherapi.write","write",new[]{ "role"}),
        };

        public static IEnumerable<ApiResource> ApiResources => new[]
        {
          new ApiResource("weatherapi")
          {
            Scopes = new List<string> {"api.read", "weatherapi.write"},
            ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
            UserClaims = new List<string> {"role"}
          }
        };

        public static IEnumerable<Client> Clients => new[]
          {
            new Client
            {
              ClientId = "Plus",
              ClientName = "Plus",

              AllowedGrantTypes = GrantTypes.ClientCredentials    ,
              ClientSecrets = {new Secret("secret".Sha256())},
              AllowedScopes = { "api.read", "weatherapi.write"}
            },

            new Client
            {
              ClientId = "Extra-Teacher",
              ClientSecrets = {new Secret("secret".Sha256())},

              AllowedGrantTypes = GrantTypes.Code,

              RedirectUris = {"https://localhost:5444/signin-oidc"},
              FrontChannelLogoutUri = "https://localhost:5444/signout-oidc",
              PostLogoutRedirectUris = {"https://localhost:5444/signout-callback-oidc"},

              AllowOfflineAccess = true,
              AllowedScopes = {"openid", "profile", "api.read"},
              RequirePkce = true,
              RequireConsent = true,
              AllowPlainTextPkce = false
            },
          };
    }
}