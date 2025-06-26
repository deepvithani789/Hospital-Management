using AutoMapper;
using HospitalManagement.API.Models;
using HospitalManagement.API.Models.DTOs;
using HospitalManagement.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly IMapper _mapper;
        public StaffController(IStaffService staffService, IMapper mapper)
        {
            _staffService = staffService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStaffs()
        {
            var staffDomain = await _staffService.GetAllAsync();
            return Ok(_mapper.Map<List<StaffDto>>(staffDomain));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStaffById(int id)
        {
            var staffDomain = await _staffService.GetByIdAsync(id);
            if (staffDomain == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<StaffDto>(staffDomain));
        }

        [HttpPost]
        public async Task<IActionResult> AddStaff(StaffDto staffDto)
        {
            var staffDomain = _mapper.Map<Staff>(staffDto);
            await _staffService.AddAsync(staffDomain);
            return Ok(_mapper.Map<StaffDto>(staffDomain));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStaff(StaffDto staffDto, int id)
        {
            var staffDomain = _mapper.Map<Staff>(staffDto);
            await _staffService.UpdateAsync(staffDomain, id);
            return Ok(_mapper.Map<StaffDto>(staffDomain));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var staffDomain = await _staffService.DeleteAsync(id);
            if(staffDomain == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<StaffDto>(staffDomain));
        }
    }
}
