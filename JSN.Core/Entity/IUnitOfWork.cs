namespace JSN.Core.Entity;

public interface IUnitOfWork
{
    Task<int> CommitAsync();
}