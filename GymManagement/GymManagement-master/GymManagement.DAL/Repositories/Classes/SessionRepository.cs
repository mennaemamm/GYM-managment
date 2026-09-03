using GymManagement.DAL.Context;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext _dbContext;

        public SessionRepository(GymDbContext gymDbContext):base(gymDbContext)
        {
            _dbContext = gymDbContext;
        }
        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategory(CancellationToken ct = default)
        {
            var query = _dbContext.Sessions.AsNoTracking().Include(s => s.Trainer).Include(s => s.Category);
            return await query.ToListAsync();
        }

        public async Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default)
        {
            return await _dbContext.Bookings.AsNoTracking().CountAsync(b => b.SessionId == sessionId);
        }

        public async Task<Session?> GetSessionByIdWithTrainerAndCategoryAsync(int sessionId, CancellationToken ct = default)
        {
            return await _dbContext.Sessions.AsNoTracking()
                .Include(s => s.Trainer)
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == sessionId, ct);
        }

        public async Task<Session?> GetSessionWithTrainerAndCategory(int sessionId, CancellationToken ct = default)
        {
            var query = _dbContext.Sessions.AsNoTracking().Include(s => s.Trainer).Include(s => s.Category);
            return await query.FirstOrDefaultAsync(s=>s.Id==sessionId , ct);
        }
    }
}
