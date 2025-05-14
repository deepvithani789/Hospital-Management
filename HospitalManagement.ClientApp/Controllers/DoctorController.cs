using System.Security.Claims;
using HospitalManagement.ClientApp.Models;
using HospitalManagement.ClientApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.ClientApp.Controllers
{
    [Authorize(Roles = "Admin,Doctor,Receptionist")]
    public class DoctorController : Controller
    {
        private readonly DoctorService _doctorService;
        public DoctorController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        private string GetUserRole()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }

        private string GetUserEmail()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }

        public async Task<IActionResult> Index()
        {
            var role = GetUserRole();
            if(role != "Admin" && role != "Doctor")
            {
                return Forbid("You do not have access to this resource");
            }

            var doctors = await _doctorService.GetDoctorsAsync();
            if(role == "Doctor")
            {
                var userEmail = GetUserEmail();
                doctors = doctors.Where(d => d.Email == userEmail).ToList();
            }

            if (doctors == null || !doctors.Any())
            {
                ViewBag.Message = "No doctors found.";
                return View(new List<Doctor>());
            }

            return View(doctors);
        }

        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null) 
            {
                return NotFound();
            }

            var role = GetUserRole();
            if(role == "Doctor" && doctor.Email != GetUserEmail())
            {
                return Forbid("You can only access your profile only");
            }
            return View(doctor);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                bool success = await _doctorService.AddDoctorAsync(doctor);
                if (success)
                {
                    return RedirectToAction("Index", new { success = true });
                }
            }
            return View(doctor);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            var role = GetUserRole();
            if(role == "Doctor" && doctor.Email != GetUserEmail())
            {
                return Forbid("You can only edit your profile");
            }
            return View(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                return View(doctor);
            }

            var role = GetUserRole();
            if(role == "Doctor" && doctor.Email != GetUserEmail())
            {
                return Forbid("You can only edit your profile");
            }

            var success = await _doctorService.UpdateDoctorAsync(doctor);
            if (!success) 
            {
                ModelState.AddModelError("", "Failed to Update");
                return View(doctor);
            }
            TempData["DoctorOrderRestore"] = "true";
            return RedirectToAction("Index", new { editedDoctorId = doctor.Id });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if(doctor == null)
            {
                return NotFound();
            }
            return View(doctor);
        }

        [HttpPost, ActionName(("Delete"))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var success = await _doctorService.DeleteDoctor(id);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to Delete");
                return View();
            }
            return RedirectToAction("Index", "Doctor");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Doctor")]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            var success = await _doctorService.ToggleAvailabilityAsync(id);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to toggle availability");
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> AvailableDoctors(string specialization, string gender)
        {
            var doctors = await _doctorService.GetAvailableDoctorsAsync();

            if (!string.IsNullOrWhiteSpace(specialization))
                doctors = doctors.Where(d => d.Specialization.Equals(specialization, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(gender))
                doctors = doctors.Where(d => d.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase)).ToList();

            // Populate filter dropdowns
            var specializations = doctors.Select(d => d.Specialization).Distinct().ToList();
            var genders = doctors.Select(d => d.Gender).Distinct().ToList();

            ViewBag.Specializations = specializations;
            ViewBag.Genders = genders;
            ViewBag.SelectedSpecialization = specialization;
            ViewBag.SelectedGender = gender;

            return View(doctors);
        }
    }
}
