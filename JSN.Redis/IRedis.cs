using JSN.Core.ViewModel;

namespace JSN.Redis;

public interface IRedis<in T> where T : class
{
    void AddOrUpdate(T entity);
    Task AddOrUpdateAsync(T entity);
    ArticleView? GetById(int id);
    Task<ArticleView?> GetByIdAsync(int id);
    void Delete(int id);

    Task DeleteAsync(int id);
    //void AddRange(IEnumerable<T> entities);
    //Task AddRangeAsync(IEnumerable<T> entities);
    //void DeleteRange(IEnumerable<int> ids);
    //Task DeleteRangeAsync(IEnumerable<int> ids);
}