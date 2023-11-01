using JSN.Core.Entity;
using JSN.Core.Repository;
using JSN.Core.Repository.Interface;
using JSN.Redis.Helper;
using JSN.Redis.Impl;
using JSN.Redis.Interface;
using JSN.Service.Implement;
using JSN.Service.Interface;
using JSN.Shared.Setting;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace JSN.AutoCrawl.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        // Configure DbContext with Scoped lifetime
        services.AddDbContext<CoreDbContext>(options =>
        {
            options.UseSqlServer(AppSettings.DefaultSqlSetting?.ConnectString);
            //options.UseLazyLoadingProxies();
        });

        services.AddScoped(
            (Func<IServiceProvider, Func<CoreDbContext>>)(provider => () => provider.GetService<CoreDbContext>()!));
        services.AddScoped<DbFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IArticleRepository, ArticleRepository>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICrawlerService, CrawlerService>();

        return services;
    }

    public static IServiceCollection AddRedis(this IServiceCollection services)
    {
        if (AppSettings.RedisSetting.IsUseRedisLazy == false)
        {
            services.AddSingleton<IConnectionMultiplexer>(provider =>
                RedisHelper.GetConnectionMultiplexer());
        }

        services.AddTransient<IArticleCacheService, ArticleCacheService>();
        services.AddTransient<IArticlePaginationCacheService, ArticlePaginationCacheService>();

        return services;
    }
}