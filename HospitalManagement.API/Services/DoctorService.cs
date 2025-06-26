using HospitalManagement.API.Data;
using HospitalManagement.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.API.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly AppDbContext _context;
        public DoctorService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Doctor> AddAsync(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();
            return doctor;
        }

        public async Task<Doctor> DeleteAsync(int id)
        {
            var exists = await _context.Doctors.FindAsync(id);
            if (exists != null)
            {
                _context.Doctors.Remove(exists);
                await _context.SaveChangesAsync();
                return exists;
            }
            return null;
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _context.Doctors.AsNoTracking().OrderBy(d => d.Id).ToListAsync();
        }

        public async Task<Doctor> GetByIdAsync(int id)
        {
            var exists = await _context.Doctors.FindAsync(id);
            if (exists != null)
                return exists;

            return null;
        }

        public async Task<Doctor> UpdateAsync(Doctor doctor, int id)
        {
            var exists = await _context.Doctors.FindAsync(id);
            if(exists != null)
            {
                exists.Name = doctor.Name;
                exists.Specialization = doctor.Specialization;
                exists.Gender = doctor.Gender;
                exists.Email = doctor.Email;
                exists.Experience = doctor.Experience;
                exists.Qualification = doctor.Qualification;
                exists.Information = doctor.Information;
                exists.IsAvailable = doctor.IsAvailable;

                await _context.SaveChangesAsync();
                return exists;
            }
            return null;
        }

        public async Task<List<Doctor>> GetAvailableDoctorsAsync()
        {
            return await _context.Doctors.OrderBy(d => d.Id).Where(d => d.IsAvailable).ToListAsync();
        }
    }
}
