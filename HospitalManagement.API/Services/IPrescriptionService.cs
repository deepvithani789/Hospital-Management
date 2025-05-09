using HospitalManagement.API.Models;

namespace HospitalManagement.API.Services
{
    public interface IPrescriptionService
    {
        Task<IEnumerable<Prescription>> GetAllAsync();
        Task<Prescription> GetByIddAsync(int id);
        Task<Prescription> AddAsync(Prescription prescription);
        Task<Prescription> UpdateAsync(Prescription prescription, int id);
        Task<Prescription> DeleteAsync(int id);
    }
}
