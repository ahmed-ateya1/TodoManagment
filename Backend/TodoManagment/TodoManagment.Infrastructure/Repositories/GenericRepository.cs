﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TodoManagment.Core.Domain.RepositoryContract;
using TodoManagment.Infrastructure.Data;

namespace TodoManagment.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly TodoDbContext _db;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(TodoDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }

        public async Task AddRangeAsync(IEnumerable<T> model)
        {
            await _dbSet.AddRangeAsync(model);
            await SaveAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AnyAsync(filter);
        }

        public async Task<T> CheckEntityAsync(Expression<Func<T, bool>> predicate, string entityName)
        {
            var model = await _db.Set<T>().FirstOrDefaultAsync(predicate);
            if (model is null)
                throw new ArgumentNullException($"{entityName} not found");
            return model;
        }


        public async Task<long> CountAsync(Expression<Func<T, bool>>? filter = null)
        {
            if (filter == null)
                return await _dbSet.CountAsync();
            return await _dbSet.CountAsync(filter);
        }

        public async Task<T> CreateAsync(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            await _dbSet.AddAsync(model);
            await SaveAsync();
            return model;
        }

        public async Task<bool> DeleteAsync(T model)
        {
            if (model == null)
                return false;

            _dbSet.Remove(model);
            await SaveAsync();
            return true;
        }

        public void Detach(T entity)
        {
            _db.Entry(entity).State = EntityState.Detached;
        }

        public async Task<IEnumerable<T>> GetAllAsync(
     Expression<Func<T, bool>>? filter = null,
     string includeProperties = "",
     string? sortBy = null,
     string? sortDirection = "asc",
     int? pageIndex = null,
     int? pageSize = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            // Apply Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, sortBy);
                var lambda = Expression.Lambda(property, parameter);

                string methodName = sortDirection?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
                var method = typeof(Queryable).GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), property.Type);

                query = (IQueryable<T>)method.Invoke(null, new object[] { query, lambda });
            }

            // Apply Pagination
            if (pageIndex != null && pageSize != null)
            {
                query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }





        public async Task<T> GetByAsync(Expression<Func<T, bool>>? filter = null, bool isTracked = true, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            if (!isTracked)
                query = query.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public Task RemoveRangeAsync(IEnumerable<T> model)
        {
            _dbSet.RemoveRange(model);

            return SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<T> UpdateAsync(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            _dbSet.Update(model);
            return model;
        }
    }
}
