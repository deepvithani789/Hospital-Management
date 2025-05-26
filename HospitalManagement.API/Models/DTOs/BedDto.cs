namespace HospitalManagement.API.Models.DTOs
{
    public class BedDto
    {
        public int Id { get; set; }
        public string BedNumber { get; set; }
        public string RoomType { get; set; }
        public bool isOccupied { get; set; }
        public int? PatientId { get; set; }
        public string PatientName { get; set; }
    }
}
