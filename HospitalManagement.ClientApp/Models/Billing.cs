namespace HospitalManagement.ClientApp.Models
{
    public class Billing
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? InsuranceCoverage { get; set; }
        public decimal FinalAmount { get; set; }
        public string PaymentMethod { get; set; } // Cash , Card , Insurance , Online
        public bool IsPaid { get; set; } = false; // Pending , Paid
        public DateTime BillingDate { get; set; } = DateTime.Now;
    }
}
