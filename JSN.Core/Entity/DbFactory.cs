namespace JSN.Core.Entity;

public class DbFactory : IDisposable
{
    private readonly Func<CoreDbContext> _dbContextFactory;
    private CoreDbContext? _dbContext;
    private bool _disposed;

    public DbFactory(Func<CoreDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    }

    public CoreDbContext DbContext
    {
        get
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("DbFactory");
            }

            return _dbContext ??= _dbContextFactory.Invoke();
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _dbContext?.Dispose();
    }
}