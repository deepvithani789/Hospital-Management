using System.Data;
using System.Security.Claims;
using HospitalManagement.ClientApp.Models;
using HospitalManagement.ClientApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.ClientApp.Controllers
{
    [Authorize(Roles = "Admin,Receptionist")]
    public class StaffController : Controller
    {
        private readonly StaffService _staffService;
        public StaffController(StaffService staffService)
        {
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

            if (role == "Receptionist")
            {
                var staff = await _staffService.GetStaffsAsync();
                return staff.Any(s => s.Email == userEmail);
            }
            return true;
        }

        public async Task<IActionResult >Index()
        {
            var role = GetUserRole();
            if (role == "Receptionist" && !await isReceptionistStaff())
            {
                return Forbid();
            }
            var staff = await _staffService.GetStaffsAsync();
            staff = staff.OrderBy(s => s.Id).ToList();
            if (staff == null)
            {
                return NotFound();
            }
            return View(staff);
        }

        public async Task<IActionResult> Details(int id)
        {
            var staff = await _staffService.GetStaffById(id);
            if (staff == null)
            {
                return NotFound();
            }
            return View(staff);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Staff staff)
        {
            if (ModelState.IsValid)
            {
                staff.DateOfBirth = DateTime.SpecifyKind(staff.DateOfBirth, DateTimeKind.Utc);
                await _staffService.AddStaffAsync(staff);
                return RedirectToAction("Index", "Staff");
            }
            return View(staff);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var staff = await _staffService.GetStaffById(id);
            if(staff == null)
            {
                return NotFound();
            }
            return View(staff);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Staff staff)
        {
            if (ModelState.IsValid)
            {
                await _staffService.UpdateStaffAsync(staff);
                return RedirectToAction("Index", "Staff", new { editedId = staff.Id });
            }
            ModelState.AddModelError("", "Failed to Update");
            return View(staff);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var staff = await _staffService.GetStaffById(id);
            if (staff == null)
            {
                return NotFound();
            }
            return View(staff);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _staffService.DeleteStaffAsync(id);
            if(!success)
            {
                ModelState.AddModelError("", "Failed to Delete");
                return View(success);
            }
            return RedirectToAction("Index", "Staff");
        }
    }
}
