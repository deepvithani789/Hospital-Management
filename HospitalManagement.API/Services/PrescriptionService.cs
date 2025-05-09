using HospitalManagement.API.Data;
using HospitalManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.API.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly AppDbContext _context;
        public PrescriptionService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Prescription> AddAsync(Prescription prescription)
        {
            await _context.Prescriptions.AddAsync(prescription);
            await _context.SaveChangesAsync();
            return prescription;
        }

        public async Task<Prescription> DeleteAsync(int id)
        {
            var exists = await _context.Prescriptions.FindAsync(id);
            if (exists == null)
            {
                return null;
            }
            _context.Prescriptions.Remove(exists);
            await _context.SaveChangesAsync();
            return exists;
        }

        public async Task<IEnumerable<Prescription>> GetAllAsync()
        {
            return await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(d => d.Doctor)
                .Include(a => a.Appointment)
                .OrderBy(pr => pr.Id)
                .ToListAsync();
        }

        public async Task<Prescription> GetByIddAsync(int id)
        {
            var exists = await _context.Prescriptions.FindAsync(id);
            if (exists == null)
            {
                return null;
            }
            return await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(d => d.Doctor)
                .Include(a => a.Appointment)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Prescription> UpdateAsync(Prescription prescription, int id)
        {
            var exists = await _context.Prescriptions.FindAsync(id);
            if(exists == null)
            {
                return null;
            }

            exists.PatientId = prescription.PatientId;
            exists.DoctorId = prescription.DoctorId;
            exists.AppointmentId = prescription.AppointmentId;
            exists.Medicines = prescription.Medicines;
            exists.Dosage = prescription.Dosage;
            exists.DurationInDays = prescription.DurationInDays;
            exists.Instructions = prescription.Instructions;
            exists.DateIssued = prescription.DateIssued;
            exists.Diagnosis = prescription.Diagnosis;
            exists.Symptoms = prescription.Symptoms;

            await _context.SaveChangesAsync();
            return exists;
        }
    }
}
