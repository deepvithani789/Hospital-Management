using HospitalManagement.ClientApp.Models;
using HospitalManagement.ClientApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HospitalManagement.ClientApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly AppointmentService _appointmentService;
        private readonly BillingService _billingService;
        private readonly DoctorService _doctorService;
        public AdminController(IHttpClientFactory httpClientFactory, AppointmentService appointmentService, BillingService billingService, DoctorService doctorService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _appointmentService = appointmentService;
            _billingService = billingService;
            _doctorService = doctorService;
        }

        public async Task<IActionResult> AdminDashboard()
        {
            var response = await _httpClient.GetAsync("https://localhost:7191/api/Admin/logged-in-users");

            if(response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var loggedInUsers = JsonConvert.DeserializeObject<List<LoggedInUserViewModel>>(json);
                return View(loggedInUsers);
            }
            ViewBag.ErrorMessage = "Unable to fetch logged-in users.";
            return View(new List<LoggedInUserViewModel>());
        }

        public async Task<IActionResult> Dashboard()
        {
            var appointments = await _appointmentService.GetAppointmentsAsync();
            var billings = await _billingService.GetBillingsAsync();
            var doctors = await _doctorService.GetDoctorsAsync();

            var appointmentPerMonth = appointments
                .GroupBy(a => a.AppointmentDate.ToString("yyyy-MM"))
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .OrderBy(g => g.Month)
                .ToList();

            var revenuePerMonth = billings
                .GroupBy(b => b.BillingDate.ToString("yyyy-MM"))
                .Select(g => new { Month = g.Key, Revenue = g.Sum(b => b.FinalAmount) })
                .OrderBy(g => g.Month)
                .ToList();

            var doctorActivity = doctors
                .Select(d => new
                {
                    DoctorName = d.Name,
                    ActivityCount = appointments.Count(a => a.DoctorId == d.Id)
                })
                .OrderByDescending(d => d.ActivityCount)
                .ToList();

            ViewBag.AppointmentsPerMonth = appointmentPerMonth;
            ViewBag.RevenuePerMonth = revenuePerMonth;
            ViewBag.DoctorActivity = doctorActivity;

            return View();
        }

        public async Task<IActionResult> PendingUsers()
        {
            var response = await _httpClient.GetAsync("https://localhost:7191/api/Admin/pending-users");
            if(!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Failed to load pending users.";
                return View(new List<PendingUserViewModel>());
            }
            var json = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<PendingUserViewModel>>(json);
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveUser([FromBody] string id)
        {
            var response = await _httpClient.PostAsync($"https://localhost:7191/api/Admin/approve-user/{id}", null);
            return response.IsSuccessStatusCode ? Json(new { success = true }) : Json(new { success = false });
        }
    }
    public class LoggedInUserViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string LastLoginTime { get; set; }
    }
}
