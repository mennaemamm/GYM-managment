using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
   
            _unitOfWork=unitOfWork;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans =await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct:ct);
            return plans.Select(p => new PlanViewModel()
            {
                Id=p.Id,
                Name=p.Name,
                Description=p.Description,
                DurationDays=p.DurationDays,
                IsActive=p.IsActive,
                Price=p.Price,
            });

        }

        public async Task<PlanViewModel?> GetPlanByIdAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id,ct:ct);
            if (plan is null) return null;

            return new PlanViewModel()
            {
                Id= plan.Id,
                Name= plan.Name,
                Price= plan.Price,
                Description= plan.Description,
                DurationDays= plan.DurationDays,
                IsActive =plan.IsActive
            };
        }

        #region Update
        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int id , CancellationToken ct =default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct: ct);
            if (plan is null  || !plan.IsActive  ||
                await HasActiveMembershipsAsync(id, ct)) 
                return null;

            //Active Plan

            return new UpdatePlanViewModel()
            {
                PlanName = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationDays,
                Price = plan.Price
            };
        }

  
        public async Task<bool> UpdatePlanAsync(int id, UpdatePlanViewModel planViewModel , CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct: ct);
            if (plan is null || await HasActiveMembershipsAsync(id , ct) ) return false;

            plan.Description=planViewModel.Description;
            plan.Price=planViewModel.Price;
            plan.DurationDays= planViewModel.DurationDays;
            plan.UpdatedAt=DateTime.Now;

             _unitOfWork.GetRepository<Plan>().UpdateAsync(plan);
            return await _unitOfWork.SaveChangesAsync(ct)>0;

        }
        #endregion
        public async Task<bool> ToggleActivationStatusAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct: ct);

            if(plan is null ||await HasActiveMembershipsAsync(id,ct))  return false; 
            plan.IsActive = plan.IsActive == true ? false : true;
            plan.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Plan>().UpdateAsync(plan);

            return await _unitOfWork.SaveChangesAsync(ct)>0;


        }

        #region Helper Methods
        private  async Task<bool> HasActiveMembershipsAsync(int planId , CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId== planId  && m.EndDate> DateTime.Now , ct);

        }

        #endregion


    }
}
