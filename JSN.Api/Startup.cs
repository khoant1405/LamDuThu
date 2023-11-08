using System.Text;
using System.Text.Json.Serialization;
using JSN.Api.Extensions;
using JSN.Core.AutoMapper;
using JSN.Shared.Setting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace JSN.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        AppConfig.ConfigurationBuilder = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });
        services.AddDatabase();
        services.AddRepositories();
        services.AddRedis();
        services.AddServices();
        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfig.JwtSetting.Token!)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidAudience = AppConfig.JwtSetting.ValidAudience,
                    ValidIssuer = AppConfig.JwtSetting.ValidIssuer
                };
            });
        services.AddCors(options => options.AddPolicy("NgOrigins", policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader();
        }));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors("NgOrigins");

        //app.UseAuthentication();

        //app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}