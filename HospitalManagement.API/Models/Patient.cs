namespace HospitalManagement.API.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public string BloodGroup { get; set; }
        public string ChronicDisease { get; set; }
        public string AdhaarCard { get; set; }
    }
}
