using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.API.Models.DTOs
{
    public class AddDoctorDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Specialization { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email")]
        public string Email { get; set; }

        [Required]
        public string Qualification { get; set; }

        [Required]
        public string Experience { get; set; } //In Years

        [Required]
        public string Information { get; set; } //Heart related , Eyes related , etc
        public bool IsAvailable { get; set; }
    }
}
