using Hospital_Management.Dto;
using Hospital_Management.Services.Iservice;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Services
{
    public class DashboardService : IdashBoardServive
    {
        private readonly ApiContext _db;
        public DashboardService(ApiContext db)
        {
            _db = db;
        }
        public async Task<object> DailyAppointmentsCountByDoctor()
        {
            var query = @"select DoctorId,count(PatientId)as patients
                          from appointments
                          group by DoctorId;";

            var todayUtc = DateTime.UtcNow.Date;

            var result = await _db.appointments
                .Where(a => a.AppointmentDate.Date == todayUtc ||
                           (a.AppointmentRescheduled != null && a.RescheduledAt.HasValue && a.RescheduledAt.Value.Date == todayUtc))
                .GroupBy(a => a.DoctorId)
                .Select(g => new
                {
                    DoctorId = g.Key,
                    NumberOfPatients = g.Count()
                })
                .ToListAsync();


            return result;
        }


        public async Task<object> DailyAppointmentsCountByDepartment()
        {
            var query = @"select d.DepartmentId,count(a.PatientId)as patients
                          from appointments a
                          left join doctors d on a.DoctorId=d.DoctorId
                          left join departments dp on d.DepartmentId=dp.DepartmentId
                          group by d.DepartmentId;";
            var todayUtc = DateTime.UtcNow.Date;

            var result = await (from a in _db.appointments
                                where a.AppointmentDate.Date == todayUtc ||
                                      (a.RescheduledAt != null && a.RescheduledAt.Value.Date == todayUtc)
                                join d in _db.doctors on a.DoctorId equals d.DoctorId into adGroup
                                from d in adGroup.DefaultIfEmpty()
                                join dp in _db.departments on d.DepartmentId equals dp.DepartmentId into ddGroup
                                from dp in ddGroup.DefaultIfEmpty()
                                group a by d.DepartmentId into g
                                select new
                                {
                                    DepartmentId = g.Key,
                                    NumberOfPatients = g.Count(x => x.PatientId != null)
                                }).ToListAsync();

            return result;
        }


        public async Task<List<PatientVisitFrequencyDto>> GetPatientVisitsFrequency(int month)
        {
            var monthParam = new SqlParameter("@month", month);


            var result = await _db.Database
                .SqlQueryRaw<PatientVisitFrequencyDto>("EXEC GetPatientVisitsFrequency @month", monthParam)
                .ToListAsync();

            if (result == null || result.Count < 0)
            {
                return null;
            }

            return result;
        }

    }
}
