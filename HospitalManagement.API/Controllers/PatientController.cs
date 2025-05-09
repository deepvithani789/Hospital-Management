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
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IMapper _mapper;
        public PatientController(IPatientService patientService, IMapper mapper)
        {
            _patientService = patientService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var patientDomain = await _patientService.GetAllAsync();
            return Ok(_mapper.Map<List<PatientDto>>(patientDomain));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var patientDomain = await _patientService.GetByIdAsync(id);
            if (patientDomain == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PatientDto>(patientDomain));
        }

        [HttpPost]
        public async Task<IActionResult> AddPatient(AddPatientDto addPatient)
        {
            var patientDomain = _mapper.Map<Patient>(addPatient);
            await _patientService.AddAsync(patientDomain);
            return Ok(_mapper.Map<PatientDto>(patientDomain));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(AddPatientDto updatePatient, int id)
        {
            var patientDomain = _mapper.Map<Patient>(updatePatient);
            await _patientService.UpdateAsync(patientDomain, id);
            return Ok(_mapper.Map<PatientDto>(patientDomain));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patientDomain = await _patientService.DeleteAsync(id);
            return Ok(_mapper.Map<PatientDto>(patientDomain));
        }

    }
}
