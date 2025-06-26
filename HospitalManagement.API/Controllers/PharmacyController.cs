using AutoMapper;
using HospitalManagement.API.Data;
using HospitalManagement.API.Models;
using HospitalManagement.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacyController : ControllerBase
    {
        private readonly IPharmacyService _pharmacyService;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public PharmacyController(IPharmacyService pharmacyService, IMapper mapper, AppDbContext context)
        {
            _mapper = mapper;
            _pharmacyService = pharmacyService;
            _context = context;
        }

        [HttpGet("medicines")]
        public async Task<IActionResult> GetAllMedicines()
        {
            var medicines = await _pharmacyService.GetAllMedicinesAsync();
            return Ok(_mapper.Map<List<MedicineInventoryDto>>(medicines));
        }

        [HttpGet("medicines/{id}")]
        public async Task<IActionResult> GetMedicineById(int id)
        {
            var medicine = await _pharmacyService.GetMedicineById(id);
            if(medicine == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<MedicineInventoryDto>(medicine));
        }

        [HttpPost("medicines")]
        public async Task<IActionResult> AddMedicine(MedicineInventoryDto medicineDto)
        {
            var medicine = _mapper.Map<MedicineInventory>(medicineDto);
            var addedMedicine = await _pharmacyService.AddMedicineAsync(medicine);
            return Ok(_mapper.Map<MedicineInventoryDto>(addedMedicine));
        }

        [HttpPut("medicines/{id}")]
        public async Task<IActionResult> UpdateMedicine(int id, MedicineInventoryDto medicineDto)
        {
            if(id != medicineDto.Id)
            {
                return BadRequest("Medicine ID Mismatch...");
            }

            var medicine = _mapper.Map<MedicineInventory>(medicineDto);
            var updatedMedicine = await _pharmacyService.UpdateMedicineAsync(medicine);
            if(updatedMedicine == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<MedicineInventoryDto>(updatedMedicine));
        }

        [HttpPost("medicines/{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] MedicineStockUpdateDto medicineStockUpdateDto)
        {
            if(id != medicineStockUpdateDto.MedicineId)
            {
                return BadRequest("Medicine ID Mismatches...");
            }
            var updatedMedicine = await _pharmacyService.UpdateStockAsync(medicineStockUpdateDto.MedicineId, medicineStockUpdateDto.QuantityChange);
            if(updatedMedicine == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<MedicineInventoryDto>(updatedMedicine));
        }

        [HttpGet("alert/low-stocks")]
        public async Task<IActionResult> GetLowStocksAlert()
        {
            var lowStockMedicines = await _pharmacyService.GetLowStockedMedicinesAsync();
            return Ok(_mapper.Map<List<MedicineInventoryDto>>(lowStockMedicines));
        }

        [HttpGet("alert/expired")]
        public async Task<IActionResult> GetExpiredAlert()
        {
            var expiredMedicines = await _pharmacyService.GetExpiredMedicinesAsync();
            return Ok(_mapper.Map<List<MedicineInventoryDto>>(expiredMedicines));
        }

        [HttpPost("dispense")]
        public async Task<IActionResult> DispenseMedicine([FromBody] DispenseMedicineRequestDto request)
        {
            var result = await _pharmacyService.DispenseMedicineAsync(request);
            if(!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpGet("dispensed")]
        public async Task<IActionResult> GetDispensedMedicines()
        {
            var records = await _context.MedicineDispenses
                .Include(d => d.Patient)
                .Include(d => d.Medicine)
                .OrderByDescending(d => d.DispensedOn)
                .ToListAsync();

            var result = records.Select(d => new
            {
                d.Id,
                PatientName = d.Patient.Name,
                MedicineName = d.Medicine.MedicineName,
                d.Quantity,
                d.DispensedOn
            });

            return Ok(result);
        }
    }
}
