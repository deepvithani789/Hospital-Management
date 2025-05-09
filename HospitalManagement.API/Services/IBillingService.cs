using HospitalManagement.API.Models;

namespace HospitalManagement.API.Services
{
    public interface IBillingService
    {
        Task<IEnumerable<Billing>> GetAllASync();
        Task<Billing> GetByIdAsync(int id);
        Task<Billing> AddASync(Billing billing);
        Task<Billing> UpdateASync(Billing billing, int id);
        Task<Billing> DeleteASync(int id);
        Task<byte[]> GenerateInvoice(int id);
    }
}
