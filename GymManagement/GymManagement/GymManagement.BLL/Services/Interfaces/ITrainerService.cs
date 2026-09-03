using GymManagement.BLL.ViewModels.TrainerViewModels;

namespace GymManagement.BLL.Services.Interfaces
{
	public interface ITrainerService
	{

        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct=default);
        Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default);
        Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default);

        Task<bool> CreateTrainerAsync(CreateTrainerViewModel createTrainer, CancellationToken ct = default);
		Task<bool> UpdateTrainerDetailsAsync(TrainerToUpdateViewModel updatedTrainer, int trainerId, CancellationToken ct = default);
		Task<bool> RemoveTrainerAsync(int trainerId, CancellationToken ct = default);
	}
}
