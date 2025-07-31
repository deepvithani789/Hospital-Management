using System.Text;
using HospitalManagement.ClientApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HospitalManagement.ClientApp.Controllers
{
    public class RegisterController : Controller
    {
        private readonly HttpClient _httpClient;
        public RegisterController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var repsonse = await _httpClient.PostAsync("https://localhost:7191/api/Auth/register", content);
            if (repsonse.IsSuccessStatusCode)
            {
                TempData["Message"] = "User Registered Successfully !!";
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Error = "Registration Failed";
            return View("Index");

        }
    }
}
