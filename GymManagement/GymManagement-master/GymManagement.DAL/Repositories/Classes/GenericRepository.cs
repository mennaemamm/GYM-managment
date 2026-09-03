using GymManagement.DAL.Context;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity , new()
    {
        private readonly GymDbContext _dbContext;
        private readonly DbSet<TEntity> _set;

        public GenericRepository(GymDbContext dbContext)
        {
            _dbContext = dbContext;
            _set = _dbContext.Set<TEntity>();
        }
        public async void AddAsync(TEntity entity, CancellationToken ct = default)
        {
            _set.Add(entity);
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return _set.AnyAsync(predicate, ct);
        }

        public async void DeleteAsync(TEntity entity, CancellationToken ct = default)
        {
            _set.Remove(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? _set : _set.AsNoTracking();
            return await query.ToListAsync(ct);
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _set.FindAsync(id, ct);
        }

        public async void UpdateAsync(TEntity entity, CancellationToken ct = default)
        {
            _set.Update(entity);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> Query = tracking ? _set : _set.AsNoTracking();
            return await Query.FirstOrDefaultAsync(predicate, ct);

        }

    }
}
