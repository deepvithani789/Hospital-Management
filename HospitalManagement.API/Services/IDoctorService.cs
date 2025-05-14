using HospitalManagement.API.Models;

namespace HospitalManagement.API.Services
{
    public interface IDoctorService
    {
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<Doctor> GetByIdAsync(int id);
        Task<Doctor> AddAsync(Doctor doctor);
        Task<Doctor> UpdateAsync(Doctor doctor, int id);
        Task<Doctor> DeleteAsync(int id);
        Task<List<Doctor>> GetAvailableDoctorsAsync();
    }
}
