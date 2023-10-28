using Microsoft.EntityFrameworkCore;

namespace JSN.Core.Entity
{
    public class DbFactory : IDisposable
    {
        private readonly AsyncLocal<bool> _asyncLocalFlag = new();
        private readonly Func<CoreDbContext> _instanceFunc;
        private DbContext? _dbContext;
        private bool _disposed;

        public DbFactory(Func<CoreDbContext> dbContextFactory)
        {
            _instanceFunc = dbContextFactory;
        }

        public DbContext DbContext
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException("DbFactory");

                if (_dbContext == null)
                {
                    if (_asyncLocalFlag.Value) throw new InvalidOperationException("Nested DbContext creation is not allowed.");

                    _asyncLocalFlag.Value = true;

                    try
                    {
                        _dbContext = _instanceFunc.Invoke();
                    }
                    finally
                    {
                        _asyncLocalFlag.Value = false;
                    }
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
    }
}
