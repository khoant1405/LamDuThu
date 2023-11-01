using JSN.Core.ViewModel;
using JSN.Redis.Interface;
using JSN.Shared.Model;
using JSN.Shared.Setting;
using StackExchange.Redis;

namespace JSN.Redis.Impl;

public class ArticlePaginationCacheService : Redis<PaginatedList<ArticleView>>, IArticlePaginationCacheService
{
    private readonly IDatabase _redisDatabase;

    public ArticlePaginationCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        if (AppSettings.RedisSetting.IsUseRedisLazy == true)
        {
            return;
        }

        _redisDatabase = connectionMultiplexer.GetDatabase();
    }

    public void AddPage(PaginatedList<ArticleView> entity)
    {
        var key = $"PaginatedList:ArticleView:{entity.PageIndex}";
        AddOrUpdate(key, entity, _redisDatabase);
    }

    public async Task AddPageAsync(PaginatedList<ArticleView> entity)
    {
        var key = $"PaginatedList:ArticleView:{entity.PageIndex}";
        await AddOrUpdateAsync(key, entity, _redisDatabase);
    }

    public void DeletePage(int page)
    {
        var key = $"PaginatedList:ArticleView:{page}";
        Delete(key, _redisDatabase);
    }

    //public void DeleteAllPage()
    //{
    //    cache.Clear();
    //}

    public async Task DeletePageAsync(int page)
    {
        var key = $"PaginatedList:ArticleView:{page}";
        await DeleteAsync(key, _redisDatabase);
    }

    //public async Task DeleteAllPageAsync()
    //{
    //    // Simulate an asynchronous operation (e.g., deleting all entries from a database or external cache).
    //    await Task.Delay(1);

    //    DeleteAllPage();
    //}

    public PaginatedList<ArticleView>? GetPage(int page)
    {
        var key = $"PaginatedList:ArticleView:{page}";
        return Get(key, _redisDatabase);
    }

    public async Task<PaginatedList<ArticleView>?> GetPageAsync(int page)
    {
        var key = $"PaginatedList:ArticleView:{page}";
        return await GetAsync(key, _redisDatabase);
    }
}