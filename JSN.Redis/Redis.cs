﻿using JSN.Redis.Helper;
using JSN.Shared.Setting;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JSN.Redis;

public class Redis<T> where T : class
{
    public void AddOrUpdate(T entity, IDatabase db)
    {
        var database = GetDatabase(db);
        var key = GetKeyByEntity(entity);
        var serializedEntity = SerializeEntity(entity);
        database.StringSet(key, serializedEntity);
    }

    public async Task AddOrUpdateAsync(T entity, IDatabase db)
    {
        var database = GetDatabase(db);
        var key = GetKeyByEntity(entity);
        var serializedEntity = SerializeEntity(entity);
        await database.StringSetAsync(key, serializedEntity);
    }

    public void AddOrUpdate(string key, T entity, IDatabase db)
    {
        var database = GetDatabase(db);
        var serializedEntity = SerializeEntity(entity);
        database.StringSet(key, serializedEntity);
    }

    public async Task AddOrUpdateAsync(string key, T entity, IDatabase db)
    {
        var database = GetDatabase(db);
        var serializedEntity = SerializeEntity(entity);
        await database.StringSetAsync(key, serializedEntity);
    }

    public T? Get(string key, IDatabase db)
    {
        var database = GetDatabase(db);
        string? serializedEntity = database.StringGet(key);
        return DeserializeEntity(serializedEntity);
    }

    public async Task<T?> GetAsync(string key, IDatabase db)
    {
        var database = GetDatabase(db);
        string? serializedEntity = await database.StringGetAsync(key);
        return DeserializeEntity(serializedEntity);
    }

    public void Delete(string key, IDatabase db)
    {
        var database = GetDatabase(db);
        database.KeyDelete(key);
    }

    public async Task DeleteAsync(string key, IDatabase db)
    {
        var database = GetDatabase(db);
        await database.KeyDeleteAsync(key);
    }

    public void AddRange(IEnumerable<T> entities, IDatabase db)
    {
        var database = GetDatabase(db);
        var tasks = entities.Select(entity =>
        {
            var key = GetKeyByEntity(entity);
            var serializedEntity = SerializeEntity(entity);
            return database.StringSetAsync(key, serializedEntity);
        });

        Task.WhenAll(tasks)
            .Wait();
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, IDatabase db)
    {
        var database = GetDatabase(db);
        var tasks = entities.Select(async entity =>
        {
            var key = GetKeyByEntity(entity);
            var serializedEntity = SerializeEntity(entity);
            await database.StringSetAsync(key, serializedEntity);
        });

        await Task.WhenAll(tasks);
    }

    public void DeleteRange(IEnumerable<string> keys, IDatabase db)
    {
        var database = GetDatabase(db);
        var tasks = keys.Select(key => database.KeyDeleteAsync(key));
        Task.WhenAll(tasks)
            .Wait();
    }

    public async Task DeleteRangeAsync(IEnumerable<string> keys, IDatabase db)
    {
        var database = GetDatabase(db);
        var tasks = keys.Select(key => database.KeyDeleteAsync(key));
        await Task.WhenAll(tasks);
    }

    protected string GetKeyByEntity(T entity)
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

    private IDatabase GetDatabase(IDatabase db)
    {
        if (!AppSettings.RedisSetting.IsUseRedisLazy)
        {
            return db;
        }

        var redisLazy = new RedisLazy();
        var connection = redisLazy.LazyConnection;
        return connection.GetDatabase();
    }
}