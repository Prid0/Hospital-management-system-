using Hospital_Management.Dto;
using Hospital_Management.Services.Iservice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorOnLeaveController : ControllerBase
    {
        private readonly IdoctorOnLeaveService _doctorOnLeave;

        public DoctorOnLeaveController(IdoctorOnLeaveService doctorOnLeave)
        {
            _doctorOnLeave = doctorOnLeave;
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet]
        public async Task<IActionResult> GetAllDoctorLeaves()
        {
            var doctors = await _doctorOnLeave.GetAllDoctorOnLeave();
            if (doctors != null && doctors.Count > 0)
                return Ok(doctors);

            return NotFound(new { Message = "No doctor leave records found." });
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPost]
        public async Task<IActionResult> AddDoctorLeave([FromBody] DoctorLeaveDto d)
        {
            if (d == null)
                return BadRequest(new { Message = "Doctor leave data is required." });

            var result = await _doctorOnLeave.AddLeave(d);
            return Ok(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateDoctorLeave(int Id, [FromBody] DoctorLeaveDto d)
        {
            if (d == null)
                return BadRequest(new { Message = "Doctor leave data is required." });

            var result = await _doctorOnLeave.UpdateLeave(d, Id);
            if (result == null)
                return NotFound(new { Message = $"No leave record found for Doctor ID {Id}." });

            return Ok(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteDoctorLeave(int Id)
        {
            var result = await _doctorOnLeave.CancelLeave(Id);
            if (result == null)
                return NotFound(new { Message = $"No leave record found for Doctor ID {Id}." });

            return Ok(result);
        }
    }
}
