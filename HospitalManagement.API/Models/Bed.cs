namespace HospitalManagement.API.Models
{
    public class Bed
    {
        public int Id { get; set; }
        public string BedNumber { get; set; }
        public string RoomType { get; set; }
        public bool isOccupied { get; set; }
        public int? PatientId { get; set; }
        public Patient? Patient { get; set; }
    }
}
