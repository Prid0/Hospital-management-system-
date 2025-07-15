using Hospital_Management.Dto;
using Hospital_Management.Models;
using Hospital_Management.Services.Iservice;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Services
{
    public class AdminService : IadminService
    {
        private readonly ApiContext _db;
        public AdminService(ApiContext db)
        {
            _db = db;
        }
        public async Task<List<GeneralResponseDto>> GetAllAdmin()
        {
            var admin = await _db.admins.ToListAsync();
            if (admin == null & admin.Count < 1)
            {
                return null;
            }
            var response = admin.Select(p => new GeneralResponseDto
            {
                FullName = p.FullName,
                Gender = p.Gender,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                Id = p.AdminId
            }).ToList();

            return response;
        }


        public async Task<GeneralResponseDto> AddAdmin(AddGeneralDto admin)
        {
            if (admin == null)
                throw new MissingFieldException("Patient data is missing.");

            var hasher = new PasswordHasher<AddGeneralDto>();
            admin.Password = hasher.HashPassword(admin, admin.Password);

            // Create User
            var user = new UsersModel
            {
                Role = "Admin",
                FullName = admin.FullName,
                Gender = admin.Gender,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
                Password = admin.Password,
                CreatedDate = DateTime.UtcNow
            };

            _db.users.Add(user);
            await _db.SaveChangesAsync();

            // Create Patient linked to User
            var newAdmin = new AdminModel
            {
                FullName = user.FullName,
                Gender = user.Gender,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Password = admin.Password,
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

            _db.admins.Add(newAdmin);
            await _db.SaveChangesAsync();
            return response;
        }

        public async Task<GeneralResponseDto> UpdateAdmin(AddGeneralDto admin, int adminId, int UpdatedById)
        {
            var existingAdmin = await _db.admins.Include(p => p.Users).FirstOrDefaultAsync(p => p.AdminId == adminId);

            if (existingAdmin == null)
                throw new Exception("Patient not found");

            // Update Patient fields
            existingAdmin.FullName = admin.FullName;
            existingAdmin.Email = admin.Email;
            existingAdmin.PhoneNumber = admin.PhoneNumber;
            existingAdmin.Gender = admin.Gender;
            existingAdmin.UpadatedDate = DateTime.UtcNow;
            existingAdmin.UpdatedBy = UpdatedById;
            // Update linked User fields
            if (existingAdmin.Users != null)
            {
                existingAdmin.Users.FullName = admin.FullName;
                existingAdmin.Users.Email = admin.Email;
                existingAdmin.Users.PhoneNumber = admin.PhoneNumber;
                existingAdmin.Users.Gender = admin.Gender;
                existingAdmin.Users.UpadatedDate = DateTime.UtcNow;
                existingAdmin.Users.UpdatedBy = UpdatedById;
            }
            await _db.SaveChangesAsync();

            return new GeneralResponseDto
            {
                Id = existingAdmin.AdminId,
                FullName = existingAdmin.FullName,
                Gender = existingAdmin.Gender,
                Email = existingAdmin.Email,
                PhoneNumber = existingAdmin.PhoneNumber
            };

            throw new FileNotFoundException($"no such patient with Id:{adminId}");
        }


        public async Task<GeneralResponseDto> DeleteAdmin(int adminId)
        {
            var existingAdmin = await _db.admins.FindAsync(adminId);
            if (existingAdmin != null)
            {
                _db.admins.Remove(existingAdmin);
                existingAdmin.Users.IsDeleted = true;
                await _db.SaveChangesAsync();
                return new GeneralResponseDto
                {
                    Id = (int)existingAdmin.AdminId,
                    FullName = existingAdmin.FullName,
                    Gender = existingAdmin.Gender,
                    Email = existingAdmin.Email,
                    PhoneNumber = existingAdmin.PhoneNumber
                };
            }
            throw new FileNotFoundException($"no such patient with Id:{adminId}");
        }
    }
}
