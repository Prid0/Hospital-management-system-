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

        public async Task<List<AppointmentModel>> GetAllAppointments()
        {
            var Appiontments = await _db.appointments.Where(a => a.AppointmentBooked || a.AppointmentRescheduled).ToListAsync();
            if (Appiontments != null & Appiontments.Count > 0)
            {
                return Appiontments;
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
                    .Select(a => a.SlotTime.ToLower()) // normalize casing
                    .ToList();

                var availableSlots = allSlots
                    .Where(slot => !bookedSlots.Contains(slot.ToLower())) // match normalized
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
            {
                throw new ArgumentNullException(nameof(appointment), "Appointment data is missing.");
            }

            var date = appointment.AppointmentDate.Date;
            var slot = appointment.SlotTime;
            var patientId = appointment.PatientId;
            var doctorId = appointment.DoctorId;

            // 1. Doctor already booked for this slot?
            bool isDoctorSlotTaken = await _db.appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate.Date == date &&
                a.SlotTime == slot &&
                !a.AppointmentCancled);

            if (isDoctorSlotTaken)
                throw new Exception("Doctor is already booked for this slot.");

            // 2. Patient already booked same doctor in this slot?
            bool hasPatientBookedSameDoctor = await _db.appointments.AnyAsync(a =>
                a.PatientId == patientId &&
                a.DoctorId == doctorId &&
                a.AppointmentDate.Date == date &&
                a.SlotTime == slot &&
                !a.AppointmentCancled);

            if (hasPatientBookedSameDoctor)
                throw new Exception("You have already booked this doctor in this slot.");

            // 3. Patient booked other doctor in same slot?
            bool hasPatientBookedOtherDoctor = await _db.appointments.AnyAsync(a =>
                a.PatientId == patientId &&
                a.DoctorId != doctorId &&
                a.AppointmentDate.Date == date &&
                a.SlotTime == slot &&
                !a.AppointmentCancled);

            if (hasPatientBookedOtherDoctor)
                throw new Exception("You have already booked another doctor in the same slot.");

            // All checks passed — book the appointment
            var existingAppointment = new AppointmentModel
            {
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmentDate = appointment.AppointmentDate,
                SlotTime = slot,
                AppointmentBooked = true,
                CreatedAt = DateTime.UtcNow
            };

            await _db.appointments.AddAsync(existingAppointment);
            await _db.SaveChangesAsync();

            return existingAppointment;
        }



        public async Task<AppointmentModel> UpdateAppointment(AddAppointmentDto appointment, int appointmenId, int UpdatedById)
        {
            var existingAppointment = await _db.appointments.FindAsync(appointmenId);
            if (existingAppointment != null)
            {
                existingAppointment.PatientId = appointment.PatientId;
                existingAppointment.DoctorId = appointment.DoctorId;

                existingAppointment.RescheduledAt = appointment.AppointmentDate;

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
    }
}
