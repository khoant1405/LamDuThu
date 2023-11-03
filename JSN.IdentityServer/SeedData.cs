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
        // 1. Cấu hình và tạo container phụ thuộc.
        var services = ConfigureServices(connectionString);
        var serviceProvider = services.BuildServiceProvider();

        // 2. Tạo một phạm vi (scope) cho container phụ thuộc.
        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

        // 3. Migrate cơ sở dữ liệu của PersistedGrantDbContext.
        MigrateDatabase(scope.ServiceProvider.GetService<PersistedGrantDbContext>() ??
                        throw new InvalidOperationException());

        // 4. Lấy ConfigurationDbContext để làm việc với dữ liệu cấu hình IdentityServer.
        var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
        if (context != null)
        {
            // 5. Migrate cơ sở dữ liệu của ConfigurationDbContext.
            MigrateDatabase(context);

            // 6. Thêm các clients, identity resources, API scopes và API resources.
            SeedClients(context);
            SeedIdentityResources(context);
            SeedApiScopes(context);
            SeedApiResources(context);
        }

        // 7. Lấy IdentityDbContext để làm việc với dữ liệu danh tính người dùng.
        var ctx = scope.ServiceProvider.GetService<JsnIdentityDbContext>();
        if (ctx != null)
        {
            // 8. Migrate cơ sở dữ liệu của IdentityDbContext.
            MigrateDatabase(ctx);
        }

        // 9. Đảm bảo rằng các người dùng cần thiết đã được tạo.
        EnsureUsers(scope);
    }

    private static IServiceCollection ConfigureServices(string connectionString)
    {
        // 10. Tạo một bộ sưu tập dịch vụ để cấu hình và đăng ký các dịch vụ.
        var services = new ServiceCollection();
        services.AddLogging();

        // 11. Thêm IdentityDbContext để làm việc với dữ liệu danh tính người dùng.
        services.AddDbContext<JsnIdentityDbContext>(
            options => options.UseSqlServer(connectionString)
        );

        // 12. Cấu hình và đăng ký các dịch vụ liên quan đến Identity, bao gồm quản lý người dùng và vai trò.
        services
            .AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<JsnIdentityDbContext>()
            .AddDefaultTokenProviders();

        // 13. Thêm DbContext hoạt động cho IdentityServer (đối với các token, mã và sự đồng thuận).
        services.AddOperationalDbContext(options =>
        {
            options.ConfigureDbContext = db =>
                db.UseSqlServer(
                    connectionString,
                    sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName)
                );
        });

        // 14. Thêm DbContext cấu hình cho IdentityServer (đối với clients, identity resources, vv.).
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

    private static void MigrateDatabase(DbContext context)
    {
        // 15. Migrate cơ sở dữ liệu liên quan đến DbContext đã cung cấp.
        context.Database.Migrate();
    }

    private static void SeedClients(ConfigurationDbContext context)
    {
        // 16. Kiểm tra xem đã tồn tại các clients trong ConfigurationDbContext chưa.
        if (context.Clients.Any())
        {
            return;
        }

        // 17. Nếu không có clients nào tồn tại, thì tạo các clients từ dữ liệu cấu hình.
        foreach (var client in Config.Clients) context.Clients.Add(client.ToEntity());

        // 18. Lưu các thay đổi vào cơ sở dữ liệu.
        context.SaveChanges();
    }

    private static void SeedIdentityResources(ConfigurationDbContext context)
    {
        // 19. Kiểm tra xem đã tồn tại các identity resources trong ConfigurationDbContext chưa.
        if (context.IdentityResources.Any())
        {
            return;
        }

        // 20. Nếu không có identity resources nào tồn tại, thì tạo chúng từ dữ liệu cấu hình.
        foreach (var resource in Config.IdentityResources) context.IdentityResources.Add(resource.ToEntity());

        // 21. Lưu các thay đổi vào cơ sở dữ liệu.
        context.SaveChanges();
    }

    private static void SeedApiScopes(ConfigurationDbContext context)
    {
        // 22. Kiểm tra xem đã tồn tại các API scopes trong ConfigurationDbContext chưa.
        if (context.ApiScopes.Any())
        {
            return;
        }

        // 23. Nếu không có API scopes nào tồn tại, thì tạo chúng từ dữ liệu cấu hình.
        foreach (var resource in Config.ApiScopes) context.ApiScopes.Add(resource.ToEntity());

        // 24. Lưu các thay đổi vào cơ sở dữ liệu.
        context.SaveChanges();
    }

    private static void SeedApiResources(ConfigurationDbContext context)
    {
        // 25. Kiểm tra xem đã tồn tại các API resources trong ConfigurationDbContext chưa.
        if (context.ApiResources.Any())
        {
            return;
        }

        // 26. Nếu không có API resources nào tồn tại, thì tạo chúng từ dữ liệu cấu hình.
        foreach (var resource in Config.ApiResources) context.ApiResources.Add(resource.ToEntity());

        // 27. Lưu các thay đổi vào cơ sở dữ liệu.
        context.SaveChanges();
    }

    private static async void EnsureUsers(IServiceScope scope)
    {
        // 28. Đảm bảo rằng các người dùng cụ thể đã được tạo để thực hiện xác thực.
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var superAdmin = await userMgr.FindByNameAsync("superAdmin");

        if (superAdmin != null)
        {
            return;
        }

        superAdmin = new IdentityUser
        {
            UserName = "superAdmin",
            Email = "vinamilk1634@gmail.com",
            EmailConfirmed = false
        };

        // 29. Tạo người dùng superAdmin.
        var result = await userMgr.CreateAsync(superAdmin, "1405");

        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }

        // 30. Thêm các quyền cho người dùng superAdmin.
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