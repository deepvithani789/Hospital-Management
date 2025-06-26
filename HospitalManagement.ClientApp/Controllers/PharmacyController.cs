using System.Security.Claims;
using System.Threading.Tasks;
using HospitalManagement.ClientApp.Models;
using HospitalManagement.ClientApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.ClientApp.Controllers
{
    [Authorize(Roles = "Admin, Pharmacist")]
    public class PharmacyController : Controller
    {
        private readonly PharmacyService _pharmacyService;
        private readonly StaffService _staffService;
        private readonly PatientService _patientService;
        public PharmacyController(PharmacyService pharmacyService, StaffService staffService, PatientService patientService)
        {
            _pharmacyService = pharmacyService;
            _staffService = staffService;
            _patientService = patientService;
        }

        private string GetUserRole()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }

        private string GetUserEmail()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }

        private async Task<bool> isPharmacyStaff()
        {
            var role = GetUserRole();
            var userEmail = GetUserEmail();

            if(role == "Pharmacist")
            {
                var staff = await _staffService.GetStaffsAsync();
                return staff.Any(s => s.Email == userEmail);
            }
            return true;
        }

        public async Task<IActionResult> Index()
        {
            var role = GetUserRole();
            var userEmail = GetUserEmail();

            if(role == "Pharmacist" && !await isPharmacyStaff())
            {
                return Forbid();
            }

            var medicines = await _pharmacyService.GetMedicinesAsync();
            if (medicines == null)
            {
                return NotFound();
            }
            return View(medicines);
        }

        public async Task<IActionResult> Details(int id)
        {
            var role = GetUserRole();
            var userEmail = GetUserEmail();

            if (role == "Pharmacist" && !await isPharmacyStaff())
            {
                return Forbid();
            }
            var medicine = await _pharmacyService.GetMedicineByIdAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }

        public async Task<IActionResult> Create()
        {
            if(!await isPharmacyStaff())
            {
                return Forbid();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MedicineInventoryViewModel model)
        {
            if (!await isPharmacyStaff())
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                bool success = await _pharmacyService.AddMedicineAsync(model);
                if (success)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!await isPharmacyStaff())
            {
                return Forbid();
            }
            var medicine = await _pharmacyService.GetMedicineByIdAsync(id);
            if(medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MedicineInventoryViewModel model)
        {
            if (!await isPharmacyStaff())
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                bool success = await _pharmacyService.UpdateMedicineAsync(model);
                if (success)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> LowStock()
        {
            if (!await isPharmacyStaff())
            {
                return Forbid();
            }
            var medicine = await _pharmacyService.LowStockAsync();
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }

        public async Task<IActionResult> ExpiredMedicine()
        {
            if (!await isPharmacyStaff())
            {
                return Forbid();
            }
            var medicine = await _pharmacyService.Expired();
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }

        public async Task<IActionResult> Dispense()
        {
            var medicines = await _pharmacyService.GetMedicinesAsync();
            ViewBag.Medicines = medicines;
            ViewBag.Patients = await _patientService.GetPatientsAsync();
            var model = new DispenseMedicineRequestDto
            {
                Items = new List<DispenseItemDto> { new DispenseItemDto() }
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Dispense(DispenseMedicineRequestDto model)
        {
            var responseMessage = await _pharmacyService.DispenseMedicineAsync(model);
            TempData["Message"] = responseMessage;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DispensedList()
        {
            var dispensed = await _pharmacyService.GetDispensedMedicinesAsync();
            return View(dispensed);
        }
    }
}
