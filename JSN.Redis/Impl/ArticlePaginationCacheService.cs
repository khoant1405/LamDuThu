using JSN.Core.ViewModel;
using JSN.Redis.Interface;
using JSN.Shared.Model;
using StackExchange.Redis;

namespace JSN.Redis.Impl;

public class ArticlePaginationCacheService : Redis<PaginatedList<ArticleView>>, IArticlePaginationCacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _redisDatabase;

    public ArticlePaginationCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        (_connectionMultiplexer, _redisDatabase) = GetDatabaseOptions(connectionMultiplexer);
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

    public async Task DeleteAllPageAsync()
    {
        const string pattern = "PaginatedList:ArticleView:*";
        var server = _connectionMultiplexer.GetServers().FirstOrDefault(x => x.Role().Value == "master");
        var keys = server?.Keys(pattern: pattern).Select(x => x.ToString());
        if (keys != null)
        {
            await DeleteRangeAsync(keys, _redisDatabase);
        }
    }

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