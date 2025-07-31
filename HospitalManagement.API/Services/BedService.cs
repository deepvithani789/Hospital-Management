using HospitalManagement.API.Data;
using HospitalManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.API.Services
{
    public class BedService : IBedService
    {
        private readonly AppDbContext _context;
        public BedService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Bed> AddBedAsync(Bed bed)
        {
            await _context.Beds.AddAsync(bed);
            await _context.SaveChangesAsync();
            return bed;
        }

        public async Task<bool> AssignBedToPatientAsync(int bedId, int patientId)
        {
            var bed = await _context.Beds.FindAsync(bedId);
            if (bed == null || bed.isOccupied)
            {
                return false;
            }

            var patient = await _context.Patients.FindAsync(patientId);
            if(patient == null)
            {
                return false;
            }

            bed.PatientId = patientId;
            bed.isOccupied = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Bed> DeleteBedAsync(int id)
        {
            var bed = await _context.Beds.FindAsync(id);
            if (bed == null)
                return null;

            _context.Beds.Remove(bed);
            await _context.SaveChangesAsync();
            return bed;
        }

        public async Task<List<Bed>> GetAllBedsAsync()
        {
            return await _context.Beds.Include(b => b.Patient).OrderBy(b => b.BedNumber).ToListAsync();
        }

        public async Task<Bed> GetBedByBedNumber(string bedNumber)
        {
            return await _context.Beds.FirstOrDefaultAsync(b => b.BedNumber == bedNumber);
        }

        public async Task<Bed> GetBedByIdAsync(int id)
        {
            var bed = await _context.Beds.Include(p =>p.Patient).FirstOrDefaultAsync(b => b.Id == id);
            if (bed == null)
                return null;

            return bed;
        }

        public async Task<IActionResult> GetBedSummary()
        {
            var summary = await _context.Beds.OrderBy(c => c.BedNumber).GroupBy(b => b.RoomType).Select(g => new
            {
                RoomType = g.Key,
                TotalBeds = g.Count(),
                OccupiedBeds = g.Count(b => b.isOccupied),
                AvailableBeds = g.Count(b => !b.isOccupied)
            }).ToListAsync();

            return summary.Any()
                ? new OkObjectResult(summary)
                : new NotFoundResult();
        }

        public async Task<bool> ReleaseBedForPatientAsync(int patientId)
        {
            var bed = await _context.Beds.FirstOrDefaultAsync(b => b.PatientId == patientId);
            if (bed == null)
                return false;

            bed.PatientId = null;
            bed.isOccupied = false;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Bed> UpdateBedAsync(Bed bed, int id)
        {
            var exists = await _context.Beds.FindAsync(id);
            if (exists == null)
            {
                return null;
            }

            exists.BedNumber = bed.BedNumber;
            exists.RoomType = bed.RoomType;
            exists.isOccupied = bed.isOccupied;
            await _context.SaveChangesAsync();
            return bed;
        }
    }
}
