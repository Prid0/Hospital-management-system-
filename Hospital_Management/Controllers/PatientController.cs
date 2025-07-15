using Hospital_Management.Dto;
using Hospital_Management.Services.Iservice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly Ipatientservice _patientservice;

        public PatientController(Ipatientservice patientservice)
        {
            _patientservice = patientservice;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _patientservice.GetAllPatient();
            if (patients != null && patients.Count > 0)
                return Ok(patients);

            return NotFound(new { Message = "No patients found." });
        }

        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        [HttpGet("name")]
        public async Task<IActionResult> GetPatientByName(string patientName)
        {
            var patient = await _patientservice.GetPatientByName(patientName);
            if (patient != null)
                return Ok(patient);

            return NotFound(new { Message = $"No patient found with name: {patientName}" });
        }

        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        [HttpGet("email")]
        public async Task<IActionResult> GetPatientByEmail(string patientEmail)
        {
            var patient = await _patientservice.GetPatientByEmail(patientEmail);
            if (patient != null)
                return Ok(patient);

            return NotFound(new { Message = $"No patient found with email: {patientEmail}" });
        }

        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        [HttpGet("phone")]
        public async Task<IActionResult> GetPatientByPhoneNumber(string patientPhoneNumber)
        {
            var patient = await _patientservice.GetPatientByPhoneNumber(patientPhoneNumber);
            if (patient != null)
                return Ok(patient);

            return NotFound(new { Message = $"No patient found with phone number: {patientPhoneNumber}" });
        }

        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        [HttpGet("{id}/medical-records")]
        public async Task<IActionResult> GetPatientMedicalRecord(int patientId)
        {
            var patient = await _patientservice.GetPatientMedicalRecord(patientId);
            if (patient != null)
                return Ok(patient);

            return NotFound(new { Message = $"No medical record found for Patient ID: {patientId}" });
        }


        [HttpPost]
        public async Task<IActionResult> AddPatient([FromBody] AddGeneralDto patient)
        {
            if (patient == null)
                return BadRequest(new { Message = "Patient data is required." });

            var result = await _patientservice.AddPatient(patient);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Patient")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int patientId, [FromBody] AddGeneralDto patient)
        {
            int UpdatedById = int.Parse(User.FindFirst("UserID")?.Value);

            if (patient == null)
                return BadRequest(new { Message = "Patient data is required." });

            var result = await _patientservice.UpdatePatient(patient, patientId, UpdatedById);

            if (result == null)
                return NotFound(new { Message = $"Patient with ID {patientId} not found." });

            return Ok(result);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> DeletePatient(int patientId)
        {
            var result = await _patientservice.DeletePatient(patientId);
            if (result == null)
                return NotFound(new { Message = $"Patient with ID {patientId} not found." });

            return Ok(result);
        }
    }
}
