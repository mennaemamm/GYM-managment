using GymManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Models
{
    public class Membership: BaseEntity
    {
        public int MemberId { get; set; }
        public Member Member { get; set; } = default!;

        public Plan  Plan { get; set; } = default!;
        public int PlanId { get; set; }


        public DateTime EndDate { get; set; }


        public string Status => EndDate > DateTime.Now ? "Active" : "Expired";
        public bool IsActive => EndDate > DateTime.Now ;
    }
}
