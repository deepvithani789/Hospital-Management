namespace HospitalManagement.API.Models
{
    public class Staff
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Position { get; set; } //Nurse , Wardboy , etc
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Shift { get; set; } //Morning , Evening , Night
        public decimal Salary { get; set; }
        public string AdharCard { get; set; }
        public DateTime DateJoined { get; set; } = DateTime.UtcNow;
    }
}
