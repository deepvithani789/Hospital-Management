using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.ClientApp.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }

        [FutureDate(ErrorMessage = "Appointment date must be today or in the future.")]
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } //Scheduled , Approved , Cancelled
        public string AppointmentType { get; set; } //Type of appointment
        public string ReasonForVisit { get; set; }
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var date = value as DateTime? ?? new DateTime();
            return date >= DateTime.UtcNow;
        }
    }
}
