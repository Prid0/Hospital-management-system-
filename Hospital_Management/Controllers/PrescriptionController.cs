using Hospital_Management.Dto;
using Hospital_Management.Models;
using Hospital_Management.Services.Iservice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IprescriptionService _prescriptionService;

        public PrescriptionController(IprescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<PrescriptionModel>>> GetAllPrescription()
        {
            var prescriptions = await _prescriptionService.GetAllPrescription();

            if (prescriptions == null || prescriptions.Count == 0)
                return NotFound("No prescriptions found.");

            return Ok(prescriptions);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<ActionResult<PrescriptionModel>> Add([FromBody] AddPrescriptionDto dto)
        {
            if (dto == null || dto.Medicines == null || dto.Medicines.Count == 0)
                return BadRequest("Invalid prescription data.");

            var newPrescription = await _prescriptionService.AddPrescription(dto);

            if (newPrescription == null)
                return StatusCode(500, "Failed to create prescription.");

            return CreatedAtAction(nameof(GetAllPrescription), new { id = newPrescription.PrescriptionId }, newPrescription);
        }

        //[HttpPut("add-medicines/{patientId}/{prescriptionId}")]
        //public async Task<ActionResult<PrescriptionModel>> AddMoreMedicinInPrescription(int patientId, int prescriptionId, [FromBody] AddPrescriptionDto dto)
        //{
        //    var updated = await _prescriptionService.AddMoreMedicinInPrescription(patientId, prescriptionId, dto);

        //    if (updated == null)
        //        return NotFound($"No existing prescription found for PatientId = {patientId}.");

        //    return Ok(updated);
        //}
        [Authorize(Roles = "Doctor")]
        [HttpPut("update/{patientId}/{prescriptionId}")]
        public async Task<ActionResult<PrescriptionModel>> Update(int patientId, int prescriptionId, [FromBody] AddPrescriptionDto dto)
        {
            var updated = await _prescriptionService.UpdatePrescription(patientId, prescriptionId, dto);

            if (updated == null)
                return NotFound($"No existing prescription found for PatientId = {patientId}.");

            return Ok(updated);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpDelete("{Id}")]
        public async Task<ActionResult<PrescriptionModel>> Delete([FromRoute] int patientId)
        {
            var deleted = await _prescriptionService.DeletePrescription(patientId);

            if (deleted == null)
                return NotFound($"Prescription not found or already deleted for PatientId = {patientId}.");

            return Ok(deleted);
        }
    }
}
