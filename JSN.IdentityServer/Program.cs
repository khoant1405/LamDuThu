using JSN.IdentityServer;
using JSN.IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly.GetName().Name;
var defaultConnString = builder.Configuration.GetConnectionString("DefaultConnection");

var seed = args.Contains("/seed");
if (seed)
{
    args = args.Except(new[] { "/seed" }).ToArray();
    SeedData.EnsureSeedData(defaultConnString);
}

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(defaultConnString, b => b.MigrationsAssembly(assembly)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityDbContext>();

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

builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

app.Run();