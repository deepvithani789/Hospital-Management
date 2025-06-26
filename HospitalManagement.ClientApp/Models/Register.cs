namespace HospitalManagement.ClientApp.Models
{
    public class Register
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }  // Can be "Patient" "Admin" "Doctor"
    }
}
