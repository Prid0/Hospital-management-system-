using Hospital_Management.Dto;
using Hospital_Management.Models;
using Hospital_Management.Services.Iservice;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Services
{
    public class Appiontmentservice : IappointmentService
    {
        private readonly ApiContext _db;
        public Appiontmentservice(ApiContext db)
        {
            _db = db;
        }

        public async Task<List<ResponseAppointments>> GetAllAppointments()
        {
            var AllAppiontments = await _db.appointments.Include(p => p.Patient).Include(d => d.Doctor).Where(a => a.AppointmentBooked || a.AppointmentRescheduled).ToListAsync();
            if (AllAppiontments != null & AllAppiontments.Count > 0)
            {

                var appointments = AllAppiontments.Select(a => new ResponseAppointments
                {
                    AppointmentId = a.AppointmentId,
                    PatientId = (int)a.PatientId,
                    PatientName = a.Patient.FullName,
                    PatientEmail = a.Patient.Email,
                    DoctorId = a.DoctorId,
                    DoctorFullName = a.Doctor.FullName,
                    DoctorEmail = a.Doctor.Email,
                    AppointmentDate = a.AppointmentDate,
                    SlotTime = a.SlotTime,
                    RescheduledAt = a.RescheduledAt,
                    AppointmentBooked = a.AppointmentBooked,
                    AppointmentRescheduled = a.AppointmentRescheduled,
                    AppointmentCancled = a.AppointmentCancled,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    UpdatedBy = a.UpdatedBy

                }).ToList();

                return appointments;
            }
            return null;
        }
        public async Task<List<object>> GetAvailableAppointments()
        {
            var allSlots = new List<string> { "Morning", "Afternoon", "Evening" };
            var today = DateTime.UtcNow.Date;

            var doctors = _db.doctors.Include(d => d.Department).ToList();
            var result = new List<object>();

            foreach (var doctor in doctors)
            {
                var bookedSlots = _db.appointments
                    .Where(a =>
                        a.DoctorId == doctor.DoctorId &&
                        a.AppointmentDate.Date == today &&
                        a.AppointmentBooked &&
                        !a.AppointmentCancled)
                    .Select(a => a.SlotTime.ToLower())
                    .ToList();

                var availableSlots = allSlots
                    .Where(slot => !bookedSlots.Contains(slot.ToLower()))
                    .ToList();

                result.Add(new
                {
                    doctorId = doctor.DoctorId,
                    doctorName = doctor.FullName,
                    department = doctor.Department?.DepartmentName ?? "NA",
                    availableSlots
                });
            }

            return result;
        }

        public async Task<AppointmentModel> Addappointment(AddAppointmentDto appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment data is missing.");

            var date = appointment.AppointmentDate.Date;
            var slot = appointment.SlotTime;
            var patientId = appointment.PatientId;
            var doctorId = appointment.DoctorId;

            var today = DateTime.UtcNow.Date;

            // Check if doctor has a leave entry
            var leave = await _db.doctorOnLeaves
                .FirstOrDefaultAsync(a => a.DoctorId == doctorId);

            if (leave != null)
            {
                // If doctor is currently on leave
                if (leave.StartDate.Date <= today && leave.EndDate.Date >= today)
                    throw new Exception("Doctor is on leave.");

                // If leave is over, update OnLeave = false
                if (leave.EndDate.Date < today && leave.OnLeave)
                {
                    leave.OnLeave = false;
                    _db.doctorOnLeaves.Update(leave);
                }
            }

            // Doctor already booked for this slot?
            bool isDoctorSlotTaken = await _db.appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate.Date == date &&
                a.SlotTime == slot &&
                !a.AppointmentCancled);

            if (isDoctorSlotTaken)
                throw new Exception("Doctor is already booked for this slot.");

            // Patient already booked same doctor in this slot?
            bool hasPatientBookedSameDoctor = await _db.appointments.AnyAsync(a =>
                a.PatientId == patientId &&
                a.DoctorId == doctorId &&
                a.AppointmentDate.Date == date &&
                a.SlotTime == slot &&
                !a.AppointmentCancled);

            if (hasPatientBookedSameDoctor)
                throw new Exception("You have already booked this doctor in this slot.");

            // Patient booked other doctor in same slot?
            bool hasPatientBookedOtherDoctor = await _db.appointments.AnyAsync(a =>
                a.PatientId == patientId &&
                a.DoctorId != doctorId &&
                a.AppointmentDate.Date == date &&
                a.SlotTime == slot &&
                !a.AppointmentCancled);

            if (hasPatientBookedOtherDoctor)
                throw new Exception("You have already booked another doctor in the same slot.");

            // All checks passed — book the appointment
            var newAppointment = new AppointmentModel
            {
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmentDate = appointment.AppointmentDate,
                SlotTime = slot,
                AppointmentBooked = true,
                CreatedAt = DateTime.UtcNow
            };

            await _db.appointments.AddAsync(newAppointment);
            await _db.SaveChangesAsync();

            return newAppointment;
        }




        public async Task<AppointmentModel> UpdateAppointment(UpdateAppointment appointment, int appointmenId, int UpdatedById)
        {
            var existingAppointment = await _db.appointments.FindAsync(appointmenId);
            if (existingAppointment != null)
            {
                existingAppointment.PatientId = appointment.PatientId;
                existingAppointment.DoctorId = appointment.DoctorId;

                existingAppointment.RescheduledAt = appointment.RescheduledAt;

                existingAppointment.SlotTime = appointment.SlotTime;
                existingAppointment.AppointmentRescheduled = true;
                existingAppointment.AppointmentBooked = false;
                existingAppointment.UpdatedAt = DateTime.UtcNow;
                existingAppointment.UpdatedBy = UpdatedById;

                await _db.SaveChangesAsync();
                return existingAppointment;
            }
            return null;


        }
        public async Task<AppointmentModel> DeleteAppointment(int appointmenId)
        {
            var existingAppointment = await _db.appointments.FindAsync(appointmenId);
            if (existingAppointment != null)
            {
                existingAppointment.AppointmentCancled = true;
                //_db.appointments.Remove(existingAppointment);
                await _db.SaveChangesAsync();
                return existingAppointment;
            }
            return null;
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
    }
}
