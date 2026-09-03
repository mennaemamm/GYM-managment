using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllAsync(CancellationToken ct=default);

        Task<bool> CreateMemberAsync(CreateMemberViewModel member, CancellationToken ct = default);
        Task<MemberViewModel?> GetMemberDetailsByIdAsync(int memberId, CancellationToken ct = default);

        Task<HealthRecordViewModel?> GetMemberHealthRecordViewModel(int memberId, CancellationToken ct = default);
        Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default);

        Task<bool> UpdateMemberAsync(int id, MemberToUpdateViewModel model, CancellationToken ct);
        Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct);
    }
}
