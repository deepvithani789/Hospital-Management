namespace HospitalManagement.API.Models.DTOs
{
    public class AddBedDto
    {
        public string BedNumber { get; set; }
        public string RoomType { get; set; }
    }

    public class AssignBedDto
    {
        public int BedId { get; set; }
        public int PatientId { get; set; }
    }

}
