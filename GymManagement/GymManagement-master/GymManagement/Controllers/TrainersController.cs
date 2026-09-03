using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Controllers
{
	public class TrainersController : Controller
	{
		private readonly ITrainerService _trainerService;

		public TrainersController(ITrainerService trainerService)
		{
			_trainerService = trainerService;
		}


		// GET: Trainer/Index
		public async Task<IActionResult> Index()
		{
			var trainers =await _trainerService.GetAllTrainersAsync();
			return View(trainers);
		}


		public IActionResult Create()
		{
			return View();
		}

		// POST: Trainer/Create
		[HttpPost]
		public async Task<IActionResult> CreateTrainer(CreateTrainerViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(nameof(Create), model);
			}


			var result =await _trainerService.CreateTrainerAsync(model);

			if (result)
			{
				TempData["SuccessMessage"] = "Trainer created successfully!";
				return RedirectToAction(nameof(Index));
			}
			else
			{
				ModelState.AddModelError("DataMissed", "Trainer with this email or phone already exists.");
				return View(nameof(Create), model);
			}
		}

		// GET: Trainers/Details/5
		public async Task<IActionResult> Details(int id)
		{
			var trainer =await _trainerService.GetTrainerDetailsAsync(id);

			if (trainer == null)
			{
				TempData["ErrorMessage"] = "Trainer not found.";
				return RedirectToAction(nameof(Index));
			}

			return View(trainer);
		}

		public async Task<IActionResult> Edit(int id)
		{
			var trainer =await _trainerService.GetTrainerToUpdateAsync(id);

			if (trainer == null)
			{
				TempData["ErrorMessage"] = "Trainer not found.";
				return RedirectToAction(nameof(Index));
			}
			return View(trainer);
		}

		// POST: Trainer/Edit/5
		[HttpPost]
		public async Task<IActionResult> Edit(int id, TrainerToUpdateViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var result =await _trainerService.UpdateTrainerDetailsAsync(model, id);

			if (result)
			{
				TempData["SuccessMessage"] = "Trainer updated successfully!";
			}
			else
			{
				TempData["ErrorMessage"] = "Failed to update trainer.";
			}

			return RedirectToAction(nameof(Index));

		}


		// GET: Trainer/Delete/5
		public async Task<IActionResult> Delete(int id)
		{
			var trainer =await _trainerService.GetTrainerDetailsAsync(id);

			if (trainer == null)
			{
				TempData["ErrorMessage"] = "Trainer not found.";
				return RedirectToAction(nameof(Index));
			}

			ViewBag.TrainerId = trainer.Id;
			return View();
		}

		// POST: Trainer/DeleteConfirmed
		[HttpPost]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var result =await _trainerService.RemoveTrainerAsync(id);

			if (result)
			{
				TempData["SuccessMessage"] = "Trainer deleted successfully!";
			}
			else
			{
				TempData["ErrorMessage"] = "Failed to delete trainer";
			}


			return RedirectToAction(nameof(Index));
		}
	}
}