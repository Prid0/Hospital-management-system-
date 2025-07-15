using Hospital_Management.Dto;
using Hospital_Management.Models;
using Hospital_Management.Services.Iservice;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Services
{
    public class DoctorService : IdoctorService
    {
        private readonly ApiContext _db;
        public DoctorService(ApiContext db)
        {
            _db = db;
        }
        public async Task<List<ResponseDoctorDto>> GetAllDoctor()
        {
            var today = DateTime.Today;

            var doctorsdto = await _db.doctors
                .Include(d => d.LeaveRecords)
                .Include(d => d.Department)
                .Select(p => new ResponseDoctorDto
                {
                    DoctorId = p.DoctorId,
                    FullName = p.FullName,
                    Gender = p.Gender,
                    Email = p.Email,
                    PhoneNumber = p.PhoneNumber,
                    DepartmentId = p.DepartmentId,
                    Specilization = p.Department.DepartmentName,
                    Onleave = p.LeaveRecords.Where(d => d.DoctorId == p.DoctorId).Any(l => l.StartDate <= today && l.EndDate >= today),
                    LeaveEndDate = p.LeaveRecords.Where(l => l.StartDate <= today && l.EndDate >= today)
                    .Select(l => (DateTime?)l.EndDate)
                    .FirstOrDefault()
                })
                .ToListAsync();

            if (doctorsdto == null || doctorsdto.Count < 1)
            {
                return null;
            }

            return doctorsdto;
        }

        public async Task<List<ResponseDoctorDto>> GetAvailableDoctors()
        {
            var today = DateTime.UtcNow;

            var doctorsdto = await _db.doctors
                .Include(p => p.LeaveRecords)
                .Include(p => p.Department)
                .Where(p => !p.LeaveRecords.Any(l => l.OnLeave
                                                    && l.StartDate <= today
                                                    && l.EndDate >= today))
                .Select(p => new ResponseDoctorDto
                {
                    DoctorId = p.DoctorId,
                    FullName = p.FullName,
                    Gender = p.Gender,
                    Email = p.Email,
                    PhoneNumber = p.PhoneNumber,
                    DepartmentId = p.DepartmentId,
                    Specilization = p.Department.DepartmentName,

                })
                .ToListAsync();

            return doctorsdto;
        }

        public async Task<DoctorDto> AddDoctor(DoctorDto d)
        {
            if (d != null)
            {
                var haser = new PasswordHasher<DoctorDto>();
                d.Password = haser.HashPassword(d, d.Password);
                var NewUser = new UsersModel
                {
                    Email = d.Email,
                    Gender = d.Gender,
                    FullName = d.FullName,
                    PhoneNumber = d.PhoneNumber,
                    Password = d.Password,
                    Role = "Doctor"
                };
                await _db.users.AddAsync(NewUser);
                await _db.SaveChangesAsync();

                var NewDoctor = new DoctorModel
                {
                    FullName = d.FullName,
                    Gender = d.Gender,
                    Email = d.Email,
                    Password = d.Password,
                    PhoneNumber = d.PhoneNumber,
                    DepartmentId = d.DepartmentId,
                    UserId = NewUser.UserId,
                    Role = NewUser.Role
                };

                await _db.doctors.AddAsync(NewDoctor);
                await _db.SaveChangesAsync();
                return d;
            }
            ;
            return null;
        }
        public async Task<ResponseDoctorDto> UpdateDoctor(DoctorDto doctor, int doctorId, int UpdatedById)
        {
            var existingDoctor = await _db.doctors.Include(d => d.Department).Include(d => d.Users).FirstOrDefaultAsync(d => d.DoctorId == doctorId);

            if (existingDoctor == null)
                return null;

            // Update Doctor fields
            existingDoctor.FullName = string.IsNullOrEmpty(doctor.FullName) ? existingDoctor.FullName : doctor.FullName;
            existingDoctor.Gender = string.IsNullOrEmpty(doctor.Gender) ? existingDoctor.Gender : doctor.Gender;
            existingDoctor.Email = string.IsNullOrEmpty(doctor.Email) ? existingDoctor.Email : doctor.Email;
            existingDoctor.PhoneNumber = string.IsNullOrEmpty(doctor.PhoneNumber) ? existingDoctor.PhoneNumber : doctor.PhoneNumber;
            existingDoctor.DepartmentId = doctor.DepartmentId <= 0 ? existingDoctor.DepartmentId : doctor.DepartmentId;
            existingDoctor.UpadatedDate = DateTime.UtcNow;
            existingDoctor.UpdatedBy = UpdatedById;

            // Update linked User fields
            if (existingDoctor.Users != null)
            {
                existingDoctor.Users.FullName = existingDoctor.FullName;
                existingDoctor.Users.Email = existingDoctor.Email;
                existingDoctor.Users.PhoneNumber = existingDoctor.PhoneNumber;
                existingDoctor.Users.Gender = existingDoctor.Gender;
                existingDoctor.Users.UpadatedDate = DateTime.UtcNow;
                existingDoctor.Users.UpdatedBy = UpdatedById;

            }

            await _db.SaveChangesAsync();

            return new ResponseDoctorDto
            {
                FullName = existingDoctor.FullName,
                Gender = existingDoctor.Gender,
                Email = existingDoctor.Email,
                PhoneNumber = existingDoctor.PhoneNumber,
                DoctorId = existingDoctor.DoctorId,
                DepartmentId = existingDoctor.DepartmentId,
                Specilization = existingDoctor.Department?.DepartmentName
            };
        }

        public async Task<DoctorDto> DeleteDoctor(int doctorId)
        {
            var existingdoctor = await _db.doctors.FindAsync(doctorId);
            if (existingdoctor != null)
            {
                existingdoctor.Users.IsDeleted = true;
                _db.doctors.Remove(existingdoctor);
                await _db.SaveChangesAsync();
                return new DoctorDto
                {
                    FullName = existingdoctor.FullName,
                    Gender = existingdoctor.Gender,
                    Email = existingdoctor.Email,
                    PhoneNumber = existingdoctor.PhoneNumber,
                    DepartmentId = existingdoctor.DepartmentId
                };
            }
            return null;
        }
    }

}
