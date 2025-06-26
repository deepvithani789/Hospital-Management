namespace HospitalManagement.ClientApp.Models
{
    public class DispenseMedicineRequestDto
    {
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public List<DispenseItemDto> Items { get; set; } = new List<DispenseItemDto> { new DispenseItemDto() };
    }

    public class DispenseItemDto
    {
        public int MedicineId { get; set; }
        public int Quantity { get; set; }
    }
}
