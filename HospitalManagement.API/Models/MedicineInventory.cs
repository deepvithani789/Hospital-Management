namespace HospitalManagement.API.Models
{
    public class MedicineInventory
    {
        public int Id { get; set; }
        public string MedicineName { get; set; }
        public int QuantityInStock { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int LowStockThreshold { get; set; } = 10;
    }

    public class MedicineInventoryDto
    {
        public int Id { get; set; }
        public string MedicineName { get; set; }
        public int QuantityInStock { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int LowStockThreshold { get; set; }
    }

    public class MedicineStockUpdateDto
    {
        public int MedicineId { get; set; }
        public int QuantityChange { get; set; } // Positive for stock in, negative for stock out
    }

    public class DispenseMedicineRequestDto
    {
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public List<DispenseItemDto> Items { get; set; }
    }

    public class DispenseItemDto
    {
        public int MedicineId { get; set; }
        public int Quantity { get; set; }
    }

    public class DispenseResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

}