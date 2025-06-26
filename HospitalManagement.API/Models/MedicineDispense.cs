namespace HospitalManagement.API.Models
{
    public class MedicineDispense
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int MedicineId { get; set; }
        public MedicineInventory Medicine { get; set; }

        public int Quantity { get; set; }
        public DateTime DispensedOn { get; set; } = DateTime.UtcNow;
    }
}
