namespace HospitalManagement.ClientApp.Models
{
    public class MedicineInventoryViewModel
    {
        public int Id { get; set; }
        public string MedicineName { get; set; }
        public int QuantityInStock { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int LowStockThreshold { get; set; }
    }
}
