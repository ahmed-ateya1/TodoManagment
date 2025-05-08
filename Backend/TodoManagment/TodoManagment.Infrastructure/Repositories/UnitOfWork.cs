using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TodoManagment.Core.Domain.Entities.Common;
using TodoManagment.Core.Domain.RepositoryContract;
using TodoManagment.Core.ServiceContract;
using TodoManagment.Core.Services;
using TodoManagment.Infrastructure.Data;

namespace TodoManagment.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TodoDbContext _db;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
        public UnitOfWork(TodoDbContext db, IDomainEventDispatcher domainEventDispatcher)
        {
            _db = db;
            _domainEventDispatcher = domainEventDispatcher;
        }
        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return _repositories[typeof(T)] as IGenericRepository<T>;
            }
            var repository = new GenericRepository<T>(_db);
            _repositories.Add(typeof(T), repository);

            return repository;
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _db.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _db.Database.CommitTransactionAsync();
        }

        public async Task<int> CompleteAsync()
        {
            var entitiesWithEvents = _db.ChangeTracker.Entries<Entity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var result = await _db.SaveChangesAsync();

            foreach (var entity in entitiesWithEvents)
            {
                foreach (var domainEvent in entity.DomainEvents)
                {
                    await _domainEventDispatcher.Dispatch(domainEvent);
                }
                entity.ClearDomainEvents();
            }

            return result;
        }
        public async Task RollbackTransactionAsync()
        {
            await _db.Database.RollbackTransactionAsync();
        }
        public void Dispose()
        {
            _db.Dispose();
        }

    }
}
