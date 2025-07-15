using Hospital_Management.Dto;
using Hospital_Management.Services.Iservice;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IadminService _iadmin;
        public AdminController(IadminService iadmin)
        {
            _iadmin = iadmin;
        }

        [HttpGet]
        public async Task<IActionResult> GetAlliadmins()
        {
            var iadmins = await _iadmin.GetAllAdmin();
            if (iadmins != null && iadmins.Count > 0)
                return Ok(iadmins);

            return NotFound(new { Message = "No iadmins found." });
        }


        [HttpPost]
        public async Task<IActionResult> Addadmin([FromBody] AddGeneralDto admin)
        {
            if (admin == null)
                return BadRequest(new { Message = "admin data is required." });

            var result = await _iadmin.AddAdmin(admin);
            return Ok(result);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Updateadmin(int adminId, [FromBody] AddGeneralDto admin)
        {
            int UpdatedById = int.Parse(User.FindFirst("UserID")?.Value);
            if (admin == null)
                return BadRequest(new { Message = "admin data is required." });

            var result = await _iadmin.UpdateAdmin(admin, adminId, UpdatedById);
            if (result == null)
                return NotFound(new { Message = $"admin with ID {adminId} not found." });

            return Ok(result);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Deleteadmin(int adminId)
        {
            var result = await _iadmin.DeleteAdmin(adminId);
            if (result == null)
                return NotFound(new { Message = $"admin with ID {adminId} not found." });

            return Ok(result);
        }
    }
}
