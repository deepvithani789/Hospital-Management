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
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IMapper _mapper;
        public PrescriptionController(IPrescriptionService prescriptionService, IMapper mapper)
        {
            _prescriptionService = prescriptionService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPrescriptions()
        {
            var prescriptionDomain = await _prescriptionService.GetAllAsync();
            return Ok(_mapper.Map<List<PrescriptionDto>>(prescriptionDomain));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrescriptionById(int id)
        {
            var prescriptionDomain = await _prescriptionService.GetByIddAsync(id);
            if (prescriptionDomain == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PrescriptionDto>(prescriptionDomain));
        }

        [HttpPost]
        public async Task<IActionResult> AddPrescription(AddPrescriptionDto addPrescription)
        {
            var prescriptionDomain = _mapper.Map<Prescription>(addPrescription);
            await _prescriptionService.AddAsync(prescriptionDomain);
            return Ok(_mapper.Map<PrescriptionDto>(prescriptionDomain));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(AddPrescriptionDto updatePrescription, int id)
        {
            var prescriptionDomain = _mapper.Map<Prescription>(updatePrescription);
            await _prescriptionService.UpdateAsync(prescriptionDomain, id);
            return Ok(_mapper.Map<PrescriptionDto>(prescriptionDomain));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            var prescriptionDomain = await _prescriptionService.DeleteAsync(id);
            if (prescriptionDomain == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PrescriptionDto>(prescriptionDomain));
        }
    }
}
