using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HospitalManagement.ClientApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HospitalManagement.ClientApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Google Login - Redirect to API
        public IActionResult GoogleLogin()
        {
            var googleLoginUrl = "https://localhost:7191/api/Auth/external-login?provider=Google&returnUrl="
                                 + Uri.EscapeDataString("https://localhost:7242/Login/GoogleCallback");
            return Redirect(googleLoginUrl);
        }

        // Callback after Google authentication
        public async Task<IActionResult> GoogleCallback(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Login");
            }

            // Store token in session
            HttpContext.Session.SetString("JWT", token);

            // Decode token to inspect claims
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Console.WriteLine("Decoded JWT Claims:");
            foreach (var claim in jwtToken.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }

            // Extracting correct user details
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email")?.Value;
            var name = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "name")?.Value ?? email;

            Console.WriteLine($"Extracted Email: {email}");
            Console.WriteLine($"Extracted Name: {name}");

            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("⚠️ Email claim is missing from the token!");
                return RedirectToAction("Index", "Login");
            }

            var role = "Patient";

            // Create claims for authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim("JWT", token)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "Home");
        }


        public IActionResult Index()
        {
            if (TempData.ContainsKey("AccessDeniedMessage"))
            {
                ViewBag.AccessDeniedMessage = TempData["AccessDeniedMessage"];
            }
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login model)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7191/api/Auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

                if (responseObj != null && responseObj.ContainsKey("token"))
                {
                    var token = responseObj["token"];
                    HttpContext.Session.SetString("JWT", token);

                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    Console.WriteLine("Extracted JWT Claims:");
                    foreach (var claim in jwtToken.Claims)
                    {
                        Console.WriteLine($"{claim.Type}: {claim.Value}");
                    }

                    // Debugging - Print token to console
                    Console.WriteLine("Token received: " + token);

                    var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "UnknownUser";
                    var name = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? email; // Use email if name is not present
                    var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Unknown";

                    // Create identity claims for authentication
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, name),
                        new Claim(ClaimTypes.Email, email),
                        new Claim(ClaimTypes.Role, role),
                        new Claim("JWT", token)
                    };

                    Console.WriteLine("Decoded JWT Claims:");
                    foreach (var claim in jwtToken.Claims)
                    {
                        Console.WriteLine($"{claim.Type}: {claim.Value}");
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties { IsPersistent = true };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Invalid Credentials or \n ⛔ Your account is pending approval by the Admin.";
            return View("Index");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("JWT");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
