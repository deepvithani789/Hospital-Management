using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.API.Models.DTOs
{
    public class AddPrescriptionDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public string Medicines { get; set; }

        [Required]
        public string Dosage { get; set; }

        [Required]
        public int DurationInDays { get; set; }

        [Required]
        public string Instructions { get; set; }
        
        public DateTime DateIssued { get; set; } = DateTime.Now;

        [Required]
        public string Diagnosis { get; set; } // condition or illness being treated

        [Required]
        public string Symptoms { get; set; } //List of symptoms observed
    }
}
