﻿using JSN.Redis.Helper;
using JSN.Shared.Config;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JSN.Redis;

public class Redis<T> where T : class
{
    protected void AddOrUpdate(T entity, IDatabase database)
    {
        var key = GetKeyByEntity(entity);
        var serializedEntity = SerializeEntity(entity);
        database.StringSet(key, serializedEntity);
    }

    protected async Task AddOrUpdateAsync(T entity, IDatabase database)
    {
        var key = GetKeyByEntity(entity);
        var serializedEntity = SerializeEntity(entity);
        await database.StringSetAsync(key, serializedEntity);
    }

    protected void AddOrUpdate(string key, T entity, IDatabase database)
    {
        var serializedEntity = SerializeEntity(entity);
        database.StringSet(key, serializedEntity);
    }

    protected async Task AddOrUpdateAsync(string key, T entity, IDatabase database)
    {
        var serializedEntity = SerializeEntity(entity);
        await database.StringSetAsync(key, serializedEntity);
    }

    protected T? Get(string key, IDatabase database)
    {
        string? serializedEntity = database.StringGet(key);
        return DeserializeEntity(serializedEntity);
    }

    protected async Task<T?> GetAsync(string key, IDatabase database)
    {
        string? serializedEntity = await database.StringGetAsync(key);
        return DeserializeEntity(serializedEntity);
    }

    protected void Delete(string key, IDatabase database)
    {
        database.KeyDelete(key);
    }

    protected async Task DeleteAsync(string key, IDatabase database)
    {
        await database.KeyDeleteAsync(key);
    }

    protected void AddRange(IEnumerable<T> entities, IDatabase database)
    {
        var tasks = entities.Select(entity =>
        {
            var key = GetKeyByEntity(entity);
            var serializedEntity = SerializeEntity(entity);
            return database.StringSetAsync(key, serializedEntity);
        });

        Task.WhenAll(tasks).Wait();
    }

    protected async Task AddRangeAsync(IEnumerable<T> entities, IDatabase database)
    {
        var tasks = entities.Select(async entity =>
        {
            var key = GetKeyByEntity(entity);
            var serializedEntity = SerializeEntity(entity);
            await database.StringSetAsync(key, serializedEntity);
        });

        await Task.WhenAll(tasks);
    }

    protected void DeleteRange(IEnumerable<string> keys, IDatabase database)
    {
        var tasks = keys.Select(key => database.KeyDeleteAsync(key));
        Task.WhenAll(tasks).Wait();
    }

    protected async Task DeleteRangeAsync(IEnumerable<string> keys, IDatabase database)
    {
        var tasks = keys.Select(key => database.KeyDeleteAsync(key));
        await Task.WhenAll(tasks);
    }

    private static string GetKeyByEntity(T entity)
    {
        var typeName = typeof(T).Name;
        var idProperty = typeof(T).GetProperty("Id");

        if (idProperty != null)
        {
            var idValue = idProperty.GetValue(entity, null);
            return $"Database:{typeName}:{idValue}";
        }

        throw new InvalidOperationException($"Type {typeName} does not have an 'Id' property.");
    }

    protected string GetKeyById(int id)
    {
        var typeName = typeof(T).Name;
        return $"Database:{typeName}:{id}";
    }

    private static string SerializeEntity(T entity)
    {
        return JsonConvert.SerializeObject(entity);
    }

    private static T? DeserializeEntity(string? serializedEntity)
    {
        return string.IsNullOrEmpty(serializedEntity) ? null : JsonConvert.DeserializeObject<T>(serializedEntity);
    }

    protected (IConnectionMultiplexer, IDatabase) GetDatabaseOptions(IConnectionMultiplexer connectionMultiplexer)
    {
        if (!AppConfig.RedisConfig.IsUseRedisLazy)
        {
            return (connectionMultiplexer, connectionMultiplexer.GetDatabase());
        }

        var redisLazy = new RedisLazy();
        return (redisLazy.GetConnectionMultiplexer(), redisLazy.GetLazyDatabase());
    }
}