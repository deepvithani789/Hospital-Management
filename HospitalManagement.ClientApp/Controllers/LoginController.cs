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

            var errorContent = await response.Content.ReadAsStringAsync();
            try
            {
                var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(errorContent);
                if(errorObj != null && errorObj.TryGetValue("error", out string errorMsg))
                {
                    if (errorMsg.Contains("not approved", StringComparison.OrdinalIgnoreCase))
                        ViewBag.Error = "⛔ Your account is not yet approved by the admin. Please wait for approval.";
                    else if (errorMsg.Contains("no account found", StringComparison.OrdinalIgnoreCase))
                        ViewBag.Error = "⚠️ No account associated with this email.";
                    else if (errorMsg.Contains("invalid password", StringComparison.OrdinalIgnoreCase))
                        ViewBag.Error = "❌ Invalid password, Please Enter Correct password";
                    else
                        ViewBag.Error = "❌ Invalid email or password. Please try again.";
                }
                else
                {
                    ViewBag.Error = "❌ Login failed. Please try again.";
                }
            }
            catch
            {
                ViewBag.Error = "❌ Unexpected error occurred. Please try again.";
            }
            return View("Index");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7191/api/Auth/forgot-password", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

                if (result != null && result.ContainsKey("token"))
                {
                    string email = result["email"];
                    string token = result["token"];

                    // Redirect to reset password page with token
                    return RedirectToAction("ResetPassword", new { token, email });
                }
            }

            ViewBag.Error = "Enter Valid Email to reset your password !!";
            return View();
        }

        public IActionResult ResetPassword(string token, string email)
        {
            return View(new ResetPasswordDto { Token = token, Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7191/api/Auth/reset-password", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "✅ Password has been reset successfully..";
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Error = "❌ Unable to reset password. Token might be invalid or expired. Please try again..";
            return View("ResetPassword");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("JWT");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
