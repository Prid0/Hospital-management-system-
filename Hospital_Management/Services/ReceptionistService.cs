using Hospital_Management.Dto;
using Hospital_Management.Models;
using Hospital_Management.Services.Iservice;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Services
{
    public class ReceptionistService : IreceptionistService
    {
        private readonly ApiContext _db;
        public ReceptionistService(ApiContext db)
        {
            _db = db;
        }
        public async Task<List<GeneralResponseDto>> GetAllReceptionist()
        {
            var receptionist = await _db.receptionists.ToListAsync();
            if (receptionist == null & receptionist.Count < 1)
            {
                return null;
            }
            var response = receptionist.Select(p => new GeneralResponseDto
            {
                FullName = p.FullName,
                Gender = p.Gender,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                Id = p.ReceptionistId
            }).ToList();

            return response;
        }

        public async Task<GeneralResponseDto> AddReceptionist(AddGeneralDto receptionist)
        {
            if (receptionist == null)
                throw new MissingFieldException("Patient data is missing.");

            var haser = new PasswordHasher<AddGeneralDto>();
            receptionist.Password = haser.HashPassword(receptionist, receptionist.Password);
            // Step 1: Create User
            var user = new UsersModel
            {
                Role = "Receptionist",
                FullName = receptionist.FullName,
                Gender = receptionist.Gender,
                Email = receptionist.Email,
                PhoneNumber = receptionist.PhoneNumber,
                Password = receptionist.Password,
                CreatedDate = DateTime.UtcNow
            };

            _db.users.Add(user);
            await _db.SaveChangesAsync();

            // Step 2: Create Patient linked to User
            var newreceptionist = new ReceptionistModel
            {
                FullName = user.FullName,
                Gender = user.Gender,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Password = receptionist.Password,
                Role = user.Role,
                UserId = user.UserId,
                CreatedDate = DateTime.UtcNow
            };

            var response = new GeneralResponseDto
            {
                FullName = user.FullName,
                Gender = user.Gender,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };

            _db.receptionists.Add(newreceptionist);
            await _db.SaveChangesAsync();
            return response;
        }

        public async Task<GeneralResponseDto> UpdateReceptionist(AddGeneralDto receptionist, int receptionistId, int UpdatedById)
        {
            var existingreceptionist = await _db.receptionists.Include(p => p.Users).FirstOrDefaultAsync(p => p.ReceptionistId == receptionistId);

            if (existingreceptionist == null)
                throw new Exception("Patient not found");

            // Update Patient fields
            existingreceptionist.FullName = receptionist.FullName;
            existingreceptionist.Email = receptionist.Email;
            existingreceptionist.PhoneNumber = receptionist.PhoneNumber;
            existingreceptionist.Gender = receptionist.Gender;
            existingreceptionist.UpadatedDate = DateTime.UtcNow;
            existingreceptionist.UpdatedBy = UpdatedById;

            // Update linked User fields
            if (existingreceptionist.Users != null)
            {
                existingreceptionist.Users.FullName = receptionist.FullName;
                existingreceptionist.Users.Email = receptionist.Email;
                existingreceptionist.Users.PhoneNumber = receptionist.PhoneNumber;
                existingreceptionist.Users.Gender = receptionist.Gender;
                existingreceptionist.Users.UpadatedDate = DateTime.UtcNow;
                existingreceptionist.Users.UpdatedBy = UpdatedById;
            }
            await _db.SaveChangesAsync();

            return new GeneralResponseDto
            {
                Id = existingreceptionist.ReceptionistId,
                FullName = existingreceptionist.FullName,
                Gender = existingreceptionist.Gender,
                Email = existingreceptionist.Email,
                PhoneNumber = existingreceptionist.PhoneNumber
            };

            throw new FileNotFoundException($"no such patient with Id:{receptionistId}");
        }


        public async Task<GeneralResponseDto> DeleteReceptionist(int receptionistId)
        {
            var existingreceptionist = await _db.receptionists.FindAsync(receptionistId);
            if (existingreceptionist != null)
            {
                _db.receptionists.Remove(existingreceptionist);
                existingreceptionist.Users.IsDeleted = true;
                await _db.SaveChangesAsync();
                return new GeneralResponseDto
                {
                    Id = (int)existingreceptionist.ReceptionistId,
                    FullName = existingreceptionist.FullName,
                    Gender = existingreceptionist.Gender,
                    Email = existingreceptionist.Email,
                    PhoneNumber = existingreceptionist.PhoneNumber
                };
            }
            throw new FileNotFoundException($"no such patient with Id:{receptionistId}");
        }
    }
}
