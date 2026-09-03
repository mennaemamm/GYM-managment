using GymManagement.DAL.Models;

namespace GymManagement.DAL.Repositories.Interfaces
{
	public interface IBookingRepository : IGenericRepository<Booking>
	{
        public Task<List<Booking>> GetBySessionIdAsync(int sessionId, CancellationToken ct = default);

    }
}
