using HospitalManagement.API.Models;
using HospitalManagement.API.Models.DTOs;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.API.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<MedicineInventory> MedicineInventories { get; set; }
        public DbSet<Bed> Beds { get; set; }
        public DbSet<MedicineDispense> MedicineDispenses { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

    }
}
