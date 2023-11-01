namespace JSN.Core.Entity;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly DbFactory _dbFactory;
    private CoreDbContext _dbContext;
    private bool _disposed;

    public UnitOfWork(DbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public CoreDbContext DbContext
    {
        get
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("UnitOfWork");
            }

            if (_dbContext == null)
            {
                _dbContext = _dbFactory.DbContext;
            }

            return _dbContext;
        }
    }

    public void Dispose()
    {
        if (!_disposed && _dbContext != null)
        {
            _disposed = true;
            _dbContext.Dispose();
        }
    }

    public async Task<int> CommitAsync()
    {
        return await DbContext.SaveChangesAsync();
    }
}