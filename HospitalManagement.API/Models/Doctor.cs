namespace HospitalManagement.API.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Qualification { get; set; }
        public string Experience { get; set; } //In Years
        public string Information { get; set; } //Heart related , Eyes related , etc
        public bool IsAvailable { get; set; }
    }
}
