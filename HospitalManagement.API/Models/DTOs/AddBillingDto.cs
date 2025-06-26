using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.API.Models.DTOs
{
    public class AddBillingDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? InsuranceCoverage { get; set; }

        //[Required]
        //public decimal FinalAmount { get; set; }

        [Required]
        public string PaymentMethod { get; set; } // Cash , Card , Insurance

        [Required]
        public bool IsPaid { get; set; }
        public DateTime BillingDate { get; set; }
    }
}
