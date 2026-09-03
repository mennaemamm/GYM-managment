using GymManagement.DAL.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Models
{
    public abstract class GymUser : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public DateOnly DateOfBirth { get; set; } = default!;
        public Gender Gender { get; set; } = default!;
        public Address Address { get; set; } = default!;
    }

    [Owned]
    public class Address
    {
        public int BuildingNumber { get; set; } = default!;
        public string Street { get; set; } = default!;
        public string City { get; set; } = default!;
    }
}
