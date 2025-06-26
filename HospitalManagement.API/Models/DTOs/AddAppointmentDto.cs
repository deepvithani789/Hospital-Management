using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.API.Models.DTOs
{
    public class AddAppointmentDto
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; //Pending , Approved , Cancelled

        [Required]
        public string AppointmentType { get; set; } //Type of appointment

        [Required]
        public string ReasonForVisit { get; set; }
    }
}
