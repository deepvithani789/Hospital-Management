using System.Security.Claims;
using HospitalManagement.ClientApp.Models;
using HospitalManagement.ClientApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.ClientApp.Controllers
{
    [Authorize(Roles = "Admin,Patient,Receptionist")]
    public class PatientController : Controller
    {
        private readonly PatientService _patientService;
        private readonly StaffService _staffService;
        public PatientController(PatientService patientService, StaffService staffService)
        {
            _patientService = patientService;
            _staffService = staffService;
        }

        private string GetUserRole()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }

        private string GetUserEmail()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }
        private async Task<bool> isReceptionistStaff()
        {
            var role = GetUserRole();
            var userEmail = GetUserEmail();

            if (role == "Receptionist")
            {
                var staff = await _staffService.GetStaffsAsync();
                return staff.Any(s => s.Email == userEmail);
            }
            return true;
        }

        public async Task<IActionResult> Index()
        {
            var role = GetUserRole();
            if (role == "Receptionist" && !await isReceptionistStaff())
            {
                return RedirectToAction("StaffAccessDenied", "Home");
            }

            if (role != "Admin" && role != "Patient" && role != "Receptionist")
            {
                return Forbid();
            }

            var patient = await _patientService.GetPatientsAsync();

            if(role == "Patient")
            {
                var userEmail = GetUserEmail();
                patient = patient.Where(p => p.Email == userEmail).ToList();
            }

            var sortedPatients = patient.OrderBy(p => p.Id).ToList();

            return View(sortedPatients);
        }

        public async Task<IActionResult> Details(int id)
        {
            var role = GetUserRole();

            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            if(role == "Patient" && patient.Email != GetUserEmail())
            {
                return Forbid();
            }
            return View(patient);
        }

        [Authorize(Roles = "Admin,Receptionist")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create(Patient patient)
        {
            if(!await isReceptionistStaff())
            {
                return RedirectToAction("StaffAccessDenied", "Home");
            }

            if(ModelState.IsValid)
            {
                patient.DateOfBirth = DateTime.SpecifyKind(patient.DateOfBirth, DateTimeKind.Utc); // Convert to UTC
                await _patientService.AddPatientAsync(patient);
                return RedirectToAction("Index", "Patient");
            }
            return View(patient);
        }

        [Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> Edit(int id)
        {
            var role = GetUserRole();

            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            if(role == "Patient" && patient.Email != GetUserEmail())
            {
                return Forbid();
            }
            return View(patient);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> Edit(Patient patient)
        {
            if (ModelState.IsValid)
            {
                var role = GetUserRole();
                if (role == "Patient" && patient.Email != GetUserEmail())
                {
                    return Forbid();
                }
                patient.DateOfBirth = DateTime.SpecifyKind(patient.DateOfBirth, DateTimeKind.Utc); // Convert to UTC
                await _patientService.UpdatePatientAsync(patient);
                return RedirectToAction("Index", "Patient", new { editedId = patient.Id });
            }
            ModelState.AddModelError("", "Failed to Update");
            return View(patient);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        [HttpPost, ActionName(("Delete"))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _patientService.DeletePatientAsync(id);

            if(!success)
            {
                ModelState.AddModelError("", "Failed to Delete");
                return View();
            }
            return RedirectToAction("Index", "Patient");
        }
    }
}
