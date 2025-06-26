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

        [HttpGet("pending-users")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var users = _userManager.Users.Where(u => !u.IsApproved).ToList();

            var result = new List<object>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    Role = roles.FirstOrDefault() ?? "Patient",
                });
            }
            return Ok(result);
        }

        [HttpPost("approve-user/{id}")]
        public async Task<IActionResult> ApproveUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            user.IsApproved = true;
            await _userManager.UpdateAsync(user);
            return Ok(new { message = "User approved successfully.." });
        }
    }
}
