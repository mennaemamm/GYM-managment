using GymManagement.BLL.Services.AttachmentService;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttachmentService _attachmentService;

        public MembersController(IMemberService memberService, IAttachmentService attachmentService)
        {
            _memberService = memberService;
            _attachmentService = attachmentService;
        }
        
        //List All Members
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetAllAsync(ct: ct);
            return View(members);
        }

        [HttpGet]
        public IActionResult Create()
            => View();

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);
            var result = await _memberService.CreateMemberAsync(model, ct);


            if (result)
                TempData["SuccessMessage"] = "Member Added Successfully ";
            else
                TempData["ErrorMessage"] = "An Error Ocured During Creation";

            return RedirectToAction(nameof(Index));//Temp
        }

        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            //Get Member By Id
            var member = await _memberService.GetMemberDetailsByIdAsync(id, ct: ct);
            //Check If Member Is Null => Return Index With Error Msg
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
            //Member Fount -> Return View With Data
        }

        [HttpGet]
        public async Task<IActionResult> Picture(int id)
        {
            var member = await _memberService.GetMemberDetailsByIdAsync(id);
            if (member is null || string.IsNullOrWhiteSpace(member.Photo)) return NotFound();


            var result = _attachmentService.GetFile(member.Photo, "MemberPhoto");
            if (result is null) return NotFound();
            return File(result.Value.stream, result.Value.contentType);
        }


        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var result = await _memberService.GetMemberHealthRecordViewModel(id, ct);
            if (result is null)
            {
                TempData["ErrorMessage"] = "Health Record Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(result);
        }


        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberToUpdateAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member  Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute] int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _memberService.UpdateMemberAsync(id, model, ct);
            if (result)
                TempData["SuccessMessage"] = "Member Updated Successfully ";
            else
                TempData["ErrorMessage"] = "An Error Ocured During Update";

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct)
        {
            var resutl = await _memberService.DeleteMemberAsync(id, ct);
            if (resutl)
            {
                TempData["SuccessMessage"] = "Member Deleted Successfully";
            }
            else TempData["ErrorMessage"] = "Failed To Remove Member";

            return RedirectToAction(nameof(Index));
        }

    }
}
