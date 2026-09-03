using Microsoft.AspNetCore.Identity;

namespace GymManagement.DAL.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string FirstName { get; set; } = default!;
		public string LastName { get; set; } = default!;
	}
}
