using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace GymManagement.DAL.Repositories.Interfaces
{
	public interface IMembershipRepository : IGenericRepository<Membership>
	{
        Task<List<Membership>> GetAllMembershipsWithMemberAndPlanAsync(Expression<Func<Membership, bool>>? predicate = null,CancellationToken ct = default);
    }
}
