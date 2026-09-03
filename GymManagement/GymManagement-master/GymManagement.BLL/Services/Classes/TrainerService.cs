using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;

namespace GymManagement.BLL.Services.Classes
{
	public class TrainerService : ITrainerService
	{

        private readonly IUnitOfWork _unitOfWork;

        public TrainerService(IUnitOfWork unitOfWork)
		{
	
            _unitOfWork=unitOfWork;
        }
        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers =await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct:ct);

			return trainers.Select(t => new TrainerViewModel()
			{
				Id = t.Id,
				Name = t.Name,
				Email = t.Email,
				Phone = t.Phone,
				Specialties = t.Specialty.ToString()
			});
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);
            if (trainer is null) return null;

			return new TrainerViewModel()
			{
				Name = trainer.Name,
				Specialties = trainer.Specialty.ToString(),
				Email = trainer.Email,
				Phone = trainer.Phone,
				DateOfBirth = trainer.DateOfBirth.ToString(),
				Address = $"{trainer.Address.BuildingNumber} - {trainer.Address.Street} - {trainer.Address.City}"

			};
        }


        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer =await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId,ct);
            if (trainer is null) return null;

			return new TrainerToUpdateViewModel()
			{
				Name = trainer.Name,
				Specialties = trainer.Specialty,
				Email = trainer.Email,
				Phone = trainer.Phone,
				City = trainer.Address.City,
				Street = trainer.Address.Street,
				BuildingNumber = trainer.Address.BuildingNumber
			};



        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel vm, CancellationToken ct = default)
        {
            try
			{

				if (await IsEmailExists(vm.Email) ||await IsPhoneExists(vm.Phone)) return false;
				var TrainerEntity = new Trainer()
				{
				    Name = vm.Name,
				    DateOfBirth = vm.DateOfBirth,
				    Gender = vm.Gender,
				    Phone = vm.Phone,
				    Email = vm.Email,
                    Specialty = vm.Specialties,
				    Address	= new Address()
				    {
				    	City = vm.City,
				    	Street = vm.Street,
				    	BuildingNumber = vm.BuildingNumber
				    }
				};

            _unitOfWork.GetRepository<Trainer>().AddAsync(TrainerEntity) ;

			return await _unitOfWork.SaveChangesAsync()>0;

            }
			catch (Exception)
			{
			return false;
			}
		}

		

        public async Task<bool> RemoveTrainerAsync(int trainerId, CancellationToken ct = default)
        {
			var trainerToRemove =await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId);
			if (trainerToRemove is null ||await HasActiveSessions(trainerId,ct)) return false;
             _unitOfWork.GetRepository<Trainer>().DeleteAsync(trainerToRemove);
			return await _unitOfWork.SaveChangesAsync(ct)>0;
		}
  
        public async Task<bool> UpdateTrainerDetailsAsync(TrainerToUpdateViewModel model, int trainerId, CancellationToken ct = default)
		{

			var trainerRepo = _unitOfWork.GetRepository<Trainer>();
            var trainer = await trainerRepo.GetByIdAsync(trainerId);
			if (trainer is null) return false;

			 if(await trainerRepo.AnyAsync(t => t.Email ==model.Email && t.Id != trainerId, ct))
				return false;

			 if (await trainerRepo.AnyAsync(t => t.Phone ==model.Phone && t.Id != trainerId, ct))
				return false;

			trainer.Email = model.Email;
			trainer.Phone = model.Phone;
			trainer.Specialty = model.Specialties;
			trainer.UpdatedAt = DateTime.Now;
			trainer.Address.City = model.City;
			trainer.Address.Street = model.Street;
			trainer.Address.BuildingNumber = model.BuildingNumber;

            trainerRepo.UpdateAsync(trainer);

            return await _unitOfWork.SaveChangesAsync(ct)>0;


        }

        #region Helper Methods
        private async Task<bool> IsEmailExists(string email)
		{
			var existing = await _unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Email == email);
			return existing;
		}
		private async Task<bool> IsPhoneExists(string phone)
		{
			var existing = await _unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Phone == phone);
			return existing;
		}
		private async Task<bool> HasActiveSessions(int Id , CancellationToken ct)
		{
			var activeSessions = await _unitOfWork.GetRepository<Session>().AnyAsync(s => s.TrainerId == Id && s.StartDate > DateTime.Now, ct);
            return activeSessions;
		}
		#endregion
	}
}
