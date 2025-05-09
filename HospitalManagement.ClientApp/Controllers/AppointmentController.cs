using System.Security.Claims;
using HospitalManagement.ClientApp.Models;
using HospitalManagement.ClientApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.ClientApp.Controllers
{
    [Authorize(Roles = "Admin,Doctor,Patient,Receptionist")]
    public class AppointmentController : Controller
    {
        private readonly AppointmentService _appointmentService;
        private readonly DoctorService _doctorService;
        private readonly PatientService _patientService;
        private readonly StaffService _staffService;
        public AppointmentController(AppointmentService appointmentService, DoctorService doctorService, PatientService patientService, StaffService staffService)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
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

            if(role == "Receptionist")
            {
                var staff = await _staffService.GetStaffsAsync();
                return staff.Any(s => s.Email == userEmail);
            }
            return true;
        }

        public async Task<IActionResult> DoctorAppointments(int doctorId)
        {
            var role = GetUserRole();
            var userEmail = GetUserEmail();

            if(role != "Admin" && role != "Doctor")
            {
                return Forbid("You do not have access to this resource");
            }
            var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
            if(doctor == null || (role == "Doctor" && doctor.Email != userEmail))
            {
                return Forbid("You can only access your profile only");
            }

            var appointments = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctorId);
            var patients = await _patientService.GetPatientsAsync();

            var patientNames = patients.ToDictionary(p => p.Id, p => p.Name);
             ViewBag.DoctorName = "Dr. "+doctor.Name;

            ViewBag.PatientNames = patientNames;
            return View(appointments);
        }

        public async Task<IActionResult> Index(int? editedId = null)
        {
            var role = GetUserRole();
            var userEmail = GetUserEmail();

            if(role == "Receptionist" && !await isReceptionistStaff())
            {
                return Forbid();
            }

            var appointments = await _appointmentService.GetAppointmentsAsync();
            var doctors = await _doctorService.GetDoctorsAsync();
            var patients = await _patientService.GetPatientsAsync();

            if(role == "Doctor")
            {
                var doctor = doctors.FirstOrDefault(d => d.Email == userEmail);
                if(doctor == null)
                {
                    ViewBag.Message = "No appointments found.";
                    return View(new List<Appointment>());
                }
                appointments = appointments.Where(a => a.DoctorId == doctor.Id).ToList();
            }
            else if(role == "Patient")
            {
                var patient = patients.FirstOrDefault(p => p.Email == userEmail);
                if (patient == null)
                {
                    ViewBag.Message = "No appointments found.";
                    return View(new List<Appointment>());
                }
                appointments = appointments.Where(a => a.PatientId == patient.Id).ToList();
            }
            else if(role != "Admin" && role != "Receptionist")
            {
                TempData["AccessDeniedMessage"] = $"You cannot access this as your role is {role}.";
                return RedirectToAction("Index", "Home");
            }

            appointments = appointments.OrderBy(a => a.Id).ToList();

            ViewBag.EditedId = editedId;

            ViewBag.DoctorNames = doctors.ToDictionary(d => d.Id, d => d.Name);
            ViewBag.PatientNames = patients.ToDictionary(p => p.Id, p => p.Name);

            return View(appointments);
        }


        public async Task<IActionResult> Details(int id)
        {
            var role = GetUserRole();
            var userEmail = GetUserEmail();

            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            var doctor = await _doctorService.GetDoctorByIdAsync(appointment.DoctorId);
            var patient = await _patientService.GetPatientByIdAsync(appointment.PatientId);

            if(role == "Doctor" && doctor.Email != userEmail)
            {
                return Forbid("You can only access your profile only");
            }
            if(role == "Patient" && patient.Email != userEmail)
            {
                return Forbid("You can only access your profile only");
            }


            ViewBag.DoctorName = doctor?.Name ?? "Unknown";
            ViewBag.PatientName = patient?.Name ?? "Unknown";
            return View(appointment);
        }

        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create()
        {
            if (!await isReceptionistStaff())
            {
                return Forbid();
            }

            var doctors = await _doctorService.GetDoctorsAsync();
            var patients = await _patientService.GetPatientsAsync();

            ViewBag.Doctors = doctors;
            ViewBag.Patients = patients;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (appointment.AppointmentDate < DateTime.UtcNow)
            {
                ModelState.AddModelError("AppointmentDate", "Appointment date cannot be in the past.");
            }

            if (!await isReceptionistStaff())
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                appointment.AppointmentDate = DateTime.SpecifyKind(appointment.AppointmentDate, DateTimeKind.Utc);
                await _appointmentService.AddAppointmentAsync(appointment);
                return RedirectToAction("Index", "Appointment");
            }
            return View(appointment);
        }

        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Edit(int id)
        {
            var role = GetUserRole();
            var userEmail = GetUserEmail();

            if (!await isReceptionistStaff())
            {
                return Forbid();
            }

            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            var doctor = await _doctorService.GetDoctorByIdAsync(appointment.DoctorId);
            var patient = await _patientService.GetPatientByIdAsync(appointment.PatientId);

            if(role == "Doctor" && doctor.Email != userEmail)
            {
                return Forbid("You can only access your profile only");
            }
            if(role == "Patient" && patient.Email != userEmail)
            {
                return Forbid("You can only access your profile only");
            }


            var doctors = await _doctorService.GetDoctorsAsync();
            var patients = await _patientService.GetPatientsAsync();

            ViewBag.Doctors = doctors;
            ViewBag.Patients = patients;
            return View(appointment);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Edit(Appointment appointment)
        {
            if (appointment.AppointmentDate < DateTime.UtcNow)
            {
                ModelState.AddModelError("AppointmentDate", "Appointment date cannot be in the past.");
            }

            if (!await isReceptionistStaff())
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                var role = GetUserRole();
                var userEmail = GetUserEmail();

                var doctor = await _doctorService.GetDoctorByIdAsync(appointment.DoctorId);
                var patient = await _patientService.GetPatientByIdAsync(appointment.PatientId);

                if (role == "Doctor" && doctor.Email != userEmail)
                {
                    return Forbid("You can only access your profile only");
                }
                if (role == "Patient" && patient.Email != userEmail)
                {
                    return Forbid("You can only access your profile only");
                }


                appointment.AppointmentDate = DateTime.SpecifyKind(appointment.AppointmentDate, DateTimeKind.Utc);
                await _appointmentService.UpdateAppointmentAsync(appointment);
                return RedirectToAction("Index", "Appointment", new { editId = appointment.Id });
            }
            ModelState.AddModelError("", "Failed to Update");
            return View(appointment);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            var doctor = await _doctorService.GetDoctorByIdAsync(appointment.DoctorId);
            var patient = await _patientService.GetPatientByIdAsync(appointment.PatientId);

            ViewBag.DoctorName = doctor?.Name ?? "Unknown";
            ViewBag.PatientName = patient?.Name ?? "Unknown";

            return View(appointment);
        }


        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _appointmentService.DeleteAppointmentAsync(id);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to Delete");
                return View();
            }
            return RedirectToAction("Index", "Appointment");
        }
    }
}
