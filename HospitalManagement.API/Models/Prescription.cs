using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.API.Models
{
    public class Prescription
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Required]
        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

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
