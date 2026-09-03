using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.MembershipViewModels;

namespace GymManagement.BLL.Services.Interfaces
{
	public interface IMembershipService
	{
        Task<IEnumerable<MemberShipViewModel>> GetAllMembershipsAsync(CancellationToken ct = default);
        Task<IEnumerable<PlanSelectListViewModel>> GetPlansForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropDownAsync(CancellationToken ct = default);
        Task<Result> CreateMembershipAsync(CreateMemberShipViewModel model, CancellationToken ct = default);
        Task<Result> DeleteActiveMembershipAsync(int memberId, CancellationToken ct = default);

    }
}
