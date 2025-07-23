using Hospital_Management.Dto;

namespace Hospital_Management.Services.Iservice
{
    public interface IdashBoardServive
    {
        public Task<object> DailyAppointmentsCountByDoctor();
        public Task<object> DailyAppointmentsCountByDepartment();
        public Task<List<PatientVisitFrequencyDto>> GetPatientVisitsFrequency(int month);

    }
}
