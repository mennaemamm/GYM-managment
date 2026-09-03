using GymManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategory(CancellationToken ct = default);
        Task<Session?> GetSessionWithTrainerAndCategory(int sessionId,CancellationToken ct = default);
        Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default);

        Task<Session?> GetSessionByIdWithTrainerAndCategoryAsync(int sessionId, CancellationToken ct = default);
    }
}