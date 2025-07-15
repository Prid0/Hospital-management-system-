using Hospital_Management.Dto;
using Hospital_Management.Models;

namespace Hospital_Management.Services.Iservice
{
    public interface IdoctorOnLeaveService
    {
        public Task<List<ResponseDoctorOnLeaveDto>> GetAllDoctorOnLeave();
        public Task<DoctorLeaveModel> AddLeave(DoctorLeaveDto doctor);
        public Task<DoctorLeaveModel> UpdateLeave(DoctorLeaveDto doctor, int doctorId);
        public Task<DoctorLeaveModel> CancelLeave(int doctorId);

    }
}
