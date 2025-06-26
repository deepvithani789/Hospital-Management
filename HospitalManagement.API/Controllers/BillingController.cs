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
    public class BillingController : ControllerBase
    {
        private readonly IBillingService _billingService;
        private readonly IMapper _mapper;
        public BillingController(IBillingService billingService, IMapper mapper)
        {
            _billingService = billingService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBillings()
        {
            var billingDomain = await _billingService.GetAllASync();
            return Ok(_mapper.Map<List<BillingDto>>(billingDomain));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBillingById(int id)
        {
            var billingDomain = await _billingService.GetByIdAsync(id);
            if (billingDomain == null) 
            {
                return NotFound();
            }
            return Ok(_mapper.Map<BillingDto>(billingDomain));
        }

        [HttpPost]
        public async Task<IActionResult> AddBilling(AddBillingDto addBilling)
        {
            var billingDomain = _mapper.Map<Billing>(addBilling);
            await _billingService.AddASync(billingDomain);
            return Ok(_mapper.Map<BillingDto>(billingDomain));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBilling(AddBillingDto updateBilling, int id)
        {
            var billingDomain = _mapper.Map<Billing>(updateBilling);
            await _billingService.UpdateASync(billingDomain, id);
            return Ok(_mapper.Map<BillingDto>(billingDomain));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBilling(int id)
        {
            var billingDomain = await _billingService.DeleteASync(id);
            if (billingDomain == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<BillingDto>(billingDomain));
        }

        [HttpGet("invoice/{id}")]
        public async Task<IActionResult> GenerateInvoice(int id)
        {
            try
            {
                var pdfBytes = await _billingService.GenerateInvoice(id);
                return File(pdfBytes, "application/pdf", $"invoice_{id}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
