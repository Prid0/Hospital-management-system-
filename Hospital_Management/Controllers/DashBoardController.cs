using Hospital_Management.Services.Iservice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        private readonly IdashBoardServive _dashBoardServive;
        public DashBoardController(IdashBoardServive idashBoardServive)
        {
            _dashBoardServive = idashBoardServive;
        }


        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet("DailyAppointmentsCountByDoctor")]
        public async Task<IActionResult> DailyAppointmentsCountByDoctor()
        {
            var Appointments = await _dashBoardServive.DailyAppointmentsCountByDoctor();
            if (Appointments != null)
                return Ok(Appointments);

            return NotFound(new { Message = "No Records Avalable." });
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet("DailyAppointmentsCountByDepartment")]
        public async Task<IActionResult> DailyAppointmentsCountByDepartment()
        {
            var Appointments = await _dashBoardServive.DailyAppointmentsCountByDepartment();
            if (Appointments != null)
                return Ok(Appointments);

            return NotFound(new { Message = "No Records Avalable." });
        }


        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet("PatientVisitFrequency")]
        public async Task<IActionResult> PatientVisitFrequencyDto(int month)
        {
            var result = await _dashBoardServive.GetPatientVisitsFrequency(month);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound("no data found");
        }

    }
}
