using Hospital_Management.Dto;
using Hospital_Management.Services.Iservice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IdepartmentService _department;

        public DepartmentController(IdepartmentService department)
        {
            _department = department;
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _department.GetAllDepartment();
            if (departments == null || departments.Count == 0)
                return NotFound(new { Message = "No departments found." });

            return Ok(departments);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentDto department)
        {
            if (department == null)
                return BadRequest(new { Message = "Department data is required." });

            var result = await _department.AddDepartment(department);
            if (result == null)
                return BadRequest(new { Message = "Failed to add department." });

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateDepartment(int departmentId, [FromBody] DepartmentDto department)
        {
            if (department == null)
                return BadRequest(new { Message = "Department data is required." });

            var result = await _department.UpdateDepartment(department, departmentId);
            if (result == null)
                return NotFound(new { Message = $"Department with ID {departmentId} not found." });

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteDepartment(int departmentId)
        {
            var result = await _department.DeleteDepartment(departmentId);
            if (result == null)
                return NotFound(new { Message = $"Department with ID {departmentId} not found." });

            return Ok(result);
        }
    }
}
