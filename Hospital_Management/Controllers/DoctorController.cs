using Hospital_Management.Dto;
using Hospital_Management.Services.Iservice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IdoctorService _doctorService;

        public DoctorController(IdoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await _doctorService.GetAllDoctor();
            if (doctors == null || doctors.Count == 0)
                return NotFound(new { Message = "No doctors found." });

            return Ok(doctors);
        }

        [Authorize(Roles = "Admin,Receptionist,Patient")]
        [HttpGet("GetAvailableDoctors")]
        public async Task<IActionResult> GetAvailableDoctors()
        {
            var doctors = await _doctorService.GetAvailableDoctors();
            if (doctors == null || doctors.Count == 0)
                return NotFound(new { Message = "No available doctors found." });

            return Ok(doctors);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddDoctor([FromBody] DoctorDto doctor)
        {
            if (doctor == null)
                return BadRequest(new { Message = "Doctor data is required." });

            var result = await _doctorService.AddDoctor(doctor);
            if (result == null)
                return BadRequest(new { Message = "Failed to add doctor." });

            return Ok(result);
        }

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateDoctor(int doctorId, [FromBody] DoctorDto doctor)
        {
            int UpdatedById = int.Parse(User.FindFirst("UserID")?.Value);
            if (doctor == null)
                return BadRequest(new { Message = "Doctor data is required." });

            var result = await _doctorService.UpdateDoctor(doctor, doctorId, UpdatedById);
            if (result == null)
                return NotFound(new { Message = $"Doctor with ID {doctorId} not found." });

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteDoctor(int doctorId)
        {
            var result = await _doctorService.DeleteDoctor(doctorId);
            if (result == null)
                return NotFound(new { Message = $"Doctor with ID {doctorId} not found." });

            return Ok(result);
        }
    }
}
