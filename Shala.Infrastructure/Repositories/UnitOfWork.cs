//using Microsoft.EntityFrameworkCore.Storage;
//using Shala.Application.Common;
//using Shala.Infrastructure.Data;

//namespace Shala.Infrastructure.Repositories;

//public class UnitOfWork : IUnitOfWork
//{
//    private readonly AppDbContext _db;
//    private IDbContextTransaction? _transaction;

//    public UnitOfWork(AppDbContext db)
//    {
//        _db = db;
//    }

//    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
//    {
//        return await _db.SaveChangesAsync(cancellationToken);
//    }

//    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
//    {
//        if (_transaction is not null)
//            return;

//        _transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
//    }

//    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
//    {
//        if (_transaction is null)
//            return;

//        await _db.SaveChangesAsync(cancellationToken);
//        await _transaction.CommitAsync(cancellationToken);
//        await _transaction.DisposeAsync();
//        _transaction = null;
//    }

//    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
//    {
//        if (_transaction is null)
//            return;

//        await _transaction.RollbackAsync(cancellationToken);
//        await _transaction.DisposeAsync();
//        _transaction = null;
//    }
//}


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shala.Application.Common;
using Shala.Infrastructure.Data;
using System.Data;

namespace Shala.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(
        CancellationToken cancellationToken = default,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        if (_transaction is not null)
            return;

        _transaction = await _db.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            return;

        await _db.SaveChangesAsync(cancellationToken);
        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            return;

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }
}