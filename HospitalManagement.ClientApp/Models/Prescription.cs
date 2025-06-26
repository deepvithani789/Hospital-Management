using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.ClientApp.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int AppointmentId { get; set; }
        public string Medicines { get; set; }
        public string Dosage { get; set; }
        public int DurationInDays { get; set; }
        public string Instructions { get; set; }
        public DateTime DateIssued { get; set; } = DateTime.Now;
        public string Diagnosis { get; set; } // condition or illness being treated
        public string Symptoms { get; set; } //List of symptoms observed
    }
}