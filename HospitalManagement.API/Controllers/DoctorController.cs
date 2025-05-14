using AutoMapper;
using HospitalManagement.API.Models;
using HospitalManagement.API.Models.DTOs;
using HospitalManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDoctorService _doctorService;
        public DoctorController(IDoctorService doctorService, IMapper mapper)
        {
            _doctorService = doctorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctorDomain = await _doctorService.GetAllAsync();
            return Ok(_mapper.Map<List<DoctorDto>>(doctorDomain));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            var doctorDomain = await _doctorService.GetByIdAsync(id);
            return Ok(_mapper.Map<DoctorDto>(doctorDomain));
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor(AddDoctorDto addDoctor)
        {
            var doctorDomain = _mapper.Map<Doctor>(addDoctor);
            await _doctorService.AddAsync(doctorDomain);
            return Ok(_mapper.Map<DoctorDto>(doctorDomain));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(AddDoctorDto updateDoctor, int id)
        {
            var doctorDomain = _mapper.Map<Doctor>(updateDoctor);
            await _doctorService.UpdateAsync(doctorDomain, id);
            return Ok(_mapper.Map<DoctorDto>(doctorDomain));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctorDomain = await _doctorService.DeleteAsync(id);
            return Ok(_mapper.Map<DoctorDto>(doctorDomain));
        }

        [HttpGet("AvailableDoctors")]
        public async Task<IActionResult> GetAvailableDoctors()
        {
            var availableDoctors = await _doctorService.GetAvailableDoctorsAsync();
            return Ok(_mapper.Map<List<DoctorDto>>(availableDoctors));
        }

        [HttpPatch("{id}/toggle-availability")]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            var doctor = await _doctorService.GetByIdAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            doctor.IsAvailable = !doctor.IsAvailable;
            await _doctorService.UpdateAsync(doctor, id);
            return Ok(new { doctor.Id, doctor.IsAvailable });
        }
    }
}
