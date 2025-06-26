using System.Security.Claims;
using HospitalManagement.ClientApp.Models;
using HospitalManagement.ClientApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagement.ClientApp.Controllers
{
    [Authorize(Roles = "Admin,Cashier,Patient")]
    public class BillingController : Controller
    {
        private readonly BillingService _billingService;
        private readonly PatientService _patientService;
        private readonly AppointmentService _appointmentService;
        private readonly StaffService _staffService;
        public BillingController(BillingService billingService, PatientService patientService, AppointmentService appointmentService, StaffService staffService)
        {
            _billingService = billingService;
            _patientService = patientService;
            _appointmentService = appointmentService;
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestAppointment(int patientId)
        {
            var appointments = await _appointmentService.GetAppointmentsAsync();
            var latestAppointment = appointments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .FirstOrDefault();

            if (latestAppointment != null)
            {
                return Json(new
                {
                    success = true,
                    appointmentId = latestAppointment.Id,
                    appointmentDate = latestAppointment.AppointmentDate.ToString("yyyy-MM-dd")
                });
            }

            return Json(new { success = false });
        }

        private string GetUserRole()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }

        private string GetUserEmail()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }

        private async Task<bool> IsCashierStaff()
        {
            var role = GetUserRole();
            var userEmail = GetUserEmail();

            if(role == "Cashier")
            {
                var staff = await _staffService.GetStaffsAsync();
                return staff.Any(s => s.Email == userEmail);
            }
            return true;
        }

        public async Task<IActionResult> Index(int? PatientId, DateTime? BillingDate)
        {
            var role = GetUserRole();
            var userEmail = GetUserEmail();
            if (role == "Cashier" && !await IsCashierStaff())
            {
                return Forbid();
            }
            var patients = await _patientService.GetPatientsAsync();

            var billings = await _billingService.GetBillingsAsync();
            if(role == "Patient")
            {
                var patient = patients.FirstOrDefault(p => p.Email == userEmail);
                if(patient == null)
                {
                    TempData["AccessDeniedMessage"] = "No appointments found for you.";
                    return RedirectToAction("Index", "Login");
                }
                billings = billings.Where(b => b.PatientId == patient.Id).ToList();
            }

            billings = billings.OrderBy(b => b.Id).ToList();

            if (PatientId.HasValue)
            {
                billings = billings.Where(b => b.PatientId == PatientId.Value).ToList();
            }
            if (BillingDate.HasValue)
            {
                billings = billings.Where(b => b.BillingDate.Date == BillingDate.Value.Date).ToList();
            }

            ViewBag.Patients = new SelectList(patients, "Id", "Name");

            ViewBag.PatientList = await _patientService.GetPatientsAsync();

            return View(billings);
        }

        public async Task<IActionResult> Details(int id)
        {
            var role = GetUserRole();
            var userEmail = GetUserEmail();

            var billing = await _billingService.GetBillingByIdAsync(id);
            if (billing == null)
            {
                return NotFound();
            }
            var patient = await _patientService.GetPatientByIdAsync(billing.PatientId);
            var appointment = await _appointmentService.GetAppointmentByIdAsync(billing.AppointmentId);

            if(role == "Patient" && patient.Email != userEmail)
            {
                return Forbid("You can only access your profile only");
            }

            ViewBag.PatientName = patient.Name ?? "Unknown";
            ViewBag.AppointmentDate = appointment.AppointmentDate.ToString("yyyy-MM-dd") ?? "Unknown";
            return View(billing);
        }

        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> Create()
        {
            if(GetUserRole() == "Cashier" && !await IsCashierStaff())
            {
                return Forbid();
            }
            var patients = await _patientService.GetPatientsAsync();
            var appointments = await _appointmentService.GetAppointmentsAsync();

            ViewBag.Patients = new SelectList(patients, "Id", "Name");
            ViewBag.Appointments = new SelectList(appointments, "Id", "AppointmentDate");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> Create(Billing billing)
        {
            if (GetUserRole() == "Cashier" && !await IsCashierStaff())
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                if (billing.AppointmentId == null)
                {
                    TempData["ErrorMessage"] = "Please take the appointment of the patient first before billing.";
                    return RedirectToAction("Index", "Billing");
                }
                billing.BillingDate = DateTime.UtcNow;
                billing.FinalAmount = billing.TotalAmount - (billing.Discount ?? 0) - (billing.InsuranceCoverage ?? 0);
                await _billingService.AddBillingAsync(billing);
                return RedirectToAction("Index", "Billing");
            }
            return View(billing);
        }

        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> Edit(int id)
        {
            if (GetUserRole() == "Cashier" && !await IsCashierStaff())
            {
                return Forbid();
            }
            var billing = await _billingService.GetBillingByIdAsync(id);
            if(billing == null)
            {
                return NotFound();
            }
            var patients = await _patientService.GetPatientsAsync();
            var appointments = await _appointmentService.GetAppointmentsAsync();

            ViewBag.Patients = new SelectList(patients, "Id", "Name", billing.PatientId);
            ViewBag.Appointments = new SelectList(appointments, "Id", "AppointmentDate", billing.AppointmentId);
            return View(billing);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> Edit(Billing billing)
        {
            if (GetUserRole() == "Cashier" && !await IsCashierStaff())
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                if (billing.AppointmentId == null)
                {
                    TempData["Error"] = "⚠️ Please schedule an appointment before billing!";
                    return RedirectToAction("Index");
                }
                billing.FinalAmount = billing.TotalAmount - (billing.Discount ?? 0) - (billing.InsuranceCoverage ?? 0);
                await _billingService.UpdateBillingAsync(billing);
                return RedirectToAction("Index", "Billing", new { editedId = billing.Id });
            }
            return View(billing);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var billing = await _billingService.GetBillingByIdAsync(id);
            if (billing == null)
            {
                return NotFound();
            }
            var patient = await _patientService.GetPatientByIdAsync(billing.PatientId);
            var appointment = await _appointmentService.GetAppointmentByIdAsync(billing.AppointmentId);

            ViewBag.PatientName = patient.Name ?? "Unknown";
            ViewBag.AppointmentDate = appointment.AppointmentDate.ToString("yyyy-MM-dd") ?? "Unknown";
            return View(billing);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _billingService.DeleteBillingAsync(id);
            if(!success)
            {
                ModelState.AddModelError("", "Failed to Delete");
                return View();
            }
            return RedirectToAction("Index", "Billing");
        }
    }
}
