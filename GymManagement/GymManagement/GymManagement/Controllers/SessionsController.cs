using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }


        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await _sessionService.GetAllSessionsAsync(ct);
            return View(sessions);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulatDropdownListAsync();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulatDropdownListAsync();
                return View(model);
            }

            var result = await _sessionService.CreateSessionAsync(model, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Session Created Successfully ";
                return RedirectToAction(nameof(Index));
            }
            else
                TempData["ErrorMessage"] = result.error ?? "An Error Ocured During Create Session";

            await PopulatDropdownListAsync();
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionByIdAsync(id);
            if (result.Success)
                return View(result.value);
            else
            {
                TempData["ErrorMessage"] = result.error ?? "An Error Ocured During Getting Session";
                return RedirectToAction(nameof(Index));
            }
        }
        private async Task PopulatDropdownListAsync()
        {
            ViewBag.Categories = new SelectList(await _sessionService.GetCategoriesForDropdownAsync(), "Id", "CategoryName");
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropdownAsync(), "Id", "Name");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionToUpdateAsync(id, ct);
            if (result.Success)
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropdownAsync(), "Id", "Name");
                return View(result.value);

            }
            else
            {
                TempData["ErrorMessage"] = result.error ?? "An Error Ocured During Getting Session";
                return RedirectToAction(nameof(Index));

            }
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropdownAsync(), "Id", "Name");
                return View(model);
            }
            var result = await _sessionService.UpdateSessionAsync(id, model, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Session Updated Successfully ";
                return RedirectToAction(nameof(Index));
            }
            else
                TempData["ErrorMessage"] = result.error ?? "An Error Ocured During Create Session";


            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropdownAsync(), "Id", "Name");
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionByIdAsync(id);
            if (result.Success)
                return View(result.value);
            else
            {
                TempData["ErrorMessage"] = result.error ?? "An Error Ocured During Getting Session";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _sessionService.RemoveSessionAsync(id, ct);

            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Session Deleted" :
                result.error ?? "An Error Ocured During Getting Session";

            return RedirectToAction(nameof(Index));


        }

    }
}