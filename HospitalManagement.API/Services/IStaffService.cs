using HospitalManagement.API.Models;

namespace HospitalManagement.API.Services
{
    public interface IStaffService
    {
        Task<IEnumerable<Staff>> GetAllAsync();
        Task<Staff> GetByIdAsync(int id);
        Task<Staff> AddAsync(Staff staff);
        Task<Staff> UpdateAsync(Staff staff, int id);
        Task<Staff> DeleteAsync(int id);
    }
}
