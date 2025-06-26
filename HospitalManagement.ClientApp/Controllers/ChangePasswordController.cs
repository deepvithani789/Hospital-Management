using System.Security.Claims;
using System.Text;
using HospitalManagement.ClientApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HospitalManagement.ClientApp.Controllers
{
    public class ChangePasswordController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ChangePasswordController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var model = new ChangePasswordDto { Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:7191/api/Auth/change-password", model);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Password changed successfully..";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Error"] = "Enter correct Current Password!!";
                return RedirectToAction("Index", "ChangePassword");
            }
            
        }
    }
}
