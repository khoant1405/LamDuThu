using JSN.IdentityServer;
using JSN.IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// Tạo một instance của WebApplication (ASP.NET Core 6)
var builder = WebApplication.CreateBuilder(args);

// Lấy tên của assembly (dự án chính) để sử dụng trong việc cấu hình migrations.
var assembly = typeof(Program).Assembly.GetName().Name;

// Lấy chuỗi kết nối từ file cấu hình dự án.
var defaultConnString = builder.Configuration.GetConnectionString("DefaultConnection");

// Kiểm tra xem có yêu cầu seed dữ liệu không.
var seed = args.Contains("/seed");
if (seed)
{
    // Loại bỏ tùy chọn "/seed" nếu yêu cầu seed dữ liệu.
    args = args.Except(new[] { "/seed" }).ToArray();

    // Gọi phương thức EnsureSeedData để seed dữ liệu vào cơ sở dữ liệu.
    SeedData.EnsureSeedData(defaultConnString);
}

// Cấu hình và đăng ký dịch vụ DbContext cho Identity Framework.
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(defaultConnString, b => b.MigrationsAssembly(assembly)));

// Cấu hình và đăng ký dịch vụ quản lý danh tính cho Identity Framework và liên kết nó với DbContext đã cấu hình trước đó.
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityDbContext>();

// Cấu hình và đăng ký dịch vụ Identity Server trong ứng dụng.
builder.Services.AddIdentityServer()
    .AddAspNetIdentity<IdentityUser>()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b =>
            b.UseSqlServer(defaultConnString, opt => opt.MigrationsAssembly(assembly));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b =>
            b.UseSqlServer(defaultConnString, opt => opt.MigrationsAssembly(assembly));
    })
    .AddDeveloperSigningCredential();

// Cấu hình và đăng ký dịch vụ cho MVC (Model-View-Controller) để xây dựng giao diện người dùng.
builder.Services.AddControllersWithViews();

// Xây dựng ứng dụng web bằng cách sử dụng đối tượng WebApplication.
var app = builder.Build();

// Kích hoạt middleware để phục vụ các tệp tĩnh như hình ảnh, CSS, và JavaScript.
app.UseStaticFiles();

// Kích hoạt middleware định tuyến để xử lý định tuyến yêu cầu HTTP.
app.UseRouting();

// Kích hoạt Identity Server để xử lý xác thực và ủy quyền.
app.UseIdentityServer();

// Kích hoạt middleware xác thực và ủy quyền cho ứng dụng.
app.UseAuthorization();

// Cấu hình điểm cuối (endpoint) mặc định để xử lý yêu cầu và định tuyến đến các controller và action của ứng dụng.
app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

// Bắt đầu chạy ứng dụng web và lắng nghe các yêu cầu HTTP từ client.
app.Run();