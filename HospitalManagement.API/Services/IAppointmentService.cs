using HospitalManagement.API.Models;

namespace HospitalManagement.API.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment> GetByIdAsync(int id);
        Task<Appointment> AddAsync(Appointment appointment);
        Task<Appointment> UpdateAsync(Appointment appointment, int id);
        Task<Appointment> DeleteAsync(int id);
    }
}
