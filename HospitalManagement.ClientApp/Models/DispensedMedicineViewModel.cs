namespace HospitalManagement.ClientApp.Models
{
    public class DispensedMedicineViewModel
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public DateTime DispensedOn { get; set; }
    }
}
