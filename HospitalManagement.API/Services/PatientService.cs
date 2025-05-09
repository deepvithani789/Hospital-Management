using HospitalManagement.API.Data;
using HospitalManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.API.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;
        public PatientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Patient> AddAsync(Patient patient)
        {
            await _context.AddAsync(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<Patient> DeleteAsync(int id)
        {
            var exists = await _context.Patients.FindAsync(id);
            if(exists != null)
            {
                _context.Patients.Remove(exists);
                await _context.SaveChangesAsync();
                return exists;
            }
            return null;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients.OrderBy(p => p.Id).ToListAsync();
        }

        public async Task<Patient> GetByIdAsync(int id)
        {
            var exists = await (_context.Patients.FindAsync(id));
            if(exists != null)
            {
                return exists;
            }
            return null;
        }

        public async Task<Patient> UpdateAsync(Patient patient, int id)
        {
            var exists = await _context.Patients.FindAsync(id);
            if (exists != null)
            {
                exists.Name = patient.Name;
                exists.Email = patient.Email;
                exists.DateOfBirth = patient.DateOfBirth;
                exists.Gender = patient.Gender;
                exists.ContactNumber = patient.ContactNumber;
                exists.Address = patient.Address;
                exists.BloodGroup = patient.BloodGroup;
                exists.ChronicDisease = patient.ChronicDisease;
                exists.AdhaarCard = patient.AdhaarCard;

                await _context.SaveChangesAsync();
                return exists;
            }
            return null;
        }
    }
}
