using Microsoft.AspNetCore.Identity;

namespace HospitalManagement.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Role { get; set; }
        public DateTime LastLoginTime { get; set; }
        public bool IsApproved { get; set; }
    }
}
