using GymManagement.DAL.Context;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.DAL.Repositories.Classes
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
	{
		private readonly GymDbContext _dbContext;

		public BookingRepository(GymDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

        public Task<List<Booking>> GetBySessionIdAsync(int sessionId, CancellationToken ct = default)
            => _dbContext.Bookings.AsNoTracking().Include(b => b.Member).Where(b => b.SessionId == sessionId).ToListAsync(ct);

      
    }
}
