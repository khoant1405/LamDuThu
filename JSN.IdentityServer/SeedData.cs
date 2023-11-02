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
        //Tạo một đối tượng ServiceCollection
        var services = new ServiceCollection();

        //Đăng ký dịch vụ ghi log.
        services.AddLogging();

        //Đăng ký DbContext của IdentityDbContext để làm việc với cơ sở dữ liệu IdentityServer4.
        services.AddDbContext<IdentityDbContext>(
            options => options.UseSqlServer(connectionString)
        );

        //Đăng ký dịch vụ xác thực và quản lý theo role sử dụng Identity Framework Core.
        services
            .AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        //Đăng ký DbContext cho thông tin vận hành của IdentityServer4.
        services.AddOperationalDbContext(
            options =>
            {
                options.ConfigureDbContext = db =>
                    db.UseSqlServer(
                        connectionString,
                        sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName)
                    );
            }
        );

        //Đăng ký DbContext cho cấu hình của IdentityServer4.
        services.AddConfigurationDbContext(
            options =>
            {
                options.ConfigureDbContext = db =>
                    db.UseSqlServer(
                        connectionString,
                        sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName)
                    );
            }
        );

        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

        //Migrates cơ sở dữ liệu cho PersistedGrantDbContext. Điều này đảm bảo rằng cơ sở dữ liệu của các phiên và mã thông báo được cập nhật đúng cách
        scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();

        //Lấy DbContext cho cấu hình của IdentityServer4.
        var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
        context.Database.Migrate();

        //Gọi phương thức EnsureSeedData để đảm bảo dữ liệu cấu hình được tạo nếu nó chưa tồn tại.
        EnsureSeedData(context);

        //Lấy DbContext cho quản lý người dùng và vai trò.
        var ctx = scope.ServiceProvider.GetService<IdentityDbContext>();
        ctx.Database.Migrate();

        //Gọi phương thức EnsureUsers để đảm bảo người dùng được tạo nếu họ chưa tồn tại trong hệ thống.
        EnsureUsers(scope);
    }

    private static void EnsureUsers(IServiceScope scope)
    {
        //Lấy UserManager để quản lý người dùng từ phạm vi dịch vụ.
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        //Tìm kiếm một người dùng với tên người dùng "superadmin" trong cơ sở dữ liệu.
        var superadmin = userMgr.FindByNameAsync("superadmin").Result;

        //Tạo mới người dùng superadmin với claim
        if (superadmin != null)
        {
            return;
        }

        superadmin = new IdentityUser
        {
            UserName = "superAdmin",
            Email = "vinamilk1634@gmail.com",
            EmailConfirmed = false
        };
        var result = userMgr.CreateAsync(superadmin, "1405").Result;
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }

        result =
            userMgr.AddClaimsAsync(
                superadmin,
                new Claim[]
                {
                    new(JwtClaimTypes.Name, "superAdmin")
                }
            ).Result;
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }
    }

    private static void EnsureSeedData(ConfigurationDbContext context)
    {
        //Kiểm tra xem trong cơ sở dữ liệu đã có cài đặt về Clients (ứng dụng khách) hay chưa.
        if (!context.Clients.Any())
        {
            //Nếu chưa có, thêm các cài đặt về Clients từ danh sách cài đặt Config.Clients vào cơ sở dữ liệu.
            foreach (var client in Config.Clients.ToList()) context.Clients.Add(client.ToEntity());

            context.SaveChanges();
        }

        if (!context.IdentityResources.Any())
        {
            foreach (var resource in Config.IdentityResources.ToList())
                context.IdentityResources.Add(resource.ToEntity());

            context.SaveChanges();
        }

        if (!context.ApiScopes.Any())
        {
            foreach (var resource in Config.ApiScopes.ToList()) context.ApiScopes.Add(resource.ToEntity());

            context.SaveChanges();
        }

        if (!context.ApiResources.Any())
        {
            foreach (var resource in Config.ApiResources.ToList()) context.ApiResources.Add(resource.ToEntity());

            context.SaveChanges();
        }
    }
}