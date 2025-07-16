using Hospital_Management.Dto;
using Hospital_Management.Services.Iservice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReceptionistController : ControllerBase
    {
        private readonly IreceptionistService _receptionist;
        public ReceptionistController(IreceptionistService ireceptionist)
        {
            _receptionist = ireceptionist;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllreceptionists()
        {
            var receptionists = await _receptionist.GetAllReceptionist();
            if (receptionists != null && receptionists.Count > 0)
                return Ok(receptionists);

            return NotFound(new { Message = "No receptionists found." });
        }


        [HttpPost]
        public async Task<IActionResult> Addreceptionist([FromBody] AddGeneralDto receptionist)
        {

            if (receptionist == null)
                return BadRequest(new { Message = "receptionist data is required." });

            var result = await _receptionist.AddReceptionist(receptionist);
            return Ok(result);
        }
        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPut("{Id}")]
        public async Task<IActionResult> Updatereceptionist(int Id, [FromBody] AddGeneralDto receptionist)
        {
            int UpdatedById = int.Parse(User.FindFirst("UserID")?.Value);
            if (receptionist == null)
                return BadRequest(new { Message = "receptionist data is required." });

            var result = await _receptionist.UpdateReceptionist(receptionist, Id, UpdatedById);
            if (result == null)
                return NotFound(new { Message = $"receptionist with ID {Id} not found." });

            return Ok(result);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Deletereceptionist(int Id)
        {
            var result = await _receptionist.DeleteReceptionist(Id);
            if (result == null)
                return NotFound(new { Message = $"receptionist with ID {Id} not found." });

            return Ok(result);
        }
    }
}


