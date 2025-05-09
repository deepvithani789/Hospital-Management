using HospitalManagement.API.Data;
using HospitalManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.API.Services
{
    public class StaffService : IStaffService
    {
        private readonly AppDbContext _dbContext;
        public StaffService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Staff> AddAsync(Staff staff)
        {
            await _dbContext.Staffs.AddAsync(staff);
            await _dbContext.SaveChangesAsync();
            return staff;
        }

        public async Task<Staff> DeleteAsync(int id)
        {
            var exists = await _dbContext.Staffs.FindAsync(id);
            if(exists ==  null)
            {
                return null;
            }
            _dbContext.Staffs.Remove(exists);
            await _dbContext.SaveChangesAsync();
            return exists;
        }

        public async Task<IEnumerable<Staff>> GetAllAsync()
        {
            return await _dbContext.Staffs.OrderBy(s => s.Id).ToListAsync();
        }

        public async Task<Staff> GetByIdAsync(int id)
        {
            return await _dbContext.Staffs.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Staff> UpdateAsync(Staff staff, int id)
        {
            var exists = await _dbContext.Staffs.FindAsync(id);
            if (exists == null)
            {
                return null;
            }
            exists.FullName = staff.FullName;
            exists.Gender = staff.Gender;
            exists.DateOfBirth = staff.DateOfBirth;
            exists.Position = staff.Position;
            exists.ContactNumber = staff.ContactNumber;
            exists.Email = staff.Email;
            exists.Address = staff.Address;
            exists.Shift = staff.Shift;
            exists.Salary = staff.Salary;
            exists.AdharCard = staff.AdharCard;
            await _dbContext.SaveChangesAsync();
            return exists;
        }
    }
}
