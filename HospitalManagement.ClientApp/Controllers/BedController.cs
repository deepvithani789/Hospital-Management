using System.Security.Claims;
using System.Threading.Tasks;
using HospitalManagement.ClientApp.Models;
using HospitalManagement.ClientApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.ClientApp.Controllers
{
    [Authorize(Roles = "Admin, Receptionist")]
    public class BedController : Controller
    {
        private readonly BedService _bedService;
        private readonly PatientService _patientService;
        private readonly StaffService _staffService;
        public BedController(BedService bedService, PatientService patientService, StaffService staffService)
        {
            _bedService = bedService;
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
            var userEmail = GetUserEmail();

            if (role == "Receptionist" && !await isReceptionistStaff())
            {
                return Forbid();
            }

            var beds = await _bedService.GetBedsAsync();
            var patients = await _patientService.GetPatientsAsync();

            ViewBag.PatientMap = patients.ToDictionary(p => p.Id, p => p.Name);

            return View(beds);
        }

        public async Task<IActionResult> Details(int id)
        {
            var role = GetUserRole();
            var userEmail = GetUserEmail();

            if (role == "Receptionist" && !await isReceptionistStaff())
            {
                return Forbid();
            }

            var bed = await _bedService.GetBedByIdAsync(id);

            var patients = await _patientService.GetPatientsAsync();

            ViewBag.PatientMap = patients.ToDictionary(p => p.Id, p => p.Name);
            if (bed == null)
            {
                return NotFound();
            }
            return View(bed);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            if (!await isReceptionistStaff())
            {
                return Forbid();
            }

                return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Bed bed)
        {
            if (ModelState.IsValid)
            {
                await _bedService.AddAsync(bed);
                return RedirectToAction(nameof(Index));
            }
            return View(bed);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (!await isReceptionistStaff())
            {
                return Forbid();
            }
            var bed = await _bedService.GetBedByIdAsync(id);
            if(bed == null)
                return NotFound();

            return View(bed);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Bed bed)
        {
            if (!await isReceptionistStaff())
            {
                return Forbid();
            }
            if (bed.BedNumber != null && bed.RoomType != null)
            {
                await _bedService.UpdateAsync(bed);
                return RedirectToAction("Index");
            }
            return View(bed);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var bed = await _bedService.GetBedByIdAsync(id);
            if(bed == null)
                return NotFound();

            return View(bed);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bedService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Assign()
        {
            if (!await isReceptionistStaff())
            {
                return Forbid();
            }
            ViewBag.Beds = await _bedService.GetBedsAsync();
            ViewBag.Patients = await _patientService.GetPatientsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Assign(int bedId, int patientId)
        {
            if (!await isReceptionistStaff())
            {
                return Forbid();
            }
            var beds = await _bedService.GetBedsAsync();
            bool alreadyAssigned = beds.Any(b => b.PatientId == patientId && b.isOccupied);

            if (alreadyAssigned)
            {
                TempData["Error"] = "This patient already has a bed assigned.";
                ViewBag.Beds = beds;
                ViewBag.Patients = await _patientService.GetPatientsAsync();
                return RedirectToAction("Index");
            }
            
            var result = await _bedService.AssignToPatientAsync(bedId, patientId);
            if (!result)
            {
                ModelState.AddModelError("", "Bed Assignment failed...Bed may be occupied");
                ViewBag.Beds = await _bedService.GetBedsAsync();
                ViewBag.Patients = await _patientService.GetPatientsAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Release(int patientId)
        {
            if (!await isReceptionistStaff())
            {
                return Forbid();
            }
            var result = await _bedService.ReleaseForPatient(patientId);
            if(!result)
            {
                TempData["Error"] = "Failed to release bed.";
            }
            else
            {
                TempData["Success"] = "Bed released successfully.";
            }
            return RedirectToAction("Index");
        }
    }
}
