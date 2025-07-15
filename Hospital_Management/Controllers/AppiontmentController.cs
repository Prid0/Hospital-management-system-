using Hospital_Management.Dto;
using Hospital_Management.Services.Iservice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppiontmentController : ControllerBase
    {
        private readonly IappointmentService _iappointment;
        public AppiontmentController(IappointmentService iappointment)
        {
            _iappointment = iappointment;
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var Appointments = await _iappointment.GetAllAppointments();
            if (Appointments != null && Appointments.Count > 0)
                return Ok(Appointments);

            return NotFound(new { Message = "No Appointments found." });
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet("GetAvailableAppointments")]
        public async Task<IActionResult> GetAvailableAppointments()
        {
            var Appointments = await _iappointment.GetAvailableAppointments();
            if (Appointments != null && Appointments.Count > 0)
                return Ok(Appointments);

            return NotFound(new { Message = "No Appointments found." });
        }

        [Authorize(Roles = "Admin,Receptionist,Patient")]
        [HttpPost]
        public async Task<IActionResult> AddAppointment([FromBody] AddAppointmentDto patient)
        {
            if (patient == null)
                return BadRequest(new { Message = "Patient data is required." });

            var result = await _iappointment.Addappointment(patient);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int appointmenttId, [FromBody] AddAppointmentDto patient)
        {
            int UpdatedById = int.Parse(User.FindFirst("UserID")?.Value);

            if (patient == null)
                return BadRequest(new { Message = "Patient data is required." });

            var result = await _iappointment.UpdateAppointment(patient, appointmenttId, UpdatedById);

            if (result == null)
                return NotFound(new { Message = $"Patient with ID {appointmenttId} not found." });

            return Ok(result);
        }


        [Authorize(Roles = "Admin,Receptionist")]
        [HttpDelete]
        public async Task<IActionResult> DeletePatient(int appointmenttId)
        {
            var result = await _iappointment.DeleteAppointment(appointmenttId);
            if (result == null)
                return NotFound(new { Message = $"Patient with ID {appointmenttId} not found." });

            return Ok(result);
        }
    }
}
