namespace JSN.Core.Entity;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly DbFactory _dbFactory;
    private CoreDbContext? _dbContext;
    private bool _disposed;

    public UnitOfWork(DbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public CoreDbContext DbContext
    {
        get
        {
            if (_disposed) throw new ObjectDisposedException("UnitOfWork");

            return _dbContext ??= _dbFactory.DbContext;
        }
    }

    public void Dispose()
    {
        if (_disposed || _dbContext == null) return;
        _disposed = true;
        _dbContext.Dispose();
    }

    public async Task<int> CommitAsync()
    {
        return await DbContext.SaveChangesAsync();
    }
}