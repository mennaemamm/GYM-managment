using AutoMapper;
using GymManagement.BLL.Services.AttachmentService;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper, IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }


        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            //Check Email
            var emailExisit = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email);
            //Check Phone
            var phoneExisit = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone);


            if (emailExisit || phoneExisit) return false;

            //Upload Photo
            var uploadedPhotoName = await _attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "MemberPhoto");


            var member = _mapper.Map<Member>(model);
            member.Photo = uploadedPhotoName;

            _unitOfWork.GetRepository<Member>().AddAsync(member);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
                return true;
            else
                _attachmentService.Delete(uploadedPhotoName, "MemberPhoto");
                return false;

        }

        public async Task<IEnumerable<MemberViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any()) return Enumerable.Empty<MemberViewModel>();

            return _mapper.Map<IEnumerable<MemberViewModel>>(members);
        }

        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (member is null) return null;

            var model = _mapper.Map<MemberViewModel>(member);

            var activeMembership = await _unitOfWork.GetRepository<Membership>().FirstOrDefaultAsync(m => m.MemberId == memberId && m.EndDate > DateTime.Now);
            
            if (activeMembership is not null)
            {
                var activePlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMembership.PlanId, ct);

                model.PlanName = activePlan?.Name;
                model.MembershipStartDate = activeMembership.CreatedAt.ToString();
                model.MembershipEndDate = activeMembership.EndDate.ToString();

            }

            return model;

        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordViewModel(int memberId, CancellationToken ct = default)
        {
            var record = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(h => h.MemberId == memberId, ct: ct);
            if (record is null) return null;
            else return _mapper.Map<HealthRecordViewModel>(record);
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct: ct);
            if (member is null) return null;
            else return _mapper.Map<MemberToUpdateViewModel>(member);
        }

        public async Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId);
            if (member is null) return false;

            //Cannot Remove Member With Future Booking

            var hasFutureBooking = await _unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == memberId &&
                                                                          b.Session.StartDate > DateTime.Now);

            if (hasFutureBooking) return false;

            _unitOfWork.GetRepository<Member>().DeleteAsync(member);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        //private const string MembersPhotosFolder = "Images/Members"; // match whatever folder convention you use elsewhere

        public async Task<bool> UpdateMemberAsync(int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id);
            if (member is null) return false;

            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id != id);
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != id);

            if (emailExists || phoneExists) return false;

            var oldPhoto = member.Photo;

            _mapper.Map(model, member); // Name, Photo ignored; Address + UpdatedAt handled in AfterMap

            if (model.PhotoFile is not null && model.PhotoFile.Length > 0)
            {
                await using var stream = model.PhotoFile.OpenReadStream();
                var storedFileName = await _attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "MemberPhoto");

                if (storedFileName is null) return false; // upload rejected: bad extension / too large / IO error

                member.Photo = storedFileName;

                if (!string.IsNullOrWhiteSpace(oldPhoto))
                    _attachmentService.Delete(oldPhoto, "MemberPhoto");
            }
            else
            {
                member.Photo = oldPhoto; // no new upload — keep existing photo
            }

            _unitOfWork.GetRepository<Member>().UpdateAsync(member, ct);
            var result = await _unitOfWork.SaveChangesAsync();

            return result > 0;
        }
    }
}
