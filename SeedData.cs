using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;
using IDP.Data;
using IDP;
using IDP.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;
using AutoMapper;

namespace DemoIDP
{
    public class SeedData
    {
        public static void EnsureSeedData(
            string connectionString,
            string identityConnnection,
            IMapper mapper)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer(connectionString));

            services.AddIdentity<IdentityUser, IdentityRole>()
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

            services.AddOperationalDbContext(options =>
            {
                options.ConfigureDbContext = db =>
                  db.UseSqlServer(identityConnnection, sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
            });
            services.AddConfigurationDbContext(options =>
            {
                options.ConfigureDbContext = db =>
                  db.UseSqlServer(identityConnnection, sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
            });

            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
                context.Database.Migrate();

                EnsureSeedData(context, mapper);

                var ctx = scope.ServiceProvider.GetService<ApplicationDbContext>();
                ctx.Database.Migrate();
                EnsureUsers(scope);
            }
        }

        private static void EnsureUsers(IServiceScope scope)
        {
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var alice = userMgr.FindByNameAsync("Ramadan").Result;
            if (alice == null)
            {
                alice = new IdentityUser
                {
                    UserName = "Ramadan",
                    Email = "AbdelazezRamadan@email.com",
                    EmailConfirmed = true,
                };
                var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(alice, new Claim[]
                {
                      new Claim(JwtClaimTypes.Name, "Abdelazez Ramadan"),
                      new Claim(JwtClaimTypes.GivenName, "Abdelazez"),
                      new Claim(JwtClaimTypes.FamilyName, "Ramadan"),
                      new Claim(JwtClaimTypes.WebSite, "http://Ramadan.com"),

                     
                }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                Log.Debug("Abdelazez created");
            }
            else
            {
                Log.Debug("Abdelazez already exists");
            }

            var fandary = userMgr.FindByNameAsync("fandary").Result;
            if (fandary == null)
            {
                fandary = new IdentityUser
                {
                    UserName = "fandary",
                    Email = "fandary@email.com",
                    EmailConfirmed = true
                };
                var result = userMgr.CreateAsync(fandary, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(fandary, new Claim[]
                {
                     new Claim(JwtClaimTypes.Name, "fandary "),
                     new Claim(JwtClaimTypes.GivenName, "fandary"),
                     new Claim(JwtClaimTypes.FamilyName, "fandary"),
                     new Claim(JwtClaimTypes.WebSite, "http://fandary.com"),
                     new Claim("location", "somewhere")
                }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                Log.Debug("bob created");
            }
            else
            {
                Log.Debug("bob already exists");
            }
        }


        private static void EnsureSeedData(ConfigurationDbContext context, IMapper mapper)
        {
            if (!context.Clients.Any())
            {
                Log.Debug("Clients being populated");

                foreach (var client in Config.Clients.ToList())
                {
                    context.Clients.Add(client.ToEntity(mapper));
                }

                context.SaveChanges();
            }
            else
            {
                Log.Debug("Clients already populated");
            }

            if (!context.IdentityResources.Any())
            {
                Log.Debug("IdentityResources being populated");
                foreach (var resource in Config.IdentityResources.ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity(mapper));
                }

                context.SaveChanges();
            }
            else
            {
                Log.Debug("IdentityResources already populated");
            }

            if (!context.ApiScopes.Any())
            {
                Log.Debug("ApiScopes being populated");
                foreach (var resource in Config.ApiScopes.ToList())
                {
                    context.ApiScopes.Add(resource.ToEntity(mapper));
                }

                context.SaveChanges();
            }
            else
            {
                Log.Debug("ApiScopes already populated");
            }

            if (!context.ApiResources.Any())
            {
                Log.Debug("ApiResources being populated");
                foreach (var resource in Config.ApiResources.ToList())
                {
                    context.ApiResources.Add(resource.ToEntity(mapper));
                }

                context.SaveChanges();
            }
            else
            {
                Log.Debug("ApiScopes already populated");
            }
        }
    }
}