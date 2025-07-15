using Hospital_Management.Dto;
using Hospital_Management.Models;
using Hospital_Management.Services.Iservice;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Services
{
    public class DoctorOnLeaveService : IdoctorOnLeaveService
    {
        private readonly ApiContext _db;
        public DoctorOnLeaveService(ApiContext db)
        {
            _db = db;
        }

        public async Task<List<ResponseDoctorOnLeaveDto>> GetAllDoctorOnLeave()
        {
            var doctors = await _db.doctorOnLeaves.Include(x => x.Doctor).Where(x => x.OnLeave)
                        .Select(x => new ResponseDoctorOnLeaveDto
                        {
                            DoctorId = x.Doctor.DoctorId,
                            FullName = x.Doctor.FullName,
                            Email = x.Doctor.Email,
                            PhoneNumber = x.Doctor.PhoneNumber,
                            Gender = x.Doctor.Gender,
                            DepartmentId = x.Doctor.DepartmentId,
                            StartDate = x.StartDate,
                            EndDate = x.EndDate,
                            Reason = x.Reason
                        })
                        .ToListAsync();


            if (doctors != null & doctors.Count > 0)
            {
                return doctors;
            }

            return null;
        }
        public async Task<DoctorLeaveModel> AddLeave(DoctorLeaveDto d)
        {
            if (d == null)
            {
                return null;
            }
            var OnLeave = new DoctorLeaveModel
            {
                DoctorId = d.DoctorId,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                Reason = d.Reason,
                //Onleave = true

            };
            _db.doctorOnLeaves.AddAsync(OnLeave);
            await _db.SaveChangesAsync();
            return OnLeave;
        }
        public async Task<DoctorLeaveModel> UpdateLeave(DoctorLeaveDto d, int doctorId)
        {
            var Doctor = await _db.doctorOnLeaves.FindAsync(doctorId);
            if (Doctor == null)
            {
                return null;
            }
            Doctor.StartDate = d.StartDate;
            Doctor.EndDate = d.EndDate;
            //Doctor.Onleave = d.Onleave;
            Doctor.Reason = d.Reason;
            await _db.SaveChangesAsync();
            return Doctor;
        }
        public async Task<DoctorLeaveModel> CancelLeave(int doctorId)
        {
            var Doctor = await _db.doctorOnLeaves.FindAsync(doctorId);
            if (Doctor == null)
            {
                return null;
            }
            _db.doctorOnLeaves.Remove(Doctor);
            await _db.SaveChangesAsync();
            return Doctor;
        }
    }
}
