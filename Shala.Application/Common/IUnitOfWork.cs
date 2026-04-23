//namespace Shala.Application.Common;

//public interface IUnitOfWork
//{
//    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

//    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

//    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

//    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
//}


using System.Data;

namespace Shala.Application.Common;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(
        CancellationToken cancellationToken = default,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}