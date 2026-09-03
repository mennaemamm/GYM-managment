using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ISessionService
    {

        Task<Result<SessionViewModel?>> GetSessionByIdAsync(int sessionId  , CancellationToken ct = default);
        Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct=default);

        Task<Result> CreateSessionAsync(CreateSessionViewModel model ,CancellationToken ct=default);



        Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropdownAsync(CancellationToken ct = default);
        Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropdownAsync(CancellationToken ct = default);


        Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int id,CancellationToken ct = default);
        Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default);



        Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct);

    }
}
