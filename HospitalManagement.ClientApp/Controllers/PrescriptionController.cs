using System.Security.Claims;
using HospitalManagement.ClientApp.Models;
using HospitalManagement.ClientApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.ClientApp.Controllers
{
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public class PrescriptionController : Controller
    {
        private readonly DoctorService _doctorService;
        private readonly PatientService _patientService;
        private readonly AppointmentService _appointmentService;
        private readonly PrescriptionService _prescriptionService;

        public PrescriptionController(PrescriptionService prescriptionService, DoctorService doctorService, PatientService patientService, AppointmentService appointmentService)
        {
            _prescriptionService = prescriptionService;
            _doctorService = doctorService;
            _patientService = patientService;
            _appointmentService = appointmentService;
        }

        private string GetUserRole()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }

        private string GetUserEmail()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }

        public async Task<IActionResult> Index(int? PatientId, int? DoctorId, DateTime? DateIssued)
        {
            var role = GetUserRole();
            var userEmail = GetUserEmail();

            var prescriptions = await _prescriptionService.GetPrescriptionsAsync();
            var doctors = await _doctorService.GetDoctorsAsync();
            var patients = await _patientService.GetPatientsAsync();

            if(role == "Doctor")
            {
                var doctor = doctors.FirstOrDefault(d => d.Email == userEmail);
                if(doctor == null)
                {
                    ViewBag.Message = "No prescriptions found.";
                    return View(new List<Prescription>());
                }
                prescriptions = prescriptions.Where(p => p.DoctorId == doctor.Id).ToList();
            }
            else if(role == "Patient")
            {
                var patient = patients.FirstOrDefault(p => p.Email == userEmail);
                if(patient == null)
                {
                    ViewBag.Message = "No prescriptions found.";
                    return View(new List<Prescription>());
                }
                prescriptions = prescriptions.Where(p => p.PatientId == patient.Id).ToList();
            }
            else if(role != "Admin")
            {
                TempData["AccessDeniedMessage"] = $"You cannot access this as your role is {role}.";
                return RedirectToAction("Index", "Home");
            }

            prescriptions = prescriptions.OrderBy(p => p.Id).ToList();

            if(PatientId.HasValue)
            {
                prescriptions = prescriptions.Where(p => p.Id == PatientId.Value).ToList();
            }
            if(DoctorId.HasValue)
            {
                prescriptions = prescriptions.Where(p => p.Id == DoctorId.Value).ToList();
            }
            if(DateIssued.HasValue)
            {
                prescriptions = prescriptions.Where(p => p.DateIssued.Date == DateIssued.Value.Date).ToList();
            }
            ViewBag.Patients = await _patientService.GetPatientsAsync();
            ViewBag.Doctors = await _doctorService.GetDoctorsAsync();
            ViewBag.EditedId = TempData["EditedId"];

            return View(prescriptions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var role = GetUserRole();
            var userEmail = GetUserEmail();

            var prescription = await _prescriptionService.GetPrescriptionById(id);
            if (prescription == null)
            {
                return NotFound();
            }
            var doctor = await _doctorService.GetDoctorByIdAsync(prescription.DoctorId);
            var patient = await _patientService.GetPatientByIdAsync(prescription.PatientId);

            if(role == "Doctor" && doctor.Email != userEmail)
            {
                return Forbid("You can only access your profile only");
            }
            if(role == "Patient" && patient.Email != userEmail)
            {
                return Forbid("You can only access your profile only");
            }

            
            ViewBag.DoctorName = doctor.Name ?? "Unknown";
            ViewBag.PatientName = patient.Name ?? "Unknown";
            return View(prescription);
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> Create(int? doctorId, int? patientId)
        {
            var doctor = await _doctorService.GetDoctorsAsync();
            var patient = await _patientService.GetPatientsAsync();
            var appointment = await _appointmentService.GetAppointmentsAsync();

            ViewBag.Doctor = doctor;
            ViewBag.Patient = patient;
            ViewBag.Appointment = appointment;

            var prescriptions = await _prescriptionService.GetPrescriptionsAsync();
            var exists = prescriptions.FirstOrDefault(d => d.DoctorId == doctorId && d.PatientId == patientId);

            if (doctorId != null && patientId != null && exists == null)
            {
                TempData["ErrorMessage"] = "No appointment found for this Doctor and Patient. Please create an appointment first";
                return RedirectToAction("Create", "Appointment");
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> Create(Prescription prescription)
        {
            if(ModelState.IsValid)
            {
                prescription.DateIssued = DateTime.SpecifyKind(prescription.DateIssued, DateTimeKind.Utc);
                await _prescriptionService.AddPrescriptionAsync(prescription);
                return RedirectToAction("Index", "Prescription");
            }
            return View(prescription);
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var role = GetUserRole();
            var userEmail = GetUserEmail();

            var prescription = await _prescriptionService.GetPrescriptionById(id);
            if (prescription == null)
            {
                return NotFound();
            }

            var doctor = await _doctorService.GetDoctorByIdAsync(prescription.DoctorId);
            var patient = await _patientService.GetPatientByIdAsync(prescription.PatientId);

            if (role == "Doctor" && doctor.Email != userEmail)
            {
                return Forbid("You can only access your profile only");
            }
            if (role == "Patient" && patient.Email != userEmail)
            {
                return Forbid("You can only access your profile only");
            }

            var doctors = await _doctorService.GetDoctorsAsync();
            var patients = await _patientService.GetPatientsAsync();
            var appointments = await _appointmentService.GetAppointmentsAsync();

            ViewBag.Doctor = doctors;
            ViewBag.Patient = patients;
            ViewBag.Appointment = appointments;
            return View(prescription);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> Edit(Prescription prescription)
        {
            if (ModelState.IsValid)
            {
                var role = GetUserRole();
                var userEmail = GetUserEmail();

                var doctor = await _doctorService.GetDoctorByIdAsync(prescription.DoctorId);
                var patient = await _patientService.GetPatientByIdAsync(prescription.PatientId);

                if (role == "Doctor" && doctor.Email != userEmail)
                {
                    return Forbid("You can only access your profile only");
                }
                if (role == "Patient" && patient.Email != userEmail)
                {
                    return Forbid("You can only access your profile only");
                }

                prescription.DateIssued = DateTime.SpecifyKind(prescription.DateIssued, DateTimeKind.Utc);
                await _prescriptionService.UpdatePrescriptionAsync(prescription);

                TempData["EditedId"] = prescription.Id;
                return RedirectToAction("Index", "Prescription");
            }
            ModelState.AddModelError("", "Failed to Update");
            return View(prescription);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var prescription = await _prescriptionService.GetPrescriptionById(id);
            if (prescription == null)
            {
                return NotFound();
            }
            var patient = await _patientService.GetPatientByIdAsync(prescription.PatientId);
            var doctor = await _doctorService.GetDoctorByIdAsync(prescription.DoctorId);
            ViewBag.PatientName = patient.Name;
            ViewBag.DoctorName = doctor.Name;
            return View(prescription);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _prescriptionService.DeletePrescriptionAsync(id);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to Delete");
                return View();
            }
            return RedirectToAction("Index", "Prescription");
        }
    }
}
