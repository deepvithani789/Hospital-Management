namespace HospitalManagement.API.Models.DTOs
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = "Pending"; //Pending , Approved , Cancelled
        public string AppointmentType { get; set; } //Type of appointment
        public string ReasonForVisit { get; set; }

        public PatientDto Patient { get; set; }
        public DoctorDto Doctor { get; set; }
    }
}
