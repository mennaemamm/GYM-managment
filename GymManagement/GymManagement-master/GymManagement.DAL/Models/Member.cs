using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Models
{
    public class Member : GymUser
    {
        public string? Photo { get; set; }

        public HealthRecord HealthRecord { get; set; } = default!;
        public ICollection<Membership> Memberships { get; set; } = default!;
        public ICollection<Booking> MemberSession { get; set; } = default!;

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
