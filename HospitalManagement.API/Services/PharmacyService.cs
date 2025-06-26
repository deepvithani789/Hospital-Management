using HospitalManagement.API.Data;
using HospitalManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.API.Services
{
    public class PharmacyService : IPharmacyService
    {
        private readonly AppDbContext _context;
        public PharmacyService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<MedicineInventory> AddMedicineAsync(MedicineInventory medicine)
        {
            medicine.ExpiryDate = DateTime.SpecifyKind(medicine.ExpiryDate, DateTimeKind.Utc);
            await _context.MedicineInventories.AddAsync(medicine);
            await _context.SaveChangesAsync();
            return medicine;
        }

        public async Task<DispenseResult> DispenseMedicineAsync(DispenseMedicineRequestDto request)
        {
            if (request == null || request.Items == null || !request.Items.Any())
            {
                return new DispenseResult { Success = false, Message = "No medicines to dispense." };
            }

            // Load all medicines involved in dispense to check stock
            var medicineIds = request.Items.Select(i => i.MedicineId).ToList();
            var medicines = await _context.MedicineInventories
                .Where(m => medicineIds.Contains(m.Id))
                .ToListAsync();

            foreach (var item in request.Items)
            {
                var medicine = medicines.FirstOrDefault(m => m.Id == item.MedicineId);
                if (medicine == null)
                {
                    return new DispenseResult { Success = false, Message = $"Medicine ID {item.MedicineId} not found." };
                }

                if (item.Quantity <= 0)
                {
                    return new DispenseResult { Success = false, Message = $"Invalid quantity for Medicine {medicine.MedicineName}." };
                }

                if (medicine.QuantityInStock < item.Quantity)
                {
                    return new DispenseResult
                    {
                        Success = false,
                        Message = $"Insufficient stock for {medicine.MedicineName}. Available: {medicine.QuantityInStock}, Requested: {item.Quantity}"
                    };
                }
            }

            // Deduct stock
            foreach (var item in request.Items)
            {
                var medicine = medicines.First(m => m.Id == item.MedicineId);
                medicine.QuantityInStock -= item.Quantity;
                // Optionally, add a record to a dispense log table here

                _context.MedicineDispenses.Add(new MedicineDispense
                {
                    PatientId = request.PatientId,
                    MedicineId = item.MedicineId,
                    Quantity = item.Quantity,
                    DispensedOn = DateTime.UtcNow
                });
            }

            try
            {
                await _context.SaveChangesAsync();
                return new DispenseResult { Success = true, Message = $"Medicines dispensed successfully." };
            }
            catch (Exception ex)
            {
                // Log error here as needed
                return new DispenseResult { Success = false, Message = "Error occurred while dispensing medicines." };
            }
        }

        public async Task<List<MedicineInventory>> GetAllMedicinesAsync()
        {
            return await _context.MedicineInventories.OrderBy(m => m.Id).ToListAsync();
        }

        public async Task<List<MedicineInventory>> GetExpiredMedicinesAsync()
        {
            return await _context.MedicineInventories.Where(m => m.ExpiryDate.Date <= DateTime.UtcNow.Date).ToListAsync();
        }

        public async Task<List<MedicineInventory>> GetLowStockedMedicinesAsync()
        {
            return await _context.MedicineInventories.Where(m => m.QuantityInStock <= m.LowStockThreshold).ToListAsync();
        }

        public async Task<MedicineInventory> GetMedicineById(int id)
        {
            var medicine = await _context.MedicineInventories.FindAsync(id);
            if (medicine == null)
            {
                return null;
            }
            return medicine;
        }

        public async Task<MedicineInventory> UpdateMedicineAsync(MedicineInventory medicine)
        {
            var exists = await _context.MedicineInventories.FindAsync(medicine.Id);
            if (exists == null)
            {
                return null;
            }
            medicine.ExpiryDate = DateTime.SpecifyKind(medicine.ExpiryDate, DateTimeKind.Utc);
            exists.MedicineName = medicine.MedicineName;
            exists.QuantityInStock = medicine.QuantityInStock;
            exists.ExpiryDate = medicine.ExpiryDate;

            await _context.SaveChangesAsync();
            return exists;
        }

        public async Task<MedicineInventory> UpdateStockAsync(int medicineId, int quantityChange)
        {
            var exists = await _context.MedicineInventories.FindAsync(medicineId);
            if (exists == null)
            {
                return null;
            }
            exists.QuantityInStock += quantityChange;
            await _context.SaveChangesAsync();
            return exists;
        }
    }
}
