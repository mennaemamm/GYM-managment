using GymManagement.DAL.Context;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext _dbContext;
        private readonly Dictionary<string, object> _repositories = [];
        public UnitOfWork(GymDbContext dbContext, ISessionRepository sessionRepository, IMembershipRepository membershipRepository, IBookingRepository bookingRepository)
        {
            _dbContext=dbContext;
            SessionRepository = sessionRepository;
            MembershipRepository = membershipRepository;
            BookingRepository = bookingRepository;
        }

        public ISessionRepository SessionRepository { get; }
        public IMembershipRepository MembershipRepository { get; }
        public IBookingRepository BookingRepository { get; }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            //Check TEntity ?? Trainer , Plan , Member
            var TypeName = typeof(TEntity).Name;
            //If Exist  In _repositories -> Return 
            if (_repositories.TryGetValue(TypeName, out object? value))
                return (IGenericRepository<TEntity>)value;

            else
            {//Not Fount In _repositories ->Crete , Store , Return

                var repo = new GenericRepository<TEntity>(_dbContext);
                _repositories[TypeName] = repo;
                return repo;
            }
        }


        public async Task<int> SaveChangesAsync(CancellationToken ct = default) 
            => await _dbContext.SaveChangesAsync(ct);
    }
}
