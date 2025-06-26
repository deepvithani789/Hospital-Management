using AutoMapper;
using HospitalManagement.API.Data;
using HospitalManagement.API.Models;
using HospitalManagement.API.Models.DTOs;
using HospitalManagement.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        public AppointmentController(IAppointmentService appointmentService, IMapper mapper, AppDbContext context)
        {
            _appointmentService = appointmentService;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointmentDomain = await _appointmentService.GetAllAsync();
            return Ok(_mapper.Map<List<AppointmentDto>>(appointmentDomain));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var appointmentDomain = await _appointmentService.GetByIdAsync(id);
            if (appointmentDomain == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<AppointmentDto>(appointmentDomain));
        }

        [HttpPost]
        public async Task<IActionResult> AddAppointment(AddAppointmentDto addAppointment)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appointmentDomain = _mapper.Map<Appointment>(addAppointment);
            await _appointmentService.AddAsync(appointmentDomain);
            return Ok(_mapper.Map<AppointmentDto>(appointmentDomain));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(AddAppointmentDto updateAppointment, int id)
        {
            var appointmentDomain = _mapper.Map<Appointment>(updateAppointment);
            await _appointmentService.UpdateAsync(appointmentDomain, id);
            return Ok(_mapper.Map<AppointmentDto>(appointmentDomain));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointmentDomain = await _appointmentService.DeleteAsync(id);
            if (appointmentDomain == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<AppointmentDto>(appointmentDomain));
        }

        [HttpGet("Doctor/{doctorId}")]
        public async Task<IActionResult> GetAppointmentsByDoctor(int doctorId)
        {
            var appointments = await _context.Appointments
                                             .Where(a => a.DoctorId == doctorId)
                                             .Include(d => d.Doctor)
                                             .Include(p => p.Patient)
                                             .ToListAsync();
            return Ok(appointments);
        }

    }
}
