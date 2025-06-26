using HospitalManagement.API.Models;

namespace HospitalManagement.API.Services
{
    public interface IPharmacyService
    {
        Task<List<MedicineInventory>> GetAllMedicinesAsync();
        Task<MedicineInventory> GetMedicineById(int id);
        Task<MedicineInventory> AddMedicineAsync(MedicineInventory medicine);
        Task<MedicineInventory> UpdateMedicineAsync(MedicineInventory medicine);
        Task<MedicineInventory> UpdateStockAsync(int medicineId, int quantityChange);
        Task<List<MedicineInventory>> GetLowStockedMedicinesAsync();
        Task<List<MedicineInventory>> GetExpiredMedicinesAsync();
        Task<DispenseResult> DispenseMedicineAsync(DispenseMedicineRequestDto request);

    }
}
