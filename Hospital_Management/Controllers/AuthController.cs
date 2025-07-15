using Hospital_Management.Dto;
using Hospital_Management.Services.Iservice;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IauthService _auth;
        public AuthController(IauthService auth)
        {
            _auth = auth;
        }

        [HttpPost]
        public async Task<IActionResult> Login(AuthDto user)
        {
            var token = await _auth.Login(user);
            if (token != null)
            {
                return Ok(token);
            }
            return NotFound();
        }
    }
}
