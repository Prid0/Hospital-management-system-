using Hospital_Management.Dto;
using Hospital_Management.Models;
using Hospital_Management.Services.Iservice;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Services
{
    public class Patientservice : Ipatientservice
    {
        private readonly ApiContext _db;
        public Patientservice(ApiContext db)
        {
            _db = db;
        }
        public async Task<List<GeneralResponseDto>> GetAllPatient()
        {
            var patients = await _db.patients.Where(p => !p.IsDeleted).ToListAsync();
            if (patients == null & patients.Count < 1)
            {
                return null;
            }
            var patientdto = patients.Select(p => new GeneralResponseDto { FullName = p.FullName, Gender = p.Gender, Email = p.Email, PhoneNumber = p.PhoneNumber, Id = p.PatientId }).ToList();

            return patientdto;
        }

        public async Task<GeneralResponseDto> GetPatientByName(string patientName)
        {
            var patient = await _db.patients.FirstOrDefaultAsync(d => d.FullName == patientName);
            if (patient == null)
            {
                return null;
            }
            var Patient = new GeneralResponseDto
            {
                Id = (int)patient.PatientId,
                Email = patient.Email,
                Gender = patient.Gender,
                FullName = patient.FullName,
                PhoneNumber = patient.PhoneNumber,

            };
            return Patient;

        }
        public async Task<GeneralResponseDto> GetPatientByEmail(string patientEmail)
        {
            var patient = await _db.patients.FirstOrDefaultAsync(d => d.Email == patientEmail);
            if (patient == null)
            {
                return null;
            }
            var Patient = new GeneralResponseDto
            {
                Id = (int)patient.PatientId,
                Email = patient.Email,
                Gender = patient.Gender,
                FullName = patient.FullName,
                PhoneNumber = patient.PhoneNumber,

            };
            return Patient;
        }
        public async Task<GeneralResponseDto> GetPatientByPhoneNumber(string patientPhoneNumber)
        {
            var patient = await _db.patients.FirstOrDefaultAsync(d => d.PhoneNumber == patientPhoneNumber);
            if (patient == null)
            {
                return null;
            }
            var Patient = new GeneralResponseDto
            {
                Id = (int)patient.PatientId,
                Email = patient.Email,
                Gender = patient.Gender,
                FullName = patient.FullName,
                PhoneNumber = patient.PhoneNumber,

            };
            return Patient;
        }

        public async Task<PatientMedicalRecordDto> GetPatientMedicalRecord(int patientId)
        {
            var record = await _db.patients
                .Include(x => x.Prescriptions)
                    .ThenInclude(p => p.Medicines)
                .Include(x => x.Prescriptions)
                    .ThenInclude(p => p.Doctor)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (record == null)
                return null;

            var patient = new PatientMedicalRecordDto
            {
                PatientId = record.PatientId,
                PatientName = record.FullName,
                PatientEmail = record.Email,
                PatientPhone = record.PhoneNumber,

                PrescriptionRecord = record.Prescriptions.Select(x => new PatientPrescriptionRecoredDto
                {
                    PrescriptionId = x.PrescriptionId,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = DateTime.UtcNow,
                    DoctorId = x.DoctorId,
                    DoctorName = x.Doctor.FullName,
                    DoctorEmail = x.Doctor.Email,
                    Decease = x.Decease,
                    Medicines = x.Medicines.Select(m => new PrescribedMedicineDto
                    {
                        MedicineName = m.MedicineName,
                        StartDate = m.StartDate,
                        EndDate = m.EndDate,
                        TimesPerDay = m.TimesPerDay
                    }).ToList()
                }).ToList()
            };

            return patient;
        }


        public async Task<GeneralResponseDto> AddPatient(AddGeneralDto patient)
        {
            if (patient == null)
                throw new MissingFieldException("Patient data is missing.");

            var haser = new PasswordHasher<AddGeneralDto>();
            patient.Password = haser.HashPassword(patient, patient.Password);
            // Step 1: Create User
            var user = new UsersModel
            {
                Role = "Patient",
                FullName = patient.FullName,
                Gender = patient.Gender,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                Password = patient.Password,
                CreatedDate = DateTime.UtcNow
            };

            _db.users.Add(user);
            await _db.SaveChangesAsync(); // Generates user.UserId

            // Step 2: Create Patient linked to User
            var newPatient = new PatientModel
            {
                FullName = user.FullName,
                Gender = user.Gender,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Password = patient.Password,
                Role = user.Role,
                UserId = user.UserId,
                CreatedDate = DateTime.UtcNow
            };

            var CreatedPatient = new GeneralResponseDto
            {
                FullName = user.FullName,
                Gender = user.Gender,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };



            _db.patients.Add(newPatient);
            await _db.SaveChangesAsync();
            return CreatedPatient;
        }

        public async Task<GeneralResponseDto?> UpdatePatient(AddGeneralDto patient, int patientId, int UpdatedById)
        {
            //var role = user.FindFirst(ClaimTypes.Role)?.Value;
            var existingPatient = await _db.patients.Include(p => p.Users).FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (existingPatient == null)
                throw new Exception("Patient not found");

            // Update Patient fields
            existingPatient.FullName = patient.FullName;
            existingPatient.Email = patient.Email;
            existingPatient.PhoneNumber = patient.PhoneNumber;
            existingPatient.Gender = patient.Gender;
            existingPatient.UpadatedDate = DateTime.UtcNow;
            existingPatient.UpdatedBy = UpdatedById;

            // Update linked User fields
            if (existingPatient.Users != null)
            {
                existingPatient.Users.FullName = patient.FullName;
                existingPatient.Users.Email = patient.Email;
                existingPatient.Users.PhoneNumber = patient.PhoneNumber;
                existingPatient.Users.Gender = patient.Gender;
                existingPatient.Users.UpadatedDate = DateTime.UtcNow;
                existingPatient.Users.UpdatedBy = UpdatedById;

            }
            await _db.SaveChangesAsync();

            return new GeneralResponseDto
            {
                Id = existingPatient.PatientId,
                FullName = existingPatient.FullName,
                Gender = existingPatient.Gender,
                Email = existingPatient.Email,
                PhoneNumber = existingPatient.PhoneNumber
            };

            throw new FileNotFoundException($"no such patient with Id:{patientId}");
        }

        public async Task<GeneralResponseDto> DeletePatient(int patientId)
        {
            var existingPatient = await _db.patients.FindAsync(patientId);
            if (existingPatient != null)
            {
                //_db.patients.Remove(existingPatient);
                existingPatient.IsDeleted = true;
                existingPatient.Users.IsDeleted = true;
                await _db.SaveChangesAsync();
                return new GeneralResponseDto
                {
                    Id = (int)existingPatient.PatientId,
                    FullName = existingPatient.FullName,
                    Gender = existingPatient.Gender,
                    Email = existingPatient.Email,
                    PhoneNumber = existingPatient.PhoneNumber
                };
            }
            throw new FileNotFoundException($"no such patient with Id:{patientId}");
        }
    }
}
