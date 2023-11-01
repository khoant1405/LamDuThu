using JSN.Core.ViewModel;
using JSN.Redis.Interface;
using StackExchange.Redis;

namespace JSN.Redis.Impl;

public class ArticleCacheService : Redis<ArticleView>, IArticleCacheService
{
    private readonly IDatabase _redisDatabase;

    public ArticleCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _redisDatabase = connectionMultiplexer.GetDatabase();
    }

    public void AddOrUpdate(ArticleView entity)
    {
        AddOrUpdate(entity, _redisDatabase);
    }

    public async Task AddOrUpdateAsync(ArticleView entity)
    {
        await AddOrUpdateAsync(entity, _redisDatabase);
    }

    public ArticleView? GetById(int id)
    {
        var key = GetKeyById(id);
        return Get(key, _redisDatabase);
    }

    public async Task<ArticleView?> GetByIdAsync(int id)
    {
        var key = GetKeyById(id);
        return await GetAsync(key, _redisDatabase);
    }

    public void Delete(int id)
    {
        var key = GetKeyById(id);
        Delete(key, _redisDatabase);
    }

    public async Task DeleteAsync(int id)
    {
        var key = GetKeyById(id);
        await DeleteAsync(key, _redisDatabase);
    }
}