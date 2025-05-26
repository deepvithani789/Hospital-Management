using HospitalManagement.API.Models;

namespace HospitalManagement.API.Services
{
    public interface IBedService
    {
        Task<List<Bed>> GetAllBedsAsync();
        Task<Bed> GetBedByIdAsync(int id);
        Task<Bed> AddBedAsync(Bed bed);
        Task<Bed> UpdateBedAsync(Bed bed, int id);
        Task<Bed> DeleteBedAsync(int id);
        Task<bool> AssignBedToPatientAsync(int bedId, int patientId);
        Task<bool> ReleaseBedForPatientAsync(int patientId);
        Task<Bed> GetBedByBedNumber(string bedNumber);
    }
}
