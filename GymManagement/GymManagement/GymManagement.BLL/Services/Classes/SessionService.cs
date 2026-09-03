using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Models.Enums;
using GymManagement.DAL.Repositories.Interfaces;
using GymManagement.DAL.Repositories.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate<= model.StartDate) return Result.Validation("End Date Must Be After Start Date ");
            if (model.StartDate<= DateTime.Now) return Result.Validation("Start Date Must Be In The Future"); ;
            if (model.Capacity<1 || model.Capacity > 25) return  Result.Validation("Capacity Must Be Between 1 And 25 ");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer is null) return Result.NotFound("Trainer Not Found"); 

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId);
            if (category is null) return  Result.NotFound("Category Not Found");

            var isValid = Enum.TryParse<Specialty>(category.CategoryName, true, out var CategorySpeciality);
            if (!isValid || trainer.Specialty!= CategorySpeciality) return Result.Validation("Cannot Create Session To This Trainer "); 

            var session = _mapper.Map<Session>(model);

            _unitOfWork.GetRepository<Session>().AddAsync(session);
            return await _unitOfWork.SaveChangesAsync()>0 ? Result.Ok() : Result.Fail("Failed To Create Session");
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategory(ct: ct);
            //Categoty , Trainer : Navigational Property Not Loaded By Default
            if (sessions is null || !sessions.Any()) return null;

            var mappedSessions = sessions.Select(s => new SessionViewModel()
            {
                Id= s.Id,
                Capacity= s.Capacity,
                CategoryName=s.Category.CategoryName,
                TrainerName=s.Trainer.Name,
                Description=s.Description,
                EndDate=s.EndDate,
                StartDate=s.StartDate,
                //AvailableSlots
            });

            foreach (var session in mappedSessions)
            {
                session.AvailableSlots=session.Capacity - 
                    await _unitOfWork.SessionRepository
                    .GetCountOfBookedSlotsAsync(session.Id);
                
            }

            return mappedSessions;

        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropdownAsync(CancellationToken ct = default)
        {
            var result = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct:ct);

            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(result);
        }
        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropdownAsync(CancellationToken ct = default)
        {
            var result = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);

            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(result);
        }


        public async Task<Result<SessionViewModel?>> GetSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategory(sessionId);

            if (session is null) return Result<SessionViewModel?>.NotFound("Session Is Not Found");
            else
            {
                var mappedSession = _mapper.Map<SessionViewModel>(session);
                mappedSession.AvailableSlots = mappedSession.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId,ct);
                return Result<SessionViewModel?>.Ok(mappedSession);
            }

        }


        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int id, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id);
            if (session is null) return Result<UpdateSessionViewModel>.NotFound($"Session  #{id} Is Not Found");

            if (session.StartDate <= DateTime.Now) return Result<UpdateSessionViewModel>.Fail($"Cannot Update Old Sessions");
            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);

            if(bookingCount>0) return Result<UpdateSessionViewModel>.Fail($"Cannot Update  Sessions That Already Has Booking");


            var mappedSession = _mapper.Map<UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.Ok(mappedSession);

        }


        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id);
            if (session is null) return Result.NotFound($"Session  #{id} Is Not Found");


            if (session.StartDate <= DateTime.Now) return Result.Fail($"Cannot Update Old Sessions");

            if (model.EndDate <= model.StartDate) return Result.Validation("End Date Must Be After Start Date ");

            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);
            if (bookingCount>0) return Result.Fail($"Cannot Update  Sessions That Already Has Booking");


            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer is null) return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId);
            if (category is null) return Result.NotFound("Category Not Found");

            var isValid = Enum.TryParse<Specialty>(category.CategoryName, true, out var CategorySpeciality);
            if (!isValid || trainer.Specialty!= CategorySpeciality) return Result.Validation("Cannot Update Session With This Trainer ");


            _mapper.Map(model, session);
            session.UpdatedAt=DateTime.Now;

            _unitOfWork.SessionRepository.UpdateAsync(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);


            return result>0 ? Result.Ok() : Result.Fail("Failed To Ipdate Session");
        }

        public async Task<Result> RemoveSessionAsync(int id, CancellationToken ct)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id);
            if (session is null) return Result.NotFound($"Session  #{id} Is Not Found");

            if (session.StartDate >= DateTime.Now) return Result.Fail($"Cannot Delete Ongoing Sessions");
            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);

            if (bookingCount>0) return Result.Fail($"Cannot Delete  Sessions That Already Has Booking");


            _unitOfWork.SessionRepository.DeleteAsync(session);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res>0 ? Result.Ok(): Result.Fail("Failed To Delete Session");
        }
    }
}
