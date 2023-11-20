namespace JSN.Core.Entity;

public class DbFactory(Func<CoreDbContext> dbContextFactory) : IDisposable
{
    private readonly Func<CoreDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    private CoreDbContext? _dbContext;
    private bool _disposed;

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