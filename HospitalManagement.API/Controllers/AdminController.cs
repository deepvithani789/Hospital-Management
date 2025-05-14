using HospitalManagement.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AdminController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet("logged-in-users")]
        public async Task<IActionResult> GetLoggedInUsers()
        {
            var users = _userManager.Users.OrderByDescending(u => u.LastLoginTime).ToList();
            var loggedInUsers = new List<object>();

            foreach (var user in users)
            {
                var isLoggedIn = await _signInManager.CanSignInAsync(user);
                if (isLoggedIn)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    loggedInUsers.Add(new
                    {
                        user.Id,
                        user.FullName,
                        user.UserName,
                        user.Email,
                        Roles = roles,
                        user.LastLoginTime
                    });
                }
            }
            return Ok(loggedInUsers);
        }
    }
}
