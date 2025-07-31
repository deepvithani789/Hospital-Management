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
    public class BedController : ControllerBase
    {
        private readonly IBedService _bedService;
        private readonly IMapper _mapper;
        public BedController(IBedService bedService, IMapper mapper)
        {
            _bedService = bedService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBeds()
        {
            var beds = await _bedService.GetAllBedsAsync();
            return Ok(_mapper.Map<List<BedDto>>(beds));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBedById(int id)
        {
            var bed = await _bedService.GetBedByIdAsync(id);
            if (bed == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<BedDto>(bed));
        }

        [HttpGet("bednumber/{bedNumber}")]
        public async Task<IActionResult> GetBedByBedNumber(string bedNumber)
        {
            var bed = await _bedService.GetBedByBedNumber(bedNumber);
            if (bed == null)
                return NotFound();

            return Ok(_mapper.Map<BedDto>(bed));
        }


        [HttpPost]
        public async Task<IActionResult> AddBed(AddBedDto addedBedDto)
        {
            var bed = _mapper.Map<Bed>(addedBedDto);
            var added = await _bedService.AddBedAsync(bed);
            return Ok(_mapper.Map<BedDto>(added));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBed(int id, AddBedDto addedBedDto)
        {
            var bed = _mapper.Map<Bed>(addedBedDto);
            var updated = await _bedService.UpdateBedAsync(bed, id);
            return Ok(_mapper.Map<BedDto>(updated));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBed(int id)
        {
            var bed = await _bedService.DeleteBedAsync(id);
            if (bed == null)
                return NotFound();

            return Ok(_mapper.Map<BedDto>(bed));
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignBedToPatient(AssignBedDto assignBedDto)
        {
            var bed = await _bedService.AssignBedToPatientAsync(assignBedDto.BedId, assignBedDto.PatientId);
            if (!bed)
                return BadRequest("Bed is already occupied or invalid patient/bed");

            return Ok("Bed assigned Successfully");
        }

        [HttpPost("release/{patientId}")]
        public async Task<IActionResult> ReleaseBedForPatient(int patientId)
        {
            var bed = await _bedService.ReleaseBedForPatientAsync(patientId);
            if (!bed)
                return NotFound("Bed assigned to patient not found");

            return Ok("Bed released successfully..");
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetBedSummary()
        {
            var summary = await _bedService.GetBedSummary();
            if (summary is BadRequestObjectResult)
            {
                return BadRequest("Error fetching bed summary");
            }
            return Ok(summary);
        }
    }
}
