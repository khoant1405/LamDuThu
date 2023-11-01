namespace JSN.Redis;

public interface IRedis<T> where T : class
{
    void AddOrUpdate(T entity);
    Task AddOrUpdateAsync(T entity);
    T GetById(int id);
    Task<T> GetByIdAsync(int id);
    void Delete(int id);
    Task DeleteAsync(int id);

    //void AddRange(IEnumerable<T> entities);
    //Task AddRangeAsync(IEnumerable<T> entities);
    //void DeleteRange(IEnumerable<int> ids);
    //Task DeleteRangeAsync(IEnumerable<int> ids);
}