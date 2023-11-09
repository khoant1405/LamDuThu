using JSN.Core.Model;
using JSN.Redis.Interface;
using StackExchange.Redis;

namespace JSN.Redis.Impl;

public class ArticleCacheService : Redis<Article>, IArticleCacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _redisDatabase;

    public ArticleCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        (_connectionMultiplexer, _redisDatabase) = GetDatabaseOptions(connectionMultiplexer);
    }

    public void AddOrUpdate(Article entity)
    {
        AddOrUpdate(entity, _redisDatabase);
    }

    public async Task AddOrUpdateAsync(Article entity)
    {
        await AddOrUpdateAsync(entity, _redisDatabase);
    }

    public Article? GetById(int id)
    {
        var key = GetKeyById(id);
        return Get(key, _redisDatabase);
    }

    public async Task<Article?> GetByIdAsync(int id)
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