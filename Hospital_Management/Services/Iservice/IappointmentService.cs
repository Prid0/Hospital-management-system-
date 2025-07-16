using Hospital_Management.Dto;
using Hospital_Management.Models;

namespace Hospital_Management.Services.Iservice
{
    public interface IappointmentService
    {
        public Task<List<ResponseAppointments>> GetAllAppointments();
        public Task<List<object>> GetAvailableAppointments();
        public Task<AppointmentModel> Addappointment(AddAppointmentDto appointment);
        public Task<AppointmentModel> UpdateAppointment(UpdateAppointment appointment, int appointmenId, int UpdatedById);
        public Task<AppointmentModel> DeleteAppointment(int appointmenId);
        public Task<object> DailyAppointmentsCountByDoctor();
        public Task<object> DailyAppointmentsCountByDepartment();
    }
}
