namespace HospitalManagement.API.Models.DTOs
{
    public class BillingDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? InsuranceCoverage { get; set; }
        public decimal FinalAmount { get; set; }
        public string PaymentMethod { get; set; } // Cash , Card , Insurance
        public bool IsPaid { get; set; }
        public DateTime BillingDate { get; set; }

        public PatientDto Patient { get; set; }
        public AppointmentDto Appointment { get; set; }
    }
}
