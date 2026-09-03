using GymManagement.DAL.Context;
using GymManagement.DAL.Repositories.Interfaces;
using GymManagement.DAL.Repositories.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagement.DAL.Models;

namespace GymManagement.Controllers
{
    public class PlanController : Controller
    {
        private readonly IGenericRepository<Plan> _planRepository;

        public PlanController(IGenericRepository<Plan> planRepository)//Constructor Injection
        {
            _planRepository = planRepository;
        }
        //Get : BaseUrl/Plans/Index
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var plans = await _planRepository.GetAllAsync(ct:ct);
            return View(plans);
        }

        //Get : BaseUrl/Plans/Details/1

        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetByIdAsync(id ,ct);

            if (plan is null)  
                return RedirectToAction(nameof(Index));
            else 
                return View(plan);

        }



    }
}
