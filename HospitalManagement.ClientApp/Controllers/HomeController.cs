using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using HospitalManagement.ClientApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.ClientApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (TempData.ContainsKey("AccessDeniedMessage"))
            {
                ViewBag.AccessDeniedMessage = TempData["AccessDeniedMessage"];
            }
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendContact(string name, string email, string message)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("deepvithani789@gmail.com", "hezrihvadbxczhzi"),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("deepvithani789@gmail.com", "Hospital Management"),
                Subject = "New Contact Us Message",
                Body = $"Name : {name}\nEmail : {email}\nMessage : {message}",
                IsBodyHtml = false,
            };
            mailMessage.To.Add("deepvithani789@gmail.com");

            try
            {
                smtpClient.Send(mailMessage);
                TempData["ContactMessage"] = "Thank you for reaching out to us! We will get back to you shortly.";
            }
            catch (Exception ex)
            {
                TempData["ContactMessage"] = "There was an issue sending your message. Please try again later.";
            }
            return RedirectToAction("ContactUs");
        }

        public IActionResult StaffAccessDenied()
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            ViewBag.UserRole = userRole;
            return View();
        }
    }
}
