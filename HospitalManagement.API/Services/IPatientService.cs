using HospitalManagement.API.Models;

namespace HospitalManagement.API.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient> GetByIdAsync(int id);
        Task<Patient> AddAsync(Patient patient);
        Task<Patient> UpdateAsync(Patient patient, int id);
        Task<Patient> DeleteAsync(int id);
    }
}
