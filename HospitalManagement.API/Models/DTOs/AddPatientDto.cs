using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.API.Models.DTOs
{
    public class AddPatientDto
    {
        [Required]
        public string Name { get; set; }
        public string Email { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string ContactNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string BloodGroup { get; set; }
        
        [Required]
        public string ChronicDisease { get; set; }

        [Required]
        public string AdhaarCard { get; set; }

    }
}
