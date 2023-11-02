using System.Security.Claims;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;
using JSN.IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JSN.IdentityServer;

public class SeedData
{
    public static void EnsureSeedData(string connectionString)
    {
        var services = ConfigureServices(connectionString);
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        MigrateDatabase(scope, scope.ServiceProvider.GetService<PersistedGrantDbContext>());

        var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
        MigrateDatabase(scope, context);
        SeedClients(context);
        SeedIdentityResources(context);
        SeedApiScopes(context);

        var ctx = scope.ServiceProvider.GetService<IdentityDbContext>();
        MigrateDatabase(scope, ctx);
        EnsureUsers(scope);
    }

    private static IServiceCollection ConfigureServices(string connectionString)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<IdentityDbContext>(
            options => options.UseSqlServer(connectionString)
        );

        services
            .AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        services.AddOperationalDbContext(options =>
        {
            options.ConfigureDbContext = db =>
                db.UseSqlServer(
                    connectionString,
                    sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName)
                );
        });

        services.AddConfigurationDbContext(options =>
        {
            options.ConfigureDbContext = db =>
                db.UseSqlServer(
                    connectionString,
                    sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName)
                );
        });

        return services;
    }

    private static void MigrateDatabase(IServiceScope scope, DbContext context)
    {
        context.Database.Migrate();
    }

    private static void SeedClients(ConfigurationDbContext context)
    {
        if (!context.Clients.Any())
        {
            foreach (var client in Config.Clients) context.Clients.Add(client.ToEntity());

            context.SaveChanges();
        }
    }

    private static void SeedIdentityResources(ConfigurationDbContext context)
    {
        if (!context.IdentityResources.Any())
        {
            foreach (var resource in Config.IdentityResources) context.IdentityResources.Add(resource.ToEntity());

            context.SaveChanges();
        }
    }

    private static void SeedApiScopes(ConfigurationDbContext context)
    {
        if (!context.ApiScopes.Any())
        {
            foreach (var resource in Config.ApiScopes) context.ApiScopes.Add(resource.ToEntity());

            context.SaveChanges();
        }
    }

    private static async void EnsureUsers(IServiceScope scope)
    {
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var superAdmin = await userMgr.FindByNameAsync("superAdmin");

        if (superAdmin == null)
        {
            superAdmin = new IdentityUser
            {
                UserName = "superAdmin",
                Email = "vinamilk1634@gmail.com",
                EmailConfirmed = false
            };

            var result = await userMgr.CreateAsync(superAdmin, "1405");

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = await userMgr.AddClaimsAsync(superAdmin, new Claim[]
            {
                new(JwtClaimTypes.Name, "superAdmin")
            });

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
        }
    }
}