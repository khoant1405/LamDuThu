using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace JSN.Core.Entity;

public class Repository<T>(DbFactory dbFactory) : IRepository<T>, IDisposable where T : class
{
    private readonly DbFactory _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
    private DbSet<T>? _dbSet;

    protected DbSet<T> DbSet => _dbSet ??= _dbFactory.DbContext.Set<T>();

    public void Dispose()
    {
        _dbFactory.Dispose();
    }

    public void Add(T entity)
    {
        DbSet.Add(entity);
    }

    public async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        DbSet.AddRange(entities);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await DbSet.AddRangeAsync(entities);
    }

    public void Delete(T entity)
    {
        DbSet.Remove(entity);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        DbSet.RemoveRange(entities);
    }

    public IQueryable<T> Where(Expression<Func<T, bool>>? filter = null)
    {
        return filter == null ? DbSet : DbSet.Where(filter);
    }

    public void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        DbSet.UpdateRange(entities);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbFactory.DbContext.SaveChangesAsync();
    }
}