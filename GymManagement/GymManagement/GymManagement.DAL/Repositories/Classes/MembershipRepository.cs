using GymManagement.DAL.Context;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GymManagement.DAL.Repositories.Classes
{
	public class MembershipRepository : GenericRepository<Membership>, IMembershipRepository
    {
		private readonly GymDbContext _dbContext;

		public MembershipRepository(GymDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

        public async Task<List<Membership>> GetAllMembershipsWithMemberAndPlanAsync(Expression<Func<Membership, bool>>? predicate = null,
           CancellationToken ct = default)
        {
            IQueryable<Membership> query = _dbContext.Memberships.AsNoTracking().Include(m => m.Plan).Include(m => m.Member);

            if (predicate is not null) query = query.Where(predicate);

            return await query.ToListAsync(ct);
        }

	}
}
