using Hospital_Management.Dto;
using Hospital_Management.Services.Iservice;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hospital_Management.Services
{
    public class AuthService : IauthService
    {
        private readonly IConfiguration _configuration;
        private readonly ApiContext _db;
        public AuthService(ApiContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }
        public async Task<string> Login(AuthDto LoginUser)
        {
            if (LoginUser == null)
                throw new ArgumentNullException(nameof(LoginUser), "Please fill all the required details.");

            var user = await _db.users.FirstOrDefaultAsync(u => u.Email == LoginUser.Email && !u.IsDeleted);

            if (user == null)
                throw new FileNotFoundException("User not found.");

            var hasher = new PasswordHasher<AuthDto>();
            var result = hasher.VerifyHashedPassword(LoginUser, user.Password, LoginUser.Password);

            if (result != PasswordVerificationResult.Success)
                throw new InvalidDataException("Invalid credentials");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["JwtSettings:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserID", user.UserId.ToString()),
                new Claim("UserEmail", user.Email),
                new Claim("UserName", user.FullName),
                new Claim(ClaimTypes.Role, user.Role),
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
