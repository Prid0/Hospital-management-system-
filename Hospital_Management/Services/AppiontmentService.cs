using Hospital_Management.Dto;
using Hospital_Management.Models;
using Hospital_Management.Services.Iservice;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Services
{
    public class Appiontmentservice : IappointmentService
    {
        private readonly ApiContext _db;
        private readonly IEmailService _emailService;
        public Appiontmentservice(ApiContext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        public async Task<List<ResponseAppointmentDto>> GetAllAppointments()
        {
            var AllAppiontments = await _db.appointments.Include(p => p.Patient).Include(d => d.Doctor).Where(a => a.AppointmentBooked || a.AppointmentRescheduled).ToListAsync();
            if (AllAppiontments != null & AllAppiontments.Count > 0)
            {

                var appointments = AllAppiontments.Select(a => new ResponseAppointmentDto
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

        public async Task<ResponseAppointmentDto> Addappointment(AddAppointmentDto appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment data is missing.");

            var date = appointment.AppointmentDate.Date;
            var slot = appointment.SlotTime;
            var patientId = appointment.PatientId;
            var doctorId = appointment.DoctorId;
            var today = DateTime.UtcNow.Date;

            var patient = await _db.patients.FindAsync(patientId);
            var doctor = await _db.doctors.FindAsync(doctorId);

            if (patient == null || doctor == null)
                throw new Exception("Patient or doctor not found.");
            if (date < today)
            {
                throw new Exception("Please enter thr valid date for booking appointment.");
            }

            // Check doctor's leave
            var leave = await _db.doctorOnLeaves.FirstOrDefaultAsync(l => l.DoctorId == doctorId);

            if (leave != null)
            {
                if (leave.StartDate.Date <= today && leave.EndDate.Date >= today)
                    throw new Exception("Doctor is on leave.");

                if (leave.EndDate.Date < today && leave.OnLeave)
                {
                    leave.OnLeave = false;
                    _db.doctorOnLeaves.Update(leave);
                }
            }

            // Check conflicts
            bool isDoctorSlotTaken = await _db.appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate.Date == date &&
                a.SlotTime == slot &&
                !a.AppointmentCancled);

            if (isDoctorSlotTaken)
                throw new Exception("Doctor is already booked for this slot.");

            bool hasPatientBookedSameDoctor = await _db.appointments.AnyAsync(a =>
                a.PatientId == patientId &&
                a.DoctorId == doctorId &&
                a.AppointmentDate.Date == date &&
                a.SlotTime == slot &&
                !a.AppointmentCancled);

            if (hasPatientBookedSameDoctor)
                throw new Exception("You have already booked this doctor in this slot.");

            bool hasPatientBookedOtherDoctor = await _db.appointments.AnyAsync(a =>
                a.PatientId == patientId &&
                a.DoctorId != doctorId &&
                a.AppointmentDate.Date == date &&
                a.SlotTime == slot &&
                !a.AppointmentCancled);

            if (hasPatientBookedOtherDoctor)
                throw new Exception("You have already booked another doctor in the same slot.");

            // Book appointment
            var newAppointment = new AppointmentModel
            {
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmentDate = appointment.AppointmentDate,
                SlotTime = slot,
                AppointmentBooked = true,
                CreatedAt = DateTime.UtcNow
            };

            var appointments = new ResponseAppointmentDto
            {
                PatientId = (int)patientId,
                PatientName = patient.FullName,
                PatientEmail = patient.Email,
                DoctorId = doctorId,
                DoctorFullName = doctor.FullName,
                DoctorEmail = doctor.Email,
                AppointmentDate = appointment.AppointmentDate,
                SlotTime = appointment.SlotTime,
                AppointmentBooked = true,

            };

            // Send confirmation email
            await _emailService.SendAppointmentConfirmationAsync(
            toEmail: patient.Email,
            patientName: patient.FullName,
            doctorName: $"Dr. {doctor.FullName}",
            appointmentDate: appointment.AppointmentDate,
            slotTime: slot);


            await _db.appointments.AddAsync(newAppointment);
            await _db.SaveChangesAsync();
            return appointments;
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

    }
}
