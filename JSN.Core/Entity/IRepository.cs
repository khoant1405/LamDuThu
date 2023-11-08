using System.Linq.Expressions;

namespace JSN.Core.Entity;

public interface IRepository<T> where T : class
{
    void Add(T entity);
    Task AddAsync(T entity);
    void AddRange(IEnumerable<T> entities);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    Task<int> SaveChangesAsync();
    IQueryable<T> Where(Expression<Func<T, bool>>? filter = null);
}