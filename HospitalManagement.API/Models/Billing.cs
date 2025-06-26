using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.API.Models
{
    public class Billing
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Patient")]
        public int PatientId { get; set; }

        [Required]
        public Patient Patient { get; set; }

        [Required]
        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }

        [Required]
        public Appointment Appointment { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public decimal? Discount { get; set; }

        public decimal? InsuranceCoverage { get; set; }

        //[Required]
        public decimal FinalAmount { get; set; }

        [Required]
        public string PaymentMethod { get; set; } // Cash , Card , Insurance

        [Required]
        public bool IsPaid { get; set; } = false; // Pending , Paid

        public DateTime BillingDate { get; set; } = DateTime.Now;
    }
}
