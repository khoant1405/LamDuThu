namespace JSN.Core.Entity;

public class UnitOfWork(DbFactory dbFactory) : IUnitOfWork, IDisposable
{
    private readonly DbFactory _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
    private CoreDbContext? _dbContext;

    public CoreDbContext DbContext
    {
        get { return _dbContext ??= _dbFactory.DbContext; }
    }

    public void Dispose()
    {
        _dbFactory.Dispose();
    }

    public async Task<int> CommitAsync()
    {
        return await DbContext.SaveChangesAsync();
    }
}